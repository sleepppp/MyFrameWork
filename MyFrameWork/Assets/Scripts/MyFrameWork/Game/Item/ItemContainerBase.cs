using System.Collections.Generic;

namespace MyFrameWork.Game
{
    public interface IItemContainer
    {
        Muid Muid { get; }
        List<IInventoryItem> GetAllItemList();
        IInventoryItem GetItem(Muid muid);
        ItemExeption.ExeptionType IsPossibleAdd(IItem item);
        ItemExeption.ExeptionType RequestAdd(IItem item);
        ItemExeption.ExeptionType IsPossibleEquipToSlot(IInventoryItem item, Muid slotMuid);
        ItemExeption.ExeptionType RequestEquipToSlot(IInventoryItem item, Muid slotMuid);
        //ItemExeption.ExeptionType IsPossibleAddType(int itemTypeID);
    }
}