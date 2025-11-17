using System.Threading;
using Cysharp.Threading.Tasks;

namespace TestTaskMike.Gameplay
{
    public interface IGameState
    {
        UniTask OnStateBegan(CancellationToken token);
        GameStateEnum OnStateEnded();
    }
}
