using System.IO;

namespace Mozlite.Mvc.Templates.Declarings
{
    /// <summary>
    /// 声明写入器。
    /// </summary>
    public interface IDeclaringWriter : ISingletonServices
    {
        /// <summary>
        /// 声明名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 将当前声明写入到当前写入实例中。
        /// </summary>
        /// <param name="writer">当前写入实例中。</param>
        /// <param name="declaring">当前声明实例对象。</param>
        /// <param name="model">当前模型对象。</param>
        void Write(TextWriter writer, Declaring declaring, object model);
    }
}