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
        private List<Dictionary<string, object>> gameHistory = new List<Dictionary<string, object>>(); // Змінюємо тип для зберігання різних типів даних

        public Tournament(string assetsPath, int playerCount)
        {
            this.assetsPath = @"C:\Users\ADMIN\OneDrive\Desktop\Course_Work\Content";
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
            DateTime endTime = DateTime.Now; // Фіксуємо час закінчення турніру

            for (int i = 0; i < 3; i++)
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

                // Серіалізуємо результати раунду разом з часом закінчення
                SerializeResults(roundResults, endTime);
            }

            Console.WriteLine("=== Final Results ===");
            foreach (var kv in overallResults)
                Console.WriteLine($"{kv.Key}: {kv.Value}");

            var resultWindow = new ResultWindow(overallResults, Path.Combine(assetsPath, "Lato-Regular.ttf"));
            resultWindow.Show();
        }

        private void SerializeResults(Dictionary<string, int> roundResults, DateTime endTime)
        {
            try
            {
                var record = new Dictionary<string, object>();
                foreach (var kvp in roundResults)
                {
                    record[kvp.Key] = kvp.Value;
                }
                record["EndTime"] = endTime.ToString("yyyy.MM.dd/HH:mm"); // Зберігаємо час як рядок
                gameHistory.Add(record);
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(gameHistory, options);
                File.WriteAllText(Path.Combine(assetsPath, ResultsFileName), jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при серіалізації результатів: {ex.Message}");
            }
        }
    }
}