namespace MyGame.Gameplay.Props;

public interface IConversationProp : IWorldProp
{
    string DialogueSpeakerId { get; }

    string DisplayName { get; }
}
