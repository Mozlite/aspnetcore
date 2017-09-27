using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 自增长特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IdentityAttribute : DatabaseGeneratedAttribute
    {
        /// <summary>初始化 <see cref="IdentityAttribute" /> 类的新实例。</summary>
        public IdentityAttribute() : base(DatabaseGeneratedOption.Identity)
        {
        }
    }
}