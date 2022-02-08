using System.Collections.Generic;

namespace Generator.Code
{
    public class MethodInfo
    {
        public string Name;
        public AccessorType AccessorType;
        public string ReturnType;
        public bool IsStatic;
        public Dictionary<string, string> Params = new Dictionary<string, string>();
        public List<string> Bodys = new List<string>();

        public MethodInfo(string name, string returnType,AccessorType accessorType = AccessorType.Private,bool isStatic = false)
        {
            Name = name;
            ReturnType = returnType;
            AccessorType = accessorType;
            IsStatic = isStatic;
        }

        public MethodInfo AddParam(string typeName, string valueName)
        {
            Params.Add(typeName, valueName);
            return this;
        }

        public MethodInfo AddBody(string str)
        {
            Bodys.Add(str);
            return this;
        }
    }
}