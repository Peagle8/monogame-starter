using Microsoft.Xna.Framework;
using MyGame.Gameplay.Narrative;

namespace MyGame.Gameplay.Props;

public sealed class ShopkeeperProp : IConversationProp
{
    public ShopkeeperProp(Vector2 position, Point size)
    {
        Position = position;
        Size = size;
    }

    public Vector2 Position { get; }

    public Point Size { get; }

    public string DialogueSpeakerId => NarrativeIds.SpeakerShopkeeper;

    public string DisplayName => "Shopkeeper";

    public bool BlocksMovement => false;

    public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Size.X, Size.Y);

    public Rectangle CollisionBounds => Bounds;
}
