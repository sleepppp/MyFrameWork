using System.Collections.Generic;
using UnityEngine;
namespace MyFramework
{
    public class WorldMap
    {
        readonly Dictionary<CellGroupIndex, CellGroup> _cellGroups = new Dictionary<CellGroupIndex, CellGroup>(new MapIndexComparer());

        public WorldMap(int worldMapID)
        {
        }

        public CellGroup GetCellGroup(CellGroupIndex index)
        {
            _cellGroups.TryGetValue(index, out CellGroup result);
            return result;
        }

        public CellGroup GetCellGroup(Vector3 location)
        {
            return GetCellGroup(LocationToCellGroupIndex(location));
        }

        public Cell GetCell(Vector3 location)
        {
            CellGroup cellGroup = GetCellGroup(location);
            if (cellGroup == null) return null;
            return cellGroup.GetCell(location);

        }

        public static CellGroupIndex LocationToCellGroupIndex(Vector3 location)
        {
            int indexX = (int)(location.x / CellGroup.GetSize());
            int indexY = (int)(location.z / CellGroup.GetSize());
            return new CellGroupIndex(indexX, indexY);
        }
    }
}
