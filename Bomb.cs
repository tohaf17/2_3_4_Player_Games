using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace My_Game
{
    public class Bomb
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 direction;
        private float rotation;
        private float speed = 5f;
        private bool isActive = true;

        public Bomb(Texture2D texture, Vector2 startPosition, Vector2 direction, float rotation)
        {
            this.texture = texture;
            this.position = startPosition;
            this.direction = direction; // тепер напрямок передається прямо
            this.rotation = rotation;
        }


        private Vector2 mapOffset = new Vector2(500, 250); // зміщення з Draw

        public void Update(int[,] map, int tileSize, Tank[] tanks)
        {
            if (!isActive) return;

            Vector2 newPosition = position + direction * speed;

            // Враховуємо зміщення на екрані
            Vector2 checkPos = newPosition - mapOffset;

            if (!IsInsideMap(checkPos, map, tileSize) || CollidesWithMap(map, tileSize, checkPos))
            {
                isActive = false;
                return;
            }

            position = newPosition;

            foreach (var tank in tanks)
            {
                if (tank != null && Intersects(tank))
                {
                    tank.SetDestroyed();
                    isActive = false;
                    break;
                }
            }
        }

        private bool IsInsideMap(Vector2 pos, int[,] map, int tileSize)
        {
            int x = (int)pos.X / tileSize;
            int y = (int)pos.Y / tileSize;
            return x >= 0 && y >= 0 && y < map.GetLength(0) && x < map.GetLength(1);
        }

        private bool CollidesWithMap(int[,] map, int tileSize, Vector2 checkPos)
        {
            int tileX = (int)checkPos.X / tileSize;
            int tileY = (int)checkPos.Y / tileSize;
            return map[tileY, tileX] != 0;
        }


        private bool Intersects(Tank tank)
        {
            if (tank != null && tank.Texture != null)
            {
                Rectangle bombRect = new Rectangle((int)position.X, (int)position.Y, 16, 16);
                Rectangle tankRect = new Rectangle((int)tank.Position.X, (int)tank.Position.Y, 64, 64);
                return bombRect.Intersects(tankRect);
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isActive)
            {
                spriteBatch.Draw(
                    texture,
                    position,
                    null,
                    Color.White,
                    rotation + MathF.PI,
                    new Vector2(texture.Width / 2f, texture.Height / 2f),
                    0.25f,
                    SpriteEffects.None,
                    0f
                );

            }
        }

        public bool IsActive => isActive;
        public Vector2 Position => position;
    }
}
