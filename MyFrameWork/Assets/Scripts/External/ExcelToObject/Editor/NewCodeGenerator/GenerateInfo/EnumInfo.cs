using System.Collections.Generic;

namespace Generator.Code
{
    public sealed class EnumInfo
    {
        public string EnumName;
        public AccessorType AccessorType;
        public List<string> ElementNames = new List<string>();
        public List<string> ElementValues = new List<string>();
        
        public EnumInfo(string enumName,AccessorType accessorType = AccessorType.Public)
        {
            EnumName = enumName;
            AccessorType = accessorType;
        }

        public EnumInfo AddElement(string name)
        {
            ElementNames.Add(name);
            return this;
        }

        public EnumInfo AddElement(string name, string value)
        {
            ElementNames.Add(name);
            ElementValues.Add(value);
            return this;
        }
    }
}