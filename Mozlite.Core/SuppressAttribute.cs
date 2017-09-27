using System;

namespace Mozlite
{
    /// <summary>
    /// 挂起服务类型，用于替换已有的一些默认服务类型。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class SuppressAttribute : Attribute
    {
        /// <summary>
        /// 初始化类<see cref="SuppressAttribute"/>。
        /// </summary>
        /// <param name="fullName">全名。</param>
        public SuppressAttribute(string fullName)
        {
            FullName = fullName;
        }

        /// <summary>
        /// 初始化类<see cref="SuppressAttribute"/>。
        /// </summary>
        /// <param name="type">要被替换的类型。</param>
        public SuppressAttribute(Type type)
        {
            FullName = type.FullName;
        }

        /// <summary>
        /// 被替换类型的全名。
        /// </summary>
        public string FullName { get; }
    }
}
