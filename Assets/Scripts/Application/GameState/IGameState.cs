using System.Threading;
using Cysharp.Threading.Tasks;

namespace TestTaskMike.Application
{
    public interface IGameState
    {
        UniTask OnStateBegan(CancellationToken token);
        GameStateEnum OnStateEnded();
    }
}
