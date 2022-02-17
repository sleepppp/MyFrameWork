using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MyFramework.EditorCode
{
    public class GridEditorWindow : EditorWindow
    {
        [MenuItem("MyFramework/Tools/GridEditor")]
        public static void Open()
        {
            GridEditorWindow window = GetWindow<GridEditorWindow>();
        }

        GridRenderer _targetGridRenderer;

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("TargetGrid");
                _targetGridRenderer = EditorGUILayout.ObjectField(_targetGridRenderer, typeof(GridRenderer), true) as GridRenderer;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}