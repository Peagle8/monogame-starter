using Microsoft.Xna.Framework;
using MyGame.Gameplay.Shops;

namespace MyGame.Tests.Gameplay.Shops;

public sealed class ShopDialogueControllerTests
{
    private readonly ShopDialogueController _controller = new();
    private static readonly Rectangle CounterBounds = new(352, 232, 96, 24);
    private static readonly Rectangle NearCounterPlayerBounds = new(384, 264, 32, 32);

    [Fact]
    public void Update_WhenPlayerIsNearCounter_ShowsPrompt()
    {
        var state = _controller.Update(
            ShopDialogueState.Default,
            NearCounterPlayerBounds,
            CounterBounds,
            interactJustPressed: false,
            cancelJustPressed: false,
            previousTabJustPressed: false,
            nextTabJustPressed: false);

        Assert.True(state.IsPromptVisible);
        Assert.False(state.IsOpen);
        Assert.Equal(ShopDialogueTab.Buy, state.ActiveTab);
    }

    [Fact]
    public void Update_WhenInteractPressedNearCounter_OpensDialogue()
    {
        var state = _controller.Update(
            ShopDialogueState.Default,
            NearCounterPlayerBounds,
            CounterBounds,
            interactJustPressed: true,
            cancelJustPressed: false,
            previousTabJustPressed: false,
            nextTabJustPressed: false);

        Assert.True(state.IsOpen);
        Assert.True(state.IsPromptVisible);
    }

    [Fact]
    public void Update_WhenDialogueOpen_NextTabSwitchesToSell()
    {
        var state = _controller.Update(
            new ShopDialogueState(true, true, ShopDialogueTab.Buy),
            NearCounterPlayerBounds,
            CounterBounds,
            interactJustPressed: false,
            cancelJustPressed: false,
            previousTabJustPressed: false,
            nextTabJustPressed: true);

        Assert.Equal(ShopDialogueTab.Sell, state.ActiveTab);
        Assert.True(state.IsOpen);
    }

    [Fact]
    public void Update_WhenDialogueOpen_CancelClosesDialogue()
    {
        var state = _controller.Update(
            new ShopDialogueState(true, true, ShopDialogueTab.Sell),
            NearCounterPlayerBounds,
            CounterBounds,
            interactJustPressed: false,
            cancelJustPressed: true,
            previousTabJustPressed: false,
            nextTabJustPressed: false);

        Assert.False(state.IsOpen);
        Assert.True(state.IsPromptVisible);
    }

    [Fact]
    public void Update_WhenPlayerLeavesCounter_HidesPromptAndDialogue()
    {
        var state = _controller.Update(
            new ShopDialogueState(true, true, ShopDialogueTab.Sell),
            new Rectangle(520, 240, 32, 32),
            CounterBounds,
            interactJustPressed: false,
            cancelJustPressed: false,
            previousTabJustPressed: false,
            nextTabJustPressed: false);

        Assert.Equal(ShopDialogueState.Default, state);
    }
}
