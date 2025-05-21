using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k
{
    static class Constants
    {
        public const float SpeedTank = 400f;
        public const float DampingTank = 0.80f;
        public const float RotationSpeedTank = 200f;
        public const float PushStrengthTank = 8f;
        public const int TileSize = 64;

        public const float SpeedBomb = 150f;

        public const int WallCount = 20;
        public const int BoxCount = 5;

        public const float ScaleNumber = 0.5f;

        public const string AssetsPath = @"C:\Users\ADMIN\OneDrive\Desktop\Course_Work\Content";
        public const string Font= @"C:\Users\ADMIN\OneDrive\Desktop\Course_Work\Content\Lato-Regular.ttf";
        public const string ResultsFileName = "tournament_results.json";
    }
}
