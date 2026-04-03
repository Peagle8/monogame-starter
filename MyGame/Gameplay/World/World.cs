using Microsoft.Xna.Framework;
using MyGame.Core;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.Props;

namespace MyGame.Gameplay.World;

public sealed class World
{
    private readonly List<TreeProp> _treeProps;

    public World(PlayerActor player)
        : this(
            player,
            [
                new TreeProp(new Vector2(120f, 120f), new Point(72, 104)),
                new TreeProp(new Vector2(560f, 160f), new Point(64, 96)),
                new TreeProp(new Vector2(620f, 320f), new Point(80, 112))
            ])
    {
    }

    public World(PlayerActor player, IEnumerable<TreeProp> treeProps)
    {
        Player = player;
        _treeProps = treeProps.ToList();
    }

    public PlayerActor Player { get; }

    public IReadOnlyList<TreeProp> TreeProps => _treeProps;

    public void Update(FrameTime frameTime)
    {
        Player.Update(frameTime);
    }

    public IReadOnlyDictionary<string, string> GetDebugState()
    {
        return new Dictionary<string, string>
        {
            ["PlayerPosition"] = $"{Player.Position.X:0.00}, {Player.Position.Y:0.00}",
            ["PlayerFacing"] = Player.Facing.ToString(),
            ["TreePropCount"] = _treeProps.Count.ToString()
        };
    }
}
