using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;

namespace k;

public class Tank : GameEntity, IControllable
{
    private Sprite sprite;
    public Sprite Sprite
    {
        get { return sprite; }
        set { sprite = value; }
    }
    private byte[] collisionMask;
    public byte[] CollisionMask
    {
        get { return collisionMask; }
        set { collisionMask = value; }
    }
    private Texture destroyedTexture;
    private Texture bombTexture;
    private Bomb bomb;
    private Keyboard.Key fireKey;
    private float cooldown;
    private const float BombDelay = 3f;
    private const float dAcc = 250f;
    private const float damping = 0.80f;
    private const float rotationSpeed = 200f;
    private const float pushStrength = 8f; // сила відштовхування при оберті

    private Vector2f velocity = new Vector2f(0, 0);
    private PlayerData data = new();

    private int[,] map;
    private int tileSize;
    private Vector2f offset;

    public Tank(Texture texture,
                 Vector2f position,
                 Keyboard.Key key,
                 Texture greyTexture,
                 Texture bombTex,
                 int[,] map,
                 int tileSize,
                 Vector2f offset)
    {
        this.map = map;
        this.tileSize = tileSize;
        this.offset = offset;
        this.fireKey = key;
        this.destroyedTexture = greyTexture;
        this.bombTexture = bombTex;

        SetupSprite(texture, position);
    }

    private void SetupSprite(Texture tex, Vector2f? pos = null)
    {
        sprite = new Sprite(tex)
        {
            Origin = new Vector2f(tex.Size.X / 2f, tex.Size.Y / 2f),
            Scale = new Vector2f(64f / tex.Size.X, 64f / tex.Size.Y),
            Position = pos ?? sprite?.Position ?? new Vector2f()
        };
        collisionMask = PixelPerfectCollision.CreateMask(tex);
    }

    public override void Update(Time deltaTime, List<GameEntity> entities, int[,] map, Vector2f offset)
    {
        this.map = map;
        this.offset = offset;
        float dt = deltaTime.AsSeconds();

        bomb?.Update(deltaTime, entities, map, offset);
        if (cooldown > 0f) cooldown -= dt;

        HandleInput(dt, entities);

        var step = velocity * deltaTime.AsSeconds();
        var pos = sprite.Position;

        // 2) Спроба руху по X — перевіряємо і карту, і танки:
        var tryX = new Vector2f(pos.X - step.X, pos.Y);
        bool freeX = CanMoveTo(tryX)
                  && !CollidesWithTank(tryX, entities);
        if (freeX)
            pos.X = tryX.X;
        else
            velocity.X = 0f;

        // 3) Спроба руху по Y — теж саме:
        var tryY = new Vector2f(pos.X, pos.Y - step.Y);
        bool freeY = CanMoveTo(tryY)
                  && !CollidesWithTank(tryY, entities);
        if (freeY)
            pos.Y = tryY.Y;
        else
            velocity.Y = 0f;

        // 4) Фіксуємо нову позицію і ганяємо damping
        sprite.Position = pos;
        velocity *= damping;
    }

