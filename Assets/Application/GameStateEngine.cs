using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;
using VContainer.Unity;

namespace TestTaskMike.Gameplay
{
    public enum GameStateEnum
    {
        GenerateGrid = 0,
        LoadSavedData = 1,
        MainGameLoop = 2
    }

    public class GameStateEngine : IAsyncStartable, IDisposable
    {
        private readonly IReadOnlyList<IGameState> _gameStates;
        private readonly CancellationTokenSource _lifeTimeCancellationTokenSource = new CancellationTokenSource();

        private IGameState this[GameStateEnum gameState] => _gameStates[(int)gameState];

        public GameStateEngine(IReadOnlyList<IGameState> gameStates)
        {
            _gameStates = gameStates;
        }

        public async UniTask StartAsync(CancellationToken cancellationToken)
        {
            var token = cancellationToken == default ? _lifeTimeCancellationTokenSource.Token : cancellationToken;

            await UniTask.Delay(TimeSpan.FromSeconds(.5f), cancellationToken: token);

            RunStateEngine(token).Forget();
        }

        GameStateEnum currentState = GameStateEnum.GenerateGrid;
        private async UniTaskVoid RunStateEngine(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var activeState = this[currentState];
                await activeState.OnStateBegan(token);
                currentState = activeState.OnStateEnded();
                await UniTask.Yield();
            }
        }

        public void Dispose()
        {
            _lifeTimeCancellationTokenSource?.Cancel();
            _lifeTimeCancellationTokenSource?.Dispose();
        }
    }
}