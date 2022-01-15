using UnityEngine;

namespace MyFrameWork
{
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        static T _instance = null;
        public static T  Instance
        {
            get
            {
                if(_instance == null)
                {
                    T[] instances = FindObjectsOfType<T>();
                    if(instances == null || instances.Length == 0)
                    {
                        GameObject newObject = new GameObject(typeof(T).Name);
                        _instance = newObject.AddComponent<T>();
                    }
                    else if(instances.Length == 1)
                    {
                        _instance = instances[0];
                    }
                    else if(instances.Length >= 2)
                    {
#if UNITY_EDITOR
                        throw new System.Exception("Too Many Instance : " + typeof(T).Name);
#endif
                    }
                }

                return _instance;
            }
        }
    }

    public class Singleton<T> where T : class,new()
    {
        static T _instance;
        public static T Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
    }
}