using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyFramework
{
    interface IObjectPool<T>
    {
        T Get();
        T Instantiate();
    }

    public class GameObjectPool<T> : IObjectPool<T> where T : Component
    {
        List<T> _objectList = new List<T>();
        public GameObject Prefab;
        public Transform Parent;
        public bool IsSleepAfterInstantiate;
        public bool IsAutoActive;

        public GameObjectPool(GameObject prefab, Transform parent = null, int count = 0, bool isSleepAfterInstantiate = true, bool isAutoActive = true)
        {
            Prefab = prefab;
            Parent = parent;
            IsSleepAfterInstantiate = isSleepAfterInstantiate;
            IsAutoActive = isAutoActive;

            for (int i = 0; i < count; ++i)
            {
                Instantiate();
            }
        }

        public T Get()
        {
            foreach (var item in _objectList)
            {
                if (item.gameObject.activeSelf == false)
                {
                    if (IsAutoActive)
                        item.gameObject.SetActive(true);

                    return item;
                }
            }

            T result = Instantiate();
            if (IsAutoActive)
                result.gameObject.SetActive(true);
            return result;
        }

        public T Instantiate()
        {
            GameObject newObject = null;
            if (Parent)
                newObject = GameObject.Instantiate(Prefab, Parent);
            else
                newObject = GameObject.Instantiate(Prefab);

            T result = newObject.GetComponent<T>();
            if (result == null)
            {
                throw new System.Exception("No Component : " + typeof(T).Name);
            }
            _objectList.Add(result);

            if (IsSleepAfterInstantiate)
                result.gameObject.SetActive(false);

            return result;
        }

        public void Foreach(Action<T> query)
        {
            foreach (var item in _objectList)
            {
                query.Invoke(item);
            }
        }

        public void SleepAll()
        {
            foreach (var item in _objectList)
            {
                item.gameObject.SetActive(false);
            }
        }

    }
}