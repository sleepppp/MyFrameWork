
namespace MyFrameWork.Game
{
    public partial class Item : IInventoryItem
    {
        public Muid Muid { get; private set; }
        public int ID { get; private set; }
        public bool IsLock { get; private set; }
        public int Amount { get; private set; }
        public string Name { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int MaxStackAmount { get; private set; }
        public bool IsStackable { get; private set; }
        public bool IsConsumeable { get; private set; }
        public int ItemType { get; private set; }
        public IItemSlot ItemSlot { get; set; }

        private Item(IItem itemData)
        {
            DataTableManager.ItemRecords.TryGetValue(itemData.ID, out ItemRecord record);
            Debugger.Throw(record == null, "ItemRecord is NUll. itemID : " + itemData.ID);
            DataTableManager.ItemTypeRecords.TryGetValue(record.TypeID, out ItemTypeRecord typeRecord);
            Debugger.Throw(typeRecord == null, "ItemTypeRecord is NULL. itemTypeID : " + record.TypeID);

            Muid = new Muid();
            ID = itemData.ID;
            ItemType = typeRecord.ID;
            Name = record.Name;
            Amount = itemData.Amount;
            Width = record.Width;
            Height = record.Height;
            MaxStackAmount = record.MaxStackAmount;
            IsStackable = typeRecord.IsStackable;
            IsConsumeable = typeRecord.IsConsumeable;
            IsLock = false;
        }

        public int GetPossiblyAddedAmount()
        {
            if (IsStackable == false) return 0;
            return MaxStackAmount - Amount;
        }

        public void TryAddAmount(int requestAmount, out int remainAmount)
        {
            int addAmount = GetPossiblyAddedAmount() < requestAmount ? requestAmount - GetPossiblyAddedAmount() : requestAmount;
            remainAmount = requestAmount - addAmount;
            Amount += addAmount;
            if (remainAmount < 0)
            {
                throw new System.Exception("error");
            }
        }
    }
}