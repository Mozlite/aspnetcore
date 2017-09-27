using System;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 目标类型特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TargetAttribute : Attribute
    {
        /// <summary>
        /// 初始化类<see cref="TargetAttribute"/>。
        /// </summary>
        /// <param name="type">目标模型类型。</param>
        public TargetAttribute(Type type)
        {
            Target = type;
        }

        /// <summary>
        /// 目标类型。
        /// </summary>
        public Type Target { get; }
    }
}
