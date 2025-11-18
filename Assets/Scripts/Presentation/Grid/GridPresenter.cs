using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TestTaskMike.Presentation
{
    public class GridPresenter : MonoBehaviour
    {
        [SerializeField] CellView cellViewPrefab;
        [SerializeField] float cellDistance;

        [Inject] private IObjectResolver resolver;

        public bool initialized;

        internal void Initialize(int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var go = resolver.Instantiate(cellViewPrefab.gameObject, this.transform);
                    CellView cellView = go.GetComponent<CellView>();
                    cellView.name = $"Cell_{x + 1}_{y + 1}";
                    cellView.transform.position = new Vector3(x * cellDistance, y * cellDistance, 0);
                    cellView.Initialize(new Vector2Int(x, y));
                }
            }

            initialized = true;
        }
    }
}