using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonDrivenGameSample.Gameplay.Dialogue
{
    public sealed class WeightedRandomSelector
    {
        private readonly Random _random;

        public WeightedRandomSelector(Random random)
        {
            _random = random;
        }

        public T Select<T>(IReadOnlyList<T> items, Func<T, int> weightSelector)
        {
            if (items.Count == 0)
            {
                throw new InvalidOperationException("Cannot select from an empty list.");
            }

            int totalWeight = items.Sum(weightSelector);
            if (totalWeight <= 0)
            {
                throw new InvalidOperationException("Total weight must be greater than zero.");
            }

            int roll = _random.Next(1, totalWeight + 1);
            int running = 0;

            foreach (T item in items)
            {
                running += weightSelector(item);
                if (roll <= running)
                {
                    return item;
                }
            }

            return items[^1];
        }
    }
}
