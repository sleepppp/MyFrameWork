using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFramework.UI
{
    public class ManagementUIBase : UIBase, IWorldMessageReceiver
    {
        [SerializeField] ManagementUIEnum _uiKey;
        public ManagementUIEnum UIKey { get { return _uiKey; } }
        public void SetUIKey(ManagementUIEnum uiKey) { _uiKey = uiKey; }

        public virtual void OnClose()
        {
            //todo override if needed
        }

        public virtual void Close()
        {
            Game.UIManager.CloseUI(_uiKey);
        }
        /// <summary>
        /// 메세지 구독이 필요하면 해당 메서드 오버라이딩
        /// </summary>
        public virtual void ProcessWorldMessage(WorldMessageName name, IWorldMessage message)
        {
            
        }
    }
}
