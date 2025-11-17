using System;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;

[Serializable]
public class Cell : IDisposable
{
    public int x;
    public int y;
    public (bool, Building) HasBuilding;

    private IDisposable _clickSubscription;

    public Cell(int x, int y, IAsyncSubscriber<CellClicked> cellClickSubscriber = null)
    {
        this.x = x;
        this.y = y;

        if (cellClickSubscriber != null)
        {
            // Subscribe to CellClicked messages and call OnCellClicked when coordinates match.
            _clickSubscription = cellClickSubscriber.Subscribe((msg, ct) =>
            {
                if (msg.x == this.x && msg.y == this.y)
                {
                    OnCellClicked();
                }
                return UniTask.CompletedTask;
            });
        }
    }

    // Called when a CellClicked message targets this cell.
    private void OnCellClicked()
    {
        Debug.Log($"Cell clicked: ({x}, {y})");
        // TODO: handle selection / command logic here
    }

    public bool PlaceBuilding(Building building)
    {
        if (HasBuilding.Item1)
        {
            return false;
        }
        HasBuilding = (true, building);
        return true;
    }

    public bool RemoveBuilding()
    {
        if (!HasBuilding.Item1)
        {
            return false;
        }
        HasBuilding = (false, null);
        return true;
    }

    public void Dispose()
    {
        _clickSubscription?.Dispose();
    }
}