using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;

namespace k
{
    public class Bomb : GameEntity
    {
        private Sprite sprite;
        private Vector2f direction;
        private float speed = 150f;
        private int tileSize;
        private Vector2f offset;
        private Tank owner;

        public bool IsActive { get; private set; } = true;
        public Vector2f Position => sprite.Position;

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

            Vector2f newPos = sprite.Position + direction * speed * dt;

            if (!CanMoveTo(newPos, map))
            {
                IsActive = false;
                return;
            }

            sprite.Position = newPos;

            var hit = entities
                .OfType<Tank>()
                .FirstOrDefault(t => t != owner && PixelPerfectCollision.Test(
                    sprite, PixelPerfectCollision.CreateMask(sprite.Texture),
                    t.Sprite, t.CollisionMask,
                    alphaLimit: 10
                ));

            if (hit != null)
            {
                hit.TakeDamage();
                IsActive = false;
            }
        }
        private bool IntersectsAABB(Tank t)
        {
            var bombRect = sprite.GetGlobalBounds();
            var tankRect = new FloatRect(
                t.Position.X - 32, t.Position.Y - 32,
                64, 64
            );
            return bombRect.Intersects(tankRect);
        }


        public override void Draw(RenderWindow window)
        {
            if (IsActive)
                window.Draw(sprite);
        }

        // перевірка колізії з картою
        private bool CanMoveTo(Vector2f newPos, int[,] map)
        {
            var gb = sprite.GetGlobalBounds();
            float w = gb.Width, h = gb.Height;

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

        // перевірка колізії з танками (AABB + pixel-perfect)
        private bool CollidesWithAnyTank(Vector2f testPos, List<GameEntity> entities)
        {
            var old = sprite.Position;
            sprite.Position = testPos;

            bool hit = entities.OfType<Tank>()
                               .Where(t => t != owner)
                               .Any(t => PixelPerfectCollision.Test(
                                    sprite, PixelPerfectCollision.CreateMask(sprite.Texture),
                                    t.Sprite, t.CollisionMask,
                                    alphaLimit: 10));

            sprite.Position = old;
            return hit;
        }

        // простий AABB-перевірка центру бомби на влучання в танк
        private bool Intersects(Tank t)
        {
            var bombRect = sprite.GetGlobalBounds();
            var tankRect = new FloatRect(
                t.Position.X - 32, t.Position.Y - 32,
                64, 64
            );
            return bombRect.Intersects(tankRect);
        }
    }
}
