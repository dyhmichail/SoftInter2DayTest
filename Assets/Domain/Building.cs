using Sirenix.OdinInspector;
using System.Diagnostics;

[Searchable]
public class Building
{
    Cell cell;
    public BuildingType BuildingType;
    public int Level { get; private set; } = 1;

    public Building(BuildingType buildingType, Cell cell, int upgradeLevel = 1)
    {
        BuildingType = buildingType;
        this.cell = cell;
        Level = upgradeLevel;
    }

    public void Move(Cell newCell)
    {
        cell.RemoveBuilding();

        newCell.PlaceBuilding(this);

        cell = newCell;
    }

    public void Demolish()
    {
        cell.RemoveBuilding();
    }

    public void Upgrade()
    {
        if (Level < BuildingType.maxUpgradeLevel)
        {
            Level++;
        }
    }

    public int GetIncomePerTick()
    {
        return BuildingType.goldIncomePerLevel * Level;
    }
}
