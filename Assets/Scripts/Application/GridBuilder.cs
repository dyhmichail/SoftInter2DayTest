using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using TestTaskMike.Application;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Grid = TestTaskMike.Domain.Grid;

namespace TestTaskMike
{
    public readonly struct GridInitialized
    {
        public readonly int Width;
        public readonly int Height;

        public GridInitialized(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }

    public readonly struct RequestGridInitialization
    {

    }

    public class GridBuilder : IInitializable, IDisposable
    {
        private readonly IAsyncSubscriber<GameStateEnum> _gameStateSubscriber;
        private readonly Grid _grid;

        [Inject]
        private readonly IAsyncPublisher<RequestGridInitialization> _gridInitPublisher;
        private readonly IAsyncPublisher<GridInitialized> _gridInitializedPublisher;

        private IDisposable _subscription;

        public GridBuilder(
            IAsyncSubscriber<GameStateEnum> gameStateSubscriber,
            Grid grid,
            IAsyncPublisher<RequestGridInitialization> gridInitPublisher,
            IAsyncPublisher<GridInitialized> gridInitializedPublisher)
        {
            _gameStateSubscriber = gameStateSubscriber;
            _grid = grid;
            _gridInitPublisher = gridInitPublisher;
            _gridInitializedPublisher = gridInitializedPublisher;
        }

        public void Initialize()
        {
            var bag = DisposableBag.CreateBuilder();

            _gameStateSubscriber.Subscribe(async (stateEnum, token) =>
            {
                if (stateEnum != GameStateEnum.GenerateGrid)
                    return;

                Debug.Log("GridBuilder Initialize");

                await _gridInitPublisher.PublishAsync(new RequestGridInitialization());

            }).AddTo(bag);

            _subscription = bag.Build();
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
