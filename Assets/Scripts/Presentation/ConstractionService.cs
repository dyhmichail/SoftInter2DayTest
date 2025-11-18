using MessagePipe;
using System;
using System.Numerics;
using TestTaskMike.Application;
using TestTaskMike.Domain;
using UnityEngine;
using VContainer;
using Grid = TestTaskMike.Domain.Grid;

namespace TestTaskMike.Presentation
{
    public class ConstractionService : IConstractionService
    {
        public BuildingTypeEnum ChoosedBuildingType = BuildingTypeEnum.None;


        private readonly Grid grid;


        [Inject]
        private IAsyncPublisher<BuildingSet> _buildingSetPublisher;

        public ConstractionService(
            Grid grid)
        {
            this.grid = grid;
        }

        public bool HasBuilding(Cell cell)
        {
            return cell.HasBuilding.Item1;
        }

        public bool HasBuilding(Vector2Int position)
        {
            var cell = grid.GetCell(position.x, position.y);
            return cell.HasBuilding.Item1;
        }

        public void TryToBuild(CellClicked msg)
        {
            if (ChoosedBuildingType != BuildingTypeEnum.None)
            {
                var cell = grid.GetCell(msg.x, msg.y);

                if (cell.HasBuilding.Item1)
                {
                    return;
                }

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
    }
}