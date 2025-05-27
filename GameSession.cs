
using k;
using SFML.Graphics;
using SFML.System;
using static k.Constants;
using SFML.Window;

namespace k
{
    public class GameSession
    {
        private readonly TankGame game;

        public GameSession((string, string) level, int playerCount, RenderWindow window)
        {
            game = new TankGame(level,playerCount, window);
        }

        public bool Run(RenderWindow window)
        {
            var clock = new Clock();

            while (window.IsOpen && !game.IsGameOver())
            {
                window.DispatchEvents();

                if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
                {
                    return false; 
                }

                var dt = clock.Restart();
                game.Update(dt, window);

                window.Clear(new Color(40, 40, 40));
                game.DrawMap(window);
                game.DrawEntities(window);
                window.Display();
            }

            if (!window.IsOpen)
            {
                return false;
            }
            return true;
        }

        public Dictionary<string, int> GetResults()
            => game.Entities
                     .OfType<Tank>()
                     .ToDictionary(t => t.Data.Color, t => t.Data.Score);
    }
}