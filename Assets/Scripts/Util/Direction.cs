using UnityEngine;
using System.Collections.Generic;

public struct Direction
{
    public readonly int rowDelta;
    public readonly int colDelta;
    public readonly Vector2 vector;

    private Direction(int rowDelta, int colDelta, Vector2 vector)
    {
        this.rowDelta = rowDelta;
        this.colDelta = colDelta;
        this.vector = vector;
    }

    public static Direction right = new Direction(0, 1, Vector2.right);
    public static Direction down = new Direction(1, 0, Vector2.down);
    public static Direction left = new Direction(0, -1, Vector2.left);
    public static Direction up = new Direction(-1, 0, Vector2.up);

    public static IList<Direction> directions = new List<Direction> { right, down, left, up }.AsReadOnly();
}
