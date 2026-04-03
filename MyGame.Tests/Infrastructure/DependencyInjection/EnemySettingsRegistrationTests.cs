using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Gameplay.Enemies;
using MyGame.Infrastructure.Configuration;
using MyGame.Infrastructure.Logging;

namespace MyGame.Tests.Infrastructure.DependencyInjection;

public sealed class EnemySettingsRegistrationTests
{
    [Fact]
    public void ResolvedEnemySettings_UsesRegisteredSettings()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ILogger, InMemoryLogger>();
        services.AddSingleton<JsonFileLoader<EnemySettings>>();
        services.AddSingleton(new EnemySettings
        {
            MaxHealth = 7,
            MoveSpeed = 140f,
            ChaseRange = 200f
        });

        using var serviceProvider = services.BuildServiceProvider();

        var settings = serviceProvider.GetRequiredService<EnemySettings>();

        Assert.Equal(7, settings.MaxHealth);
        Assert.Equal(140f, settings.MoveSpeed);
        Assert.Equal(200f, settings.ChaseRange);
    }

    [Fact]
    public void ResolvedEnemyActor_UsesRegisteredEnemySettings()
    {
        var services = new ServiceCollection();
        services.AddSingleton(new EnemySettings
        {
            MaxHealth = 6,
            MoveSpeed = 135f,
            ChaseRange = 180f
        });
        services.AddTransient(provider => new EnemyActor(
            provider.GetRequiredService<EnemySettings>(),
            new Vector2(10f, 20f)));

        using var serviceProvider = services.BuildServiceProvider();

        var enemy = serviceProvider.GetRequiredService<EnemyActor>();

        Assert.Equal(6, enemy.MaxHealth);
        Assert.Equal(new Vector2(10f, 20f), enemy.Position);
    }
}
