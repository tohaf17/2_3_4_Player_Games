using SFML.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

namespace k
{
    public class Tournament
    {
        private readonly IMapSelector selector;
        private readonly string assetsPath;
        private readonly int playerCount;

        public Tournament(IMapSelector selector, string assetsPath, int playerCount)
        {
            this.selector = selector;
            this.assetsPath = "C:\\Users\\ADMIN\\OneDrive\\Desktop\\Course_Work\\bin\\Content";
            this.playerCount = playerCount;
        }

        public void Start(RenderWindow window)
        {
            var levels = selector.SelectMaps(3);
            var overall = new Dictionary<string, int>();

            for (int i = 0; i < 1; i++)
            {
                var session = new GameSession(levels[i], assetsPath, playerCount);
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
