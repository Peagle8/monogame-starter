using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Props;

public sealed class TownsfolkProp : IConversationProp
{
    public TownsfolkProp(Vector2 position, Point size, string dialogueSpeakerId, string displayName)
    {
        Position = position;
        Size = size;
        DialogueSpeakerId = dialogueSpeakerId;
        DisplayName = displayName;
    }

    public Vector2 Position { get; }

    public Point Size { get; }

    public string DialogueSpeakerId { get; }

    public string DisplayName { get; }

    public bool BlocksMovement => false;

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Size.X, Size.Y);

    public Rectangle CollisionBounds => Bounds;
}
