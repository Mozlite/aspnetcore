using System.Reflection;
using Mozlite.Extensions;

namespace Mozlite.Data
{
    /// <summary>
    /// 类型扩展。
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// 判断是否能够新建操作。
        /// </summary>
        /// <param name="info">当前属性实例。</param>
        /// <returns>返回判断结果。</returns>
        public static bool IsCreating(this PropertyInfo info)
        {
            if (!info.CanWrite)
                return false;
            if (!info.CanRead)
                return false;
            if (info.IsDefined(typeof(IdentityAttribute)))
                return false;
            if (info.IsDefined(typeof(RowVersionAttribute)))
                return false;
            return true;
        }
    }
}