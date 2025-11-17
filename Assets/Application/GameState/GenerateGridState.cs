using Cysharp.Threading.Tasks;
using MessagePipe;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace TestTaskMike.Gameplay
{
    public class GenerateGridState : IGameState
    {
        //private readonly GameModel _gameModel;
        private readonly IAsyncPublisher<GameStateEnum> finishPublisher;

        private const GameStateEnum OwnState = GameStateEnum.GenerateGrid;

        public GenerateGridState(IAsyncPublisher<GameStateEnum> finishPublisher)
        {
            this.finishPublisher = finishPublisher;
        }

        public async UniTask OnStateBegan(CancellationToken cancellationToken)
        {
            Debug.Log("Generating Grid OnStateBegan");
            await finishPublisher.PublishAsync(OwnState, cancellationToken);
        }

        public GameStateEnum OnStateEnded()
        {
            Debug.Log("Generating Grid OnStateEnded");
            return GameStateEnum.LoadSavedData;
        }
    }
}