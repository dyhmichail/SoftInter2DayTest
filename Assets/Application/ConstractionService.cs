using Cysharp.Threading.Tasks;
using MessagePipe;
using System;
using System.Threading;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public readonly struct BuildingSet
{
    public readonly int x;
    public readonly int y;
    public readonly BuildingTypeEnum BuildingType;
    public readonly int Level;

    public BuildingSet(int x, int y, BuildingTypeEnum BuildingType, int Level)
    {
        this.x = x;
        this.y = y;
        this.BuildingType = BuildingType;
        this.Level = Level;
    }
}

public class ConstractionService : IInitializable, IDisposable
{
    public BuildingTypeEnum ChoosedBuildingType = BuildingTypeEnum.None;


    private readonly Grid grid;
    private readonly GridView gridView;
    private readonly IAsyncSubscriber<CellClicked> cellClickedSubscriber;
    private IDisposable _clickSubscription;

    [Inject]
    private IAsyncPublisher<BuildingSet> _buildingSetPublisher;

    public ConstractionService(
        Grid grid,
        GridView gridView,
        IAsyncSubscriber<CellClicked> cellClickedSubscriber)
    {
        this.grid = grid;
        this.gridView = gridView;
        this.cellClickedSubscriber = cellClickedSubscriber;
    }

    // Called by VContainer when the scope starts.
    public void Initialize()
    {
        if (cellClickedSubscriber != null)
        {
            _clickSubscription = cellClickedSubscriber.Subscribe((msg, ct) =>
            {
                OnCellClicked(msg);
                return UniTask.CompletedTask;
            });
        }
        else
        {
            Debug.LogWarning("ConstractionService: cellClickedSubscriber is null.");
        }
    }

    public bool HasBuilding(Cell cell)
    {
        return cell.HasBuilding.Item1;
    }

    private void OnCellClicked(CellClicked msg)
    {
        if (ChoosedBuildingType != BuildingTypeEnum.None)
        {
            var cell = grid.GetCell(msg.x, msg.y);
            Debug.Log($"ConstractionService: Cell clicked at ({msg.x}, {msg.y}) - cell instance: {cell.ToString()}");
            var building = new Building(GetBuildingType(ChoosedBuildingType), cell);
            cell.PlaceBuilding(building);

            BuildingSet buildingSet = new BuildingSet(msg.x, msg.y, ChoosedBuildingType, 1);
            PublishBuilding(buildingSet);

            ChoosedBuildingType = BuildingTypeEnum.None;
        }
    }

    async void PublishBuilding(BuildingSet buildingSet)
    {
        await _buildingSetPublisher.PublishAsync(buildingSet);
    }

    //TODO move later
    BuildingType GetBuildingType(BuildingTypeEnum buildingTypeKey)
    {
        switch (buildingTypeKey)
        {
            case BuildingTypeEnum.Residential:
                return new ResidentialBuildingType();
            case BuildingTypeEnum.Commercial:
                return new CommercialBuildingType();
            case BuildingTypeEnum.Industrial:
                return new IndustrialBuildingType();
            default:
                throw new ArgumentOutOfRangeException(nameof(buildingTypeKey), buildingTypeKey, null);
        }
    }

    public void Dispose()
    {
        _clickSubscription?.Dispose();
    }
}