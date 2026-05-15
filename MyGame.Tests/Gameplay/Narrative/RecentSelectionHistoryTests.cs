using MyGame.Gameplay.Narrative;

namespace MyGame.Tests.Gameplay.Narrative;

public sealed class RecentSelectionHistoryTests
{
    [Fact]
    public void Record_WhenHistoryExceedsLimit_DropsOldestId()
    {
        var history = new RecentSelectionHistory(maxItemsPerSystem: 2);

        history.Record("Dialogue", "one");
        history.Record("Dialogue", "two");
        history.Record("Dialogue", "three");

        Assert.False(history.WasRecentlyUsed("Dialogue", "one"));
        Assert.True(history.WasRecentlyUsed("Dialogue", "two"));
        Assert.True(history.WasRecentlyUsed("Dialogue", "three"));
    }

    [Fact]
    public void Replace_UsesMostRecentIdsWithinLimit()
    {
        var history = new RecentSelectionHistory(maxItemsPerSystem: 2);

        history.Replace("Dialogue", ["one", "two", "three"]);

        Assert.Equal(["two", "three"], history.GetRecentIds("Dialogue"));
    }
}
