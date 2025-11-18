using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using TestTaskMike.Domain;
using VContainer;
using VContainer.Unity;

public readonly struct CellClicked
{
    public readonly int x;
    public readonly int y;

    public CellClicked(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public readonly struct ToolClicked
{
    public readonly ToolType toolType;

    public ToolClicked(ToolType toolType)
    {
        this.toolType = toolType;
    }
}

public enum ToolType
{
    None = 0,
    Constraction,
    Upgrade,
    Remove,
    Move
}

namespace TestTaskMike.Application
{
    public class ToolsServiceLocator : IInitializable, IDisposable
    {
        public ToolType selectedTool;

        private readonly Grid grid;
        private readonly IAsyncSubscriber<ToolClicked> toolClickedSubscriber;
        private readonly IAsyncSubscriber<CellClicked> cellClickedSubscriber;
        private readonly IConstractionService _constractionService;
        private readonly UpgradeService _uprgadeService;
        private readonly RemoveService _removeService;
        private readonly MoveService _moveService;

        [Inject]
        private IAsyncPublisher<BuildingSet> _buildingSetPublisher;

        private IDisposable _cellClickSubscription;
        private IDisposable _toolClickSubscription;
        public ToolsServiceLocator(
            Grid grid,
            IAsyncSubscriber<CellClicked> cellClickedSubscriber,
            IAsyncSubscriber<ToolClicked> toolClickedSubscriber,
            IConstractionService constractionService,
            UpgradeService uprgadeService,
            RemoveService removeService,
            MoveService moveService
            )
        {
            this.grid = grid;
            this.cellClickedSubscriber = cellClickedSubscriber;
            this.toolClickedSubscriber = toolClickedSubscriber;
            _constractionService = constractionService;
            _uprgadeService = uprgadeService;
            _removeService = removeService;
            _moveService = moveService;
        }

        public void Initialize()
        {
            _cellClickSubscription = cellClickedSubscriber.Subscribe((msg, ct) =>
            {
                OnCellClicked(msg);
                return UniTask.CompletedTask;
            });

            _toolClickSubscription = toolClickedSubscriber.Subscribe((msg, ct) =>
            {
                OnToolClicked(msg);
                return UniTask.CompletedTask;
            });
        }

        private void OnCellClicked(CellClicked msg)
        {
            switch (selectedTool)
            {
                case ToolType.Constraction:
                    _constractionService.TryToBuild(msg);
                    selectedTool = ToolType.None;
                    break;
                case ToolType.Upgrade:
                    _uprgadeService.TryToUpgrade(msg);
                    selectedTool = ToolType.None;
                    break;

            }
        }

        private void OnToolClicked(ToolClicked msg)
        {
            selectedTool = msg.toolType;
        }

        public void Dispose()
        {
            _cellClickSubscription?.Dispose();
            _toolClickSubscription?.Dispose();
        }
    }
}