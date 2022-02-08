using UnityEditor;
using UnityEngine;

namespace MyFramework
{
    using MyFramework.Presentation;
    using MyFramework.UI;
    using MyFramework.GameData;

    public class Game : MonoBehaviourSingleton<Game>
    {
        public static UIManager UIManager { get; private set; }
        public static World World { get; private set; }
        public static PTWorld PTWorld{ get; private set; }

        [SerializeField] Initializer _initializer;

        private void Start()
        {
            Init();
        }

        public static void Init()
        {
            if(UIManager == null)
                UIManager = new UIManager();
            if (World == null)
                World = new World();
            if (PTWorld == null)
                PTWorld = new PTWorld();

            DataTableManager.Init();
            DataTableManager.Load();

            UIManager.Init(Instance._initializer.MainCanvas, Instance._initializer.EventSystem);
        }

        private void LateUpdate()
        {
            if (World != null)
                World.ExecuteWorldMessage();
        }
    }
}