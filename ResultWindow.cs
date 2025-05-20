using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Collections.Generic;
using System.Linq; // Added for OrderByDescending

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

            window = new RenderWindow(new VideoMode(600, 500), "Tournament Results", Styles.Titlebar | Styles.Close); // Increased height for more space
            window.Closed += (_, __) => window.Close();
            window.SetFramerateLimit(60); // Limit framerate for smoother display
        }

        public void Show()
        {
            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear(new Color(50, 50, 60)); // Darker, slightly blue-gray background

                DrawTitle();
                DrawResults();

                window.Display();
            }
        }

        private void DrawTitle()
        {
            Text title = new Text("Tournament Results", font, 36)
            {
                FillColor = new Color(255, 220, 100) // Golden-yellow title
            };

            // Center the title horizontally
            FloatRect textRect = title.GetLocalBounds();
            title.Origin = new Vector2f(textRect.Left + textRect.Width / 2f, textRect.Top + textRect.Height / 2f);
            title.Position = new Vector2f(window.Size.X / 2f, 50); // Position at top, centered

            window.Draw(title);
        }

        private void DrawResults()
        {
            float y = 120; // Start drawing results lower to accommodate the title
            int rank = 1;

            // Sort results by score in descending order
            var sortedResults = results.OrderByDescending(kv => kv.Value);

            foreach (var kv in sortedResults)
            {
                Color playerColor;
                switch (kv.Key.ToLower()) // Determine color based on player name/color
                {
                    case "red":
                        playerColor = new Color(255, 100, 100); // Softer red
                        break;
                    case "blue":
                        playerColor = new Color(120, 180, 255); // Softer blue
                        break;
                    case "green":
                        playerColor = new Color(120, 255, 120); // Softer green
                        break;
                    case "yellow":
                        playerColor = new Color(255, 255, 120); // Softer yellow
                        break;
                    default:
                        playerColor = Color.White; // Default for others
                        break;
                }

                var text = new Text($"{rank}. {kv.Key}: {kv.Value} points", font, 28) // Slightly larger font, "points" for clarity
                {
                    FillColor = playerColor
                };

                // Center each result string
                FloatRect textRect = text.GetLocalBounds();
                text.Origin = new Vector2f(textRect.Left + textRect.Width / 2f, textRect.Top + textRect.Height / 2f);
                text.Position = new Vector2f(window.Size.X / 2f, y);

                window.Draw(text);
                y += 50; // Increased spacing between lines
                rank++;
            }
        }
    }
}