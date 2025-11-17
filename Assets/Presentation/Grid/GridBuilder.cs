using System;
using Cysharp.Threading.Tasks;
using MessagePipe;
using TestTaskMike.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using VContainer.Unity;

namespace TestTaskMike.Presenter
{
    public class GridBuilder : IInitializable, IDisposable
    {
        private readonly IAsyncSubscriber<GameStateEnum> _gameStateSubscriber;
        private readonly Grid grid;
        private readonly GridView gridView;

        private IDisposable subscription;

        public GridBuilder(
            IAsyncSubscriber<GameStateEnum> gameStateSubscriber,
            Grid grid,
            GridView gridView)
        {
            _gameStateSubscriber = gameStateSubscriber;
            this.grid = grid;
            this.gridView = gridView;
        }

        public void Initialize()
        {
            var bag = DisposableBag.CreateBuilder();

            _gameStateSubscriber.Subscribe(async (stateEnum, token) =>
            {
                if (stateEnum != GameStateEnum.GenerateGrid)
                    return;

                Debug.Log("GridBuilder Initialize");

                gridView.Initialize(grid);

                while (!gridView.initialized)
                {
                    await UniTask.Yield();
                }

            }).AddTo(bag);

            subscription = bag.Build();
        }

        public void Dispose()
        {
            subscription?.Dispose();
        }
    }
}
