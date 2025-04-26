using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;

namespace k
{
    public class Bomb : GameEntity
    {
        private Sprite sprite;
        private Vector2f direction;
        private float speed = 150f;
        public bool IsActive { get; private set; } = true;
        public Vector2f Position => sprite.Position;
        private int tileSize;

        private List<Tank> tanks = new List<Tank>();
        private Tank owner;

        public Bomb(Texture texture, Vector2f startPosition, Vector2f direction, float rotation,Tank owner)
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
            tileSize = 64;
        }

        public override void Update(Time delta, List<GameEntity> entities, int[,] map)
        {
            if (!IsActive) return;

            tanks = entities.OfType<Tank>().ToList();

            Vector2f newPosition = sprite.Position + direction * speed * delta.AsSeconds();
            Vector2f checkPos = newPosition - new Vector2f(500, 250);

            if (!IsInsideMap(checkPos, map, tileSize) || CollidesWithMap(map, tileSize, checkPos))
            {
                IsActive = false;
                return;
            }

            sprite.Position = newPosition;

            foreach (var tank in tanks)
            {
                if (tank != null && Intersects(tank) && tank != owner)
                {
                    tank.TakeDamage();
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

        private bool IsInsideMap(Vector2f pos, int[,] map, int tileSize)
        {
            int x = (int)(pos.X / tileSize);
            int y = (int)(pos.Y / tileSize);
            return x >= 0 && y >= 0 && y < map.GetLength(0) && x < map.GetLength(1);
        }

        private bool CollidesWithMap(int[,] map, int tileSize, Vector2f checkPos)
        {
            int tileX = (int)(checkPos.X / tileSize);
            int tileY = (int)(checkPos.Y / tileSize);
            return map[tileY, tileX] != 0;
        }

        private bool Intersects(Tank tank)
        {
            
                FloatRect bombRect = new FloatRect(Position.X, Position.Y, 16, 16);
                FloatRect tankRect = new FloatRect(tank.Position.X - 32, tank.Position.Y - 32, 64, 64);
                return bombRect.Intersects(tankRect);
            
        }
    }
}
