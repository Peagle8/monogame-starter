using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Input;
using MyGame.Configuration;
using MyGame.Core.Diagnostics;
using MyGame.Core.Input;
using MyGame.Core.Rendering;
using MyGame.Core.Scenes;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.World;
using MyGame.Infrastructure.Input;
using MyGame.Infrastructure.Logging;
using MyGame.Rendering.Gameplay;
using MyGame.Rendering.MainMenu;
using MyGame.Rendering.Player;
using MyGame.Scenes.Gameplay;
using MyGame.Scenes.MainMenu;

namespace MyGame.Infrastructure.DependencyInjection;

public static class ServiceRegistration
{
    public static ServiceProvider Build(GameRoot gameRoot)
    {
        var services = new ServiceCollection();

        services.AddSingleton(gameRoot);
        services.AddSingleton<SceneManager>();
        services.AddSingleton<DebugOverlay>();
        services.AddSingleton<GameRecorder>();
        services.AddSingleton<IRenderContext, RenderContext>();
        services.AddSingleton<IWorldRectangleRenderer, WorldRectangleRenderer>();
        services.AddSingleton<IWorldSpriteRenderer, WorldSpriteRenderer>();
        services.AddSingleton<ILogger, InMemoryLogger>();
        services.AddSingleton<DefaultInputBindings>();
        services.AddSingleton<IReadOnlyDictionary<GameAction, Keys[]>>(provider =>
            provider.GetRequiredService<DefaultInputBindings>().Create());

        services.AddSingleton<IInputService, InputService>();
        services.AddSingleton(new PlayerMovementSettings());
        services.AddTransient<PlayerMovementController>();
        services.AddTransient<PlayerActor>();
        services.AddTransient(provider => new World(provider.GetRequiredService<PlayerActor>()));
        // TODO: eventually let's move all of these render injections into an extension method and call that method here so this doesn't get out of hand
        services.AddTransient<IRenderer<PlayerActor>, PlayerRenderer>();
        services.AddTransient<IGameplayEntityRenderer, TreeEntityRenderer>();
        services.AddTransient<IGameplayEntityRenderer, PlayerEntityRenderer>();
        services.AddTransient<IRenderer<GameplayPauseMenu>, GameplayPauseMenuRenderer>();
        services.AddTransient<GameplayOverlayRenderer>();
        services.AddTransient<MainMenuRenderer>();
        services.AddTransient<GameplayWorldRenderer>();
        services.AddTransient<IRenderer<GameplayScene>, GameplaySceneRenderer>();
        services.AddTransient<MainMenuBackgroundRenderer>();
        services.AddTransient<IRenderer<MainMenuScene>, MainMenuSceneRenderer>();

        return services.BuildServiceProvider();
    }
}
