using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace MyFramework.UI
{
    public enum ManagementUIEnum : int
    {
        TestPopup = 1,
        UITileCreatePopup =2,
    }

    public partial class UIManager : IWorldMessageReceiver
    {
        public Canvas MainCanvas;
        public EventSystem EventSystem;

        Dictionary<ManagementUIEnum, ManagementUIBase> _uiContainer;

        public Rect SafeArea { get { return Screen.safeArea; } }
        
        public void Init(Canvas mainCanvas, EventSystem eventSystem)
        {
            MainCanvas = mainCanvas;
            EventSystem = eventSystem;
            _uiContainer = new Dictionary<ManagementUIEnum, ManagementUIBase>();
        }

        public void CreateUI<T>(string path, ManagementUIEnum uiKey,Action<T> callback) where T : ManagementUIBase
        {
            AssetManager.Instantiate<T>(path, (ui) => 
            {
                AddUI<T>(uiKey, ui);

                callback?.Invoke(ui);

            }, MainCanvas.transform);
        }

        public void CloseUI(ManagementUIEnum uiKey)
        {
            if (_uiContainer.ContainsKey(uiKey) == false) return;

            ManagementUIBase ui = _uiContainer.GetValue(uiKey);
            ui.OnClose();
            GameObject.Destroy(ui.gameObject);
            _uiContainer.Remove(uiKey);
        }

        public void AddUI<T>(ManagementUIEnum uiEnum, T ui) where T : ManagementUIBase
        {
            if (ui == null)
            {
                throw new System.Exception("Can't create UI : " + uiEnum.ToString());
            }

            if(_uiContainer.ContainsKey(uiEnum))
            {
                throw new System.Exception("Already has ui : " + uiEnum.ToString());
            }

            ui.SetUIKey(uiEnum);

            _uiContainer.Add(uiEnum, ui);
        }

        public void ProcessWorldMessage(WorldMessageName name, IWorldMessage message)
        {
            foreach(var ui in _uiContainer.Values)
            {
                ui.ProcessWorldMessage(name, message);
            }
        }
    }
}
