using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 行版本列。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class RowVersionAttribute : DatabaseGeneratedAttribute
    {
        /// <summary>初始化 <see cref="RowVersionAttribute" /> 类的新实例。</summary>
        public RowVersionAttribute() : base(DatabaseGeneratedOption.Computed)
        {
        }
    }
}