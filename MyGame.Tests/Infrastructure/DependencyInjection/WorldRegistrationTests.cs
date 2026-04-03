using Microsoft.Extensions.DependencyInjection;
using MyGame.Configuration;
using MyGame.Core.Input;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.World;
using MyGame.Infrastructure.Configuration;
using MyGame.Infrastructure.Logging;

namespace MyGame.Tests.Infrastructure.DependencyInjection;

public sealed class WorldRegistrationTests
{
    [Fact]
    public void ResolvedWorld_UsesDefaultTreeProps()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IInputService>(new StubInputService());
        services.AddSingleton<ILogger, InMemoryLogger>();
        services.AddSingleton<JsonFileLoader<EnemySettings>>();
        services.AddSingleton<JsonFileLoader<PlayerMovementSettings>>();
        services.AddSingleton(new EnemySettings());
        services.AddSingleton(new PlayerAttackSettings());
        services.AddTransient<PlayerAttackController>();
        services.AddSingleton(new PlayerMovementSettings());
        services.AddTransient<PlayerMovementController>();
        services.AddTransient<PlayerActor>();
        services.AddTransient(provider => new World(
            provider.GetRequiredService<PlayerActor>(),
            provider.GetRequiredService<EnemySettings>()));

        using var serviceProvider = services.BuildServiceProvider();

        var world = serviceProvider.GetRequiredService<World>();

        Assert.Equal(3, world.TreeProps.Count);
    }

    [Fact]
    public void ResolvedPlayerMovementController_UsesRegisteredSettings()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ILogger, InMemoryLogger>();
        services.AddSingleton<JsonFileLoader<PlayerMovementSettings>>();
        services.AddSingleton(new PlayerMovementSettings { MoveSpeed = 240f });
        services.AddTransient<PlayerMovementController>();

        using var serviceProvider = services.BuildServiceProvider();

        var controller = serviceProvider.GetRequiredService<PlayerMovementController>();
        var result = controller.Update(
            new Microsoft.Xna.Framework.Vector2(0f, 0f),
            Direction.Down,
            new InputSnapshot(new HashSet<GameAction> { GameAction.MoveRight }),
            new global::MyGame.Core.FrameTime(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1)));

        Assert.Equal(new Microsoft.Xna.Framework.Vector2(240f, 0f), result.Position);
    }

    private sealed class StubInputService : IInputService
    {
        public InputSnapshot Current => InputSnapshot.Empty;

        public InputSnapshot Previous => InputSnapshot.Empty;

        public void Update()
        {
        }

        public bool IsPressed(GameAction action)
        {
            return false;
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
