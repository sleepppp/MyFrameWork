using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace MyFramework
{
    //데이터 테이블로 들어가기에 애매한 상수 값들은 이곳에서 관리합니다
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

        [Header("테스트")]
        [Tooltip("테스트 상수 값입니다")] public int Test;
    }
}