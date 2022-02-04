using System;
using System.Collections.Generic;

namespace MyFrameWork.Game
{
    public class ItemContainerTileBase : IItemContainer
    {
        public Muid Muid { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        readonly Dictionary<Muid, Item> m_items = new Dictionary<Muid, Item>();
        readonly Dictionary<Muid, ItemSlotTileBase> m_slotDictionary = new Dictionary<Muid, ItemSlotTileBase>();
        readonly ItemSlotTileBase[,] m_slots;

        public ItemContainerTileBase(int width, int height)
        {
            Muid = new Muid();
            Width = width;
            Height = height;

            m_slots = new ItemSlotTileBase[height, width];
            for(int y=0;y < height; ++y)
            {
                for(int x=0;x < width; ++x)
                {
                    m_slots[y, x] = new ItemSlotTileBase(this, x, y);
                    m_slotDictionary.Add(m_slots[y, x].Muid, m_slots[y, x]);
                }
            }
        }

        public List<IInventoryItem> GetAllItemList()
        {
            List<IInventoryItem> result = new List<IInventoryItem>();
            foreach(var item in m_items)
            {
                result.Add(item.Value);
            }
            return result;
        }

        public IInventoryItem GetItem(Muid muid)
        {
            m_items.TryGetValue(muid, out Item result);
            return result;
        }

        public ItemExeption.ExeptionType IsPossibleAdd(IItem itemData)
        {
            if (itemData.IsStackable)
            {
                int remainCount = itemData.Amount;
                //기존에 있는 슬롯에 추가 가능한지 검사 
                for (int y = 0; y < Height; ++y)
                {
                    for (int x = 0; x < Width; ++x)
                    {
                        ItemSlotTileBase slot = GetSlot(x, y);
                        if (slot.Item.ID == itemData.ID && slot.Item.GetPossiblyAddedAmount() != 0)
                        {
                            slot.Item.TryAddAmount(remainCount, out remainCount);
                            if (remainCount <= 0)
                            {
                                return ItemExeption.ExeptionType.Succeeded;
                            }
                        }

                    }
                }

                //여기 까지 왔다면 추가 블럭 생성필요
                int blockCount = remainCount / itemData.MaxStackAmount;
                if (remainCount % itemData.MaxStackAmount != 0) blockCount++;
                return IsPossiblyAddItemBlock(blockCount, itemData.Width, itemData.Height);
            }
            else
            {
                int blockCount = itemData.Amount;
                return IsPossiblyAddItemBlock(blockCount, itemData.Width, itemData.Height);
            }
        }

        public ItemExeption.ExeptionType IsPossibleEquipToSlot(IInventoryItem item, Muid slotMuid)
        {
            m_slotDictionary.TryGetValue(slotMuid, out ItemSlotTileBase slot);

            if (slot == null)
                return ItemExeption.ExeptionType.Failed;
            if (slot.IsEmpty() == false)
                return ItemExeption.ExeptionType.Failed;
            if (slot.IsPossiblyEquip((ItemType)item.ItemType) == false)
                return ItemExeption.ExeptionType.Failed;

            return ItemExeption.ExeptionType.Succeeded;
        }

        public ItemExeption.ExeptionType RequestAdd(IItem itemData)
        {
            ItemExeption.ExeptionType result = IsPossibleAdd(itemData);
            if (result.IsSucceeded() == false)
                return result;

            if (itemData.IsStackable)
            {
                int remainCount = itemData.Amount;
                foreach (var item in m_items.Values)
                {
                    if (item.ID == itemData.ID && item.IsStackable)
                    {
                        item.TryAddAmount(remainCount, out remainCount);
                        if (remainCount <= 0)
                        {
                            return ItemExeption.ExeptionType.Succeeded;
                        }
                    }
                }

                int blockCount = remainCount / itemData.MaxStackAmount;
                if (remainCount % itemData.MaxStackAmount != 0)
                    blockCount++;
                for (int i = 0; i < blockCount; ++i)
                {
                    int amount = remainCount > itemData.MaxStackAmount ? remainCount - itemData.MaxStackAmount : remainCount;
                    TryAddItemBlock(new ItemDummyData(itemData.ID, amount));
                    remainCount -= amount;
                }

                if (remainCount > 0)
                {
                    UnityEngine.Debug.LogError("Error");
                }

                return ItemExeption.ExeptionType.Succeeded;
            }
            else
            {
                int blockCount = itemData.Amount;

                for (int i = 0; i < blockCount; ++i)
                {
                    if(TryAddItemBlock(new ItemDummyData(itemData.ID, 1)) == false)
                    {
                        return ItemExeption.ExeptionType.Failed;
                    }
                }

                return ItemExeption.ExeptionType.Succeeded;
            }
        }

        public ItemExeption.ExeptionType RequestEquipToSlot(IInventoryItem ivnentoryItem, Muid slotMuid)
        {
            if (IsPossibleEquipToSlot(ivnentoryItem, slotMuid) != ItemExeption.ExeptionType.Succeeded)
                return ItemExeption.ExeptionType.Failed;
            m_slotDictionary.TryGetValue(slotMuid, out ItemSlotTileBase slot);

            Item item = ivnentoryItem as Item;
            EquipToSlot(item, slot);
            return ItemExeption.ExeptionType.Succeeded;
        }

        public ItemSlotTileBase GetSlot(int indexX, int indexY)
        {
            if (IsOutOfRange(indexX, indexY)) return null;
            return m_slots[indexY, indexX];
        }

        void Foreach(int indexX, int indexY, int width, int height, Func<ItemSlotTileBase, bool> query)
        {
            for (int y = indexY; y < indexY + height; ++y)
            {
                for (int x = indexX; x < indexX + width; ++x)
                {
                    ItemSlotTileBase slot = GetSlot(x, y);
                    if (slot != null)
                    {
                        if (query?.Invoke(slot) == true)
                            return;
                    }
                }
            }
        }

        bool IsOutOfRange(int indexX, int indexY)
        {
            if (indexX < 0 || indexX >= Width) return true;
            if (indexY < 0 || indexY >= Height) return true;
            return false;
        }

        bool IsEmpty(int indexX, int indexY)
        {
            if (IsOutOfRange(indexX, indexY)) return false;
            ItemSlotTileBase slot = GetSlot(indexX, indexY);
            if (slot.Item != null) return false;

            return true;
        }

        bool IsEmpty(int indexX, int indexY, int width, int height)
        {
            for (int y = indexY; y < indexY + height; ++y)
            {
                for (int x = indexX; x < indexX + width; ++x)
                {
                    if (IsOutOfRange(x, y) || GetSlot(x, y).Item != null)
                        return false;
                }
            }

            return true;
        }
        ItemExeption.ExeptionType IsPossiblyAddItemBlock(int blockCount, int width, int height)   //새로 블롯을 만들어서 Push가능한지
        {
            bool[,] isEmptyList = new bool[Height, Width];

            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    isEmptyList[y, x] = GetSlot(x, y).IsEmpty();
                }
            }

