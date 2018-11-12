using Mozlite.Data.Properties;
using Mozlite.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

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
        public static bool IsCreatable(this IProperty info)
        {
            return info.PropertyInfo.IsCreatable();
        }

        /// <summary>
        /// 判断是否能够更新操作。
        /// </summary>
        /// <param name="info">当前属性实例。</param>
        /// <returns>返回判断结果。</returns>
        public static bool IsUpdatable(this IProperty info)
        {
            return info.PropertyInfo.IsUpdatable();
        }

        /// <summary>
        /// 判断是否能够新建操作。
        /// </summary>
        /// <param name="info">当前属性实例。</param>
        /// <returns>返回判断结果。</returns>
        public static bool IsCreatable(this PropertyInfo info)
        {
            if (!info.CanWrite)
                return false;
            if (!info.CanRead)
                return false;
            if (info.IsDefined(typeof(NotMappedAttribute)))
                return false;
            if (info.IsDefined(typeof(IdentityAttribute)))
                return false;
            if (info.IsDefined(typeof(RowVersionAttribute)))
                return false;
            return true;
        }

        /// <summary>
        /// 判断是否能够更新操作。
        /// </summary>
        /// <param name="info">当前属性实例。</param>
        /// <returns>返回判断结果。</returns>
        public static bool IsUpdatable(this PropertyInfo info)
        {
            if (!info.CanWrite)
                return false;
            if (!info.CanRead)
                return false;
            if (info.IsDefined(typeof(NotUpdatedAttribute)))
                return false;
            if (info.IsDefined(typeof(NotMappedAttribute)))
                return false;
            if (info.IsDefined(typeof(IdentityAttribute)))
                return false;
            if (info.IsDefined(typeof(KeyAttribute)))
                return false;//主键也不更新
            if (info.IsDefined(typeof(RowVersionAttribute)))
                return false;
            return true;
        }

        /// <summary>
        /// 判断是否为匿名类型。
        /// </summary>
        /// <param name="type">当前类型。</param>
        /// <returns>返回判断结果。</returns>
        public static bool IsAnonymous(this Type type)
        {
            return type.IsClass && type.Name.IndexOf("f__AnonymousType", StringComparison.Ordinal) != -1;
        }

        /// <summary>
        /// 获取唯一的主键属性值，如果主键不值一个属性，则会抛出错误。
        /// </summary>
        /// <param name="entityType">实体类型。</param>
        /// <returns>返回属性实例。</returns>
        public static IProperty SingleKey(this IEntityType entityType)
        {
            var key = entityType.PrimaryKey.Properties;
            if (key.Count > 1)
                throw new IndexOutOfRangeException(string.Format(Resources.PrimaryKeyIsNotSingleField, entityType.ClrType, string.Join(", ", key)));
            return key.Single();
        }

        /// <summary>
        /// 将对象转换为键值对字典实例。
        /// </summary>
        /// <param name="parameters">参数（字典实例或者匿名对象）。</param>
        /// <param name="stringComparer">属性字符串对比实例。</param>
        /// <returns>返回键值对字典实例。</returns>
        public static IDictionary<string, object> ToDictionary(this object parameters, StringComparer stringComparer = null)
        {
            if (parameters is IDictionary<string, object> dic)
                return dic;
            return parameters
                .GetType()
                .GetProperties()
                .Where(x => x.CanRead)
                .Select(x => new KeyValuePair<string, object>(x.Name, x.GetValue(parameters)))
                .ToDictionary(stringComparer ?? StringComparer.OrdinalIgnoreCase);
        }
    }
}