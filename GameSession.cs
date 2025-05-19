using k;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using System.Linq;

namespace k
{
    public class GameSession
    {
        private readonly TankGame _game;

        public GameSession((string,string) level, string assetsPath,
                           int playerCount, RenderWindow window)
        {
            _game = new TankGame(
                                 level,
                                 assetsPath,
                                 playerCount,
                                 window);
        }

        public void Run(RenderWindow window)
        {
            var clock = new Clock();

            while (window.IsOpen && !_game.IsGameOver())
            {
                var dt = clock.Restart();
                window.DispatchEvents();

                _game.Update(dt, window);

                window.Clear(Color.Black);

                _game.DrawMap(window);

                _game.DrawEntities(window);

                window.Display();
            }
        }




        public Dictionary<string, int> GetResults()
            => _game.Entities
                   .OfType<Tank>()
                   .ToDictionary(t => t.Data.Color, t => t.Data.Score);
    }


}