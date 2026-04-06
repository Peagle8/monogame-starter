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
        services.AddSingleton<DefaultInputBindings>();
        services.AddSingleton<DefaultGamePadBindings>();
        services.AddSingleton<IReadOnlyDictionary<GameAction, Keys[]>>(provider =>
            provider.GetRequiredService<DefaultInputBindings>().Create());
        services.AddSingleton<IReadOnlyDictionary<GameAction, GamePadControl[]>>(provider =>
            provider.GetRequiredService<DefaultGamePadBindings>().Create());

        // TODO: move all input injections into their own extension method as well
        services.AddSingleton<KeyboardInputSnapshotSource>();
        services.AddSingleton<MonoGameGamePadSnapshotReader>();
        services.AddSingleton<GamePadInputSnapshotSource>();
        services.AddSingleton<IInputSnapshotSource>(provider => new CompositeInputSnapshotSource(
        [
            provider.GetRequiredService<KeyboardInputSnapshotSource>(),
            provider.GetRequiredService<GamePadInputSnapshotSource>()
        ]));
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
        services.AddSingleton<IEnemySettingsCatalog>(provider =>
        {
            var loader = provider.GetRequiredService<JsonFileLoader<EnemySettings>>();
            var configDirectory = Path.Combine(
                AppContext.BaseDirectory,
                "Content",
                "Configuration");
            var crabSettings = loader.LoadOrDefault(
                Path.Combine(configDirectory, "EnemySettings.json"),
                new EnemySettings());
            var hornedRabbitSettings = loader.LoadOrDefault(
                Path.Combine(configDirectory, "HornedRabbitSettings.json"),
                EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbit));

            return new EnemySettingsCatalog(crabSettings, hornedRabbitSettings);
        });
        services.AddSingleton(provider => provider.GetRequiredService<IEnemySettingsCatalog>().Get(EnemyKind.Crab));
        services.AddSingleton(new PlayerAttackSettings());
        services.AddSingleton<IPlayerAbilityService>(_ => new PlayerAbilityService([PlayerAbility.Dash]));
        services.AddTransient(provider => new EnemyActor(
            provider.GetRequiredService<EnemySettings>(),
            new Microsoft.Xna.Framework.Vector2(520f, 240f)));
        services.AddTransient<PlayerAttackController>();
        services.AddTransient<PlayerMovementController>();
        services.AddTransient<PlayerDashController>();
        services.AddTransient<PlayerActor>();
        services.AddTransient(provider =>
        {
            var enemySettingsCatalog = provider.GetRequiredService<IEnemySettingsCatalog>();
            var hornedRabbitSettings = enemySettingsCatalog.Get(EnemyKind.HornedRabbit);

            return new World(
                provider.GetRequiredService<PlayerActor>(),
                [
                    new MyGame.Gameplay.Props.TreeProp(new Microsoft.Xna.Framework.Vector2(120f, 120f), new Microsoft.Xna.Framework.Point(72, 104)),
                    new MyGame.Gameplay.Props.TreeProp(new Microsoft.Xna.Framework.Vector2(560f, 160f), new Microsoft.Xna.Framework.Point(64, 96)),
                    new MyGame.Gameplay.Props.TreeProp(new Microsoft.Xna.Framework.Vector2(620f, 320f), new Microsoft.Xna.Framework.Point(80, 112))
                ],
                [
                    // TODO: we need to move this out of DI directly, this will not work when we have multiple maps. We'll need to inject these per map/"level", we should introduce a factory pattern we could use for setting these enemies up (I know we do not have levels or different maps yet)
                    CreateHornedRabbit(hornedRabbitSettings, new Microsoft.Xna.Framework.Vector2(560f, 180f), EnemyAxisPreference.Horizontal),
                    CreateHornedRabbit(hornedRabbitSettings, new Microsoft.Xna.Framework.Vector2(660f, 240f), EnemyAxisPreference.Vertical),
                    CreateHornedRabbit(hornedRabbitSettings, new Microsoft.Xna.Framework.Vector2(560f, 320f), EnemyAxisPreference.Horizontal),
                    CreateHornedRabbit(hornedRabbitSettings, new Microsoft.Xna.Framework.Vector2(720f, 320f), EnemyAxisPreference.None)
                ],
                provider.GetRequiredService<EnemySettings>(),
                enemySettingsCatalog);
        });
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

    // TODO: this does NOT belong in the service registration... I would think this belongs in a horned rabbit type
    private static EnemyActor CreateHornedRabbit(
        EnemySettings settings,
        Microsoft.Xna.Framework.Vector2 position,
        EnemyAxisPreference axisPreference)
    {
        var initialPauseSeconds = Random.Shared.NextSingle()
            * (settings.InitialDashPauseMaxSeconds - settings.InitialDashPauseMinSeconds)
            + settings.InitialDashPauseMinSeconds;
        return new EnemyActor(settings, position, initialPauseSeconds, axisPreference);
    }
}
