
namespace TestTaskMike.Domain
{
    public class Grid
    {
        public readonly Cell[,] cells;
        public readonly int width;
        public readonly int height;

        public Grid(int width, int height)
        {
            this.width = width;
            this.height = height;

            cells = new Cell[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells[x, y] = new Cell(x, y);
                }
            }
        }

        public Cell GetCell(int x, int y)
        {
            return cells[x, y];
        }
    }
}
