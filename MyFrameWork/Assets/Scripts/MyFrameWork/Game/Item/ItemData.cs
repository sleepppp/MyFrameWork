using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFrameWork.Game
{
    /// <summary>
    /// �ӽ� ������ ������ ���� Ŭ����
    /// todo ������ ���ǵǸ� ����
    /// </summary>
    public sealed class ItemRecord
    {
        public int ID;
        public int TypeID;
        public string Name;
        public int Width;
        public int Height;
        public int MaxStackAmount;
    }

    public sealed class ItemTypeRecord
    {
        public int ID;
        public string TypeName;
        public bool IsStackable;
        public bool IsConsumeable;
    }

    public class DataTableManager
    {
        public static readonly Dictionary<int, ItemRecord> ItemRecords = new Dictionary<int, ItemRecord>();
        public static readonly Dictionary<int, ItemTypeRecord> ItemTypeRecords = new Dictionary<int, ItemTypeRecord>();

        public static ItemRecord GetItemRecord(int id)
        {
            ItemRecords.TryGetValue(id, out var result);
            return result;
        }

        public static ItemTypeRecord GetItemTypeRecord(int id)
        {
            ItemTypeRecords.TryGetValue(id, out var result);
            return result;
        }
    }
}