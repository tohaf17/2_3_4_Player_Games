using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Collections.Generic;
using System.Linq; 

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

            window = new RenderWindow(new VideoMode(600, 500), "Tournament Results", Styles.Titlebar | Styles.Close);
            window.Closed += (_, __) => window.Close();
            
        }

        public void Show()
        {
            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear(new Color(50, 50, 60)); 

                DrawTitle();
                DrawResults();

                window.Display();
            }
        }

        private void DrawTitle()
        {
            Text title = new Text("Tournament Results", font, 36)
            {
                FillColor = new Color(255, 220, 100) 
            };

            FloatRect textRect = title.GetLocalBounds();
            title.Origin = new Vector2f(textRect.Left + textRect.Width / 2f, textRect.Top + textRect.Height / 2f);
            title.Position = new Vector2f(window.Size.X / 2f, 50); 

            window.Draw(title);
        }

        private void DrawResults()
        {
            float y = 120;
            int rank = 1;

            var sortedResults = results.OrderByDescending(kv => kv.Value);

            foreach (var kv in sortedResults)
            {
                Color playerColor;
                switch (kv.Key.ToLower()) 
                {
                    case "red":
                        playerColor = new Color(255, 100, 100); 
                        break;
                    case "blue":
                        playerColor = new Color(120, 180, 255);
                        break;
                    case "green":
                        playerColor = new Color(120, 255, 120);
                        break;
                    case "yellow":
                        playerColor = new Color(255, 255, 120); 
                        break;
                    default:
                        playerColor = Color.White; 
                        break;
                }

                var text = new Text($"{rank}. {kv.Key}: {kv.Value} points", font, 28) 
                {
                    FillColor = playerColor
                };
                
                FloatRect textRect = text.GetLocalBounds();
                text.Origin = new Vector2f(textRect.Left + textRect.Width / 2f, textRect.Top + textRect.Height / 2f);
                text.Position = new Vector2f(window.Size.X / 2f, y);

                window.Draw(text);
                y += 50; 
                rank++;
            }
        }
    }
}