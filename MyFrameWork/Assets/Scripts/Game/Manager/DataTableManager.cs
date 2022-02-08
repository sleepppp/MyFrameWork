using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyFramework.GameData
{
    public class DataTableManager : Singleton<DataTableManager>
    {
        public static ConstConfig ConstConfig;
        public static TextRecordTable TextTable;
        public static void Init()
        {
            TextTable = new TextRecordTable();
        }

        public static void Load()
        {
            ConstConfig.Load((result)=> { ConstConfig = result; });
            TextTable.LoadJson();
        }

        public static bool IsCompleteLoad()
        {
            if (ConstConfig == null) return false;
            if (TextTable == null || TextTable.Table == null) return false;
            return true;
        }
    }
}