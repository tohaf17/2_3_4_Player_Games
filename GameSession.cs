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
            //level.CalculateOffset(1200, 720, 64);
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

                // 1) очищаємо бека
                window.Clear(Color.Black);

                // 2) малюємо кеш-фон
                _game.DrawMap(window);

                // 3) малюємо танки/бомби зверху
                _game.DrawEntities(window);

                // 4) показуємо кадр
                window.Display();
            }
        }




        public Dictionary<string, int> GetResults()
            => _game.Entities
                   .OfType<Tank>()
                   .ToDictionary(t => t.Data.Color, t => t.Data.Score);
    }


}