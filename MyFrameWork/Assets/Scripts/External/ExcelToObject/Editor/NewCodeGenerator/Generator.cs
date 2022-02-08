using System.Collections.Generic;
namespace Generator.Code
{
    public sealed class Generator
    {
        Worker _worker;
        int _codeBlockCount = 0;

        public string GetResult() { return _worker.GetString(); }
        public void AddCodeBlockCount() { _codeBlockCount++; }
        public void RemoveCodeBlockCount() { _codeBlockCount--; }

        public Generator() { _worker = new Worker(); }

        public void Remark(string text)
        {
            CheckTab().Append(Token.Remark).Append(text).EndLine();
        }

        public void Using(string name)
        {
            CheckTab().Append(Keyword.Using).Space().Append(name).Semicolon().EndLine();
        }

        public CodeBlock Namespace(string name)
        {
            CheckTab().Append(Keyword.Namespace).Space().Append(name).EndLine();
            return new CodeBlock(this);
        }

        public void Attribute(string name)
        {
            CheckTab().Append(Token.StartBigBracket).Append(name).Append(Token.EndBigBracket).EndLine();
        }

        public void Attribute(string name ,object[] param)
        {
            CheckTab().Append(Token.StartBigBracket).Append(name).Append(Token.StartSmallBracket);
            for(int i =0; i < param.Length; ++i)
            {
                _worker.Append(param.ToString());
                if (i != param.Length - 1)
                {
                    _worker.Append(Token.Comma);
                }
            }
            _worker.Append(Token.EndSmallBracket).Append(Token.EndBigBracket).EndLine();
        }

        public void DeclarationEnum(string enumName,List<string> elementList, List<string> values, AccessorType accessorType = AccessorType.Public)
        {
            CheckTab().Append(GetAccessorStr(accessorType)).Space().Append(Keyword.Enum).Space().Append(enumName).EndLine();
            using(new CodeBlock(this))
            {
                for(int i =0; i < elementList.Count; ++i)
                {
                    CheckTab().Append(elementList[i]);

                    if(i < values.Count)
                    {
                        _worker.Space().Append(Token.Substitute).Space().Append(values[i]);
                    }

                    _worker.Append(Token.Comma);
                }
            }
        }

        public CodeBlock DeclarationClass(string name, AccessorType accessorType = AccessorType.Private,bool isStatic = false, string inheritanceName = null)
        {
            CheckTab().Append(GetAccessorStr(accessorType)).Space();
            if (isStatic)
                _worker.Append(Keyword.Static).Space();
            _worker.Append(Keyword.Class).Space().Append(name).Space();

            if(string.IsNullOrEmpty(inheritanceName) == false)
            {
                _worker.Append(Token.Colon).Space().Append(inheritanceName);
            }

            _worker.EndLine();

            return new CodeBlock(this);
        }

        public void DeclarationField(string typeName,string fieldName,AccessorType accessorType = AccessorType.Private)
        {
            CheckTab().Append(GetAccessorStr(accessorType)).Space().Append(typeName).Space().Append(fieldName).Semicolon().EndLine();
        }

        public void DeclarationField(string typeName, string fieldName, string initializeValue, AccessorType accessorType = AccessorType.Private)
        {
            CheckTab().Append(GetAccessorStr(accessorType)).Space().Append(typeName).Space().Append(fieldName).Space().
                Append(Token.Substitute).Space().Append(initializeValue).Semicolon().EndLine();
        }

        public void DeclarationMethod(MethodInfo methodInfo)
        {
            CheckTab().Append(GetAccessorStr(methodInfo.AccessorType)).Space();
            if (methodInfo.IsStatic)
                _worker.Append(Keyword.Static).Space();
            _worker.Append(methodInfo.ReturnType).Space().Append(methodInfo.Name).Append(Token.StartSmallBracket);
            Dictionary<string, string> paramList = methodInfo.Params;
            int i = 0; 
            foreach(var item in paramList)
            {
                _worker.Append(item.Key).Space().Append(item.Value);
                if (i != paramList.Count - 1)
                    _worker.Append(Token.Comma);
                i++;
            }
            _worker.Append(Token.EndSmallBracket).EndLine();
            using (new CodeBlock(this))
            {
                List<string> bodyList = methodInfo.Bodys;
                foreach(var item in bodyList)
                {
                    if (item == Token.StartBlock)
                    {
                        StartCodeBlock();
                    }
                    else if(item == Token.EndBlock)
                    {
                        EndCodeBlock();
                    }
                    else
                    { 
                        CheckTab().Append(item).EndLine();
                    }
                }
            }
        }

        public void StartCodeBlock()
        {
            CheckTab().Append(Token.StartBlock).EndLine();
            AddCodeBlockCount();
        }

        public void EndCodeBlock()
        {
            RemoveCodeBlockCount();
            CheckTab().Append(Token.EndBlock).EndLine();
        }

        public void SpaceLine()
        {
            _worker.EndLine();
        }

        Worker Tab()
        {
            return _worker.Append(Token.Tab);
        }

        Worker CheckTab()
        {
            if (_codeBlockCount != 0)
            {
                _worker.Append(Token.Tab,_codeBlockCount);
            }
            return _worker;
        }

        string GetAccessorStr(AccessorType type)
        {
            switch(type)
            {
                case AccessorType.Internal:
                    return Keyword.Internal;
                case AccessorType.Public:
                    return Keyword.Public;
                case AccessorType.Private:
                    return Keyword.Private;
                case AccessorType.Protected:
                    return Keyword.Protected;
                default:
                    return Keyword.Private;
            }
        }
    }
}