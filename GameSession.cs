// GameSession.cs
using k;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using System.Linq;
using SFML.Window;
using System;
using System.IO; // Додано для Path.Combine

namespace k
{
    public class GameSession
    {
        private readonly TankGame _game;

        public GameSession((string, string) level, string assetsPath, int playerCount, RenderWindow window)
        {
            _game = new TankGame(level, assetsPath, playerCount, window);
        }

        // Returns true if session completed normally (game over condition met)
        // Returns false if session was interrupted (Escape pressed or window closed)
        public bool Run(RenderWindow window)
        {
            var clock = new Clock();

            while (window.IsOpen && !_game.IsGameOver())
            {
                window.DispatchEvents();

                // Обробка натискання клавіші Escape
                if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
                {
                    Console.WriteLine("Escape натиснуто. Сесія перервана.");
                    return false; // Сесія перервана
                }

                var dt = clock.Restart();
                _game.Update(dt, window);

                window.Clear(new Color(40, 40, 40));
                _game.DrawMap(window);
                _game.DrawEntities(window);
                window.Display();
            }

            // Якщо вікно було закрито користувачем (не Escape), повертаємо false
            if (!window.IsOpen)
            {
                Console.WriteLine("Вікно гри закрито під час сесії.");
                return false;
            }

            // Якщо гра завершилася (IsGameOver() == true)
            Console.WriteLine("Гра завершилася звичайним шляхом.");
            return true; // Сесія завершена нормально
        }

        public Dictionary<string, int> GetResults()
            => _game.Entities
                     .OfType<Tank>()
                     .ToDictionary(t => t.Data.Color, t => t.Data.Score);
    }
}