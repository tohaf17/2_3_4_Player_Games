// Tournament.cs
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
            this.assetsPath = assetsPath;
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
            // overallResults містить накопичені очки за всі зіграні раунди
            var overallResults = new Dictionary<string, int>();
            bool tournamentCompletedFully = true; // Припускаємо, що турнір завершиться повністю

            // Ініціалізація гравців в overallResults з 0 очок.
            // Припускаємо, що кольори гравців відомі або можуть бути отримані.
            // Якщо у вас гравці мають унікальні ID, використовуйте їх.
            // Тут просто для прикладу, якщо у вас 2 гравці - "Red" та "Blue".
            // Адаптуйте цей блок під вашу логіку ініціалізації танків.
            var knownPlayerColors = new string[] { "Red", "Blue", "Green", "Yellow" };
            for (int p = 0; p < playerCount; p++)
            {
                if (p < knownPlayerColors.Length)
                {
                    overallResults[knownPlayerColors[p]] = 0;
                }
            }


            for (int i = 0; i < levels.Count; i++)
            {
                // Створюємо нову сесію для кожного раунду
                GameSession session = new GameSession(levels[i], assetsPath, playerCount, window);

                // Запускаємо сесію
                bool sessionCompletedNormally = session.Run(window);

                // Завжди отримуємо результати сесії після її завершення
                var roundResults = session.GetResults();

                Console.WriteLine($"--- Результати раунду {i + 1} ---");
                foreach (var kv in roundResults)
                {
                    // Додаємо результати раунду до загальних
                    overallResults[kv.Key] = overallResults.TryGetValue(kv.Key, out var currentScore) ? currentScore + kv.Value : kv.Value;
                    Console.WriteLine($"{kv.Key}: {kv.Value} (Всього: {overallResults[kv.Key]})");
                }

                if (!sessionCompletedNormally) // Якщо сесія була перервана (Escape або вікно закрито)
                {
                    Console.WriteLine("Турнір перервано користувачем або вікно закрито.");
                    tournamentCompletedFully = false; // Турнір НЕ завершено повністю

                    // Показуємо поточні загальні результати при перериванні
                    var intermediateResultWindow = new ResultWindow(overallResults, Path.Combine(assetsPath, "Lato-Regular.ttf"));
                    intermediateResultWindow.Show();

                    break; // Виходимо з циклу раундів
                }
            }

            // Цей блок виконується після завершення всіх раундів АБО після break
            if (tournamentCompletedFully) // Якщо цикл завершився, і він не був перерваний
            {
                var finalResultWindow = new ResultWindow(overallResults, Path.Combine(assetsPath, "Lato-Regular.ttf"));
                finalResultWindow.Show();

                DateTime endTime = DateTime.Now;
                Console.WriteLine("=== Фінальні результати турніру ===");
                foreach (var kv in overallResults.OrderByDescending(p => p.Value)) // Сортуємо за очками
                    Console.WriteLine($"{kv.Key}: {kv.Value}");

                SerializeFinalResults(overallResults, endTime, true); // Турнір завершено повністю
            }
            else
            {
                // Якщо турнір був перерваний, ми вже показали вікно з проміжними результатами вище.
                // Просто серіалізуємо поточні результати як неповний турнір.
                Console.WriteLine("Фінальні результати не були показані у вікні, оскільки турнір було перервано.");
                SerializeFinalResults(overallResults, DateTime.Now, false); // Турнір не завершено повністю
            }
        }

        private void SerializeFinalResults(Dictionary<string, int> finalResults, DateTime endTime, bool tournamentCompletedFully)
        {
            try
            {
                var record = new Dictionary<string, object>();
                foreach (var kvp in finalResults)
                {
                    record[kvp.Key] = kvp.Value;
                }
                record["EndTime"] = endTime.ToString("yyyy.MM.dd/HH:mm");
                record["Completed"] = tournamentCompletedFully; // Store whether it completed fully

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