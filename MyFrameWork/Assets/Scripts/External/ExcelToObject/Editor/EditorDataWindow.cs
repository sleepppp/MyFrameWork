using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace MyFramework.EditorCode
{
    using Core.Data;
    public class EditorDataWindow : EditorWindow
    {
        [MenuItem("MyFramework/Data/LoadExcel")]
        static void Open()
        {
            EditorDataWindow window = 
                EditorWindow.GetWindow(typeof(EditorDataWindow)) as EditorDataWindow;
            if (window == null)
                return;

            window.Show();
        }

        DataScriptableObject m_loadData;

        private void OnGUI()
        {
            m_loadData = EditorGUILayout.ObjectField("Load Data", m_loadData,
                typeof(DataScriptableObject), false, null) as DataScriptableObject;

            if(GUILayout.Button("Create All Files"))
            {
                CreateAllFiles();
            }

            EditorGUILayout.Space();
            if(GUILayout.Button("Create Json File"))
            {
                CreateJsonFiles();
            }
        }

        void CreateAllFiles()
        {
            string excelFolderPath = AssetDatabase.GetAssetPath(m_loadData.ExcelFolder);
            string codeFolderPath = AssetDatabase.GetAssetPath(m_loadData.CodeFolder);
            string jsonFolderPath = AssetDatabase.GetAssetPath(m_loadData.JsonFolder);

            DirectoryInfo directoryInfo = new DirectoryInfo(excelFolderPath);
            string xlsx = ".xlsx";
            List<Core.Data.Table> tableList = new List<Table>();
            List<string> jsonPaths = new List<string>();
            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
            {
                //같으면 0을 반환함
                if (fileInfo.Extension.ToLower().CompareTo(xlsx) == 0)
                {
                    Table[] tables = TableStream.LoadTablesByXLSX(fileInfo.FullName);
                    string[] jsons = new string[tables.Length];
                    for (int i = 0; i < tables.Length; ++i)
                    {
                        jsons[i] = jsonFolderPath + "/" + tables[i].name + ".json";
                        TableStream.WriteExcelObjectByTable(codeFolderPath + "/" + tables[i].name + ".cs",
                            m_loadData.NameSpace, tables[i], jsons[i]);
                        TableStream.WriteJsonByTable(jsons[i], tables[i]);
                    }
                    tableList.AddRange(tables);
                    jsonPaths.AddRange(jsons);
                }
            }
            AssetDatabase.Refresh();
        }

        void CreateJsonFiles()
        {
            string excelFolderPath = AssetDatabase.GetAssetPath(m_loadData.ExcelFolder);
            string jsonFolderPath = AssetDatabase.GetAssetPath(m_loadData.JsonFolder);

            DirectoryInfo directoryInfo = new DirectoryInfo(excelFolderPath);
            string xlsx = ".xlsx";
            List<Core.Data.Table> tableList = new List<Table>();
            List<string> jsonPaths = new List<string>();
            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
            {
                //같으면 0을 반환함
                if (fileInfo.Extension.ToLower().CompareTo(xlsx) == 0)
                {
                    Table[] tables = TableStream.LoadTablesByXLSX(fileInfo.FullName);
                    string[] jsons = new string[tables.Length];
                    for (int i = 0; i < tables.Length; ++i)
                    {
                        jsons[i] = jsonFolderPath + "/" + tables[i].name + ".json";
                        TableStream.WriteJsonByTable(jsons[i], tables[i]);
                    }
                    tableList.AddRange(tables);
                    jsonPaths.AddRange(jsons);
                }
            }
            AssetDatabase.Refresh();
        }
    }
}
