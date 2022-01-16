
namespace MyFrameWork.Game
{
    public interface IItemData
    {
        public string Name { get; }
        public int Width { get; }
        public int Height { get; }
        public int MaxStackAmount { get; }
        public bool IsStackable { get; }
        public bool IsConsumeable { get; }
        public ItemType ItemType { get; }
    }

    public interface IItem : IItemData
    {
        public int Amount { get; }
    }

    public interface IInventoryItem : IItem
    {
        public bool IsLock { get; }
    }
}