using System;
using System.Collections.Generic;
using System.Linq;

namespace k
{
    public class RandomMapSelector : IMapSelector
    {
        private readonly List<Level> allLevels;
        private readonly Random rnd = new();

        public RandomMapSelector(IEnumerable<Level> levels)
        {
            allLevels = levels.ToList();
            if (allLevels.Count < 3)
                throw new ArgumentException("Потрібно принаймні 3 карти");
        }

        public List<Level> SelectMaps(int count)
            => allLevels.OrderBy(_ => rnd.Next()).Take(count).ToList();
    }
}
