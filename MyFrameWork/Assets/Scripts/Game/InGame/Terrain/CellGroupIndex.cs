
using System;
using System.Collections.Generic;
using UnityEngine;
namespace MyFramework
{
    public readonly struct CellGroupIndex
    {
        public readonly int IndexX;
        public readonly int IndexY;

        public CellGroupIndex(int indexX,int indexY)
        {
            IndexX = indexX;
            IndexY = indexY;
        }

        public static CellGroupIndex LocationToMapIndex(Vector3 location)
        {
            int indexX = (int)(location.x / CellGroup.GetSize());
            int indexY = (int)(location.z / CellGroup.GetSize());
            return new CellGroupIndex(indexX, indexY);
        }
    }

    public class MapIndexComparer : IEqualityComparer<CellGroupIndex>
    {
        public bool Equals(CellGroupIndex x, CellGroupIndex y)
        {
            return x.IndexX == y.IndexX && x.IndexY == y.IndexY;
        }

        public int GetHashCode(CellGroupIndex obj)
        {
            return obj.IndexX.GetHashCode() ^ obj.IndexY.GetHashCode();
        }
    }
}