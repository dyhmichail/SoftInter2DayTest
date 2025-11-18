using MessagePipe;
using R3;
using System;
using System.Collections.Generic;
using TestTaskMike;
using TestTaskMike.Application;
using TestTaskMike.Domain;
using UnityEngine;
using VContainer;
using Grid = TestTaskMike.Domain.Grid;

public class SaveLoadSystem : MonoBehaviour
{
    [Inject]
    Grid grid;

    [Inject]
    EconomyService economyService;

    [Inject]
    private IAsyncPublisher<BuildingSet> _buildingSetPublisher;

    public void SaveGame()
    {
        if (grid == null || grid.cells == null)
        {
            Debug.LogWarning("SaveGame: grid is null or grid.cells is null. Nothing to save.");
            return;
        }

        var data = new GameData
        {
            Gold = economyService.Gold.CurrentValue
        };

        int width = grid.cells.GetLength(0);
        int height = grid.cells.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var cell = grid.cells[x, y];
                if (cell == null) continue;

                if (cell.HasBuilding.Item1)
                {
                    var building = cell.HasBuilding.Item2;
                    var buildingType = building.BuildingType.BuildingTypeKey;
                    var upgradeLevel = building.Level;

                    data.GridInfo.Add(new GridCellEntry(x, y, buildingType, upgradeLevel));
                }
            }
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("save_game", json);
        PlayerPrefs.Save();
        Debug.Log($"Game saved. Gold: {economyService.Gold}, savedCells: {data.GridInfo.Count}");
    }

    public void LoadGame()
    {
        if (grid == null || grid.cells == null)
        {
            Debug.LogWarning("LoadGame: grid is null or grid.cells is null. Nothing to load.");
            return;
        }

        if (!PlayerPrefs.HasKey("save_game"))
        {
            Debug.Log("LoadGame: no save found.");
            return;
        }

        string json = PlayerPrefs.GetString("save_game", string.Empty);
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("LoadGame: save string empty.");
            return;
        }

        GameData data;
        try
        {
            data = JsonUtility.FromJson<GameData>(json);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return;
        }

        if (data == null)
        {
            Debug.LogWarning("LoadGame: deserialized GameData is null.");
            return;
        }

        int gold = data.Gold;
        economyService.AddGold(gold);

        foreach (GridCellEntry entry in data.GridInfo)
        {
            int x = entry.x;
            int y = entry.y;
            var buildingType = entry.buildingType;
            var upgradeLevel = entry.upgradeLevel;

            var cell = grid.GetCell(x, y);
            if (cell == null) continue;

            var building = new Building(GetBuildingType(buildingType), cell, upgradeLevel);

            // Place building on the cell
            cell.PlaceBuilding(building);

            PublishBuildingUpgrade(new BuildingSet(x, y, entry.buildingType, upgradeLevel));
        }

        Debug.Log($"Game loaded. Gold: {data.Gold}, loadedCells: {data.GridInfo.Count}");
    }

    async void PublishBuildingUpgrade(BuildingSet buildingSet)
    {
        await _buildingSetPublisher.PublishAsync(buildingSet);
    }

    public void ClearSaveData()
    {
        PlayerPrefs.DeleteAll();
    }

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

[Serializable]
public class GameData
{
    public int Gold;

    public List<GridCellEntry> GridInfo = new List<GridCellEntry>();
}

[Serializable]
public class GridCellEntry
{
    public int x;
    public int y;
    public BuildingTypeEnum buildingType;
    public int upgradeLevel;

    public GridCellEntry(int x, int y, BuildingTypeEnum buildingType, int upgradeLevel)
    {
        this.x = x;
        this.y = y;
        this.buildingType = buildingType;
        this.upgradeLevel = upgradeLevel;
    }
}
