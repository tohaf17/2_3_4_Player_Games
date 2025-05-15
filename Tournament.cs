using SFML.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

namespace k
{
    public class Tournament
    {
        private readonly string assetsPath;
        private readonly int playerCount;
        private List<(string,string)> levels = new();

        public Tournament(string assetsPath, int playerCount)
        {
            this.assetsPath = @"C:\Users\ADMIN\OneDrive\Desktop\Course_Work\Content";
            levels = new List<(string, string)>
            {
                (Path.Combine(assetsPath, "gray_block.png"),Path.Combine(assetsPath,"gray_wall.png")),
                (Path.Combine(assetsPath, "yellow_block.png"),Path.Combine(assetsPath,"yellow_wall.png")),
                ("","")
            };
            this.playerCount = playerCount;
        }

        public void Start(RenderWindow window)
        {
            var overall = new Dictionary<string, int>();

            for (int i = 0; i < 2; i++)
            {
                var session = new GameSession(levels[i], assetsPath, playerCount,window);
                session.Run(window);

                var results = session.GetResults();
                Console.WriteLine($"--- Round {i + 1} Results ---");
                foreach (var kv in results)
                {
                    Console.WriteLine($"{kv.Key}: {kv.Value}");
                    overall[kv.Key] = overall.TryGetValue(kv.Key, out var v) ? v + kv.Value : kv.Value;
                }
            }

            Console.WriteLine("=== Final Results ===");
            foreach (var kv in overall)
                Console.WriteLine($"{kv.Key}: {kv.Value}");
            var resultWindow = new ResultWindow(overall, Path.Combine(assetsPath, "Lato-Regular.ttf"));
            resultWindow.Show();

        }
    }
}
