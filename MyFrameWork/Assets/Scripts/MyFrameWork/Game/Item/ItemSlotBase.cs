

namespace MyFrameWork.Game
{
    public interface IItemSlot
    {
        Muid Muid { get; }
        Item Item { get; }
        IItemContainer Container { get; }
        bool IsEmpty();
        bool IsPossiblyEquip(ItemType itemType);
    }

    public abstract class ItemSlotBase : IItemSlot
    {
        public Muid Muid { get; protected set; }
        public Item Item { get; protected set; }
        public IItemContainer Container { get; protected set; }

        public ItemSlotBase(IItemContainer container)
        {
            Muid = new Muid();
            Container = container;
        }
        public bool IsEmpty() => Item == null;
        public void SetItem(Item item)
        {
            Item = item;
        }

        public bool IsPossiblyEquip(ItemType itemType)
        {
            //todo 타입에 따른 장착 가능 여부 처리
            return true;
        }
    }
}