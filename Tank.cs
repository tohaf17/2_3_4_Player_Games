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
    private const int tileSize = 64;
    private Vector2f offset;
    private Vector2u screenSize;

    public Tank(
                 MapCollider collider,          
        Texture texture,
                 Vector2f position,
                 Keyboard.Key key,
                 Texture greyTexture,
                 Texture bombTex,
                 Vector2u screenSize
        )
    {
        this.collider = collider;
        this.screenSize = screenSize;
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
        

        float delta = deltaTime.AsSeconds();
        bomb?.Update(deltaTime, entities, map, offset);
        if (cooldown > 0) cooldown -= delta;
        HandleInput(delta, entities);

        var step = velocity * delta;
        var pos = sprite.Position;

        // рух по X
        var tryX = new Vector2f(pos.X - step.X, pos.Y);
        bool hitTankX = CollidesWithTank(tryX, entities);
        bool hitWallX = CollidesWithWall(tryX);
        if (!hitTankX&&!hitWallX) pos.X = tryX.X; else velocity.X = 0;

        // рух по Y
        var tryY = new Vector2f(pos.X, pos.Y - step.Y);
        bool hitTankY = CollidesWithTank(tryY, entities);
        bool hitWallY = CollidesWithWall(tryY);
        if (!hitTankY&&!hitWallY) pos.Y = tryY.Y; else velocity.Y = 0;

        sprite.Position = pos;
        velocity *= damping;
    }


    public void HandleInput(float dt, List<GameEntity> entities)
    {
        if (!IsAlive) return;

        if ((bomb == null || !bomb.IsActive) && cooldown <= 0f && Keyboard.IsKeyPressed(fireKey))
        {
            float a = sprite.Rotation * (float)Math.PI / 180f;
            var dir = new Vector2f(-(float)Math.Sin(a), (float)Math.Cos(a));
            var spawn = sprite.Position + dir * 32f;
            bomb = new Bomb(bombTexture, spawn, dir, sprite.Rotation, this, screenSize);
            cooldown = BombDelay;
        }

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

            var other = entities.OfType<Tank>()
                                .FirstOrDefault(t => t != this && Intersects(t, sprite.Position));
            var smth= collider.Collides(this.sprite,this.collisionMask,this.sprite.Position).Item2;
            if (other != null)
            {
                sprite.Rotation = oldRot;
                var diff = sprite.Position - other.Position;
                float len = (float)Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y);
                if (len > 0) sprite.Position += (diff / len) * pushStrength;
            }
            else if (smth is not null )
            {
                sprite.Rotation = oldRot;
                var diff = sprite.Position - smth.Position;
                float len = (float)Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y);
                if (len > 0) sprite.Position += (diff / len) * pushStrength;
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
    private bool CollidesWithWall(Vector2f testPos)
        => collider.Collides(sprite, collisionMask, testPos).Item1;

   
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