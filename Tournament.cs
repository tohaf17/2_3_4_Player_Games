// Tournament.cs
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
        private List<(string, string)> levels = new();
        private List<Dictionary<string, object>> gameHistory = new List<Dictionary<string, object>>();

        public Tournament(int playerCount)
        {
            levels = new List<(string, string)>
            {
                (Path.Combine(AssetsPath, "gray_block.png"), Path.Combine(AssetsPath, "gray_wall.png")),
                (Path.Combine(AssetsPath, "yellow_block.png"), Path.Combine(AssetsPath, "yellow_wall.png")),
                (Path.Combine(AssetsPath, "blue_block.png"), Path.Combine(AssetsPath, "blue_wall.png"))
            };
            this.playerCount = playerCount;
            LoadGameHistory();
        }

        private void LoadGameHistory()
        {
            try
            {
                string filePath = Path.Combine(AssetsPath, ResultsFileName);
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
                Messanger.ShowMessage($"Помилка при {ex.Message}", "Error");
            }
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
                GameSession session = new GameSession(levels[i],playerCount, window);
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
                foreach (var kv in overallResults.OrderByDescending(p => p.Value)) 

                SerializeFinalResults(overallResults, endTime, true); 
            }
            else
            {
                
                SerializeFinalResults(overallResults, DateTime.Now, false); 
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
                record["EndTime"] = endTime.ToString("HH:mm");
                record["Completed"] = tournamentCompletedFully; 

                gameHistory.Add(record);
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(gameHistory, options);
                File.WriteAllText(Path.Combine(AssetsPath, ResultsFileName), jsonString);
            }
            catch (Exception ex)
            {
                Messanger.ShowMessage($"{ex.Message}", "Error");
            }
        }
    }
}