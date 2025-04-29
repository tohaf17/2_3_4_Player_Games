using SFML.Graphics;
using SFML.System;
using System.IO;

namespace k
{
    public class MapRenderer
    {
        private readonly int[,] map;
        private readonly int tileSize;
        private Vector2f offset;

        private readonly Texture wallTex;
        private readonly Texture grayTex;
        private readonly Texture darkTex;
        private readonly Texture sideTex;

        public MapRenderer(int[,] map, int tileSize, Vector2f offset, string assetsPath)
        {
            this.map = map;
            this.tileSize = tileSize;
            this.offset = offset;

            assetsPath = "C:\\Users\\ADMIN\\OneDrive\\Desktop\\Course_Work\\bin\\Content";

            // Завантажуємо текстури один раз
            wallTex = new Texture(Path.Combine(assetsPath, "wall.png"));
            grayTex = new Texture(Path.Combine(assetsPath, "gray_block.png"));
            darkTex = new Texture(Path.Combine(assetsPath, "dark_block.png"));
            sideTex = new Texture(Path.Combine(assetsPath, "left_side_block.png"));
        }

        public void UpdateOffset(Vector2f newOffset)
        {
            offset = newOffset;
        }

        public void Draw(RenderWindow window)
        {
            // фон
            for (int y = 0; y < window.Size.Y; y += tileSize)
                for (int x = 0; x < window.Size.X; x += tileSize)
                    DrawTile(window, darkTex, x, y);

            // сама карта
            for (int y = 0; y < map.GetLength(0); y++)
                for (int x = 0; x < map.GetLength(1); x++)
                {
                    Texture tex = map[y, x] switch
                    {
                        1 => wallTex,
                        0 => grayTex,
                        2 or 3 or 4 or 5 => sideTex,
                        _ => grayTex
                    };
                    float rotation = map[y, x] == 3 ? 180f
                                    : map[y, x] == 4 ? 90f
                                    : map[y, x] == 5 ? 270f
                                    : 0f;
                    DrawTile(window, tex,
                        (int)(x * tileSize + offset.X),
                        (int)(y * tileSize + offset.Y),
                        rotation);
                }
        }

        private void DrawTile(RenderWindow window, Texture tex, int x, int y, float rotation = 0f)
        {
            var spr = new Sprite(tex)
            {
                Position = new Vector2f(x, y),
                Scale = new Vector2f(tileSize / (float)tex.Size.X, tileSize / (float)tex.Size.Y),
                Rotation = rotation,
                Origin = new Vector2f(tex.Size.X * 0.5f, tex.Size.Y * 0.5f)
            };
            window.Draw(spr);
        }
    }
}
