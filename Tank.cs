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
    private bool wasFiring = false;

    public byte[] CollisionMask
    {
        get { return collisionMask; }
        set { collisionMask = value; }
    }
    private Texture destroyedTexture;
    private Texture bombTexture;
    private Bomb bomb;
    private int sign = 1;
    private Keyboard.Key fireKey;
    private float cooldown;
    private const float BombDelay = 5f;
    private const float dAcc = 400f;
    private const float damping = 0.80f;
    private const float rotationSpeed = 200f;
    private const float pushStrength = 8f;
    private const int tileSize = 64;

    private readonly MapCollider collider;

    private Random random = new();
    private Vector2f velocity = new Vector2f(0, 0);
    private PlayerData data = new();

    
    private Vector2f offset;
    private Vector2u screenSize;
    private IBox[] boxes;
    private bool boxChosen = false;
    private IBox? box;
    
   
    
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
        boxes = new IBox[2]
        {
            new Shield(),
            new MiniTank(this.sprite)
        };

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
    public override void Update(Time deltaTime, List<GameEntity> entities, Vector2f offset)
    {

        float delta = deltaTime.AsSeconds();
        bomb?.Update(deltaTime, entities, offset);
        if (cooldown > 0) cooldown -= delta;
        
        if (collider.CollidesWithBox(sprite, collisionMask, sprite.Position))
        {
            if (!boxChosen)
            {
                box = boxes[random.Next(2)];
                boxChosen = true;
            }
            box.Timer.Restart();
            box.InUse = true;
            if (box is MiniTank miniTank)
            {
                miniTank.ApplyEffect(); box.boxObject = sprite;
            }
            box.boxObject.Position = sprite.Position;
        }
        if (box is not null&&box.IsExpired())
        {
            box.InUse = false;
            if (box is MiniTank miniTank)
            {
                miniTank.RevertEffect();
            }
            boxChosen = false;
        }
        HandleInput(delta, entities);

        var step = velocity * delta;
        var pos = sprite.Position;

        var tryX = new Vector2f(pos.X - step.X, pos.Y);
        bool hitTankX = CollidesWithTank(tryX, entities);
        bool hitWallX = CollidesWithWall(tryX);
        if (!hitTankX&&!hitWallX) pos.X = tryX.X; else velocity.X = 0;

        var tryY = new Vector2f(pos.X, pos.Y - step.Y);
        bool hitTankY = CollidesWithTank(tryY, entities);
        bool hitWallY = CollidesWithWall(tryY);
        if (!hitTankY&&!hitWallY) pos.Y = tryY.Y; else velocity.Y = 0;

        sprite.Position = pos;
        velocity *= damping;
        sprite.Position = pos;
        if (box is not null )
        {
            box.boxObject.Position = sprite.Position;
        }
        var bounds = sprite.GetGlobalBounds();
        float halfW = bounds.Width / 2f;
        float halfH = bounds.Height / 2f;

        if (sprite.Position.X < -halfW)
            sprite.Position = new Vector2f(screenSize.X + halfW, sprite.Position.Y);
        else if (sprite.Position.X > screenSize.X + halfW)
            sprite.Position = new Vector2f(-halfW, sprite.Position.Y);

        if (sprite.Position.Y < -halfH)
            sprite.Position = new Vector2f(sprite.Position.X, screenSize.Y + halfH);
        else if (sprite.Position.Y > screenSize.Y + halfH)
            sprite.Position = new Vector2f(sprite.Position.X, -halfH);

    }


    public void HandleInput(float dt, List<GameEntity> entities)
    {
        if (!IsAlive) return;

        bool isFiring = Keyboard.IsKeyPressed(fireKey);

        if ((bomb == null || !bomb.IsActive) && cooldown <= 0f && isFiring)
        {
            float a = sprite.Rotation * (float)Math.PI / 180f;
            var dir = new Vector2f(-(float)Math.Sin(a), (float)Math.Cos(a));
            var spawn = sprite.Position + dir * 32f;
            bomb = new Bomb(bombTexture, spawn, dir, sprite.Rotation, this, screenSize, collider);
            cooldown = BombDelay;
        }

        if (isFiring)
        {
            float a = (sprite.Rotation - 90f) * (float)Math.PI / 180f;
            var dir = new Vector2f((float)Math.Cos(a), (float)Math.Sin(a));
            velocity += dir * dAcc * dt;
        }
        else
        {
            float oldRot = sprite.Rotation;
            sprite.Rotation += sign * rotationSpeed * dt;

            var other = entities.OfType<Tank>()
                                .FirstOrDefault(t => t != this && Intersects(t, sprite.Position));
            var smth = collider.Collides(this.sprite, this.collisionMask, this.sprite.Position).Item2;

            if (other != null)
            {
                sprite.Rotation = oldRot;
                var diff = sprite.Position - other.Position;
                float len = (float)Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y);
                if (len > 0) sprite.Position += (diff / len) * pushStrength;
            }
            else if (smth is not null)
            {
                sprite.Rotation = oldRot;
                var diff = sprite.Position - smth.Position;
                float len = (float)Math.Sqrt(diff.X * diff.X + diff.Y * diff.Y);
                if (len > 0) sprite.Position += (diff / len) * pushStrength;
            }
        }

        if (wasFiring && !isFiring)
        {
            sign = -sign;
        }

        wasFiring = isFiring;

    }



    public override void Draw(RenderWindow window)
    {
        if (bomb is not null && bomb.IsActive)
            bomb.Draw(window);
        if (box is Shield&& box.InUse)
        {
            box.Draw(window);
        }
        window.Draw(sprite);
    }
    public bool HasShield()
    {
        if(box is Shield)
        {
            return box.InUse;
        }
        return false;
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

   
    
    private bool CollidesWithTank(Vector2f pos, List<GameEntity> entities)
    {
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

}