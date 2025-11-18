using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Grid = TestTaskMike.Domain.Grid;

namespace TestTaskMike.Presentation
{
    public class GridInitializationService : IInitializable, IDisposable
    {
        private readonly GridPresenter _gridPresenter;
        private readonly Grid _grid;
        private readonly IAsyncSubscriber<RequestGridInitialization> _gridInitSubscriber;

        [Inject]
        private readonly IAsyncPublisher<GridInitialized> _gridInitializedPublisher;

        private IDisposable _subscription;

        public GridInitializationService(
            GridPresenter gridPresenter,
            Grid grid,
            IAsyncSubscriber<RequestGridInitialization> gridInitSubscriber,
            IAsyncPublisher<GridInitialized> gridInitializedPublisher)
        {
            _gridPresenter = gridPresenter;
            _grid = grid;
            _gridInitSubscriber = gridInitSubscriber;
            _gridInitializedPublisher = gridInitializedPublisher;
        }

        public void Initialize()
        {
            _subscription = _gridInitSubscriber.Subscribe(async (request, ct) =>
            {
                await InitializeGridAsync();
            });
        }

        private async UniTask InitializeGridAsync()
        {
            Debug.Log("GridInitializationService: Initializing grid view");

            // Initialize the grid presenter
            _gridPresenter.Initialize(_grid.width, _grid.height);

            // Wait for initialization to complete
            while (!_gridPresenter.initialized)
            {
                await UniTask.Yield();
            }

            // Notify that grid is initialized
            await _gridInitializedPublisher.PublishAsync(new GridInitialized(_grid.width, _grid.height));

            Debug.Log("GridInitializationService: Grid view initialized");
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}