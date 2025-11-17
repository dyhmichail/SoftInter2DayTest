using Cysharp.Threading.Tasks;
using MessagePipe;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace TestTaskMike.Gameplay
{
    public class LoadSavedGameState : IGameState
    {
        //private readonly GameModel _gameModel;
        private readonly IAsyncPublisher<GameStateEnum> finishPublisher;

        private const GameStateEnum OwnState = GameStateEnum.LoadSavedData;

        public LoadSavedGameState(IAsyncPublisher<GameStateEnum> finishPublisher)
        {
            this.finishPublisher = finishPublisher;
        }

        public async UniTask OnStateBegan(CancellationToken cancellationToken)
        {
            Debug.Log("LoadSavedGameState OnStateBegan");
            await finishPublisher.PublishAsync(OwnState, cancellationToken);
        }

        public GameStateEnum OnStateEnded()
        {
            Debug.Log("LoadSavedGameState OnStateEnded");
            return GameStateEnum.MainGameLoop;
        }
    }
}