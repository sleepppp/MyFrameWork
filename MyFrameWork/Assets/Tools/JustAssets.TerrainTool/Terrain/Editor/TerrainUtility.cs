using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace JustAssets.TerrainUtility
{
    public class TerrainUtility : EditorWindow
    {
        public class Option
        {
            public Option(Operation operation, GUIContent icon, IValidate<Terrain> handler, object[] @params)
            {
                Operation = operation;
                Icon = icon;
                Handler = handler;
                Params = @params;
            }

            public Operation Operation { get; set; }

            public GUIContent Icon { get; set; }

            public IValidate<Terrain> Handler { get; set; }

            public object[] Params { get; }
        }

        public class SaveInfo
        {
            public string TerrainName;
            public UnityEngine.Object PrefabFolder;
            public UnityEngine.Object DataSaveFolder;
        }

        public enum Operation
        {
            None,

            SplitTerrain,

            MergeTerrain
        }

        private Operation _operation = Operation.None;

        private List<Operation> _operations = Enum.GetValues(typeof(Operation)).OfType<Operation>().Skip(1).ToList();

        private Vector2 _view;

        private TerrainUtilityUIData _iconData;
        
        [ItemCanBeNull]
        private Terrain[] _terrainSelection;

        private bool _execute;

        private Dictionary<Operation, Option> _options;

        private UnityEngine.Object _prefabSaveFolder;
        private UnityEngine.Object _dataSaveFolder;
        private string _terrainName;

        [MenuItem("Tools/Terrain/Terrain Utility")]
        public static void ShowWindow()
        {
            var window = GetWindow<TerrainUtility>("Terrain Utility");
            window.UpdateSelection();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Operation", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            {
                var iconSize = EditorGUIUtility.GetIconSize();
                EditorGUIUtility.SetIconSize(new Vector2(64, 64));
                EditorGUILayout.BeginVertical();
                for (var index = 0; index < _operations.Count; index++)
                {
                    if (index % 3 == 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                    }

                    Operation operation = _operations[index];
                    var style = new GUIStyle(EditorStyles.miniButton) {stretchHeight = true, fixedHeight = 0, imagePosition = ImagePosition.ImageAbove};

                    if (_options.TryGetValue(operation, out var option) && GUILayout.Button(option.Icon, style, GUILayout.Width(120), GUILayout.Height(100)))
                    {
                        _operation = operation;
                    }

                    if (index % 3 == 2 || index == _operations.Count-1)
                    {
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUIUtility.SetIconSize(iconSize);
            }
            EditorGUILayout.EndHorizontal();

            _options.TryGetValue(_operation, out Option activeOption);
            if (activeOption != null)
            {
                EditorGUILayout.LabelField(activeOption.Icon.text, EditorStyles.boldLabel);

                EditorGUILayout.HelpBox(activeOption.Icon.tooltip, MessageType.Info);

                var isValid = activeOption.Handler.IsValid(out var reason);
                if (!isValid)
                    EditorGUILayout.HelpBox(reason, MessageType.Warning);

                EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
                switch (_operation)
                {
                    case Operation.SplitTerrain:
                        var tiling = (SplitTerrain.PowerOf2Squared) _options[Operation.SplitTerrain].Params[1];
                        _options[Operation.SplitTerrain].Params[1] = EditorGUILayout.EnumPopup("Tiling", tiling);
                        var splitTrees = (bool) _options[Operation.SplitTerrain].Params[0];
                        _options[Operation.SplitTerrain].Params[0] = EditorGUILayout.ToggleLeft("Split Trees", splitTrees);

                        break;
                    case Operation.MergeTerrain:
                        EditorGUILayout.Space();
                        break;
                }

                _view = EditorGUILayout.BeginScrollView(_view);
                {
                    EditorGUILayout.LabelField("Selected terrains", EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Count: ", _terrainSelection.Length.ToString(), EditorStyles.miniLabel);
                    EditorGUIUtility.labelWidth = 0;
                    int i = 0;
                    foreach (var terrain in _terrainSelection)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField(terrain, typeof(Terrain), false);
                        EditorGUILayout.EndHorizontal();
                        i++;
                    }
                }

                //=========================================================================================================================================
                //Custom
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Terrain Name", EditorStyles.boldLabel);
                    _terrainName = EditorGUILayout.TextField(_terrainName);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Prefab Save Folder", EditorStyles.boldLabel);
                    _prefabSaveFolder = EditorGUILayout.ObjectField(_prefabSaveFolder, typeof(UnityEngine.Object), false);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("DataFile Save Folder", EditorStyles.boldLabel);
                    _dataSaveFolder = EditorGUILayout.ObjectField(_dataSaveFolder, typeof(UnityEngine.Object), false);
                }
                EditorGUILayout.EndHorizontal();
                //=============================================================================================================================================


                EditorGUILayout.EndScrollView();

                EditorGUILayout.BeginHorizontal(GUILayout.Height(20));
                {
                    GUI.enabled = isValid;
                    if (activeOption.Handler != null)
                    {
                        if (GUILayout.Button(activeOption.Icon.text + " now"))
                            _execute = true;
                    }

                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal();

            }
            else
            {
                EditorGUILayout.HelpBox("Select a tool to show details.", MessageType.Info);
            }
        }

        private void Update()
        {
            if (!_execute)
                return;

            if (!_options.TryGetValue(_operation, out var option))
                return;

            switch (_operation)
            {
                case Operation.SplitTerrain:
                    SaveInfo saveInfo = new SaveInfo()
                    {
                        TerrainName = _terrainName,
                        PrefabFolder = _prefabSaveFolder,
                        DataSaveFolder = _dataSaveFolder
                    };

                    ((SplitTerrain)option.Handler).Split(saveInfo,(bool) option.Params[0], (SplitTerrain.PowerOf2Squared) option.Params[1]);
                    break;
                case Operation.MergeTerrain:
                    ((MergeTerrain)_options[Operation.MergeTerrain].Handler).Merge();
                    break;

            }

            _execute = false;
            _operation = Operation.None;
        }

        public void OnEnable()
        {
            var assetGUID = AssetDatabase.FindAssets($"t:{nameof(TerrainUtilityUIData)} {nameof(TerrainUtilityUIData)}").FirstOrDefault();
            var assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
            _iconData = AssetDatabase.LoadAssetAtPath<TerrainUtilityUIData>(assetPath);

            _options = new Dictionary<Operation, Option>
            {
                {
                    Operation.SplitTerrain,
                    new Option(Operation.SplitTerrain,
                        new GUIContent("Split terrain(s)", _iconData.SplitTerrain, "Splits all selected terrains into n pieces."), new SplitTerrain(),
                        new object[] {true, SplitTerrain.PowerOf2Squared._2x2})
                },
                {
                    Operation.MergeTerrain,
                    new Option(Operation.MergeTerrain, new GUIContent("Merge terrains", _iconData.MergeTerrain, "Merges selected terrains together."),
                        new MergeTerrain(), new object[] { })
                },
            };
        }

        public void OnSelectionChange()
        {
            UpdateSelection();
        }

        private void UpdateSelection()
        {
            _terrainSelection = Selection.gameObjects.SelectMany(x => x.GetComponentsInChildren<Terrain>()).ToArray();

            foreach (Option option in _options.Values)
            {
                option.Handler.Selection = _terrainSelection;
            }

            Repaint();
        }
    }
}