using System.Collections.Generic;
namespace Generator.Code
{
    public sealed class CsInfo
    {
        public string FileName;
        public string NameSpace;
        public List<string> UsingList = new List<string>();
        public List<EnumInfo> EnumList = new List<EnumInfo>();
        public List<ClassInfo> ClassList = new List<ClassInfo>();
        public List<string> RemarkList = new List<string>();

        public CsInfo(string fileName,string nameSpace = "CodeGenerator.Test")
        {
            FileName = fileName;
            NameSpace = nameSpace;
        }

        public CsInfo AddUsing(string str)
        {
            UsingList.Add(str);
            return this;
        }

        public CsInfo AddEnum(EnumInfo enumInfo)
        {
            EnumList.Add(enumInfo);
            return this;
        }

        public CsInfo AddClass(ClassInfo classInfo)
        {
            ClassList.Add(classInfo);
            return this;
        }

        public CsInfo AddRemark(string str)
        {
            RemarkList.Add(str);
            return this;
        }
    }
}