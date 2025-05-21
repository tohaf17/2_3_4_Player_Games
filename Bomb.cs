using SFML.Graphics;
using SFML.System;
using static k.Constants;
using SFML.Window;
using System.Collections.Generic;
using System.Linq;

namespace k
{
    public class Bomb : GameEntity
    {
        private readonly Vector2f directionBomb;
        private readonly Tank owner;
        private float lifeTimeBomb = 3f;
        public bool IsActive { get; private set; } = true;
        


        public Bomb(Texture texture,
            Vector2f startPosition,
            Vector2f direction,
            float rotation,
            Tank owner,
            Vector2u screenSize,
            MapCollider collider) :base(collider,screenSize)

        {
            this.directionBomb = direction;
            this.owner = owner;

            sprite = new Sprite(texture)
            {
                Origin = new Vector2f(texture.Size.X / 2f, texture.Size.Y / 2f),
                Position = startPosition,
                Rotation = rotation + 180f,
                Scale = new Vector2f(0.25f, 0.25f)
            };

            collisionMask = PixelPerfectCollision.CreateMask(texture);
        }

        public override void Update(Time delta, List<GameEntity> entities)
        {
            if (!IsActive) return;

            float dt = delta.AsSeconds();
            lifeTimeBomb -= dt;
            if (lifeTimeBomb <= 0f)
            {
                IsActive = false;
                return;
            }
            sprite.Position += directionBomb * SpeedBomb * dt;

            ApplyScreenWrapping();

            foreach (var tank in entities.OfType<Tank>())
            {
                if (collider.Collides(sprite, collisionMask, sprite.Position).Item1)
                {
                    IsActive = false;
                }
                if (tank != owner &&
                    PixelPerfectCollision.Test(
                        sprite, collisionMask,
                        tank.Sprite, tank.CollisionMask,
                        alphaLimit: 10))
                {
                    if (tank.HasShield()) continue;
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
