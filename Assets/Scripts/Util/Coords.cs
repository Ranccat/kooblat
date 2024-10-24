public struct Coords
{
    public int Row;
    public int Col;

    public Coords(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public Coords CoordsAt(Direction direction)
    {
        return CoordsAt(direction, 1);
    }

    public Coords CoordsAt(Direction direction, int distance)
    {
        return new Coords(Row + direction.rowDelta * distance, Col + direction.colDelta * distance);
    }
}
