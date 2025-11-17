using Cysharp.Threading.Tasks;
using MessagePipe;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace TestTaskMike.Gameplay
{
    public class MainGameState : IGameState
    {
        //private readonly GameModel _gameModel;
        private readonly IAsyncPublisher<GameStateEnum> finishPublisher;

        private const GameStateEnum OwnState = GameStateEnum.MainGameLoop;

        public MainGameState(IAsyncPublisher<GameStateEnum> finishPublisher)
        {
            this.finishPublisher = finishPublisher;
        }

        bool endlessGame;
        public async UniTask OnStateBegan(CancellationToken cancellationToken)
        {
            if (endlessGame) return;
            Debug.Log("MainGameState OnStateBegan");
            await finishPublisher.PublishAsync(OwnState, cancellationToken);
            endlessGame = true;
        }

        public GameStateEnum OnStateEnded()
        {
            return GameStateEnum.MainGameLoop;
        }
    }
}