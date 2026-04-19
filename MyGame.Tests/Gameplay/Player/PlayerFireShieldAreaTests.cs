using Microsoft.Xna.Framework;
using MyGame.Gameplay.Player;

namespace MyGame.Tests.Gameplay.Player;

public sealed class PlayerFireShieldAreaTests
{
    [Fact]
    public void GetRadius_UsesPlayerSizeAndMultiplier()
    {
        var radius = PlayerFireShieldArea.GetRadius(new Rectangle(100, 120, 32, 32), 4f);

        Assert.Equal(128f, radius);
    }

    [Fact]
    public void Intersects_WhenTargetIsInsideAura_ReturnsTrue()
    {
        var intersects = PlayerFireShieldArea.Intersects(
            new Rectangle(400, 240, 32, 32),
            new Rectangle(500, 240, 28, 28),
            4f);

        Assert.True(intersects);
    }

    [Fact]
    public void Intersects_WhenTargetIsOutsideAura_ReturnsFalse()
    {
        var intersects = PlayerFireShieldArea.Intersects(
            new Rectangle(400, 240, 32, 32),
            new Rectangle(700, 240, 28, 28),
            4f);

        Assert.False(intersects);
    }
}
