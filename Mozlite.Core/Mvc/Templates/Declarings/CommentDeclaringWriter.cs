using System.IO;
using Microsoft.Extensions.Configuration;

namespace Mozlite.Mvc.Templates.Declarings
{
    /// <summary>
    /// 注释!!。
    /// </summary>
    public class CommentDeclaringWriter : IDeclaringWriter
    {
        private readonly bool _comment;
        /// <summary>
        /// 初始化类<see cref="CommentDeclaringWriter"/>。
        /// </summary>
        /// <param name="configuration">配置接口。</param>
        public CommentDeclaringWriter(IConfiguration configuration)
        {
            _comment = configuration.GetSection("Template")["Comment"] != "false";
        }

        /// <summary>
        /// 声明名称。
        /// </summary>
        public virtual string Name => "comment";

        /// <summary>
        /// 将当前声明写入到当前写入实例中。
        /// </summary>
        /// <param name="writer">当前写入实例中。</param>
        /// <param name="declaring">当前声明实例对象。</param>
        /// <param name="model">当前模型对象。</param>
        public virtual void Write(TextWriter writer, Declaring declaring, object model)
        {
            if (_comment)
                return;
            writer.Write(declaring.Parent.Indent());
            writer.WriteLine($"<!--{declaring.Declare}-->");
        }
    }
}