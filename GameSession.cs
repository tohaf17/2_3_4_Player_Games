using k;
using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using System.Linq;
using SFML.Window; // Make sure this is included for MouseButtonEventArgs

namespace k
{
    public class GameSession
    {
        private readonly TankGame _game;
        private bool _isSessionOver = false;
        private RenderWindow _window; // Store the window reference

        public GameSession((string, string) level, string assetsPath,
                                int playerCount, RenderWindow window)
        {
            _window = window; // Assign the window reference
            _game = new TankGame(
                                     level,
                                     assetsPath,
                                     playerCount,
                                     window);

            // Subscribe to the KeyPressed event
            _window.KeyPressed += OnKeyPressed;
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Code == Keyboard.Key.Escape)
            {
                _isSessionOver = true; // Set the flag to end the session
            }
        }

        public void Run(RenderWindow window)
        {
            var clock = new Clock();

            while (window.IsOpen && !_game.IsGameOver() && !_isSessionOver)
            {
                window.DispatchEvents(); // Process events first

                var dt = clock.Restart();
                _game.Update(dt, window);

                window.Clear(Color.Black);
                _game.DrawMap(window);
                _game.DrawEntities(window);
                window.Display();
            }

            // Unsubscribe from the KeyPressed event to avoid memory leaks
            _window.KeyPressed -= OnKeyPressed;
        }

        public Dictionary<string, int> GetResults()
            => _game.Entities
                        .OfType<Tank>()
                        .ToDictionary(t => t.Data.Color, t => t.Data.Score);
    }
}