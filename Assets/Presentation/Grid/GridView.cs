using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GridView : MonoBehaviour
{
    [SerializeField] CellView cellViewPrefab;
    [SerializeField] float cellDistance;

    [Inject] private IObjectResolver resolver;

    public bool initialized;
    internal void Initialize(Grid grid)
    {
        var cells = grid.cells;
        int width = cells.GetLength(0);
        int height = cells.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = cells[x, y];
                var go = resolver.Instantiate(cellViewPrefab.gameObject, this.transform);
                CellView cellView = go.GetComponent<CellView>();
                cellView.name = $"Cell_{x + 1}_{y + 1}";
                cellView.transform.position = new Vector3(x * cellDistance, y * cellDistance, 0);
                cellView.Initialize(cell);
            }
        }

        initialized = true;
    }
}
