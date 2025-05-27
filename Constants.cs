using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k
{
    static class Constants
    {
        public const float SpeedTank = 7000f;
        public const float DampingTank = 0.9f;
        public const float RotationSpeedTank = 250f;
        public const float PushStrengthTank = 0.9f;
        public const int TileSize = 64;
        public const float BombDelay = 5f;

        public const float CircleRadius = 32f;
        public const float CircleThickness = 2f;
        public const float SpeedBomb = 300f;

        public const int WallCount = 15;
        public const int BoxCount = 5;

        public const float ScaleNumber = 0.5f;
        public const int AlphaLimit = 10;

        public const string AssetsPath = @"C:\Users\ADMIN\OneDrive\Desktop\Course_Work\Content";
        public const string Font= @"C:\Users\ADMIN\OneDrive\Desktop\Course_Work\Content\Lato-Regular.ttf";
        public const string ResultsFileName = "tournament_results.json";
    }
}
