
namespace MyFrameWork.Game
{
    public partial class Item
    {
        public static Item Create(IItem itemData)
        {
            //todo itemCreate
            return new Item(itemData);
        }
    }
}
