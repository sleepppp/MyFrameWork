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
        /// �޼��� ������ �ʿ��ϸ� �ش� �޼��� �������̵�
        /// </summary>
        public virtual void ProcessWorldMessage(WorldMessageName name, IWorldMessage message)
        {
            
        }
    }
}
