using System;
using System.Collections.Generic;
using System.Linq;

namespace Mozlite.Extensions.Internal
{
    /// <summary>
    /// 属性列表对比器。
    /// </summary>
    public class PropertyListComparer : IComparer<IReadOnlyList<IProperty>>, IEqualityComparer<IReadOnlyList<IProperty>>
    {
        /// <summary>
        /// 属性列表对比器实例。
        /// </summary>
        public static readonly PropertyListComparer Instance = new PropertyListComparer();

        private PropertyListComparer()
        {
        }

        /// <summary>比较两个对象并返回一个值，该值指示一个对象小于、等于还是大于另一个对象。</summary>
        /// <returns>一个有符号整数，指示 <paramref name="x" /> 与 <paramref name="y" /> 的相对值，如下表所示。值含义小于零<paramref name="x" /> 小于 <paramref name="y" />。零<paramref name="x" /> 等于 <paramref name="y" />。大于零<paramref name="x" /> 大于 <paramref name="y" />。</returns>
        /// <param name="x">要比较的第一个对象。</param>
        /// <param name="y">要比较的第二个对象。</param>
        public int Compare(IReadOnlyList<IProperty> x, IReadOnlyList<IProperty> y)
        {
            var result = x.Count - y.Count;

            if (result != 0)
            {
                return result;
            }

            var index = 0;
            while ((result == 0)
                   && (index < x.Count))
            {
                result = StringComparer.Ordinal.Compare(x[index].Name, y[index].Name);
                index++;
            }
            return result;
        }

        /// <summary>确定指定的对象是否相等。</summary>
        /// <returns>如果指定的对象相等，则为 true；否则为 false。</returns>
        /// <param name="x">要比较的第一个类型为 <paramref name="T" /> 的对象。</param>
        /// <param name="y">要比较的第二个类型为 <paramref name="T" /> 的对象。</param>
        public bool Equals(IReadOnlyList<IProperty> x, IReadOnlyList<IProperty> y)
            => Compare(x, y) == 0;

        /// <summary>返回指定对象的哈希代码。</summary>
        /// <returns>指定对象的哈希代码。</returns>
        /// <param name="obj">
        /// <see cref="T:System.Object" />，将为其返回哈希代码。</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="obj" /> 的类型为引用类型，<paramref name="obj" /> 为 null。</exception>
        public int GetHashCode(IReadOnlyList<IProperty> obj)
            => obj.Aggregate(0, (hash, p) => unchecked((hash * 397) ^ p.GetHashCode()));
    }
}