using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Input;
using MyGame.Configuration;
using MyGame.Core.Input;
using MyGame.Core.Rendering;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.World;
using MyGame.Infrastructure.Configuration;
using MyGame.Infrastructure.Input;
using MyGame.Rendering.Enemies;
using MyGame.Rendering.Gameplay;
using MyGame.Rendering.MainMenu;
using MyGame.Rendering.Player;
using MyGame.Scenes.Gameplay;
using MyGame.Scenes.MainMenu;

namespace MyGame.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInputServices(this IServiceCollection services)
    {
        services.AddSingleton<DefaultInputBindings>();
        services.AddSingleton<DefaultGamePadBindings>();
        services.AddSingleton<IReadOnlyDictionary<GameAction, Keys[]>>(provider =>
            provider.GetRequiredService<DefaultInputBindings>().Create());
        services.AddSingleton<IReadOnlyDictionary<GameAction, GamePadControl[]>>(provider =>
            provider.GetRequiredService<DefaultGamePadBindings>().Create());
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

        return services;
    }

    public static IServiceCollection AddConfigurationServices(this IServiceCollection services)
    {
        services.AddSingleton<JsonFileLoader<DiagnosticsSettings>>();
        services.AddSingleton<JsonFileLoader<EnemySettings>>();
        services.AddSingleton<JsonFileLoader<PlayerCombatSettings>>();
        services.AddSingleton<JsonFileLoader<PlayerMovementSettings>>();
        services.AddSingleton<JsonFileLoader<WorldCombatSettings>>();
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
            var loader = provider.GetRequiredService<JsonFileLoader<PlayerCombatSettings>>();
            var path = Path.Combine(
                AppContext.BaseDirectory,
                "Content",
                "Configuration",
                "PlayerCombatSettings.json");

            return loader.LoadOrDefault(path, new PlayerCombatSettings());
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
            var loader = provider.GetRequiredService<JsonFileLoader<WorldCombatSettings>>();
            var path = Path.Combine(
                AppContext.BaseDirectory,
                "Content",
                "Configuration",
                "WorldCombatSettings.json");

            return loader.LoadOrDefault(path, new WorldCombatSettings());
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
            var hornedRabbitBossSettings = loader.LoadOrDefault(
                Path.Combine(configDirectory, "HornedRabbitBossSettings.json"),
                EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbitBoss));
            var hornedRabbitEliteSettings = loader.LoadOrDefault(
                Path.Combine(configDirectory, "HornedRabbitEliteSettings.json"),
                EnemySettingsCatalog.CreateDefault(EnemyKind.HornedRabbitElite));
            var batSettings = loader.LoadOrDefault(
                Path.Combine(configDirectory, "BatSettings.json"),
                EnemySettingsCatalog.CreateDefault(EnemyKind.Bat));
            var batMiniBossSettings = loader.LoadOrDefault(
                Path.Combine(configDirectory, "BatMiniBossSettings.json"),
                EnemySettingsCatalog.CreateDefault(EnemyKind.BatMiniBoss));
            var grasshopperSettings = loader.LoadOrDefault(
                Path.Combine(configDirectory, "GrasshopperSettings.json"),
                EnemySettingsCatalog.CreateDefault(EnemyKind.Grasshopper));

            return new EnemySettingsCatalog(crabSettings, hornedRabbitSettings, hornedRabbitEliteSettings, batSettings, grasshopperSettings, batMiniBossSettings, hornedRabbitBossSettings);
        });
        services.AddSingleton(provider => provider.GetRequiredService<IEnemySettingsCatalog>().Get(EnemyKind.Crab));

        return services;
    }

    public static IServiceCollection AddRenderingServices(this IServiceCollection services)
    {
        services.AddSingleton<IRenderContext, RenderContext>();
        services.AddSingleton<IWorldRectangleRenderer, WorldRectangleRenderer>();
        services.AddSingleton<IWorldSpriteRenderer, WorldSpriteRenderer>();
        services.AddTransient<IEnemyKindRenderer, CrabEnemyRenderer>();
        services.AddTransient<IEnemyKindRenderer, HornedRabbitEnemyRenderer>();
        services.AddTransient<IEnemyKindRenderer, HornedRabbitBossEnemyRenderer>();
        services.AddTransient<IEnemyKindRenderer, HornedRabbitEliteEnemyRenderer>();
        services.AddTransient<IEnemyKindRenderer, BatEnemyRenderer>();
        services.AddTransient<IEnemyKindRenderer, BatMiniBossEnemyRenderer>();
        services.AddTransient<IEnemyKindRenderer, GrasshopperEnemyRenderer>();
        services.AddTransient<IRenderer<EnemyActor>, EnemyRenderer>();
        services.AddTransient<IRenderer<PlayerActor>, PlayerRenderer>();
        services.AddTransient<IGameplayEntityRenderer, GrassEntityRenderer>();
        services.AddTransient<IGameplayEntityRenderer, MountainEntityRenderer>();
        services.AddTransient<IGameplayEntityRenderer, HouseExteriorEntityRenderer>();
        services.AddTransient<IGameplayEntityRenderer, ShopExteriorEntityRenderer>();
        services.AddTransient<IGameplayEntityRenderer, DungeonEntranceEntityRenderer>();
        services.AddTransient<IGameplayEntityRenderer, ArenaEntranceEntityRenderer>();
        services.AddTransient<IGameplayEntityRenderer, ArenaBoundaryEntityRenderer>();
        services.AddTransient<IGameplayEntityRenderer, WallEntityRenderer>();
        services.AddTransient<IGameplayEntityRenderer, TreeEntityRenderer>();
        services.AddTransient<IGameplayEntityRenderer, EnemyEntityRenderer>();
        services.AddTransient<IGameplayEntityRenderer, EnemyHealthBarRenderer>();
        services.AddTransient<IGameplayEntityRenderer, PlayerProjectileRenderer>();
        services.AddTransient<IGameplayEntityRenderer, PlayerBombRenderer>();
        services.AddTransient<IGameplayEntityRenderer, ShopkeeperEntityRenderer>();
        services.AddTransient<IGameplayEntityRenderer, ShopTalkIndicatorRenderer>();
        services.AddTransient<IGameplayEntityRenderer, PlayerShieldRenderer>();
        services.AddTransient<IGameplayEntityRenderer, PlayerStunRenderer>();
        services.AddTransient<IGameplayEntityRenderer, PlayerAttackEffectRenderer>();
        services.AddTransient<IGameplayEntityRenderer, PlayerEntityRenderer>();
        services.AddTransient<IGameplayEntityRenderer, CounterEntityRenderer>();
        services.AddTransient<IGameplayEntityRenderer, WorldToastRenderer>();
        services.AddTransient<IRenderer<GameplayPauseMenu>, GameplayPauseMenuRenderer>();
        services.AddTransient<ShopDialogueRenderer>();
        services.AddTransient<GameplayOverlayRenderer>();
        services.AddTransient<MainMenuRenderer>();
        services.AddTransient<GameplayWorldRenderer>();
        services.AddTransient<IRenderer<GameplayScene>, GameplaySceneRenderer>();
        services.AddTransient<MainMenuBackgroundRenderer>();
        services.AddTransient<IRenderer<MainMenuScene>, MainMenuSceneRenderer>();

        return services;
    }
}
