using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;
using MyGame.Core.Input;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.Props;

namespace MyGame.Tests.Gameplay.World;

public sealed class WorldTests
{
    [Fact]
    public void Update_UpdatesPlayerState()
    {
        var inputService = new StubInputService(new InputSnapshot(new HashSet<GameAction> { GameAction.MoveRight }));
        var player = new PlayerActor(inputService, new PlayerMovementController(new PlayerMovementSettings { MoveSpeed = 180f }));
        var world = new global::MyGame.Gameplay.World.World(player, []);
        var frameTime = new FrameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

        world.Update(frameTime);

        Assert.Equal(new Vector2(580f, 240f), world.Player.Position);
        Assert.Equal(Direction.Right, world.Player.Facing);
        Assert.True(world.Player.IsMoving);
    }

    [Fact]
    public void Constructor_StoresTreeProps()
    {
        var player = new PlayerActor(
            new StubInputService(InputSnapshot.Empty),
            new PlayerMovementController(new PlayerMovementSettings { MoveSpeed = 180f }));
        TreeProp[] treeProps =
        [
            new(new Vector2(10f, 20f), new Point(30, 40)),
            new(new Vector2(50f, 60f), new Point(70, 80))
        ];

        var world = new global::MyGame.Gameplay.World.World(player, treeProps);

        Assert.Equal(treeProps, world.TreeProps);
    }

    [Fact]
    public void GetDebugState_ReturnsPlayerAndTreeDetails()
    {
        var inputService = new StubInputService(new InputSnapshot(new HashSet<GameAction> { GameAction.MoveUp }));
        var player = new PlayerActor(inputService, new PlayerMovementController(new PlayerMovementSettings { MoveSpeed = 60f }));
        var world = new global::MyGame.Gameplay.World.World(
            player,
            [new TreeProp(new Vector2(5f, 10f), new Point(16, 24))]);
        var frameTime = new FrameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

        world.Update(frameTime);
        var debugState = world.GetDebugState();

        Assert.Equal("400.00, 180.00", debugState["PlayerPosition"]);
        Assert.Equal("Up", debugState["PlayerFacing"]);
        Assert.Equal("1", debugState["TreePropCount"]);
    }

    private sealed class StubInputService : IInputService
    {
        public StubInputService(InputSnapshot current)
        {
            Current = current;
        }

        public InputSnapshot Current { get; }

        public InputSnapshot Previous => InputSnapshot.Empty;

        public void Update()
        {
        }

        public bool IsPressed(GameAction action)
        {
            return Current.IsPressed(action);
        }

        public bool IsJustPressed(GameAction action)
        {
            return false;
        }

        public bool IsJustReleased(GameAction action)
        {
            return false;
        }
    }
}
