
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using System.Text.RegularExpressions;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Generator.Code;
namespace Core.Data
{
    using Core.Data.Utility;
    public static class TableStream
    {
        struct LoadInfo
        {
            public string Path;
            public string JsonPath;
            public string KeyTypeName;
            public string ValueTypeName;
            public string ValueName;

            public LoadInfo(string path, string jsonPath , string keyTypeName, string valueTypeName,string valueName)
            {
                Path = path;
                JsonPath = jsonPath;
                KeyTypeName = keyTypeName;
                ValueTypeName = valueTypeName;
                ValueName = valueName;
            }
        }

        public static Table[] LoadTablesByXLSX(string path)
        {
            Table[] result = null;

            byte[] bin = File.ReadAllBytes(path);
            using (MemoryStream stream = new MemoryStream(bin))
            using (ExcelPackage excelPakage = new ExcelPackage(stream))
            {
                ExcelWorkbook workBook = excelPakage.Workbook;
                result = new Table[workBook.Worksheets.Count];
                int index = 0;
                foreach (ExcelWorksheet sheet in workBook.Worksheets)
                {
                    result[index] = Table.Create(sheet);
                    ++index;
                }
            }

            return result;
        }

        public static Table LoadTableByTSV(string path)
        {
            string splitFileName = @"/";
            string[] arrSplit = Regex.Split(path, splitFileName);
            string fileName = arrSplit[arrSplit.Length - 1];
            
            using (FileStream stream = File.Open(path,FileMode.Open))
            using (StreamReader reader = new StreamReader(stream))
            {
                string body = reader.ReadToEnd();
                return Table.Create(fileName, body);
            }
        }

        public static void WriteTSVByTable(string writePath,Table table)
        {
            using (StreamWriter stream = new StreamWriter(writePath) )
            {
                const string tabToken = "\t";
                const string openBracket = "(";
                const string closeBracket = ")";

                StringBuilder builder = new StringBuilder();
                for(int i =0; i < table.dataNames.Length; ++i)
                {
                    builder.Append(table.dataNames[i]);
                    builder.Append(openBracket);
                    builder.Append(table.typeNames[i]);
                    builder.Append(closeBracket);
                    if (i != table.dataNames.Length - 1)
                        builder.Append(tabToken);
                }

                for(int y=0; y < table.rowCount; ++y)
                {
                    builder.AppendLine();
                    for (int x = 0;x < table.colCount; ++x)
                    {
                        builder.Append(table.data[y, x]);
                        if(x != table.colCount - 1)
                            builder.Append(tabToken);
                    }
                }

                stream.Write(builder.ToString());
            }
        }

        public static void WriteJsonByTable(string writePath, Table table)
        {
            using (StreamWriter stream = new StreamWriter(writePath))
            {
                JObject json = new JObject();
                JArray listJson = new JArray();

                for(int y =0; y < table.rowCount; ++y)
                {
                    JObject colObject = new JObject();
                    for(int x= 0;x < table.colCount; ++x)
                    {
                        colObject.Add(table.dataNames[x], table.data[y, x]);
                    }
                    listJson.Add(colObject);
                }

                json.Add("List", listJson);

                stream.Write(json.ToString());
            }
        }

        public static void SaveJson<T>(string path,Dictionary<int,T> dic)
        {
            using (StreamWriter stream = new StreamWriter(path))
            {
                var arr = dic.Values;
                stream.Write(JsonConvert.SerializeObject(arr));
            }
        }

        public static void WriteExcelObjectByTable(string path, Table table, string jsonPath)
        {
            WriteExcelObject(path, null, table, jsonPath);
        }

        public static void WriteExcelObjectByTable(string path, string namesapceStr, Table table, string jsonPath)
        {
            WriteExcelObject(path, namesapceStr, table,jsonPath);
        }

