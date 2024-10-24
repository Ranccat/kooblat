using UnityEngine;
using System.Collections.Generic;

public static class AStar
{
    private static AStarNode[,] _nodes;
    private static Maze _maze;

    public static void Configure(Maze maze)
    {
        _nodes = new AStarNode[maze.RowCount, maze.ColCount];

        for (int row = 0; row < maze.RowCount; row++)
        {
            for (int col = 0; col < maze.ColCount; col++)
                _nodes[row, col] = new AStarNode(row, col);
        }

        _maze = maze;
    }

    private static readonly int[] dRow = { 1, 0, -1, 0 };
    private static readonly int[] dCol = { 0, 1, 0, -1 };

    private const int costDiag = 14;
    private const int costDirect = 10;

    private static int GetDistance(AStarNode node0, AStarNode node1)
    {
        int rowDelta = Mathf.Abs(node0.row - node1.row);
        int colDelta = Mathf.Abs(node0.col - node1.col);

        int numDiags = Mathf.Min(rowDelta, colDelta);
        int numDirects = Mathf.Max(rowDelta, colDelta) - numDiags;

        return numDiags * costDiag + numDirects * costDirect;
    }

    public static List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos)
    {
        AStarNode startNode;
        AStarNode targetNode;
        {
            Coords startCoords = _maze.GetCoords(startPos);
            Coords targetCoords = _maze.GetCoords(targetPos);

            startNode = _nodes[startCoords.Row, startCoords.Col];
            targetNode = _nodes[targetCoords.Row, targetCoords.Col];
        }

        startNode.SetGCost(0);
        startNode.SetHCost(0);

        MinHeap<AStarNode> queue = new MinHeap<AStarNode>();
        queue.Push(startNode);

        HashSet<AStarNode> visited = new HashSet<AStarNode>();
        Dictionary<AStarNode, AStarNode> prev = new Dictionary<AStarNode, AStarNode>();

        List<Vector2> path = new List<Vector2>();

        while (queue.Count > 0)
        {
            AStarNode currentNode = queue.Pop();
            visited.Add(currentNode);

            if (currentNode == targetNode)
            {
                Vector2 prevPoint = targetPos;
                Vector2 prevDirection = Vector2.zero;

                while (currentNode != startNode)
                {
                    Vector2 curPoint = _maze.GetPosition(currentNode.row, currentNode.col);
                    Vector2 curDirection = prevPoint - curPoint;

                    if (curDirection != prevDirection)
                    {
                        path.Add(prevPoint);
                        prevDirection = curDirection;
                    }

                    prevPoint = curPoint;
                    currentNode = prev[currentNode];
                }

                path.Add(prevPoint);
                path.Reverse();

                return path;
            }

            for (int d = 0; d < 4; d++)
            {
                int nRow = currentNode.row + dRow[d];
                int nCol = currentNode.col + dCol[d];

                if (nRow < 0 || nRow >= _maze.RowCount || nCol < 0 || nCol >= _maze.ColCount
                    || _maze.GetBlockAt(nRow, nCol) != null)
                    continue;

                AStarNode neighbor = _nodes[nRow, nCol];

                if (visited.Contains(neighbor))
                    continue;

                bool inQueue = queue.Contains(neighbor);
                int nGCost = currentNode.gCost + costDirect;
                if (!inQueue || nGCost < neighbor.gCost)
                {
                    neighbor.SetGCost(nGCost);
                    prev[neighbor] = currentNode;

                    if (!inQueue)
                    {
                        neighbor.SetHCost(GetDistance(neighbor, targetNode));
                        queue.Push(neighbor);
                    }
                }
            }
        }

        // This shouldn't happen; as there should always be a path between 2 points in the maze
        Debug.LogError("Failed to find path!");
        return null;
    }
}
