using MessagePipe;
using System;
using TestTaskMike.Domain;
using UnityEngine;
using VContainer;
using Grid = TestTaskMike.Domain.Grid;

namespace TestTaskMike.Application
{
    public class UpgradeService
    {
        private readonly Grid grid;

        [Inject]
        private IAsyncPublisher<BuildingSet> _buildingSetPublisher;

        public UpgradeService(
            Grid grid)
        {
            this.grid = grid;
        }

        public void TryToUpgrade(CellClicked msg)
        {
            var cell = grid.GetCell(msg.x, msg.y);
            if (HasBuilding(cell) || CanUpgradeBuilding(cell))
            {
                UpgradeBuilding(cell);
                PublishBuildingUpgrade(new BuildingSet(msg.x, msg.y, cell.HasBuilding.Item2.BuildingType.BuildingTypeKey, cell.HasBuilding.Item2.Level));
            }
        }

        public void UpgradeBuilding(Cell cell)
        {
            if (!cell.HasBuilding.Item1)
            {
                Debug.LogWarning("UpgradeService: Cannot upgrade - no building on cell");
                return;
            }

            var building = cell.HasBuilding.Item2;

            if (building.Level >= building.BuildingType.maxUpgradeLevel)
            {
                Debug.LogWarning("UpgradeService: Building is already at maximum level");
                return;
            }

            building.Upgrade();

            Debug.Log($"UpgradeService: Building upgraded to level {building.Level} at ({cell.x}, {cell.y})");

            // Publish the upgrade event
            BuildingSet buildingSet = new BuildingSet(cell.x, cell.y, GetBuildingTypeEnum(building.BuildingType), building.Level);
            PublishBuildingUpgrade(buildingSet);
        }

        public bool HasBuilding(Cell cell)
        {
            return cell.HasBuilding.Item1;
        }

        public bool CanUpgradeBuilding(Cell cell)
        {
            if (!cell.HasBuilding.Item1) return false;

            var building = cell.HasBuilding.Item2;

            return building.Level < building.BuildingType.maxUpgradeLevel;
        }

        private BuildingTypeEnum GetBuildingTypeEnum(BuildingType buildingType)
        {
            // Convert BuildingType to BuildingTypeEnum
            if (buildingType is ResidentialBuildingType) return BuildingTypeEnum.Residential;
            if (buildingType is CommercialBuildingType) return BuildingTypeEnum.Commercial;
            if (buildingType is IndustrialBuildingType) return BuildingTypeEnum.Industrial;

            throw new ArgumentException($"Unknown building type: {buildingType.GetType()}");
        }

        async void PublishBuildingUpgrade(BuildingSet buildingSet)
        {
            await _buildingSetPublisher.PublishAsync(buildingSet);
        }
    }
}