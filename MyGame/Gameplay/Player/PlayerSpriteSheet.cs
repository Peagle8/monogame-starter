using Microsoft.Xna.Framework;

namespace MyGame.Gameplay.Player;

public static class PlayerSpriteSheet
{
    private static readonly string[] RawSheetRows =
    [
        "....................hhhhhh....................hhhhhh....................hhhhhh............",
        "...................hhhhhhhh...................hhhhhhhh...................hhhhhhhh...........",
        "..................hhsssssshh..................hhsssssshh..................hhsssssshh..........",
        "..................hssssssssh..................hssssssssh..................hssssssssh..........",
        "..................hssessessh..................hssessessh..................hssessessh..........",
        "...................ssssssss...................ssssssss...................ssssssss...........",
        "..................cccssssccc..................cccssssccc..................cccssssccc..........",
        ".................cccccccccccc.................cccccccccccc.................cccccccccccc.........",
        ".................cccccccccccc.................cccccccccccc.................cccccccccccc.........",
        "..................ccccbbbbcc..................cccccbbccc...................ccccbbcc...........",
        "...................cccbbbbc...................cccbbbbccc....................ccbbbbc............",
        "...................bbb..bbb...................cccbbbbccc....................bbb..bbb...........",
        "..................bbbb..bbbb..................bbb....bbb...................bbbb..bbbb..........",
        "..................bb......bb..................bbb....bbb...................bb......bb..........",
        "................................................................................................",
        "................................................................................................",

        "....................hhhhhh....................hhhhhh....................hhhhhh............",
        "...................hhhhhhhh...................hhhhhhhh...................hhhhhhhh...........",
        "..................hhsssssshh..................hhsssssshh..................hhsssssshh..........",
        "..................hssssssssh..................hssssssssh..................hssssssssh..........",
        "..................hssessessh..................hssessessh..................hssessessh..........",
        "...................ssssssss...................ssssssss...................ssssssss...........",
        "..................cccssssccc..................cccssssccc..................cccssssccc..........",
        ".................cccccccccccc.................cccccccccccc.................cccccccccccc.........",
        ".................cccccccccccc.................cccccccccccc.................cccccccccccc.........",
        "..................cccccbbccc..................cccccbbccc..................cccccbbccc..........",
        "...................cccbbbbccc.................cccbbbbccc...................cccbbbbccc.........",
        "...................cccbbbbccc.................cccbbbbccc...................cccbbbbccc.........",
        "....................bbb..bbb..................bbbb..bbbb....................bbb..bbb..........",
        "...................bbbb..bbbb..................bb....bb...................bbbb..bbbb..........",
        "................................................................................................",
        "................................................................................................",

        "....................hhhhhh....................hhhhhh....................hhhhhh............",
        "...................hhhhhhhh...................hhhhhhhh...................hhhhhhhh...........",
        "..................hhsssssshh..................hhsssssshh..................hhsssssshh..........",
        "..................hssssssssh..................hssssssssh..................hssssssssh..........",
        "..................hssessessh..................hssessessh..................hssessessh..........",
        "...................ssssssss...................ssssssss...................ssssssss...........",
        "..................ccccsscccc..................ccccsscccc..................ccccsscccc..........",
        ".................cccccccccccc.................cccccccccccc.................cccccccccccc.........",
        ".................cccccbbccccc.................cccccbbccccc.................cccccbbccccc.........",
        "..................ccccbbbbcc...................cccbbbbccc..................ccccbbbbcc..........",
        "...................cccbbbbc....................ccbbbbcc....................cccbbbbc...........",
        "..................bbbb..bbb...................bbbb..bbb...................bbbb..bbb...........",
        "..................bb....bbbb..................bb....bbbb..................bb....bbbb..........",
        "...................b......bb..................b......bb...................b......bb...........",
        "................................................................................................",
        "................................................................................................",

        "....................hhhhhh....................hhhhhh....................hhhhhh............",
        "...................hhhhhhhh...................hhhhhhhh...................hhhhhhhh...........",
        "..................hhsssssshh..................hhsssssshh..................hhsssssshh..........",
        "..................hssssssssh..................hssssssssh..................hssssssssh..........",
        "..................hssessessh..................hssessessh..................hssessessh..........",
        "...................ssssssss...................ssssssss...................ssssssss...........",
        "..................cccssssccc..................cccssssccc..................cccssssccc..........",
        ".................cccccccccccc.................cccccccccccc.................cccccccccccc.........",
        ".................cccccccccccc.................cccccccccccc.................cccccccccccc.........",
        "...................cccbbcccc..................ccccbbcccc...................cccbbcccc..........",
        "...................ccbbbbccc..................ccbbbbccc...................ccbbbbccc..........",
        "...................bbb..bbbb..................bbb..bbbb...................bbb..bbbb..........",
        "..................bbb....bbb..................bbb....bbb..................bbb....bbb..........",
        "..................bb......bb..................bb......bb..................bb......bb..........",
        "................................................................................................",
        "................................................................................................"
    ];

    private static readonly string[] SheetRows = RawSheetRows
        .Select(row => row.PadRight(RawSheetRows.Max(candidate => candidate.Length), '.'))
        .ToArray();

    public static IReadOnlyList<string> Rows => SheetRows;

    public static int FrameWidth => 32;

    public static int FrameHeight => 16;

    public static int FramesPerDirection => 3;

    public static int SheetWidth => SheetRows[0].Length;

    public static int SheetHeight => SheetRows.Length;

    public static Rectangle GetSourceRectangle(Direction direction, int frameIndex)
    {
        return new Rectangle(frameIndex * FrameWidth, ((int)direction) * FrameHeight, FrameWidth, FrameHeight);
    }
}