        static void WriteExcelObject(string path, string namespaceStr, Table table,string jsonPath)
        {
            if (table == null)
            {
                throw new System.Exception("테이블이 존재하지 않습니다.");
            }

            CsInfo csInfo = null;
            {
                if (string.IsNullOrEmpty(namespaceStr))
                    csInfo = new CsInfo(path);
                else
                    csInfo = new CsInfo(path, namespaceStr);

                string recordName = table.name + "Record";

                // {{ Data Class ~
                csInfo.AddUsing("UnityEngine");
                csInfo.AddUsing("System.Collections.Generic");
                csInfo.AddUsing("System.IO");
                csInfo.AddUsing("Newtonsoft.Json");

                csInfo.AddRemark(@"//==============================================================================");
                csInfo.AddRemark(@"//이 코드는 자동생성을 통해 생성되었습니다");
                csInfo.AddRemark(@"//파일 내용을 수정하면 잘못된 동작이 발생할 수 있습니다");
                csInfo.AddRemark(@"//만약 생성 오류가 발생했다면 지우고 다시 시도해 주시길 바랍니다");
                csInfo.AddRemark(@"//==============================================================================");


                AttributeInfo systemSerializable = new AttributeInfo("System.Serializable");
                // {{ RecordClass ~ 
                ClassInfo recordInfo = new ClassInfo(recordName, AccessorType.Public);
                {
                    recordInfo.AddAttribute(systemSerializable);

                    for (int i = 0; i < table.colCount; ++i)
                    {
                        recordInfo.AddField(new FieldInfo(table.typeNames[i], table.dataNames[i], AccessorType.Public));
                    }
                }
                csInfo.AddClass(recordInfo);
                // }} 
                // {{ Record List Class ~ 
                string recordListName = recordName + "List";
                ClassInfo recordListInfo = new ClassInfo(recordListName, AccessorType.Public);
                {
                    recordListInfo.AddAttribute(systemSerializable);
                    recordListInfo.AddField(new FieldInfo(GenericCode.List(recordName), "List", AccessorType.Public));
                }
                csInfo.AddClass(recordListInfo);
                // }} 

                // {{ TableClass ~
                string tableName = recordName + "Table";
                ClassInfo tableClassInfo = new ClassInfo(tableName, AccessorType.Public);
                {
                    string tableType = GenericCode.Dictionary("int", recordName);
                    tableClassInfo.AddField(new FieldInfo(tableType, "Table",AccessorType.Public));
                    MethodInfo loadJsonMethod = new MethodInfo("LoadJson", Keyword.Void, AccessorType.Public);
                    {
                        loadJsonMethod.AddBody(string.Format("string filePath = \"{0}\";", jsonPath));
                        loadJsonMethod.AddBody(string.Format("AssetManager.LoadAssetAsync<TextAsset>(filePath, (loadResult) =>"));
                        loadJsonMethod.AddBody("{");
                        {
                            loadJsonMethod.AddBody(string.Format("Table = new {0}();", tableType));
                            loadJsonMethod.AddBody(string.Format("{0} list = JsonConvert.DeserializeObject<{1}>(loadResult.text);", recordListName, recordListName));
                            loadJsonMethod.AddBody(string.Format("for (int i = 0; i < list.List.Count; ++i)"));
                            loadJsonMethod.AddBody("{");
                            {
                                loadJsonMethod.AddBody("Table.Add(list.List[i].ID, list.List[i]);");
                            }
                            loadJsonMethod.AddBody("}");
                        }
                        loadJsonMethod.AddBody("}");
                        loadJsonMethod.AddBody(");");
                    }
                    tableClassInfo.AddMethod(loadJsonMethod);
                    MethodInfo getRecordMethod = new MethodInfo("GetRecord", recordName, AccessorType.Public);
                    {
                        getRecordMethod.AddParam("int", "id");

                        getRecordMethod.AddBody(string.Format("{0} result = null;",recordName));
                        getRecordMethod.AddBody("Table.TryGetValue(id, out result);");
                        getRecordMethod.AddBody("return result;");
                    }
                    tableClassInfo.AddMethod(getRecordMethod);
                }
                csInfo.AddClass(tableClassInfo);
                // }} 
            }

            using (CodeStream stream = new CodeStream())
            {
                stream.SaveFile(csInfo);
            }
        }
    }
}