using MyGame.Infrastructure.Logging;
using MyGame.Infrastructure.Save;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Player;

namespace MyGame.Tests.Infrastructure.Save;

public sealed class JsonSaveGameServiceTests : IDisposable
{
    private readonly string _directoryPath;
    private readonly string _savePath;

    public JsonSaveGameServiceTests()
    {
        _directoryPath = Path.Combine(Path.GetTempPath(), $"MyGame.SaveTests.{Guid.NewGuid():N}");
        _savePath = Path.Combine(_directoryPath, "savegame.json");
    }

    [Fact]
    public void SaveAndLoad_RoundTripsSaveGameData()
    {
        var logger = new InMemoryLogger();
        var service = new JsonSaveGameService(logger, _savePath);
        var expected = new SaveGameData
        {
            SceneName = "Gameplay",
            DefeatedEnemyCount = 1,
            Enemies =
            [
                new EnemySaveData
                {
                    Kind = EnemyKind.Crab,
                    AxisPreference = EnemyAxisPreference.None,
                    PositionX = 520f,
                    PositionY = 240f,
                    CurrentHealth = 0
                }
            ],
            PlayerAbilityPoints = 1.5f,
            UnlockedAbilities = [PlayerAbility.Dash, PlayerAbility.Fireball, PlayerAbility.BombDash],
            EquippedDashAbility = PlayerDashAbilityKind.BaseDash,
            EquippedDefenseAbility = PlayerDefenseAbilityKind.Shield,
            EquippedRangedAbility = PlayerRangedAttackKind.Fireball,
            EquippedMeleeAbility = PlayerMeleeAbilityKind.BaseAttack,
            PlayerHealth = 3,
            PlayerPositionX = 123.5f,
            PlayerPositionY = 456.25f
        };

        service.Save(expected);
        var loaded = service.Load();

        Assert.NotNull(loaded);
        Assert.Equal(expected.SceneName, loaded!.SceneName);
        Assert.Equal(expected.DefeatedEnemyCount, loaded.DefeatedEnemyCount);
        Assert.Equal(expected.PlayerAbilityPoints, loaded.PlayerAbilityPoints);
        Assert.Equal(expected.UnlockedAbilities, loaded.UnlockedAbilities);
        Assert.Equal(expected.EquippedDashAbility, loaded.EquippedDashAbility);
        Assert.Equal(expected.EquippedDefenseAbility, loaded.EquippedDefenseAbility);
        Assert.Equal(expected.EquippedRangedAbility, loaded.EquippedRangedAbility);
        Assert.Equal(expected.EquippedMeleeAbility, loaded.EquippedMeleeAbility);
        Assert.Equal(expected.PlayerHealth, loaded.PlayerHealth);
        Assert.Equal(expected.PlayerPositionX, loaded.PlayerPositionX);
        Assert.Equal(expected.PlayerPositionY, loaded.PlayerPositionY);
        Assert.Single(loaded.Enemies);
        Assert.Equal(expected.Enemies[0].Kind, loaded.Enemies[0].Kind);
        Assert.Equal(expected.Enemies[0].AxisPreference, loaded.Enemies[0].AxisPreference);
        Assert.Equal(expected.Enemies[0].PositionX, loaded.Enemies[0].PositionX);
        Assert.Equal(expected.Enemies[0].PositionY, loaded.Enemies[0].PositionY);
        Assert.Equal(expected.Enemies[0].CurrentHealth, loaded.Enemies[0].CurrentHealth);
    }

    [Fact]
    public void Load_WhenFileMissing_ReturnsNull()
    {
        var logger = new InMemoryLogger();
        var service = new JsonSaveGameService(logger, _savePath);

        var loaded = service.Load();

        Assert.Null(loaded);
    }

    [Fact]
    public void SaveExists_WhenFileMissing_ReturnsFalse()
    {
        var logger = new InMemoryLogger();
        var service = new JsonSaveGameService(logger, _savePath);

        Assert.False(service.SaveExists());
    }

    [Fact]
    public void SaveExists_AfterSave_ReturnsTrue()
    {
        var logger = new InMemoryLogger();
        var service = new JsonSaveGameService(logger, _savePath);

        service.Save(new SaveGameData
        {
            SceneName = "Gameplay",
            DefeatedEnemyCount = 0,
            Enemies = [],
            PlayerAbilityPoints = 3f,
            UnlockedAbilities = [PlayerAbility.Dash, PlayerAbility.BombDash],
            EquippedDashAbility = PlayerDashAbilityKind.BaseDash,
            EquippedDefenseAbility = PlayerDefenseAbilityKind.Shield,
            EquippedRangedAbility = PlayerRangedAttackKind.Fireball,
            EquippedMeleeAbility = PlayerMeleeAbilityKind.BaseAttack,
            PlayerHealth = 5,
            PlayerPositionX = 10f,
            PlayerPositionY = 20f
        });

        Assert.True(service.SaveExists());
    }

    public void Dispose()
    {
        if (Directory.Exists(_directoryPath))
        {
            Directory.Delete(_directoryPath, recursive: true);
        }
    }
}
