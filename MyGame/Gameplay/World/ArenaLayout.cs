using Microsoft.Xna.Framework;
using MyGame.Gameplay.Props;

namespace MyGame.Gameplay.World;

public static class ArenaLayout
{
    private const int CellSize = 12;
    private const int RowCount = 48;
    private const int ColumnCount = 80;
    private const int WorldWidth = ColumnCount * CellSize;
    private const int WorldHeight = RowCount * CellSize;
    public static Rectangle WorldBounds { get; } = new(0, 0, WorldWidth, WorldHeight);

    private static readonly ArenaWalkableSpan[] WalkableRowSpans = BuildWalkableRowSpans();
    private static readonly Rectangle[] CollisionRectangles = BuildCollisionRectangles(WalkableRowSpans);

    public static IReadOnlyList<Rectangle> CollisionBounds => CollisionRectangles;

    public static ArenaBoundaryProp[] CreateBoundaryProps(bool isVisible = false)
    {
        return CollisionRectangles
            .Select(bounds => new ArenaBoundaryProp(
                new Vector2(bounds.X, bounds.Y),
                new Point(bounds.Width, bounds.Height),
                isVisible))
            .ToArray();
    }

    private static ArenaWalkableSpan[] BuildWalkableRowSpans()
    {
        var spans = Enumerable.Repeat(ArenaWalkableSpan.None, RowCount).ToArray();
        spans[10] = new ArenaWalkableSpan(22, 51);
        spans[11] = new ArenaWalkableSpan(16, 58);
        spans[12] = new ArenaWalkableSpan(13, 64);
        spans[13] = new ArenaWalkableSpan(9, 69);
        spans[14] = new ArenaWalkableSpan(6, 73);
        spans[15] = new ArenaWalkableSpan(3, 75);
        spans[16] = new ArenaWalkableSpan(4, 77);
        spans[17] = new ArenaWalkableSpan(5, 76);
        SetRowRange(spans, 18, 32, new ArenaWalkableSpan(5, 75));
        SetRowRange(spans, 33, 41, new ArenaWalkableSpan(5, 74));
        spans[42] = new ArenaWalkableSpan(5, 75);
        spans[43] = new ArenaWalkableSpan(4, 74);
        spans[44] = new ArenaWalkableSpan(13, 50);
        SetRowRange(spans, 45, 47, new ArenaWalkableSpan(27, 50));
        return spans;
    }

    private static Rectangle[] BuildCollisionRectangles(IReadOnlyList<ArenaWalkableSpan> rowSpans)
    {
        var rectangles = new List<Rectangle>();
        var activeSegments = new Dictionary<BoundarySegmentKey, Rectangle>();

        for (var row = 0; row < rowSpans.Count; row++)
        {
            var currentKeys = new HashSet<BoundarySegmentKey>();
            foreach (var segment in CreateRowSegments(rowSpans[row]))
            {
                var key = new BoundarySegmentKey(segment.X, segment.Width);
                currentKeys.Add(key);

                if (activeSegments.TryGetValue(key, out var activeRectangle))
                {
                    activeSegments[key] = new Rectangle(
                        activeRectangle.X,
                        activeRectangle.Y,
                        activeRectangle.Width,
                        activeRectangle.Height + CellSize);
                    continue;
                }

                activeSegments[key] = new Rectangle(segment.X, row * CellSize, segment.Width, CellSize);
            }

            FlushCompletedSegments(activeSegments, currentKeys, rectangles);
        }

        rectangles.AddRange(activeSegments.Values);
        return rectangles
            .OrderBy(bounds => bounds.Y)
            .ThenBy(bounds => bounds.X)
            .ToArray();
    }

    private static IReadOnlyList<BoundaryRowSegment> CreateRowSegments(ArenaWalkableSpan walkableSpan)
    {
        if (!walkableSpan.HasWalkableTiles)
        {
            return [new BoundaryRowSegment(0, WorldWidth)];
        }

        var segments = new List<BoundaryRowSegment>(2);
        var leftWidth = walkableSpan.StartColumn * CellSize;
        if (leftWidth > 0)
        {
            segments.Add(new BoundaryRowSegment(0, leftWidth));
        }

        var rightStartColumn = walkableSpan.EndColumn + 1;
        var rightX = rightStartColumn * CellSize;
        var rightWidth = WorldWidth - rightX;
        if (rightWidth > 0)
        {
            segments.Add(new BoundaryRowSegment(rightX, rightWidth));
        }

        return segments;
    }

    private static void FlushCompletedSegments(
        Dictionary<BoundarySegmentKey, Rectangle> activeSegments,
        HashSet<BoundarySegmentKey> currentKeys,
        List<Rectangle> rectangles)
    {
        var completedKeys = activeSegments.Keys
            .Where(key => !currentKeys.Contains(key))
            .ToArray();

        foreach (var key in completedKeys)
        {
            rectangles.Add(activeSegments[key]);
            activeSegments.Remove(key);
        }
    }

    private static void SetRowRange(
        ArenaWalkableSpan[] spans,
        int startRow,
        int endRow,
        ArenaWalkableSpan walkableSpan)
    {
        for (var row = startRow; row <= endRow; row++)
        {
            spans[row] = walkableSpan;
        }
    }

    private readonly record struct ArenaWalkableSpan(int StartColumn, int EndColumn)
    {
        public static ArenaWalkableSpan None => new(-1, -2);

        public bool HasWalkableTiles => StartColumn <= EndColumn;
    }

    private readonly record struct BoundarySegmentKey(int X, int Width);

    private readonly record struct BoundaryRowSegment(int X, int Width);
}