            int maxIndexX = Width - 1;
            int maxIndexY = Height - 1;
            int remainCount = blockCount;
            ItemExeption.ExeptionType result = ItemExeption.ExeptionType.Failed;
            Foreach(0, 0, Width, Height, (slot) =>
            {
                if (isEmptyList[slot.IndexY, slot.IndexX] == true &&
                    maxIndexX - slot.IndexX >= width &&
                    maxIndexY - slot.IndexY >= height)
                {
                    bool isAllEmpty = true;
                    for (int y = slot.IndexY; y < slot.IndexY + height; ++y)
                    {
                        for (int x = slot.IndexX; x < slot.IndexX + width; ++x)
                        {
                            if (isEmptyList[y, x] == false)
                            {
                                isAllEmpty = false;
                                break;
                            }
                        }
                        if (isAllEmpty == false)
                            break;
                    }

                    if (isAllEmpty)
                    {
                        remainCount--;
                        if (remainCount <= 0)
                        {
                            result = ItemExeption.ExeptionType.Succeeded;
                            return true;
                        }
                        else
                        {
                            for (int y = slot.IndexY; y < slot.IndexY + height; ++y)
                            {
                                for (int x = slot.IndexX; x < slot.IndexX + width; ++x)
                                {
                                    isEmptyList[y, x] = false;
                                }
                            }
                        }
                    }
                }
                return false;
            });

            return result;
        }

        void EquipToSlot(Item item, ItemSlotTileBase slot)
        {
            Foreach(slot.IndexX, slot.IndexY, item.Width, item.Height, (slot) =>
            {
                slot.SetItem(item);
                return false;
            });

            item.ItemSlot = slot;

            m_items.Add(item.Muid, item);
        }

        bool TryAddItemBlock(IItem itemData)
        {
            Foreach(0, 0, Width, Height, (slot) =>
            {
                if (IsEmpty(slot.IndexX, slot.IndexY, itemData.Width, itemData.Height))
                {
                    Item item = Item.Create(itemData);

                    EquipToSlot(item, slot);
                    return true;
                }
                return false;
            });

            return true;
        }
    }
}
