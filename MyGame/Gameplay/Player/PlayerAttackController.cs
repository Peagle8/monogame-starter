using Microsoft.Xna.Framework;
using MyGame.Configuration;
using MyGame.Core;

namespace MyGame.Gameplay.Player;

public sealed class PlayerAttackController
{
    private readonly PlayerAttackSettings _settings;

    public PlayerAttackController(PlayerAttackSettings settings)
    {
        _settings = settings;
    }

    public int Damage => _settings.Damage;

    public PlayerAttackState Update(
        PlayerAttackState currentState,
        Vector2 position,
        Direction facing,
        bool attackJustPressed,
        FrameTime frameTime)
    {
        var remainingActiveSeconds = Math.Max(0f, currentState.RemainingActiveSeconds - frameTime.DeltaSeconds);
        var remainingCooldownSeconds = Math.Max(0f, currentState.RemainingCooldownSeconds - frameTime.DeltaSeconds);

        var isAttacking = remainingActiveSeconds > 0f;
        Rectangle? attackBounds = isAttacking ? currentState.AttackBounds : null;
        var attackSequence = currentState.AttackSequence;

        if (attackJustPressed && remainingCooldownSeconds <= 0f && !isAttacking)
        {
            attackSequence++;
            remainingActiveSeconds = _settings.ActiveSeconds;
            remainingCooldownSeconds = _settings.CooldownSeconds;
            isAttacking = true;
            attackBounds = CreateAttackBounds(position, facing);
        }

        return new PlayerAttackState(
            isAttacking,
            attackBounds,
            attackSequence,
            remainingActiveSeconds,
            remainingCooldownSeconds);
    }

    private Rectangle CreateAttackBounds(Vector2 position, Direction facing)
    {
        var playerBounds = new Rectangle((int)position.X, (int)position.Y, 32, 32);

        return facing switch
        {
            Direction.Up => new Rectangle(playerBounds.X, playerBounds.Y - _settings.Range, playerBounds.Width, _settings.Range),
            Direction.Down => new Rectangle(playerBounds.X, playerBounds.Bottom, playerBounds.Width, _settings.Range),
            Direction.Left => new Rectangle(playerBounds.X - _settings.Range, playerBounds.Y, _settings.Range, playerBounds.Height),
            Direction.Right => new Rectangle(playerBounds.Right, playerBounds.Y, _settings.Range, playerBounds.Height),
            _ => playerBounds
        };
    }
}
