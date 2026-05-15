using Microsoft.Xna.Framework;
using MyGame.Gameplay.Props;

namespace MyGame.Gameplay.Narrative;

public sealed class NpcDialogueController
{
    private const int HorizontalPromptRange = 30;
    private const int VerticalPromptRange = 36;
    private const string DefaultLineStyle = "greeting";

    public NpcDialogueState Update(
        NpcDialogueState state,
        Rectangle playerBounds,
        IEnumerable<IConversationProp> conversationProps,
        bool interactJustPressed,
        bool confirmJustPressed,
        bool cancelJustPressed,
        NpcDialogueService dialogueService,
        HintService? hintService,
        string zoneId,
        NarrativeState narrativeState,
        RecentSelectionHistory history)
    {
        var target = FindTarget(playerBounds, conversationProps);
        if (state.IsOpen)
        {
            return interactJustPressed || confirmJustPressed || cancelJustPressed
                ? CreateClosedState(target)
                : state;
        }

        if (target is null)
        {
            return NpcDialogueState.Default;
        }

        if (!interactJustPressed)
        {
            return CreateClosedState(target);
        }

        var line = dialogueService.SelectLine(
            new NpcDialogueRequest(target.DialogueSpeakerId, DefaultLineStyle),
            narrativeState,
            history);
        var hint = line.CanDeliverHint
            ? hintService?.SelectHint(zoneId, narrativeState, history)
            : null;
        var text = hint is null
            ? line.Text
            : $"{line.Text} Hint: {hint.Text}";
        return new NpcDialogueState(true, true, line.SpeakerId, line.SpeakerName, text, hint?.Text ?? string.Empty);
    }

    private static IConversationProp? FindTarget(
        Rectangle playerBounds,
        IEnumerable<IConversationProp> conversationProps)
    {
        return conversationProps
            .Where(prop => GetInteractionBounds(prop.Bounds).Intersects(playerBounds))
            .OrderBy(prop => GetSquaredDistance(playerBounds.Center, prop.Bounds.Center))
            .FirstOrDefault();
    }

    private static NpcDialogueState CreateClosedState(IConversationProp? target)
    {
        return target is null
            ? NpcDialogueState.Default
            : new NpcDialogueState(true, false, target.DialogueSpeakerId, target.DisplayName, string.Empty, string.Empty);
    }

    private static Rectangle GetInteractionBounds(Rectangle targetBounds)
    {
        return new Rectangle(
            targetBounds.X - HorizontalPromptRange,
            targetBounds.Y - VerticalPromptRange,
            targetBounds.Width + (HorizontalPromptRange * 2),
            targetBounds.Height + (VerticalPromptRange * 2));
    }

    private static int GetSquaredDistance(Point first, Point second)
    {
        var deltaX = first.X - second.X;
        var deltaY = first.Y - second.Y;
        return (deltaX * deltaX) + (deltaY * deltaY);
    }
}
