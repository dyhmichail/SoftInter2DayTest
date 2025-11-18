using TestTaskMike.Domain;

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

namespace TestTaskMike.Application
{
    public interface IConstractionService
    {
        bool HasBuilding(Cell cell);
        void TryToBuild(CellClicked msg);
    }
}