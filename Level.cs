using SFML.System;

namespace k
{
    public class Level
    {
        public int[,] Map { get; }
        public Vector2f Offset { get; private set; }

        public Level(int[,] map)
        {
            Map = map ?? throw new ArgumentNullException(nameof(map));
        }

        public void CalculateOffset(float windowWidth, float windowHeight, int tileSize)
        {
            float mapW = tileSize * Map.GetLength(1);
            float mapH = tileSize * Map.GetLength(0);
            Offset = new Vector2f(
                (windowWidth - mapW) * 0.5f,
                (windowHeight - mapH) * 0.5f
            );
        }
    }
}
