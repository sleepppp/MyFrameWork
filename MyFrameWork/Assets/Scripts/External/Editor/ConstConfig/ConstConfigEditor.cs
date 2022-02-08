using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MyFramework.EditorCode
{
    public class ConstConfigEditor
    {
        [MenuItem("MyFramework/Data/Create/CreateConstConfigAsset")]
        public static void CreateConstConfigAsset()
        {
            ConstConfig instance = ScriptableObject.CreateInstance<ConstConfig>();

            if (AssetDatabase.IsValidFolder("Assets/Data") == false)
            {
                AssetDatabase.CreateFolder("Assets", "Data");
            }

            if (AssetDatabase.IsValidFolder("Assets/Data/Etc") == false)
            {
                AssetDatabase.CreateFolder("Assets/Data", "Etc");
            }

            AssetDatabase.CreateAsset(instance,ConstConfig.FilePath);
        }
    }
}