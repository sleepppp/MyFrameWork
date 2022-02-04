
namespace MyFrameWork.Game
{
    public class ItemSlotTileBase : ItemSlotBase
    {
        public int IndexX { get; private set; }
        public int IndexY { get; private set; }

        public ItemSlotTileBase(ItemContainerTileBase container, int indexX, int indexY)
            :base(container)
        {
            Container = container;
            IndexX = indexX;
            IndexY = indexY;
        }
    }
}
