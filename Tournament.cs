using SFML.Graphics;
using System;
using static k.Constants;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;

namespace k
{
    public class Tournament
    {
        private readonly int playerCount;
        private GameSession session;
        private List<(string, string)> levels = new();

        public Tournament(int playerCount)
        {
            levels = new List<(string, string)>
            {
                (Path.Combine(AssetsPath, "gray_block.png"), Path.Combine(AssetsPath, "gray_wall.png")),
                (Path.Combine(AssetsPath, "yellow_block.png"), Path.Combine(AssetsPath, "yellow_wall.png")),
                (Path.Combine(AssetsPath, "blue_block.png"), Path.Combine(AssetsPath, "blue_wall.png"))
            };
            this.playerCount = playerCount;
        }

        public void Start(RenderWindow window)
        {
            var overallResults = new Dictionary<string, int>();
            bool tournamentCompletedFully = true;

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
                session = new GameSession(levels[i], playerCount, window);
                bool sessionCompletedNormally = session.Run(window);
                var roundResults = session.GetResults();

                foreach (var kv in roundResults)
                {
                    overallResults[kv.Key] = overallResults.TryGetValue(kv.Key, out var currentScore) ? currentScore + kv.Value : kv.Value;
                }

                if (!sessionCompletedNormally)
                {
                    tournamentCompletedFully = false;
                    var intermediateResultWindow = new ResultWindow(overallResults, Path.Combine(AssetsPath, "Lato-Regular.ttf"));
                    intermediateResultWindow.Show();
                    break;
                }
            }

            if (tournamentCompletedFully)
            {
                var finalResultWindow = new ResultWindow(overallResults, k.Constants.Font);
                finalResultWindow.Show();

                DateTime endTime = DateTime.Now;

                SaveGameResult(overallResults, endTime, true);
            }
            else
            {
                SaveGameResult(overallResults, DateTime.Now, false);
            }
        }

        private void SaveGameResult(Dictionary<string, int> finalResults, DateTime endTime, bool tournamentCompletedFully)
        {
            try
            {
                string filePath = Path.Combine(AssetsPath, ResultsFileName);
                List<Dictionary<string, object>> currentHistory = new List<Dictionary<string, object>>();

                if (File.Exists(filePath))
                {
                    string jsonText = File.ReadAllText(filePath);
                    if (!string.IsNullOrWhiteSpace(jsonText) && jsonText != "[]")
                    {
                        var loadedData = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(jsonText);
                        if (loadedData != null)
                        {
                            currentHistory = loadedData.Select(dict => dict.ToDictionary(pair => pair.Key, pair => (object)pair.Value)).ToList();
                        }
                    }
                }

                var record = new Dictionary<string, object>();
                foreach (var kvp in finalResults)
                {
                    record[kvp.Key] = kvp.Value;
                }
                record["EndTime"] = endTime.ToString("HH:mm");
                record["Completed"] = tournamentCompletedFully;

                currentHistory.Add(record);
                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonString = JsonSerializer.Serialize(currentHistory, options);
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                Messanger.ShowMessage($"Помилка при збереженні результатів: {ex.Message}", "Error");
            }
        }
    }
}