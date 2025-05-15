using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;

namespace k
{
    public class MapRenderer
    {
        private const int tileSize = 64;
        private readonly Texture block;
        public readonly Texture wall;
        private readonly int count = 10;
        public List<Vector2i> WallPositions { get; set; } = new();
        public List<Sprite> spritesWall { get; set; } = new();

        private bool generated = false;

        public MapRenderer((string, string) level_textures)
        {
            block = new Texture(level_textures.Item1);
            wall = new Texture(level_textures.Item2);
        }

        public void Draw(RenderWindow window)
        {
            // Генеруємо стіни лише один раз, коли вже є розмір вікна
            if (!generated)
            {
                GenerateWalls(window.Size);
                generated = true;
            }

            // Малюємо блоки фону
            for (int y = 0; y < window.Size.Y; y += tileSize)
                for (int x = 0; x < window.Size.X; x += tileSize)
                    DrawTile(window, block, x, y);

            // Малюємо стіни
            foreach (var pos in WallPositions)
                DrawTile(window, wall, pos.X, pos.Y);
        }

        private void GenerateWalls(Vector2u windowSize)
        {
            Random random = new Random();
            WallPositions.Clear();
            for (int i = 0; i < count; i++)
            {
                int x = random.Next(0, (int)(windowSize.X / tileSize)) * tileSize;
                int y = random.Next(0, (int)(windowSize.Y / tileSize)) * tileSize;
                WallPositions.Add(new Vector2i(x, y));
                spritesWall.Add(new Sprite(wall)
                {
                    Position = new Vector2f(x, y),
                    Scale = new Vector2f(tileSize / (float)wall.Size.X, tileSize / (float)wall.Size.Y),
                    Rotation = 0f,
                    Origin = new Vector2f(0, 0)
                });
            }

        }

        private void DrawTile(RenderWindow window, Texture tex, int x, int y, float rotation = 0f)
        {
            var spr = new Sprite(tex)
            {
                Position = new Vector2f(x, y),
                Scale = new Vector2f(tileSize / (float)tex.Size.X, tileSize / (float)tex.Size.Y),
                Rotation = rotation,
                Origin = new Vector2f(0, 0)
            };
            window.Draw(spr);
        }
    }
}
