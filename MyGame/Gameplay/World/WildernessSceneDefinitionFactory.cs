using Microsoft.Xna.Framework;
using MyGame.Gameplay.Enemies;
using MyGame.Gameplay.Props;
using MyGame.Scenes.Gameplay;

namespace MyGame.Gameplay.World;

public static class WildernessSceneDefinitionFactory
{
    public static WildernessSceneDefinition CreateNorth()
    {
        var bounds = new Rectangle(0, 0, OverworldLayoutMetrics.WildernessLongSize, OverworldLayoutMetrics.WildernessShortSize);
        var townGateTrigger = new Rectangle(
            (OverworldLayoutMetrics.WildernessLongSize - OverworldLayoutMetrics.TownGateWidth) / 2,
            OverworldLayoutMetrics.WildernessShortSize - 76,
            OverworldLayoutMetrics.TownGateWidth,
            OverworldLayoutMetrics.TransitionThickness);
        var props = CreateVerticalWildernessProps(bounds, townGateTrigger);
        AddNorthWildernessDecor(props);

        return new WildernessSceneDefinition(
            bounds,
            props,
            CreateNorthWildernessSpawns(),
            BuildNorthTransitions(bounds, townGateTrigger));
    }

    public static WildernessSceneDefinition CreateSouth()
    {
        var bounds = new Rectangle(0, 0, OverworldLayoutMetrics.WildernessLongSize, OverworldLayoutMetrics.WildernessShortSize);
        var townGateTrigger = new Rectangle(
            (OverworldLayoutMetrics.WildernessLongSize - OverworldLayoutMetrics.TownGateWidth) / 2,
            32,
            OverworldLayoutMetrics.TownGateWidth,
            OverworldLayoutMetrics.TransitionThickness);
        var props = CreateVerticalWildernessProps(bounds, townGateTrigger);
        AddSouthWildernessDecor(props, bounds);

        return new WildernessSceneDefinition(
            bounds,
            props,
            CreateSouthWildernessSpawns(),
            BuildSouthTransitions(bounds, townGateTrigger));
    }

    public static WildernessSceneDefinition CreateWest()
    {
        var bounds = new Rectangle(0, 0, OverworldLayoutMetrics.WildernessShortSize, OverworldLayoutMetrics.WildernessLongSize);
        var townGateTrigger = new Rectangle(
            OverworldLayoutMetrics.WildernessShortSize - 76,
            (OverworldLayoutMetrics.WildernessLongSize - OverworldLayoutMetrics.TownGateWidth) / 2,
            OverworldLayoutMetrics.TransitionThickness,
            OverworldLayoutMetrics.TownGateWidth);
        var props = CreateHorizontalWildernessProps(bounds, townGateTrigger);
        AddWestWildernessDecor(props, bounds);

        return new WildernessSceneDefinition(
            bounds,
            props,
            CreateWestWildernessSpawns(),
            BuildWestTransitions(bounds, townGateTrigger));
    }

    public static WildernessSceneDefinition CreateEast()
    {
        var bounds = new Rectangle(0, 0, OverworldLayoutMetrics.WildernessShortSize, OverworldLayoutMetrics.WildernessLongSize);
        var townGateTrigger = new Rectangle(
            32,
            (OverworldLayoutMetrics.WildernessLongSize - OverworldLayoutMetrics.TownGateWidth) / 2,
            OverworldLayoutMetrics.TransitionThickness,
            OverworldLayoutMetrics.TownGateWidth);
        var props = CreateHorizontalWildernessProps(bounds, townGateTrigger);
        AddEastWildernessDecor(props, bounds);

        return new WildernessSceneDefinition(
            bounds,
            props,
            CreateEastWildernessSpawns(),
            BuildEastTransitions(bounds, townGateTrigger));
    }

    private static List<IWorldProp> CreateVerticalWildernessProps(Rectangle bounds, Rectangle gateTrigger)
    {
        var props = new List<IWorldProp>();

        if (gateTrigger.Y < bounds.Center.Y)
        {
            props.Add(new MountainProp(
                new Vector2(bounds.Left, bounds.Bottom - OverworldLayoutMetrics.MountainThickness),
                new Point(bounds.Width, OverworldLayoutMetrics.MountainThickness)));
            AddTopMountainWithGate(props, bounds, gateTrigger);
        }
        else
        {
            props.Add(new MountainProp(
                new Vector2(bounds.Left, bounds.Top),
                new Point(bounds.Width, OverworldLayoutMetrics.MountainThickness)));
            AddBottomMountainWithGate(props, bounds, gateTrigger);
        }

        AddSharedWildernessDecor(props, bounds);
        return props;
    }

