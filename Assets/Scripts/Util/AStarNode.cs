using System;

public class AStarNode : IComparable<AStarNode>
{
    public int row { get; private set; }
    public int col { get; private set; }

    public int gCost { get; private set; }
    public int hCost { get; private set; }
    public int fCost => gCost + hCost;

    public AStarNode(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public void SetGCost(int gCost) => this.gCost = gCost;
    public void SetHCost(int hCost) => this.hCost = hCost;

    public int CompareTo(AStarNode other)
    {
        if (fCost == other.fCost)
            return hCost - other.hCost;
        return fCost - other.fCost;
    }
}
