using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
////==============================================================================
////이 코드는 자동생성을 통해 생성되었습니다
////파일 내용을 수정하면 잘못된 동작이 발생할 수 있습니다
////만약 생성 오류가 발생했다면 지우고 다시 시도해 주시길 바랍니다
////==============================================================================
namespace MyFramework.GameData
{
	public class TextRecord 
	{
		public int ID;
		public string Value;
	}
	public class TextRecordList 
	{
		public List<TextRecord> List;
	}
	public class TextRecordTable 
	{
		public Dictionary<int,TextRecord> Table;
		public void LoadJson()
		{
			string filePath = "Assets/Data/Json/Text.json";
			AssetManager.LoadAssetAsync<TextAsset>(filePath, (loadResult) =>
			{
				Table = new Dictionary<int,TextRecord>();
				TextRecordList list = JsonConvert.DeserializeObject<TextRecordList>(loadResult.text);
				for (int i = 0; i < list.List.Count; ++i)
				{
					Table.Add(list.List[i].ID, list.List[i]);
				}
			}
			);
		}
		public TextRecord GetRecord(int id)
		{
			TextRecord result = null;
			Table.TryGetValue(id, out result);
			return result;
		}
	}
}
