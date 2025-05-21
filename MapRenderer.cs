using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using static k.Constants;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace k
{
    public class MapRenderer
    {
        private readonly Texture block;
        public readonly Texture wall;
        private readonly Texture box;

        public List<Vector2i> WallPositions { get; set; } = new();
        public List<Vector2i> BoxPositions { get; set; } = new();

        public List<Sprite> SpritesWall { get; set; } = new();
        public List<Sprite> SpritesBox { get; set; } = new();


        private bool generated = false;

        public MapRenderer((string, string) level_textures)
        {
            block = new Texture(level_textures.Item1);
            wall = new Texture(level_textures.Item2);
            box = new Texture(Path.Combine(AssetsPath, "box.png"));
        }

        public void Draw(RenderWindow window)
        {
            if (!generated)
            {
                GenerateWalls(window.Size);
                GenerateBoxes(window.Size);
                generated = true;
            }

            for (int y = 0; y < window.Size.Y; y += TileSize)
                for (int x = 0; x < window.Size.X; x += TileSize)
                    DrawTile(window, block, x, y);

            foreach (var sprite in SpritesWall)
            {
                window.Draw(sprite);
            }

            foreach (var pos in BoxPositions)
            {
                var boxSprite = new Sprite(box)
                {
                    Position = new Vector2f(pos.X, pos.Y)
                };
                window.Draw(boxSprite);
            }

        }

        private void GenerateWalls(Vector2u windowSize)
        {
            Random random = new Random();
            WallPositions.Clear();
            SpritesWall.Clear();

            for (int i = 0; i < WallCount; i++)
            {
                int x = random.Next(0, (int)(windowSize.X / TileSize)) * TileSize;
                int y = random.Next(0, (int)(windowSize.Y / TileSize)) * TileSize;
                Vector2i pos = new Vector2i(x, y);

                if (!WallPositions.Contains(pos))
                {
                    WallPositions.Add(pos);
                    SpritesWall.Add(new Sprite(wall)
                    {
                        Position = new Vector2f(x, y),
                        Scale = new Vector2f(TileSize / (float)wall.Size.X, TileSize / (float)wall.Size.Y),
                        Rotation = 0f,
                        Origin = new Vector2f(0, 0)
                    });
                }
            }
        }

        private void GenerateBoxes(Vector2u windowSize)
        {
            Random random = new Random();
            BoxPositions.Clear();
            SpritesBox.Clear();

            int attempts = 0;
            while (BoxPositions.Count < BoxCount && attempts < 1000)
            {
                attempts++;
                int x = random.Next(0, (int)(windowSize.X / TileSize)) * TileSize;
                int y = random.Next(0, (int)(windowSize.Y / TileSize)) * TileSize;
                Vector2i pos = new Vector2i(x, y);

                if (!WallPositions.Contains(pos) && !BoxPositions.Contains(pos))
                {
                    BoxPositions.Add(pos);
                    SpritesBox.Add(new Sprite(box)
                    {
                        Position = new Vector2f(x, y),
                        Scale = new Vector2f(TileSize / (float)wall.Size.X, TileSize / (float)wall.Size.Y),
                        Rotation = 0f,
                        Origin = new Vector2f(0, 0)
                    });
                }
            }
        }

        private void DrawTile(RenderWindow window, Texture tex, int x, int y, float rotation = 0f)
        {
            var spr = new Sprite(tex)
            {
                Position = new Vector2f(x, y),
                Scale = new Vector2f(TileSize / (float)tex.Size.X, TileSize / (float)tex.Size.Y),
                Rotation = rotation,
                Origin = new Vector2f(0, 0)
            };
            window.Draw(spr);
        }

      
    }

}