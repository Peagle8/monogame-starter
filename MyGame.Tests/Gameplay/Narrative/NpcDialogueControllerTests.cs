using Microsoft.Xna.Framework;
using MyGame.Gameplay.Narrative;
using MyGame.Gameplay.Props;

namespace MyGame.Tests.Gameplay.Narrative;

public sealed class NpcDialogueControllerTests
{
    private readonly NpcDialogueController _controller = new();
    private readonly NpcDialogueService _service = CreateService();
    private readonly NarrativeState _narrativeState = new();
    private readonly RecentSelectionHistory _history = new();

    [Fact]
    public void Update_WhenPlayerIsNearConversationProp_ShowsPrompt()
    {
        var state = _controller.Update(
            NpcDialogueState.Default,
            new Rectangle(100, 116, 32, 32),
            [CreateTownsfolk()],
            interactJustPressed: false,
            confirmJustPressed: false,
            cancelJustPressed: false,
            _service,
            hintService: null,
            NarrativeIds.ZoneOverworld,
            _narrativeState,
            _history);

        Assert.True(state.IsPromptVisible);
        Assert.False(state.IsOpen);
        Assert.Equal(NarrativeIds.SpeakerTownsfolkOne, state.SpeakerId);
    }

    [Fact]
    public void Update_WhenInteractPressedNearConversationProp_OpensSelectedDialogue()
    {
        var state = _controller.Update(
            NpcDialogueState.Default,
            new Rectangle(100, 116, 32, 32),
            [CreateTownsfolk()],
            interactJustPressed: true,
            confirmJustPressed: false,
            cancelJustPressed: false,
            _service,
            hintService: null,
            NarrativeIds.ZoneOverworld,
            _narrativeState,
            _history);

        Assert.True(state.IsOpen);
        Assert.Equal("Townsfolk", state.SpeakerName);
        Assert.Equal("Hello from town.", state.Text);
        Assert.Contains("town_line", _history.GetRecentIds(NpcDialogueService.HistorySystemName));
    }

    [Fact]
    public void Update_WhenDialogueIsOpenAndConfirmPressed_ClosesToPrompt()
    {
        var state = _controller.Update(
            new NpcDialogueState(true, true, NarrativeIds.SpeakerTownsfolkOne, "Townsfolk", "Hello.", string.Empty),
            new Rectangle(100, 116, 32, 32),
            [CreateTownsfolk()],
            interactJustPressed: false,
            confirmJustPressed: true,
            cancelJustPressed: false,
            _service,
            hintService: null,
            NarrativeIds.ZoneOverworld,
            _narrativeState,
            _history);

        Assert.True(state.IsPromptVisible);
        Assert.False(state.IsOpen);
        Assert.Equal(string.Empty, state.Text);
    }

    [Fact]
    public void Update_WhenSelectedDialogueCanDeliverHint_AppendsHintAndRecordsHintText()
    {
        var service = CreateService(canDeliverHint: true);
        var hintService = CreateHintService();

        var state = _controller.Update(
            NpcDialogueState.Default,
            new Rectangle(100, 116, 32, 32),
            [CreateTownsfolk()],
            interactJustPressed: true,
            confirmJustPressed: false,
            cancelJustPressed: false,
            service,
            hintService,
            NarrativeIds.ZoneOverworld,
            _narrativeState,
            _history);

        Assert.True(state.IsOpen);
        Assert.Equal("Find the other townsfolk.", state.HintText);
        Assert.Contains("Hint: Find the other townsfolk.", state.Text);
        Assert.Contains("hint_1", _history.GetRecentIds(HintService.HistorySystemName));
    }

    private static TownsfolkProp CreateTownsfolk()
    {
        return new TownsfolkProp(
            new Vector2(120f, 100f),
            new Point(34, 44),
            NarrativeIds.SpeakerTownsfolkOne,
            "Townsfolk");
    }

    private static NpcDialogueService CreateService(bool canDeliverHint = false)
    {
        var data = new NpcDialogueDataFile
        {
            Entries =
            [
                new NpcDialogueEntry
                {
                    Id = "town_line",
                    SpeakerId = NarrativeIds.SpeakerTownsfolkOne,
                    SpeakerName = "Townsfolk",
                    QuestId = NarrativeIds.QuestTownIntroductions,
                    ObjectiveId = NarrativeIds.ObjectiveMeetTownsfolk,
                    LineStyle = "greeting",
                    Weight = 1,
                    CanDeliverHint = canDeliverHint,
                    Text = "Hello from town."
                }
            ]
        };

        return new NpcDialogueService(data, new WeightedRandomSelector(new Random(1)), new RecentSelectionHistory());
    }

    private static HintService CreateHintService()
    {
        return new HintService(
            new HintDataFile
            {
                Entries =
                [
                    new HintEntry
                    {
                        Id = "hint_1",
                        ZoneId = NarrativeIds.ZoneOverworld,
                        ObjectiveId = NarrativeIds.ObjectiveMeetTownsfolk,
                        Priority = 1,
                        Text = "Find the other townsfolk."
                    }
                ]
            },
            new RecentSelectionHistory());
    }
}
