using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Enemies;

public static class CrabSpriteSheet
{
    private static readonly string[] RawSheetRows =
    [
        "............................r............................r............................r...............",
        "..........................rrrrr........................rrrrr........................rrrrr..............",
        ".........................rrswsrr......................rrswsrr......................rrswsrr.............",
        "...............oo........rsssssr........oo..........oorsssssrroo..........oo........rsssssr........oo...",
        "..............oooo......rrsssssrr......oooo.........ooorrrrrrroo.........oooo......rrsssssrr......oooo..",
        "..............oorrr.....rrsssssrr.....rrroo..........oorrrrrroo..........ooorrr....rrsssssrr.....rrroo..",
        "...............orrrrr..rrrsssssrrr..rrrrro............orrrrrro............orrrrr..rrrsssssrrr..rrrrro...",
        "................orrrrrrrrrrrrrrrrrrrrrro...............orrrrrro...........orrrrrrrrrrrrrrrrrrrrrro......",
        ".................orrrrrrrrrrrrrrrrrrrro.................orrrrrro...........orrrrrrrrrrrrrrrrrrrro.......",
        "..................krrrrrrrrrrrrrrrrrrk..................krrrrrrk............krrrrrrrrrrrrrrrrrrk........",
        "..................kkrrkk....kkrrkk......................kkrrkk..............kkrrkk....kkrrkk............",
        ".................kk..kk......kk..kk....................kk..kk................kk..kk......kk..kk.........",
        "................kk...k........k...kk..................kk...k................kk...k........k...kk........",
        "........................................................................................................",
        "........................................................................................................",
        "........................................................................................................"
    ];

    public static IReadOnlyList<string> Rows => SheetRows;

    public static int FrameWidth => 32;

    public static int FrameHeight => 16;

    public static int Frames => 3;

    private static readonly string[] SheetRows = RawSheetRows
        .Select(row => row.PadRight(FrameWidth * Frames, '.')[..(FrameWidth * Frames)])
        .ToArray();

    public static int SheetWidth => SheetRows[0].Length;

    public static int SheetHeight => SheetRows.Length;

    public static Rectangle GetSourceRectangle(int frameIndex)
    {
        return new Rectangle(frameIndex * FrameWidth, 0, FrameWidth, FrameHeight);
    }
}
