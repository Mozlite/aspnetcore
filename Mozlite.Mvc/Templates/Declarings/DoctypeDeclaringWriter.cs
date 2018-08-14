using System.IO;

namespace Mozlite.Mvc.Templates.Declarings
{
    /// <summary>
    /// 文档类型!doctype
    /// </summary>
    public class DoctypeDeclaringWriter : IDeclaringWriter
    {
        /// <summary>
        /// 声明名称。
        /// </summary>
        public virtual string Name => "doctype";

        /// <summary>
        /// 将当前声明写入到当前写入实例中。
        /// </summary>
        /// <param name="writer">当前写入实例中。</param>
        /// <param name="declaring">当前声明实例对象。</param>
        /// <param name="model">当前模型对象。</param>
        public virtual void Write(TextWriter writer, Declaring declaring, object model)
        {
            var declare = declaring.Declare ?? "html";
            writer.WriteLine("<!DOCTYPE {0}>", declare);
        }
    }
}