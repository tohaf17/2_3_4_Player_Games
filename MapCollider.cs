using SFML.Graphics;
using SFML.System;

namespace k
{
    public class MapCollider
    {
        public int[,] Map { get; }
        public Vector2f Offset { get; set; }
        public int tileSize { get; }

        public MapCollider(int[,] map, Vector2f offset, int tileSize=64)
        {
            Map = map;
            Offset = offset;
            tileSize = tileSize;
        }

        public bool Collides(Sprite sprite, byte[] mask, Vector2f testPos)
        {
            var gb = sprite.GetGlobalBounds();
            float w = gb.Width, h = gb.Height;
            float left = testPos.X - w / 2f;
            float top = testPos.Y - h / 2f;
            float right = left + w;
            float bottom = top + h;

            int x0 = (int)((left - Offset.X) / 64);
            int x1 = (int)((right - Offset.X) / 64);
            int y0 = (int)((top - Offset.Y) / 64);
            int y1 = (int)((bottom - Offset.Y) / 64);

            for (int y = y0; y <= y1; y++)
                for (int x = x0; x <= x1; x++)
                {
                    if (x < 0 || y < 0
                     || y >= Map.GetLength(0)
                     || x >= Map.GetLength(1)
                     || Map[y, x] != 0)
                        return true;
                }
            return false;
        }
    }
}
