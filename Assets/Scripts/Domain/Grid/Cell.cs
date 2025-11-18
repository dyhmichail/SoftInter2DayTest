using System;

namespace TestTaskMike.Domain
{
    [Serializable]
    public class Cell
    {
        public int x;
        public int y;
        public (bool, Building) HasBuilding;

        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
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
    }
}