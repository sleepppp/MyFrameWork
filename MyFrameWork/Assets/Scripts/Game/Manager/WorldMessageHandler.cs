using System.Collections.Generic;

namespace MyFramework
{
    /// <summary>
    /// # WorldMessag
    /// 게임 제작하다보면 게임 전체에 불특정 다수(오브젝트, UI등등)에게 Noti를 보내야 할 경우가 존재함
    /// Ex) 플레이어 죽음, 게임 오버, 아이템 획득 등등
    /// 그럴 때는 WorldMessage를 만들어서 World에 던져 필요한 녀석들이 알림을 받을 수 있도록 처리하자
    /// </summary>
    public partial class World
    {
        interface IMessagePare
        {
            WorldMessageName Name { get;  }
            IWorldMessage Message { get;  }
        }

        struct MessagePare : IMessagePare
        {
            public WorldMessageName Name { get; private set; }
            public IWorldMessage Message { get; private set; }

            public MessagePare(WorldMessageName messageName,IWorldMessage message) { Name = messageName; Message = message; }
        }

        readonly Queue<IMessagePare> _messageQueue = new Queue<IMessagePare>();

        /// <summary>
        /// 해당 메서드를 통해 월드 전역에 메세지를 뿌린다. 보낼 데이터가 없을 때는 message를 null로 보낸다.
        /// </summary>
        public void SendWorldMessage(WorldMessageName messageName, IWorldMessage message = null)
        {
            IMessagePare pare = new MessagePare(messageName, message);
            _messageQueue.Enqueue(pare);
        }

        /// <summary>
        /// 해당 메서드는 World에서만 호출합니다
        /// 메세지는 World->UI 순으로 알림이 갑니다
        /// </summary>
        public void ExecuteWorldMessage()
        {
            IMessagePare messagePare = null;
            while(_messageQueue.Count != 0)
            {
                messagePare = _messageQueue.Dequeue();
                
                if (Game.World != null)
                    Game.World.ProcessWorldMessage(messagePare.Name, messagePare.Message);
                if (Game.PTWorld != null)
                    Game.PTWorld.ProcessWorldMessage(messagePare.Name, messagePare.Message);
                if (Game.UIManager != null)
                    Game.UIManager.ProcessWorldMessage(messagePare.Name, messagePare.Message);
                
            }
        }
    }
}