using k;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using System.Linq;

namespace k
{
    public class GameSession
    {
        private readonly TankGame game;

        public GameSession(Level level, string assetsPath, int playerCount)
        {
            level.CalculateOffset(1280, 720, 64);
            game = new TankGame(level.Map, assetsPath, playerCount);
        }

        public void Run(RenderWindow window)
        {
            var clock = new Clock();
            while (window.IsOpen && !game.IsGameOver())
            {
                var dt = clock.Restart();
                window.DispatchEvents();
                game.Update(dt, window);
                window.Clear();
                game.Draw(window);
                window.Display();
            }
        }

        public Dictionary<string, int> GetResults()
    => game.Entities
           .OfType<Tank>()
           .ToDictionary(t => t.Data.Color, t => t.Data.Score);

    }
}
//public Dictionary<string, int> GetResults()
//    => game.Entities
//           .OfType<Tank>()
//           .Where(t => !string.IsNullOrEmpty(t.Data.Color))
//           .ToDictionary(t => t.Data.Color, t => t.Data.Score);