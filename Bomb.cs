using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace My_Game
{
    public class Bomb
    {
        private Texture2D texture;
        private Vector2 position;
        private Vector2 direction;
        private float speed = 5f;
        private bool isActive = true;
        private Texture2D explosionTexture;
        private List<Explosion> explosions = new List<Explosion>();

        public Bomb(Texture2D texture, Vector2 startPosition, float rotation, Texture2D explosionTexture) // Додаємо виклик конструктора базового класу
        {
            this.texture = texture;
            this.position = startPosition;
            this.direction = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
            this.explosionTexture = explosionTexture;
        }

        public void Update(int[,] map, int tileSize, Tank[] tanks, GameTime gameTime)
        {
            if (!isActive)
            {
                UpdateExplosions(gameTime);
                return;
            }

            position += direction * speed;

            if (CollidesWithMap(map, tileSize))
            {
                CreateExplosion();
                isActive = false;
            }

            foreach (var tank in tanks)
            {
                if (Intersects(tank))
                {
                    tank.SetDestroyed();
                    CreateExplosion();
                    isActive = false;
                    break;
                }
            }
            UpdateExplosions(gameTime);
        }

        private void CreateExplosion()
        {
            explosions.Add(new Explosion(explosionTexture, position));
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update(gameTime);
                if (!explosions[i].IsActive)
                {
                    explosions.RemoveAt(i);
                }
            }
        }

        private bool CollidesWithMap(int[,] map, int tileSize)
        {
            int tileX = (int)position.X / tileSize;
            int tileY = (int)position.Y / tileSize;

            // Check if tileX and tileY are within the map bounds
            if (tileX >= 0 && tileX < map.GetLength(1) && tileY >= 0 && tileY < map.GetLength(0))
            {
                return map[tileY, tileX] != 0;
            }
            else
            {
                // Handle out-of-bounds cases. You can either:
                // 1. Return true (treat as collision)
                // 2. Return false (treat as no collision)
                // 3. Throw an exception (for debugging)
                return true; // Or false, depending on your game's logic
            }
        }

        private bool Intersects(Tank? tank)
        {
            if (tank != null && tank.Texture != null) // Check if tank.Texture is not null
            {
                Rectangle bombRect = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
                Rectangle tankRect = new Rectangle((int)tank.Position.X, (int)tank.Position.Y, tank.Texture.Width, tank.Texture.Height);

                return bombRect.Intersects(tankRect);
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (isActive)
            {
                spriteBatch.Draw(texture, position, Color.White);
            }
            foreach (var explosion in explosions)
            {
                explosion.Draw(spriteBatch);
            }
        }

        public bool IsActive => isActive || explosions.Count > 0;
    }
}