    public void HandleInput(float dt, List<GameEntity> entities)
    {
        if (!IsAlive) return;

        // стріляємо
        if ((bomb == null || !bomb.IsActive) && cooldown <= 0f && Keyboard.IsKeyPressed(fireKey))
        {
            float a = sprite.Rotation * (float)Math.PI / 180f;
            var dir = new Vector2f(-(float)Math.Sin(a), (float)Math.Cos(a));
            var spawn = sprite.Position + dir * 32f;
            bomb = new Bomb(bombTexture, spawn, dir, sprite.Rotation, this, offset, tileSize);
            cooldown = BombDelay;
        }

        // рух вперед
        if (Keyboard.IsKeyPressed(fireKey))
        {
            float a = (sprite.Rotation - 90f) * (float)Math.PI / 180f;
            var dir = new Vector2f((float)Math.Cos(a), (float)Math.Sin(a));
            velocity += dir * dAcc * dt;
        }
        else
        {
            float oldRot = sprite.Rotation;
            sprite.Rotation += rotationSpeed * dt;

            var hitTank = entities.OfType<Tank>()
                                 .FirstOrDefault(t => t != this && Intersects(t, sprite.Position));
            if (hitTank != null)
            {
                sprite.Rotation = oldRot;
                // обчислюємо напрямок відштовхування
                var diff = sprite.Position - hitTank.Position;
                float len = (float)Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y);
                if (len > 0f)
                    sprite.Position += (diff / len) * pushStrength;
            }

            // також можна перевірити тайли:
            if (!CanMoveTo(sprite.Position))
            {
                sprite.Rotation = oldRot;
            }
        }
    }

    public override void Draw(RenderWindow window)
    {
        window.Draw(sprite);
        bomb?.Draw(window);
    }

    public Vector2f Position => sprite.Position;
    public bool IsAlive => sprite.Texture != destroyedTexture;
    public Bomb ActiveBomb => bomb;
    public PlayerData Data => data;

    public void TakeDamage()
    {
        SetupSprite(destroyedTexture, sprite.Position);
    }

    private bool CanMoveTo(Vector2f newPos)
    {
        var gb = sprite.GetGlobalBounds();
        float w = gb.Width * sprite.Scale.X;
        float h = gb.Height * sprite.Scale.Y;

        float left = newPos.X - w / 2f;
        float top = newPos.Y - h / 2f;
        float right = left + w;
        float bottom = top + h;

        int x0 = (int)((left - offset.X) / tileSize);
        int x1 = (int)((right - offset.X) / tileSize);
        int y0 = (int)((top - offset.Y) / tileSize);
        int y1 = (int)((bottom - offset.Y) / tileSize);

        for (int y = y0; y <= y1; y++)
            for (int x = x0; x <= x1; x++)
                if (x < 0 || y < 0 || y >= map.GetLength(0) || x >= map.GetLength(1) || map[y, x] != 0)
                    return false;

        return true;
    }

    private bool CollidesWithTank(Vector2f pos, List<GameEntity> entities)
    {
        // тимчасово перемістимо спрайт
        var old = sprite.Position;
        sprite.Position = pos;

        bool hit = entities.OfType<Tank>()
                           .Where(t => t != this)
                           .Any(t => PixelPerfectCollision.Test(
                               sprite, collisionMask,
                               t.sprite, t.collisionMask,
                               alphaLimit: 10));

        sprite.Position = old;
        return hit;
    }

    /// <summary>
    /// Pixel-perfect колізія при заданій позиції.
    /// </summary>
    public bool Intersects(Tank other, Vector2f pos)
    {
        var old = sprite.Position;
        sprite.Position = pos;
        bool hit = PixelPerfectCollision.Test(
            sprite, collisionMask,
            other.sprite, other.collisionMask,
            alphaLimit: 10);
        sprite.Position = old;
        return hit;
    }
}
public static class PixelPerfectCollision
    {
        // побудова маски один раз при ініціалізації
        public static byte[] CreateMask(Texture tx)
        {
            var img = tx.CopyToImage();
            int W = (int)tx.Size.X, H = (int)tx.Size.Y;
            var mask = new byte[W * H];
            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                    mask[x + y * W] = img.GetPixel((uint)x, (uint)y).A;
            return mask;
        }

        public static bool Test(Sprite s1, byte[] mask1, Sprite s2, byte[] mask2, byte alphaLimit = 0)
        {
            var r1 = s1.GetGlobalBounds();
            var r2 = s2.GetGlobalBounds();

            if (!r1.Intersects(r2, out FloatRect inter))
                return false;
            for (int yi = 0; yi < inter.Height; yi++)
            {
                for (int xi = 0; xi < inter.Width; xi++)
                {
                    float wx = inter.Left + xi;
                    float wy = inter.Top + yi;

                    var p1 = (Vector2f)s1.InverseTransform.TransformPoint(wx, wy);
                    var p2 = (Vector2f)s2.InverseTransform.TransformPoint(wx, wy);

                    int ix1 = (int)p1.X, iy1 = (int)p1.Y;
                    int ix2 = (int)p2.X, iy2 = (int)p2.Y;

                    if (ix1 >= 0 && iy1 >= 0 && ix2 >= 0 && iy2 >= 0 &&
                        ix1 < s1.TextureRect.Width && iy1 < s1.TextureRect.Height &&
                        ix2 < s2.TextureRect.Width && iy2 < s2.TextureRect.Height &&
                        mask1[ix1 + iy1 * s1.TextureRect.Width] > alphaLimit &&
                        mask2[ix2 + iy2 * s2.TextureRect.Width] > alphaLimit)
                        return true;
                }
            }

            return false;
        

    }

}