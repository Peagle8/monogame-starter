namespace MyGame.Gameplay.Narrative;

public sealed class WeightedRandomSelector
{
    private readonly Random _random;

    public WeightedRandomSelector()
        : this(Random.Shared)
    {
    }

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

        var totalWeight = items.Sum(weightSelector);
        if (totalWeight <= 0)
        {
            throw new InvalidOperationException("Total weight must be greater than zero.");
        }

        var roll = _random.Next(1, totalWeight + 1);
        var runningWeight = 0;

        foreach (var item in items)
        {
            runningWeight += weightSelector(item);
            if (roll <= runningWeight)
            {
                return item;
            }
        }

        return items[^1];
    }
}
