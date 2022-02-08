using System;

namespace Generator.Code
{
    public sealed class FieldInfo
    {
        public bool IsStatic;
        public bool IsConst;
        public bool IsReadonly;
        public AccessorType AccessorType;
        public string TypeName;
        public string FieldName;
        public string StartValue;

        public FieldInfo(string typeName, string fieldName, AccessorType accessorType = AccessorType.Private, 
            string startValue = null, bool isStatic = false, bool isConst = false, bool isReadonly = false)
        {
            TypeName = typeName;
            FieldName = fieldName;
            AccessorType = accessorType;
            IsConst = isConst;
            IsStatic = isStatic;
            IsReadonly = isReadonly;
        }

    }
}