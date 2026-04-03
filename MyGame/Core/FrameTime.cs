using Microsoft.Xna.Framework;

namespace MyGame.Core;

public sealed class FrameTime
{
    public static readonly FrameTime Zero = new(TimeSpan.Zero, TimeSpan.Zero);

    public FrameTime(TimeSpan elapsed, TimeSpan total)
    {
        Elapsed = elapsed;
        Total = total;
        DeltaSeconds = (float)elapsed.TotalSeconds;
        TotalSeconds = (float)total.TotalSeconds;
    }

    public TimeSpan Elapsed { get; }

    public TimeSpan Total { get; }

    public float DeltaSeconds { get; }

    public float TotalSeconds { get; }

    public static FrameTime From(GameTime gameTime)
    {
        return new FrameTime(gameTime.ElapsedGameTime, gameTime.TotalGameTime);
    }
}
