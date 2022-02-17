using UnityEngine;

namespace MyFramework
{
    public sealed class CellGroup
    {
        public const int DefaultCellCount = 10;
        public const int CellSize = 1;

        Cell[,] _cells;
        public readonly CellGroupIndex MapIndex;
        public readonly Vector3 LeftBottomPosition;
        public readonly int CellCount;

        public CellGroup(int indexX, int indexY, int cellCount)
        {
            MapIndex = new CellGroupIndex(indexX, indexY);
            LeftBottomPosition = GetLeftBottomPos(indexX, indexY);
            CellCount = cellCount;

            _cells = new Cell[CellCount, CellCount];
            Vector3 tempPos = new Vector3();
            float halfCellSize = CellSize * 0.5f;
            for(int y=0;y < CellCount; ++y)
            {
                for(int x=0;x < CellCount; ++x)
                {
                    tempPos.x = LeftBottomPosition.x + (CellSize * x) + halfCellSize;
                    tempPos.y = 0f;
                    tempPos.z = LeftBottomPosition.z + (CellSize * y) + halfCellSize;
                    _cells[y, x] = new Cell()
                    {
                        CenterPosition = tempPos
                    };
                }
            }
        }

        public CellGroup(int indexX, int indexY)
            : this(indexX, indexY, DefaultCellCount) { }

        public Cell GetCell(int indexX,int indexY)
        {
            if (indexX < 0 || indexX >= CellCount) return null;
            if (indexY < 0 || indexY >= CellCount) return null;
            return _cells[indexY, indexX];
        }

        public Cell GetCell(Vector3 location)
        {
            if (location.x < LeftBottomPosition.x || location.x > LeftBottomPosition.x + GetSize()) return null;
            if (location.z < LeftBottomPosition.z || location.z > LeftBottomPosition.z + GetSize()) return null;

            Vector3 localPosition = location - LeftBottomPosition;
            int indexX = (int)(localPosition.x / CellSize);
            int indexY = (int)(localPosition.z / CellSize);
            return GetCell(indexX, indexY);
        }

        public static Vector3 GetLeftBottomPos(int indexX,int indexY, int cellCount = DefaultCellCount)
        {
            return new Vector3
                (
                indexX * GetSize(cellCount),
                0f,
                indexY * GetSize(cellCount)
                );
        }

        public static float GetSize(int cellCount = DefaultCellCount)
        {
            return cellCount * CellSize;
        }
    }
}