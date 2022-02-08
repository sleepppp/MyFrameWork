using System.Text;
namespace MyFramework.GameData
{
    public static class DataUtil
    {
        public static string GetText(this TextRecordTable table,int key)
        {
            TextRecord record = table.GetRecord(key);
            return record.Value;
        }

        public static string GetTextFormat(this TextRecordTable table,int key, params string[] arrParam)
        {
            string origin = GetText(table, key);
            StringBuilder builder = new StringBuilder(origin.Length);
            for(int i =0; i < arrParam.Length; ++i)
            {
                string token = new string(new char[] { '{', i.ToString()[0], '}' });
                int index = origin.IndexOf(token);
                builder.Append(origin.Substring(0, index));
                builder.Append(arrParam[i]);
                int splitIndex = index + 3;
                origin = origin.Substring(splitIndex,origin.Length - splitIndex);
            }

            if(origin.Length > 0)
            {
                builder.Append(origin);
            }

            return builder.ToString();
        }
    }
}