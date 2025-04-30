using k.Interfaces;
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
    private const float pushStrength = 8f;

    private readonly MapCollider collider;

    private Vector2f velocity = new Vector2f(0, 0);
    private PlayerData data = new();

    private int[,] map;
    private const int tileSize =64;
    private Vector2f offset;

    public Tank(Texture texture,
                 Vector2f position,
                 Keyboard.Key key,
                 Texture greyTexture,
                 Texture bombTex,
                 MapCollider mapCollider
        )
    {
        this.map = mapCollider.Map;
        this.offset = mapCollider.Offset;
        this.fireKey = key;
        this.destroyedTexture = greyTexture;
        this.bombTexture = bombTex;
        collider = mapCollider;

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
        // синхронізуємо карту/offset у колайдері
        collider.Offset = offset;

        float delta = deltaTime.AsSeconds();
        bomb?.Update(deltaTime, entities, map, offset);
        if (cooldown > 0) cooldown -= delta;
        HandleInput(delta, entities);

        var step = velocity * delta;
        var pos = sprite.Position;

        // рух по X
        var tryX = new Vector2f(pos.X - step.X, pos.Y);
        bool hitWallX = CollidesWithWall(tryX);
        bool hitTankX = CollidesWithTank(tryX, entities);
        if (!hitWallX && !hitTankX) pos.X = tryX.X; else velocity.X = 0;

        // рух по Y
        var tryY = new Vector2f(pos.X, pos.Y - step.Y);
        bool hitWallY = CollidesWithWall(tryY);
        bool hitTankY = CollidesWithTank(tryY, entities);
        if (!hitWallY && !hitTankY) pos.Y = tryY.Y; else velocity.Y = 0;

        sprite.Position = pos;
        velocity *= damping;
    }


    public void HandleInput(float dt, List<GameEntity> entities)
    {
        if (!IsAlive) return;

        if (Keyboard.IsKeyPressed(fireKey))
        {
            // Рух вперед
            float a = (sprite.Rotation - 90f) * (float)Math.PI / 180f;
            Vector2f dir = new Vector2f((float)Math.Cos(a), (float)Math.Sin(a));
            velocity += dir * dAcc * dt;

            // Стрільба
            if ((bomb == null || !bomb.IsActive) && cooldown <= 0f)
            {
                float b = sprite.Rotation * (float)Math.PI / 180f;
                Vector2f bdir = new Vector2f(-(float)Math.Sin(b), (float)Math.Cos(b));
                Vector2f spawn = sprite.Position + bdir * 32f;
                bomb = new Bomb(bombTexture, spawn, bdir, sprite.Rotation, this, offset, tileSize);
                cooldown = BombDelay;
            }
        }
        else
        {
            // Чисте обертання без жодних перевірок
            sprite.Rotation += rotationSpeed * dt;
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
    private bool CollidesWithWall(Vector2f testPos)
        => collider.Collides(sprite, collisionMask, testPos);

    private bool CanMoveTo(Vector2f newPos, int[,] map)
    {
        // Отримуємо AABB спрайта (вже зі Scale)
        FloatRect gb = sprite.GetGlobalBounds();
        float w = gb.Width;
        float h = gb.Height;

        // Межі у світових координатах
        float left = newPos.X - w / 2f;
        float top = newPos.Y - h / 2f;
        float right = left + w;
        float bottom = top + h;

        // Переводимо у клітинки з урахуванням offset і tileSize
        int x0 = (int)((left - offset.X) / tileSize);
        int x1 = (int)((right - offset.X) / tileSize);
        int y0 = (int)((top - offset.Y) / tileSize);
        int y1 = (int)((bottom - offset.Y) / tileSize);

        // Ітерація по всіх затронутіх клітинках
        for (int y = y0; y <= y1; y++)
        {
            for (int x = x0; x <= x1; x++)
            {
                // Якщо вийшли за межі масиву або тайл ≠ 0 — заборона руху
                if (x < 0 || y < 0
                    || y >= map.GetLength(0)
                    || x >= map.GetLength(1)
                    || map[y, x] != 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private Vector2f ComputeWallPush()
    {
        var gb = sprite.GetGlobalBounds();
        var corners = new[]
        {
            new Vector2f(gb.Left,        gb.Top),
            new Vector2f(gb.Left+gb.Width,  gb.Top),
            new Vector2f(gb.Left,        gb.Top+gb.Height),
            new Vector2f(gb.Left+gb.Width,  gb.Top+gb.Height),
        };

        Vector2f total = new Vector2f();
        int count = 0;
        for (int i = 0; i < corners.Length; i++)
        {
            var c = corners[i];
            int tx = (int)((c.X - collider.Offset.X) / 64);
            int ty = (int)((c.Y - collider.Offset.Y) / 64);
            if (tx < 0 || ty < 0 || ty >= map.GetLength(0) || tx >= map.GetLength(1)) continue;
            if (map[ty, tx] == 0) continue;
            var center = new Vector2f(
                tx *64 + collider.Offset.X + 64 / 2f,
                ty * 64 + collider.Offset.Y + 64 / 2f
            );
            var d = sprite.Position - center;
            float len = (float)Math.Sqrt(d.X * d.X + d.Y * d.Y);
            if (len > 0) { total += d / len; count++; }
        }
        if (count > 0) total /= count;
        return total;
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

    private bool CanMoveTo(Vector2f newPos)
    {
        FloatRect gb = sprite.GetGlobalBounds();
        float w = gb.Width * sprite.Scale.X;
        float h = gb.Height * sprite.Scale.Y;

        float left = newPos.X - w / 2f;
        float right = newPos.X + w / 2f;
        float top = newPos.Y - h / 2f;
        float bottom = newPos.Y + h / 2f;

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

}
