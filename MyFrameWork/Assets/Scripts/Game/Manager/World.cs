using System.Collections;
namespace MyFramework
{
    using MyFramework.GameData;
    /// <summary>
    /// 게임 내의 Player,몬스터, 아이템 등등의 인게임 요소를 관리
    /// 만약 특정 던전, 모드 가 존재한다면 해당 World를 상속받은 여러 월드 구현 필요
    /// </summary>
    public partial class World : IWorldMessageReceiver
    {
        public WorldMap WorldMap { get; private set; }

        public World()
        {
            UpdateManager.Register(WaitForDataLoad());
        }

        IEnumerator WaitForDataLoad()
        {
            yield return new UpdateManager.WaitUntil(() => { return DataTableManager.IsCompleteLoad(); });
            this.SendWorldMessage(WorldMessageName.DataLoadingEnd, null);
        }

        public virtual void ProcessWorldMessage(WorldMessageName name, IWorldMessage message)
        {
            switch(name)
            {
                case WorldMessageName.DataLoadingEnd:
                    WorldMap = new WorldMap(0);
                    break;
            }
        }
    }
}