    private static List<IWorldProp> CreateHorizontalWildernessProps(Rectangle bounds, Rectangle gateTrigger)
    {
        var props = new List<IWorldProp>();

        if (gateTrigger.X < bounds.Center.X)
        {
            props.Add(new MountainProp(
                new Vector2(bounds.Right - OverworldLayoutMetrics.MountainThickness, bounds.Top),
                new Point(OverworldLayoutMetrics.MountainThickness, bounds.Height)));
            AddLeftMountainWithGate(props, bounds, gateTrigger);
        }
        else
        {
            props.Add(new MountainProp(
                new Vector2(bounds.Left, bounds.Top),
                new Point(OverworldLayoutMetrics.MountainThickness, bounds.Height)));
            AddRightMountainWithGate(props, bounds, gateTrigger);
        }

        AddSharedWildernessDecor(props, bounds);
        return props;
    }

    private static IReadOnlyList<WorldSceneTransition> BuildNorthTransitions(Rectangle bounds, Rectangle townGateTrigger)
    {
        return
        [
            new WorldSceneTransition(
                townGateTrigger,
                GameplaySceneNames.Overworld,
                new Vector2(OverworldLayoutMetrics.TownNorthGateTrigger.X, 144f)),
            new WorldSceneTransition(
                CreateLeftEdgeTrigger(bounds),
                GameplaySceneNames.WildernessWest,
                world => new Vector2(
                    MapVerticalStripPositionToHorizontalStrip(world.Player.Position.Y, reverse: false),
                    OverworldLayoutMetrics.WildernessEdgeEntryInset)),
            new WorldSceneTransition(
                CreateRightEdgeTrigger(bounds),
                GameplaySceneNames.WildernessEast,
                world => new Vector2(
                    MapVerticalStripPositionToHorizontalStrip(world.Player.Position.Y, reverse: true),
                    OverworldLayoutMetrics.WildernessEdgeEntryInset))
        ];
    }

    private static IReadOnlyList<WorldSceneTransition> BuildSouthTransitions(Rectangle bounds, Rectangle townGateTrigger)
    {
        return
        [
            new WorldSceneTransition(
                townGateTrigger,
                GameplaySceneNames.Overworld,
                new Vector2(OverworldLayoutMetrics.TownSouthGateTrigger.X, OverworldLayoutMetrics.TownSize - 188f)),
            new WorldSceneTransition(
                CreateLeftEdgeTrigger(bounds),
                GameplaySceneNames.WildernessWest,
                world => new Vector2(
                    MapVerticalStripPositionToHorizontalStrip(world.Player.Position.Y, reverse: true),
                    OverworldLayoutMetrics.WildernessLongSize - OverworldLayoutMetrics.WildernessEdgeEntryInset)),
            new WorldSceneTransition(
                CreateRightEdgeTrigger(bounds),
                GameplaySceneNames.WildernessEast,
                world => new Vector2(
                    MapVerticalStripPositionToHorizontalStrip(world.Player.Position.Y, reverse: false),
                    OverworldLayoutMetrics.WildernessLongSize - OverworldLayoutMetrics.WildernessEdgeEntryInset))
        ];
    }

    private static IReadOnlyList<WorldSceneTransition> BuildWestTransitions(Rectangle bounds, Rectangle townGateTrigger)
    {
        return
        [
            new WorldSceneTransition(
                townGateTrigger,
                GameplaySceneNames.Overworld,
                new Vector2(144f, OverworldLayoutMetrics.TownWestGateTrigger.Y)),
            new WorldSceneTransition(
                CreateTopEdgeTrigger(bounds),
                GameplaySceneNames.WildernessNorth,
                world => new Vector2(
                    OverworldLayoutMetrics.WildernessEdgeEntryInset,
                    MapHorizontalStripPositionToVerticalStrip(world.Player.Position.X, reverse: false))),
            new WorldSceneTransition(
                CreateBottomEdgeTrigger(bounds),
                GameplaySceneNames.WildernessSouth,
                world => new Vector2(
                    OverworldLayoutMetrics.WildernessEdgeEntryInset,
                    MapHorizontalStripPositionToVerticalStrip(world.Player.Position.X, reverse: true)))
        ];
    }

    private static IReadOnlyList<WorldSceneTransition> BuildEastTransitions(Rectangle bounds, Rectangle townGateTrigger)
    {
        return
        [
            new WorldSceneTransition(
                townGateTrigger,
                GameplaySceneNames.Overworld,
                new Vector2(OverworldLayoutMetrics.TownSize - 188f, OverworldLayoutMetrics.TownEastGateTrigger.Y)),
            new WorldSceneTransition(
                CreateTopEdgeTrigger(bounds),
                GameplaySceneNames.WildernessNorth,
                world => new Vector2(
                    OverworldLayoutMetrics.WildernessLongSize - OverworldLayoutMetrics.WildernessEdgeEntryInset,
                    MapHorizontalStripPositionToVerticalStrip(world.Player.Position.X, reverse: true))),
            new WorldSceneTransition(
                CreateBottomEdgeTrigger(bounds),
                GameplaySceneNames.WildernessSouth,
                world => new Vector2(
                    OverworldLayoutMetrics.WildernessLongSize - OverworldLayoutMetrics.WildernessEdgeEntryInset,
                    MapHorizontalStripPositionToVerticalStrip(world.Player.Position.X, reverse: false)))
        ];
    }

