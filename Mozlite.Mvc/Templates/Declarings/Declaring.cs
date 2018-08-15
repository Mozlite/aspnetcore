using System.Collections.Generic;
using System.Linq;

namespace Mozlite.Mvc.Templates.Declarings
{
    /// <summary>
    /// 声明语法。
    /// </summary>
    public class Declaring
    {
        /// <summary>
        /// 初始化类<see cref="Declaring"/>。
        /// </summary>
        public Declaring() { }

        /// <summary>
        /// 初始化类<see cref="Declaring"/>。
        /// </summary>
        /// <param name="reader">代码读取器。</param>
        internal Declaring(CodeReader reader)
        {//注释!!
            if (reader.IsNext('!'))
            {
                reader.Skip();
                Name = "comment";
            }
            else
                Name = reader.ReadName();
            var source = reader.ReadUntil("\r\n")?.Trim(' ', ';');
            if (!string.IsNullOrWhiteSpace(source))
                ReadDeclaring(source);
        }

        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 声明字符串。
        /// </summary>
        public string Declare { get; set; }

        /// <summary>
        /// 参数。
        /// </summary>
        public string Parameters { get; set; }

        /// <summary>
        /// 特性。
        /// </summary>
        public IDictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// 声明类型。
        /// </summary>
        public DeclaringType DeclaringType { get; set; }

        /// <summary>
        /// 所属语法。
        /// </summary>
        public Syntax Parent { get; internal set; }

        /// <summary>
        /// 呈现声明的字符串。
        /// </summary>
        /// <returns>呈现声明的字符串。</returns>
        public override string ToString()
        {
            switch (DeclaringType)
            {
                case DeclaringType.Attributes:
                    var attributes = Attributes?
                        .Select(x => $"{x.Key}=\"{x.Value?.Replace("\"", "\\\"")}\"")
                        .ToArray();
                    if (attributes != null)
                        return $"<{Name} {string.Join(" ", attributes)} />";
                    return $"<{Name} />";
                case DeclaringType.Declare:
                    if (string.IsNullOrEmpty(Declare))
                        return $"<!{Name}>";
                    return $"<{Name} {Declare}/>";
                default:
                    if (Parameters != null)
                        return $"{Name}({Parameters})";
                    return $"{Name}()";
            }
        }

        private void ReadDeclaring(string source)
        {
            var reader = new CodeReader(source);
            if (reader.IsNextNonWhiteSpace("({"))
            {
                Attributes = reader.ReadAttributes();
                DeclaringType = DeclaringType.Attributes;
            }
            else if (reader.IsNextNonWhiteSpace('('))
            {
                Parameters = reader.ReadParameters();
                DeclaringType = DeclaringType.Parameters;
            }
            else
            {
                Declare = source;
                DeclaringType = DeclaringType.Declare;
            }
        }
    }
}