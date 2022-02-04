
namespace MyFrameWork.Game
{
    public interface IItemData
    {
        public int ID { get; }
        public string Name { get; }
        public int Width { get; }
        public int Height { get; }
        public int MaxStackAmount { get; }
        public bool IsStackable { get; }
        public bool IsConsumeable { get; }
        public int ItemType { get; }
    }

    public interface IItem : IItemData
    {
        public int Amount { get; }
    }

    public interface IInventoryItem : IItem
    {
        public Muid Muid { get; }
        public bool IsLock { get; }
        public IItemSlot ItemSlot { get; }
    }

    public struct ItemDummyData : IItem
    {
        public int ID { get; private set; }
        public int Amount { get; private set; }
        public string Name { get; private set; }
        public int Width { get; private set; }

        public int Height { get; private set; }

        public int MaxStackAmount { get; private set; }

        public bool IsStackable { get; private set; }

        public bool IsConsumeable { get; private set; }

        public int ItemType { get; private set; }

        public ItemDummyData(int itemID, int amount)
        {
            DataTableManager.ItemRecords.TryGetValue(itemID, out ItemRecord record);
            Debugger.Throw(record == null, "ItemRecord is NUll. itemID : " + itemID);
            DataTableManager.ItemTypeRecords.TryGetValue(record.TypeID, out ItemTypeRecord typeRecord);
            Debugger.Throw(typeRecord == null, "ItemTypeRecord is NULL. itemTypeID : " + record.TypeID);

            ID = itemID;
            ItemType = typeRecord.ID;
            Name = record.Name;
            Amount = amount;
            Width = record.Width;
            Height = record.Height;
            MaxStackAmount = record.MaxStackAmount;
            IsStackable = typeRecord.IsStackable;
            IsConsumeable = typeRecord.IsConsumeable;
        }
    }
}