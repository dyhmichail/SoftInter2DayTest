
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Threading;
using UnityEngine;
using MessagePipe;
using DisposableBag = MessagePipe.DisposableBag;
using VContainer.Unity;
using Grid = TestTaskMike.Domain.Grid;

namespace TestTaskMike.Application
{
    public class EconomyService : IInitializable, IDisposable
    {
        private readonly ReactiveProperty<int> _gold = new(0);
        public ReadOnlyReactiveProperty<int> Gold => _gold;

        private readonly IAsyncSubscriber<GameStateEnum> _gameStateSubscriber;
        private readonly Grid grid;

        private IDisposable subscription;
        private IDisposable incomeSubscription;

        public EconomyService(
            IAsyncSubscriber<GameStateEnum> gameStateSubscriber,
            Grid grid)
        {
            _gameStateSubscriber = gameStateSubscriber;
            this.grid = grid;
        }

        public void Initialize()
        {
            var bag = DisposableBag.CreateBuilder();

            _gameStateSubscriber.Subscribe(async (stateEnum, token) =>
            {
                if (stateEnum == GameStateEnum.MainGameLoop)
                {
                    Debug.Log("EconomyService Initialize - Entering MainGameLoop");

                    if (incomeSubscription == null)
                    {
                        incomeSubscription = StartCollectIncomeWithR3(grid, 1);
                    }
                }
                else
                {
                    if (incomeSubscription != null)
                    {
                        Debug.Log("EconomyService - Leaving MainGameLoop, stopping income collection");
                        StopCollectIncome(incomeSubscription);
                        incomeSubscription = null;
                    }
                }

            }).AddTo(bag);

            subscription = bag.Build();
        }

        public void Dispose()
        {
            subscription?.Dispose();
            incomeSubscription?.Dispose();
            incomeSubscription = null;
        }

        public void AddGold(int amount)
        {
            if (amount < 0)
                throw new System.ArgumentException("Amount must be non-negative", nameof(amount));
            Debug.Log("Add Gold: " + amount);
            _gold.Value += amount;
            Debug.Log("Total Gold: " + Gold);
        }

        public IDisposable StartCollectIncomeWithR3(Grid grid, float periodSeconds = 1)
        {
            return Observable
                .Interval(TimeSpan.FromSeconds(periodSeconds))
                .SubscribeAwait(async (_, ct) =>
                {
                    await CollectIncomeAsync(grid, ct);
                });
        }

        public void StopCollectIncome(IDisposable subscription)
        {
            subscription?.Dispose();
        }

        public async UniTask CollectIncomeAsync(Grid grid, CancellationToken cancellationToken = default)
        {
            if (grid == null || grid.cells == null) return;

            int width = grid.cells.GetLength(0);
            int height = grid.cells.GetLength(1);
            int total = 0;


            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var cell = grid.cells[x, y];
                    if (cell == null) continue;
                    if (!cell.HasBuilding.Item1) continue;
                    var building = cell.HasBuilding.Item2;

                    total += building.GetIncomePerTick();
                }
            }

            if (total > 0)
                AddGold(total);
        }
    }
}
