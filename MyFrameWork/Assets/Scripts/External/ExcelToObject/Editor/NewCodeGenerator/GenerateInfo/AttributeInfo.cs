using System.Collections.Generic;
namespace Generator.Code
{
    public sealed class AttributeInfo
    {
        public string Name;
        public List<string> Params = new List<string>();

        public AttributeInfo(string name,params string[] arrParam)
        {
            Name = name;
            Params.AddRange(arrParam);
        }

        public AttributeInfo AddParam(string param)
        {
            Params.Add(param);
            return this;
        }
    }
}