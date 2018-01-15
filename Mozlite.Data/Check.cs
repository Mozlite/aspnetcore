using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mozlite.Data.Properties;

namespace Mozlite.Data
{
    /// <summary>
    /// 参数判断辅助类。
    /// </summary>
    [DebuggerStepThrough]
    public static class Check
    {
        /// <summary>
        /// 参数不能为空。
        /// </summary>
        /// <typeparam name="T">当前参数类型。</typeparam>
        /// <param name="value">当前参数值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <returns>返回当前实例对象。</returns>
        public static T NotNull<T>(T value, string parameterName)
        {
            if (ReferenceEquals(value, null))
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        /// <summary>
        /// 参数不能为空。
        /// </summary>
        /// <typeparam name="T">当前参数类型。</typeparam>
        /// <param name="value">当前参数值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>返回当前实例对象。</returns>
        public static T NotNull<T>(T value, string parameterName, string propertyName)
        {
            if (ReferenceEquals(value, null))
            {
                NotEmpty(parameterName, nameof(parameterName));
                NotEmpty(propertyName, nameof(propertyName));

                throw new ArgumentException(string.Format(Resources.ArgumentPropertyNull, propertyName, parameterName));
            }

            return value;
        }

        /// <summary>
        /// 参数不能为空或集合数量必须大于0。
        /// </summary>
        /// <typeparam name="T">当前参数类型。</typeparam>
        /// <param name="value">当前参数值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <returns>返回当前实例对象。</returns>
        public static IReadOnlyList<T> NotEmpty<T>(IReadOnlyList<T> value, string parameterName)
        {
            NotNull(value, parameterName);

            if (value.Count == 0)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException(string.Format(Resources.CollectionArgumentIsEmpty, parameterName));
            }

            return value;
        }

        /// <summary>
        /// 参数不能为空并且长度必须大于0。
        /// </summary>
        /// <param name="value">当前参数值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <returns>返回当前实例对象。</returns>
        public static string NotEmpty(string value, string parameterName)
        {
            Exception e = null;
            if (ReferenceEquals(value, null))
            {
                e = new ArgumentNullException(parameterName);
            }
            else if (value.Trim().Length == 0)
            {
                e = new ArgumentException(string.Format(Resources.ArgumentIsEmpty, parameterName));
            }

            if (e != null)
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw e;
            }

            return value;
        }

        /// <summary>
        /// 参数可为空但是长度必须大于0。
        /// </summary>
        /// <param name="value">当前参数值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <returns>返回当前实例对象。</returns>
        public static string NullButNotEmpty(string value, string parameterName)
        {
            if (!ReferenceEquals(value, null)
                && (value.Length == 0))
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException(string.Format(Resources.ArgumentIsEmpty, parameterName));
            }

            return value;
        }

        /// <summary>
        /// 参数集合中是否包含非空实例。
        /// </summary>
        /// <typeparam name="T">当前参数类型。</typeparam>
        /// <param name="value">当前参数值。</param>
        /// <param name="parameterName">参数名称。</param>
        /// <returns>返回当前实例对象。</returns>
        public static IReadOnlyList<T> HasNoNulls<T>(IReadOnlyList<T> value, string parameterName)
            where T : class
        {
            NotNull(value, parameterName);

            if (value.Any(e => e == null))
            {
                NotEmpty(parameterName, nameof(parameterName));

                throw new ArgumentException(parameterName);
            }

            return value;
        }
    }
}