using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MyFramework.EditorCode
{
    public class GridTerrainCreate
    {
        [MenuItem("MyFramework/Create/Terrain")]
        public static void CreateTerrain()
        {
            float defaultTerrainSize = 160f;

            TerrainData terrainData = new TerrainData();
            terrainData.size = new Vector3(defaultTerrainSize, 0f, defaultTerrainSize);
            GameObject newObject = Terrain.CreateTerrainGameObject(terrainData);

            GameObject gridObject = MeshUtil.CreateGridObject(defaultTerrainSize);
            gridObject.transform.SetParent(newObject.transform,true);
            gridObject.transform.localPosition = new Vector3(0f, 0.01f, 0f);
        }
    }
}