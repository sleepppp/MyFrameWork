using System.Text;
namespace Generator.Code
{
    internal class Worker
    {
        public StringBuilder Builder;

        public string GetString() { return Builder.ToString(); }
        public Worker() { Builder = new StringBuilder(); }

        public Worker Append(char c)
        {
            Builder.Append(c);
            return this;
        }

        public Worker Append(char c, int count)
        {
            Builder.Append(c, count);
            return this;
        }

        public Worker Append(string str)
        {
            Builder.Append(str);
            return this;
        }
        public Worker AppendFormat(string str,object[] arr)
        {
            Builder.Append(string.Format(str, arr));
            return this;
        }

        public Worker Space()
        {
            Builder.Append(Token.Space);
            return this;
        }

        public Worker EndLine()
        {
            Builder.AppendLine();
            return this;
        }

        public Worker EndLine(int count)
        {
            for (int i = 0; i < count; ++i)
                Builder.AppendLine();
            return this;
        }

        public Worker Semicolon()
        {
            Builder.Append(Token.Semicolon);
            return this;
        }
    }
}
