using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace k
{
    public class MapRenderer
    {
        private const int tileSize = 64;
        private readonly Texture block;
        public readonly Texture wall;
        private readonly Texture boxTexture;
        private readonly int wallCount = 10;
        private readonly int boxCount = 5;

        public List<Vector2i> WallPositions { get; set; } = new();
        public List<Vector2i> BoxPositions { get; set; } = new();
        public List<Sprite> spritesWall { get; set; } = new();
        public List<Sprite> spritesBox { get; set; } = new();


        private bool generated = false;
        private Sprite buttonSprite = null;
        private Vector2i buttonPosition;

        public event EventHandler ButtonClicked;

        public MapRenderer((string, string) level_textures, string boxPath)
        {
            block = new Texture(level_textures.Item1);
            wall = new Texture(level_textures.Item2);
            boxTexture = new Texture(Path.Combine(boxPath, "box.png"));
        }

        public void Draw(RenderWindow window)
        {
            if (!generated)
            {
                GenerateWalls(window.Size);
                GenerateBoxes(window.Size);
                generated = true;
            }

            for (int y = 0; y < window.Size.Y; y += tileSize)
                for (int x = 0; x < window.Size.X; x += tileSize)
                    DrawTile(window, block, x, y);

            foreach (var sprite in spritesWall)
            {
                window.Draw(sprite);
            }

            foreach (var pos in BoxPositions)
            {
                var boxSprite = new Sprite(boxTexture)
                {
                    Position = new Vector2f(pos.X, pos.Y)
                };
                window.Draw(boxSprite);
            }

            if (buttonSprite != null)
            {
                window.Draw(buttonSprite);
            }
        }

        private void GenerateWalls(Vector2u windowSize)
        {
            Random random = new Random();
            WallPositions.Clear();
            spritesWall.Clear();

            for (int i = 0; i < wallCount; i++)
            {
                int x = random.Next(0, (int)(windowSize.X / tileSize)) * tileSize;
                int y = random.Next(0, (int)(windowSize.Y / tileSize)) * tileSize;
                Vector2i pos = new Vector2i(x, y);

                if (!WallPositions.Contains(pos))
                {
                    WallPositions.Add(pos);
                    spritesWall.Add(new Sprite(wall)
                    {
                        Position = new Vector2f(x, y),
                        Scale = new Vector2f(tileSize / (float)wall.Size.X, tileSize / (float)wall.Size.Y),
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
            spritesBox.Clear();

            int attempts = 0;
            while (BoxPositions.Count < boxCount && attempts < 1000)
            {
                attempts++;
                int x = random.Next(0, (int)(windowSize.X / tileSize)) * tileSize;
                int y = random.Next(0, (int)(windowSize.Y / tileSize)) * tileSize;
                Vector2i pos = new Vector2i(x, y);

                if (!WallPositions.Contains(pos) && !BoxPositions.Contains(pos))
                {
                    BoxPositions.Add(pos);
                    spritesBox.Add(new Sprite(boxTexture)
                    {
                        Position = new Vector2f(x, y),
                        Scale = new Vector2f(tileSize / (float)boxTexture.Size.X, tileSize / (float)boxTexture.Size.Y),
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
                Scale = new Vector2f(tileSize / (float)tex.Size.X, tileSize / (float)tex.Size.Y),
                Rotation = rotation,
                Origin = new Vector2f(0, 0)
            };
            window.Draw(spr);
        }

      
    }

}