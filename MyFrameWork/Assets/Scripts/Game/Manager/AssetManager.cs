using System.Collections;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

namespace MyFramework
{
    public class AssetManager
    {
        public static void Init()
        {
            //todo 
        }
        
        public static void LoadAssetAsync<T>(string filePath, Action<T> callback) where T : UnityEngine.Object
        {
            Addressables.LoadAssetAsync<T>(filePath).Completed += (handle) => 
            {
                callback?.Invoke(handle.Result);
            };
        }
        public static void LoadSpriteAsync(string filePath ,Action<Sprite> callback)
        {
            LoadAssetAsync<Sprite>(filePath, (sp) =>
            {
                Sprite newSprite = sp;
                
                callback?.Invoke(newSprite);
            });
        }

        public static void Instantiate<T>(string filePath, Action<T> callback, Transform parent = null) where T : Component
        {
            LoadAssetAsync<GameObject>(filePath, (prefab) =>
            {
                GameObject newObject = null;
                if (parent)
                    newObject = GameObject.Instantiate(prefab, parent);
                else
                    newObject = GameObject.Instantiate(prefab);

                T result = newObject.GetComponent<T>();

                callback?.Invoke(result);
            });
        }

        public static void Instantiate<T>(string filePath, Action<T> callback, Vector3 location, Quaternion rotation, Transform parent) where T : Component
        {
            LoadAssetAsync<GameObject>(filePath, (prefab) =>
            {
                GameObject newObject = null;
                if (parent)
                    newObject = GameObject.Instantiate(prefab, location, rotation, parent);
                else
                    newObject = GameObject.Instantiate(prefab, location, rotation);

                T result = newObject.GetComponent<T>();

                callback?.Invoke(result);
            });
        }
    }
}