using UnityEngine;
using UnityEditor;

namespace MyFramework.EditorCode
{
    public static class GridMeshCreateEditor
    {
        [MenuItem("MyFramework/Create/Mesh/Grid")]
        public static void CreateGridObject()
        {
            Mesh mesh = MeshUtil.CreateGridMesh(CellGroup.GetSize());
            AssetDatabase.CreateAsset(mesh, "Assets/Grid.mesh");
            AssetDatabase.Refresh();
        }
    }
}