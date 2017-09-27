using System;
using System.ComponentModel.DataAnnotations;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 字符串大小。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SizeAttribute : StringLengthAttribute
    {
        /// <summary>使用指定的最大长度初始化 <see cref="SizeAttribute" /> 类的新实例。</summary>
        /// <param name="maximumLength">字符串的最大长度。</param>
        public SizeAttribute(int maximumLength) : base(maximumLength)
        {
        }
    }
}