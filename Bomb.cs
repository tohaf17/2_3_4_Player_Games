using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Collections.Generic;
using System.Linq;

namespace k
{
    public class Bomb : GameEntity
    {
        private readonly Sprite sprite;
        private readonly Vector2f direction;
        private readonly float speed = 150f;

        private readonly Tank owner;
        private readonly Vector2u screenSize;

        private readonly byte[] collisionMask;

        public bool IsActive { get; private set; } = true;
        private float lifetime = 5f; // 5 секунд життя


        public Bomb(Texture texture,
            Vector2f startPosition,
            Vector2f direction,
            float rotation,
            Tank owner,
            Vector2u screenSize,
            int tileSize = 64)

        {
            this.direction = direction;
            this.owner = owner;
            this.screenSize = screenSize;

            sprite = new Sprite(texture)
            {
                Origin = new Vector2f(texture.Size.X / 2f, texture.Size.Y / 2f),
                Position = startPosition,
                Rotation = rotation + 180f,
                Scale = new Vector2f(0.25f, 0.25f)
            };

            collisionMask = PixelPerfectCollision.CreateMask(texture);
        }

        public override void Update(Time delta, List<GameEntity> entities, int[,] _, Vector2f __)
        {
            if (!IsActive) return;

            float dt = delta.AsSeconds();
            lifetime -= dt;
            if (lifetime <= 0f)
            {
                IsActive = false;
                return;
            }
            sprite.Position += direction * speed * dt;

            // --- Wrap по екрану ---
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

            // --- Перевірка попадання ---
            foreach (var tank in entities.OfType<Tank>())
            {
                if (tank != owner &&
                    PixelPerfectCollision.Test(
                        sprite, collisionMask,
                        tank.Sprite, tank.CollisionMask,
                        alphaLimit: 10))
                {
                    tank.TakeDamage();
                    owner.Data.Score += 1;
                    IsActive = false;
                    break;
                }
            }
        }

        public override void Draw(RenderWindow window)
        {
            if (IsActive)
                window.Draw(sprite);
        }
    }
}
