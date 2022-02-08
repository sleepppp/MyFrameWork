using System.Collections.Generic;

namespace Generator.Code
{
    public sealed class ClassInfo
    {
        public string Name;
        public AccessorType AccessorType;
        public bool IsStatic;
        public string HaritanceName;
        public List<EnumInfo> Enums = new List<EnumInfo>();
        //public List<ClassInfo> Classes = new List<ClassInfo>();
        public List<FieldInfo> Fields = new List<FieldInfo>();
        public List<MethodInfo> Methods = new List<MethodInfo>();
        public List<AttributeInfo> Attributes = new List<AttributeInfo>();

        public ClassInfo(string name,AccessorType accessorType = AccessorType.Private,string haritance = null, bool isStatic = false)
        {
            Name = name;
            AccessorType = accessorType;
            HaritanceName = haritance;
            IsStatic = isStatic;
        }

        public ClassInfo AddEnum(EnumInfo enumInfo)
        {
            Enums.Add(enumInfo);
            return this;
        }

        public ClassInfo AddField(FieldInfo fieldInfo)
        {
            Fields.Add(fieldInfo);
            return this;
        }

        public ClassInfo AddMethod(MethodInfo method)
        {
            Methods.Add(method);
            return this;
        }

        public ClassInfo AddAttribute(AttributeInfo attribute)
        {
            Attributes.Add(attribute);
            return this;
        }
    }
}