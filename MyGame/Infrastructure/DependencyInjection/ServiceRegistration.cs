using Microsoft.Extensions.DependencyInjection;
using MyGame.Configuration;
using MyGame.Core.Diagnostics;
using MyGame.Core.Scenes;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;
using MyGame.Gameplay.World;
using MyGame.Infrastructure.Logging;
using MyGame.Infrastructure.Save;

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
        services.AddSingleton<ILogger, InMemoryLogger>();
        services.AddSingleton<ISaveGameService>(provider => new JsonSaveGameService(
            provider.GetRequiredService<ILogger>(),
            Path.Combine(AppContext.BaseDirectory, "Saves", "savegame.json")));
        services.AddInputServices();
        services.AddConfigurationServices();
        services.AddRenderingServices();
        services.AddSingleton(new PlayerAttackSettings());
        services.AddSingleton<IPlayerAbilityService>(_ => new PlayerAbilityService([PlayerAbility.Dash]));
        services.AddTransient<IEnemyFactory, EnemyFactory>();
        services.AddTransient<PlayerAttackHitResolver>();
        services.AddTransient<EnemyContactResolver>();
        services.AddTransient<PlayerAttackController>();
        services.AddTransient<PlayerMovementController>();
        services.AddTransient<PlayerDashController>();
        services.AddTransient<PlayerActor>();
        services.AddTransient<GameplayLevelBuilder>();
        services.AddTransient(provider => provider.GetRequiredService<GameplayLevelBuilder>().BuildDefaultLevel(
            provider.GetRequiredService<PlayerActor>()));
        return services.BuildServiceProvider();
    }
}
