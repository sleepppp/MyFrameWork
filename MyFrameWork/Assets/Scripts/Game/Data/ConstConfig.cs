using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace MyFramework
{
    //������ ���̺�� ���⿡ �ָ��� ��� ������ �̰����� �����մϴ�
    public class ConstConfig : ScriptableObject
    {
        public const string FilePath = "Assets/Data/Etc/ConstConfig.asset";
        public static void Load(Action<ConstConfig> callback)
        {
            AssetManager.LoadAssetAsync<ConstConfig>(FilePath, (result) => 
            {
                callback?.Invoke(result);
            });
        }

        [Header("�׽�Ʈ")]
        [Tooltip("�׽�Ʈ ��� ���Դϴ�")] public int Test;
    }
}