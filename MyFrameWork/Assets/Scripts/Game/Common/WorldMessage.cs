
namespace MyFramework
{
    /// <summary>
    /// 해당 인터페이스를 상속해서 메세지 구조체를 만드세요
    /// </summary>
    public interface IWorldMessage { }

    public static class WorldMessageCaster
    {
        /// <summary>
        /// WorldMessage 캐스팅해서 사용할 때 안정성을 위해 해당 매서드를 사용하세요
        /// </summary>
        public static T Cast<T>(this IWorldMessage message) where T : struct, IWorldMessage
        {
            if(message == null)
            {
                throw new System.Exception("Message is null");
            }
            if(message is T == false)
            {
                throw new System.Exception("Can't cast");
            }

            return (T)message;
        }
    }

    /// <summary>
    /// 월드 메세지를 받을 오브젝트 들은 해당 인터페이스를 상속
    /// </summary>
    public interface IWorldMessageReceiver
    {
        public void ProcessWorldMessage(WorldMessageName name, IWorldMessage message);
    }
    /// <summary>
    /// 새로 추가되는 메세지는 이곳에 정의
    /// </summary>
    public static partial class WorldMessage
    {
        public struct GameStart : IWorldMessage
        {
            public string NotiText { get; private set; }
            public GameStart(string text) { NotiText = text; }
        }
    }
}