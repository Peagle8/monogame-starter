using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Input;
using MyGame.Configuration;
using MyGame.Core.Diagnostics;
using MyGame.Core.Input;
using MyGame.Core.Rendering;
using MyGame.Core.Scenes;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.World;
using MyGame.Infrastructure.Configuration;
using MyGame.Infrastructure.Input;
using MyGame.Infrastructure.Logging;
using MyGame.Infrastructure.Save;
using MyGame.Rendering.Enemies;
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
        services.AddSingleton<ISaveGameService>(provider => new JsonSaveGameService(
            provider.GetRequiredService<ILogger>(),
            Path.Combine(AppContext.BaseDirectory, "Saves", "savegame.json")));
        services.AddSingleton<JsonFileLoader<DiagnosticsSettings>>();
        services.AddSingleton<JsonFileLoader<EnemySettings>>();
        services.AddSingleton<JsonFileLoader<PlayerMovementSettings>>();
        // TODO: lets also move all of the capture related injections into an extension method
        // TODO: In fact, update the AGENTS.md to mention that if there are more than x of the same flavor of service being injected, move it into it's own extension method in it's own file.
        // TODO: sorry while I am thinking of it, also add rules for class and method length best practice
        services.AddSingleton<DefaultInputBindings>();
        services.AddSingleton<IReadOnlyDictionary<GameAction, Keys[]>>(provider =>
            provider.GetRequiredService<DefaultInputBindings>().Create());

        services.AddSingleton<IInputSnapshotSource, KeyboardInputSnapshotSource>();
        services.AddSingleton<InputService>();
        services.AddSingleton<IInputService>(provider => provider.GetRequiredService<InputService>());
        // TODO: move these into their own extension method and call that here to keep this clean
        services.AddSingleton(provider =>
        {
            var loader = provider.GetRequiredService<JsonFileLoader<DiagnosticsSettings>>();
            var path = Path.Combine(
                AppContext.BaseDirectory,
                "Content",
                "Configuration",
                "DiagnosticsSettings.json");

            return loader.LoadOrDefault(path, new DiagnosticsSettings());
        });
        services.AddSingleton(provider =>
        {
            var loader = provider.GetRequiredService<JsonFileLoader<PlayerMovementSettings>>();
            var path = Path.Combine(
                AppContext.BaseDirectory,
                "Content",
                "Configuration",
                "PlayerMovementSettings.json");

            return loader.LoadOrDefault(path, new PlayerMovementSettings());
        });
        services.AddSingleton(provider =>
        {
            var loader = provider.GetRequiredService<JsonFileLoader<EnemySettings>>();
            var path = Path.Combine(
                AppContext.BaseDirectory,
                "Content",
                "Configuration",
                "EnemySettings.json");

            return loader.LoadOrDefault(path, new EnemySettings());
        });
        services.AddSingleton(new PlayerAttackSettings());
        services.AddTransient(provider => new EnemyActor(
            provider.GetRequiredService<EnemySettings>(),
            new Microsoft.Xna.Framework.Vector2(520f, 240f)));
        services.AddTransient<PlayerAttackController>();
        services.AddTransient<PlayerMovementController>();
        services.AddTransient<PlayerActor>();
        services.AddTransient(provider => new World(
            provider.GetRequiredService<PlayerActor>(),
            provider.GetRequiredService<EnemySettings>()));
        // TODO: eventually let's move all of these render injections into an extension method and call that method here so this doesn't get out of hand
        services.AddTransient<IRenderer<EnemyActor>, EnemyRenderer>();
        services.AddTransient<IRenderer<PlayerActor>, PlayerRenderer>();
        services.AddTransient<IGameplayEntityRenderer, TreeEntityRenderer>();
        services.AddTransient<IGameplayEntityRenderer, EnemyEntityRenderer>();
        services.AddTransient<IGameplayEntityRenderer, PlayerAttackEffectRenderer>();
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
