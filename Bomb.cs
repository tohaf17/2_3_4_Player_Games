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
        private readonly int tileSize;
        private Vector2f offset;

        private readonly byte[] collisionMask;  // <- маска бомби

        public bool IsActive { get; private set; } = true;

        public Bomb(
            Texture texture,
            Vector2f startPosition,
            Vector2f direction,
            float rotation,
            Tank owner,
            Vector2f offset,
            int tileSize = 64
        )
        {
            sprite = new Sprite(texture)
            {
                Origin = new Vector2f(texture.Size.X / 2f, texture.Size.Y / 2f),
                Position = startPosition,
                Rotation = rotation + 180f,
                Scale = new Vector2f(0.25f, 0.25f)
            };

            // створюємо маску бомби
            collisionMask = PixelPerfectCollision.CreateMask(texture);

            this.direction = direction;
            this.owner = owner;
            this.offset = offset;
            this.tileSize = tileSize;
        }

        public override void Update(
            Time delta,
            List<GameEntity> entities,
            int[,] map,
            Vector2f offset
        )
        {
            if (!IsActive) return;

            this.offset = offset;
            float dt = delta.AsSeconds();

            // рух бомби
            var newPos = sprite.Position + direction * speed * dt;

            // перевірка по тайлах (як у тебе було)
            if (!CanMoveToTile(newPos, map, offset))
            {
                IsActive = false;
                return;
            }

            sprite.Position = newPos;

            // ————— Тепер справжня колізія з танком —————
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

        private bool CanMoveToTile(Vector2f newPos, int[,] map, Vector2f offset)
        {
            var gb = sprite.GetGlobalBounds();
            float w = gb.Width * sprite.Scale.X;
            float h = gb.Height * sprite.Scale.Y;

            float left = newPos.X - w / 2f;
            float top = newPos.Y - h / 2f;
            float right = left + w;
            float bottom = top + h;

            int x0 = (int)((left - offset.X) / tileSize);
            int y0 = (int)((top - offset.Y) / tileSize);
            int x1 = (int)((right - offset.X) / tileSize);
            int y1 = (int)((bottom - offset.Y) / tileSize);

            for (int yy = y0; yy <= y1; yy++)
                for (int xx = x0; xx <= x1; xx++)
                {
                    if (xx < 0 || yy < 0
                        || yy >= map.GetLength(0)
                        || xx >= map.GetLength(1)
                        || map[yy, xx] != 0)
                        return false;
                }
            return true;
        }
    }
}