    private static IReadOnlyList<EnemySpawnDefinition> CreateNorthWildernessSpawns()
    {
        return
        [
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(540f, 240f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(1320f, 260f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(820f, 180f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Grasshopper, new Vector2(1040f, 420f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Skeleton, new Vector2(932f, 282f), EnemyAxisPreference.None)
        ];
    }

    private static IReadOnlyList<EnemySpawnDefinition> CreateSouthWildernessSpawns()
    {
        return
        [
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(460f, 520f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(1410f, 500f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(920f, 330f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Grasshopper, new Vector2(760f, 620f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Skeleton, new Vector2(1048f, 546f), EnemyAxisPreference.None)
        ];
    }

    private static IReadOnlyList<EnemySpawnDefinition> CreateWestWildernessSpawns()
    {
        return
        [
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(260f, 540f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(240f, 1360f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(420f, 860f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Grasshopper, new Vector2(560f, 1120f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Skeleton, new Vector2(392f, 978f), EnemyAxisPreference.None)
        ];
    }

    private static IReadOnlyList<EnemySpawnDefinition> CreateEastWildernessSpawns()
    {
        return
        [
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(620f, 620f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.HornedRabbit, new Vector2(600f, 1320f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Bat, new Vector2(440f, 980f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Grasshopper, new Vector2(250f, 760f), EnemyAxisPreference.None),
            new EnemySpawnDefinition(EnemyKind.Skeleton, new Vector2(352f, 1114f), EnemyAxisPreference.None)
        ];
    }

    private static void AddSharedWildernessDecor(List<IWorldProp> props, Rectangle bounds)
    {
        props.AddRange(
        [
            new TreeProp(new Vector2(bounds.Left + 220f, bounds.Top + 180f), new Point(76, 110)),
            new TreeProp(new Vector2(bounds.Right - 340f, bounds.Top + 220f), new Point(76, 110)),
            new TreeProp(new Vector2(bounds.Left + 300f, bounds.Bottom - 340f), new Point(76, 110)),
            new GrassProp(new Vector2(bounds.Center.X - 180f, bounds.Center.Y - 40f), new Point(58, 36)),
            new GrassProp(new Vector2(bounds.Center.X + 120f, bounds.Center.Y + 60f), new Point(58, 36))
        ]);
    }

    private static void AddNorthWildernessDecor(List<IWorldProp> props)
    {
        props.AddRange(
        [
            new TreeProp(new Vector2(512f, 118f), new Point(76, 110)),
            new TreeProp(new Vector2(1308f, 128f), new Point(76, 110)),
            new GrassProp(new Vector2(728f, 314f), new Point(66, 38)),
            new GrassProp(new Vector2(1134f, 352f), new Point(66, 38))
        ]);
    }

    private static void AddSouthWildernessDecor(List<IWorldProp> props, Rectangle bounds)
    {
        props.AddRange(
        [
            new TreeProp(new Vector2(462f, bounds.Bottom - 430f), new Point(76, 110)),
            new TreeProp(new Vector2(1370f, bounds.Bottom - 420f), new Point(76, 110)),
            new GrassProp(new Vector2(638f, bounds.Bottom - 262f), new Point(66, 38)),
            new GrassProp(new Vector2(1116f, bounds.Bottom - 282f), new Point(66, 38))
        ]);
    }

    private static void AddWestWildernessDecor(List<IWorldProp> props, Rectangle bounds)
    {
        props.AddRange(
        [
            new TreeProp(new Vector2(174f, 462f), new Point(76, 110)),
            new TreeProp(new Vector2(226f, 1228f), new Point(76, 110)),
            new GrassProp(new Vector2(bounds.Center.X - 112f, 684f), new Point(66, 38)),
            new GrassProp(new Vector2(bounds.Center.X - 96f, 1436f), new Point(66, 38))
        ]);
    }

    private static void AddEastWildernessDecor(List<IWorldProp> props, Rectangle bounds)
    {
        props.AddRange(
        [
            new TreeProp(new Vector2(bounds.Right - 292f, 534f), new Point(76, 110)),
            new TreeProp(new Vector2(bounds.Right - 284f, 1314f), new Point(76, 110)),
            new GrassProp(new Vector2(bounds.Center.X - 42f, 754f), new Point(66, 38)),
            new GrassProp(new Vector2(bounds.Center.X - 64f, 1478f), new Point(66, 38))
        ]);
    }

    private static void AddTopMountainWithGate(List<IWorldProp> props, Rectangle bounds, Rectangle gateTrigger)
    {
        props.Add(new MountainProp(new Vector2(bounds.Left, bounds.Top), new Point(gateTrigger.X, OverworldLayoutMetrics.MountainThickness)));
        props.Add(new MountainProp(
            new Vector2(gateTrigger.Right, bounds.Top),
            new Point(bounds.Width - gateTrigger.Right, OverworldLayoutMetrics.MountainThickness)));
    }

    private static void AddBottomMountainWithGate(List<IWorldProp> props, Rectangle bounds, Rectangle gateTrigger)
    {
        props.Add(new MountainProp(new Vector2(bounds.Left, bounds.Bottom - OverworldLayoutMetrics.MountainThickness), new Point(gateTrigger.X, OverworldLayoutMetrics.MountainThickness)));
        props.Add(new MountainProp(
            new Vector2(gateTrigger.Right, bounds.Bottom - OverworldLayoutMetrics.MountainThickness),
            new Point(bounds.Width - gateTrigger.Right, OverworldLayoutMetrics.MountainThickness)));
    }

    private static void AddLeftMountainWithGate(List<IWorldProp> props, Rectangle bounds, Rectangle gateTrigger)
    {
        props.Add(new MountainProp(new Vector2(bounds.Left, bounds.Top), new Point(OverworldLayoutMetrics.MountainThickness, gateTrigger.Y)));
        props.Add(new MountainProp(
            new Vector2(bounds.Left, gateTrigger.Bottom),
            new Point(OverworldLayoutMetrics.MountainThickness, bounds.Height - gateTrigger.Bottom)));
    }

    private static void AddRightMountainWithGate(List<IWorldProp> props, Rectangle bounds, Rectangle gateTrigger)
    {
        props.Add(new MountainProp(new Vector2(bounds.Right - OverworldLayoutMetrics.MountainThickness, bounds.Top), new Point(OverworldLayoutMetrics.MountainThickness, gateTrigger.Y)));
        props.Add(new MountainProp(
            new Vector2(bounds.Right - OverworldLayoutMetrics.MountainThickness, gateTrigger.Bottom),
            new Point(OverworldLayoutMetrics.MountainThickness, bounds.Height - gateTrigger.Bottom)));
    }

    private static Rectangle CreateLeftEdgeTrigger(Rectangle bounds)
    {
        return new Rectangle(bounds.Left, bounds.Top, OverworldLayoutMetrics.TransitionThickness, bounds.Height);
    }

    private static Rectangle CreateRightEdgeTrigger(Rectangle bounds)
    {
        return new Rectangle(bounds.Right - OverworldLayoutMetrics.TransitionThickness, bounds.Top, OverworldLayoutMetrics.TransitionThickness, bounds.Height);
    }

    private static Rectangle CreateTopEdgeTrigger(Rectangle bounds)
    {
        return new Rectangle(bounds.Left, bounds.Top, bounds.Width, OverworldLayoutMetrics.TransitionThickness);
    }

    private static Rectangle CreateBottomEdgeTrigger(Rectangle bounds)
    {
        return new Rectangle(bounds.Left, bounds.Bottom - OverworldLayoutMetrics.TransitionThickness, bounds.Width, OverworldLayoutMetrics.TransitionThickness);
    }

    private static float MapVerticalStripPositionToHorizontalStrip(float sourcePosition, bool reverse)
    {
        var clamped = MathHelper.Clamp(sourcePosition, 0f, OverworldLayoutMetrics.WildernessShortSize);
        var mapped = reverse ? OverworldLayoutMetrics.WildernessShortSize - clamped : clamped;
        return MathHelper.Clamp(
            mapped,
            OverworldLayoutMetrics.WildernessEdgeEntryInset,
            OverworldLayoutMetrics.WildernessShortSize - OverworldLayoutMetrics.WildernessEdgeEntryInset);
    }

    private static float MapHorizontalStripPositionToVerticalStrip(float sourcePosition, bool reverse)
    {
        var clamped = MathHelper.Clamp(sourcePosition, 0f, OverworldLayoutMetrics.WildernessShortSize);
        var mapped = reverse ? OverworldLayoutMetrics.WildernessShortSize - clamped : clamped;
        return MathHelper.Clamp(
            mapped,
            OverworldLayoutMetrics.WildernessEdgeEntryInset,
            OverworldLayoutMetrics.WildernessShortSize - OverworldLayoutMetrics.WildernessEdgeEntryInset);
    }
}
