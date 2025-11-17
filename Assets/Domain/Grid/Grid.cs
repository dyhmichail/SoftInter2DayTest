using MessagePipe;

public class Grid
{
    public readonly Cell[,] cells;
    private readonly int width;
    private readonly int height;

    // Accept a CellClicked subscriber so each cell can subscribe if needed.
    public Grid(int width, int height, IAsyncSubscriber<CellClicked> cellClickedSubscriber = null)
    {
        this.width = width;
        this.height = height;

        cells = new Cell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[x, y] = new Cell(x, y, cellClickedSubscriber);
            }
        }
    }

    public Cell GetCell(int x, int y)
    {
        return cells[x, y];
    }
}
