using SFML.Graphics;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;

namespace k
{
    public class Tournament
    {
        private readonly string assetsPath;
        private readonly int playerCount;
        private List<(string, string)> levels = new();
        private const string ResultsFileName = "tournament_results.json";
        private List<Dictionary<string, object>> gameHistory = new List<Dictionary<string, object>>();

        public Tournament(string assetsPath, int playerCount)
        {
            this.assetsPath = assetsPath; // Use the passed assetsPath
            levels = new List<(string, string)>
            {
                (Path.Combine(assetsPath, "gray_block.png"), Path.Combine(assetsPath, "gray_wall.png")),
                (Path.Combine(assetsPath, "yellow_block.png"), Path.Combine(assetsPath, "yellow_wall.png")),
                (Path.Combine(assetsPath, "blue_block.png"), Path.Combine(assetsPath, "blue_wall.png"))
            };
            this.playerCount = playerCount;
            LoadGameHistory();
        }

        private void LoadGameHistory()
        {
            try
            {
                string filePath = Path.Combine(assetsPath, ResultsFileName);
                if (File.Exists(filePath))
                {
                    string jsonString = File.ReadAllText(filePath);
                    var historyData = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(jsonString);
                    if (historyData != null)
                    {
                        gameHistory = historyData;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при завантаженні історії: {ex.Message}");
            }
        }

        public void Start(RenderWindow window)
        {
            var overallResults = new Dictionary<string, int>();

            for (int i = 0; i < 3; i++) // You might want to adjust this loop if a session can end early
            {
                var session = new GameSession(levels[i], assetsPath, playerCount, window);
                session.Run(window);

                var roundResults = session.GetResults();
                Console.WriteLine($"--- Round {i + 1} Results ---");
                foreach (var kv in roundResults)
                {
                    Console.WriteLine($"{kv.Key}: {kv.Value}");
                    overallResults[kv.Key] = overallResults.TryGetValue(kv.Key, out var v) ? v + kv.Value : kv.Value;
                }

                // If a session ends prematurely (e.g., by Esc), you might want to break the tournament loop
                // You'll need a way for GameSession to report if it ended due to Esc or game over
                // For now, it will just proceed to the next round or end the tournament.
                if (!window.IsOpen) break; // If the window was closed during a session
            }

            var resultWindow = new ResultWindow(overallResults, Path.Combine(assetsPath, "Lato-Regular.ttf"));
            resultWindow.Show();
            DateTime endTime = DateTime.Now;

            Console.WriteLine("=== Final Results ===");
            foreach (var kv in overallResults)
                Console.WriteLine($"{kv.Key}: {kv.Value}");

            SerializeFinalResults(overallResults, endTime);
        }

        private void SerializeFinalResults(Dictionary<string, int> finalResults, DateTime endTime)
        {
            try
            {
                var record = new Dictionary<string, object>();
                foreach (var kvp in finalResults)
                {
                    record[kvp.Key] = kvp.Value;
                }
                record["EndTime"] = endTime.ToString("yyyy.MM.dd/HH:mm");
                gameHistory.Add(record);
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(gameHistory, options);
                File.WriteAllText(Path.Combine(assetsPath, ResultsFileName), jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при серіалізації фінальних результатів: {ex.Message}");
            }
        }
    }
}