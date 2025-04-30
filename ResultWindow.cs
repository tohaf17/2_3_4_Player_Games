using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Collections.Generic;

namespace k
{
    public class ResultWindow
    {
        private readonly Dictionary<string, int> results;
        private readonly Font font;
        private readonly RenderWindow window;

        public ResultWindow(Dictionary<string, int> results, string fontPath)
        {
            this.results = results;
            font = new Font(fontPath);

            window = new RenderWindow(new VideoMode(600, 400), "Tournament Results", Styles.Titlebar | Styles.Close);
            window.Closed += (_, __) => window.Close();
        }

        public void Show()
        {
            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear(Color.Black);

                DrawResults();

                window.Display();
            }
        }

        private void DrawResults()
        {
            float y = 50;
            int index = 1;

            foreach (var kv in results)
            {
                var text = new Text($"{index}. {kv.Key} — {kv.Value} pts", font, 24)
                {
                    FillColor = Color.White,
                    Position = new Vector2f(50, y)
                };

                window.Draw(text);
                y += 40;
                index++;
            }
        }
    }
}
