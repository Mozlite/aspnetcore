using System;
using System.Threading;

namespace Mozlite.Extensions.Internal
{
    /// <summary>
    /// 确保延迟初始化实例已经实例化类。
    /// </summary>
    public static class NonCapturingLazyInitializer
    {
        /// <summary>
        /// 确定对象已经实例化。
        /// </summary>
        /// <typeparam name="TParam">延迟实例化代理方法参数类型。</typeparam>
        /// <typeparam name="TValue">当前实例类型。</typeparam>
        /// <param name="target">目标对象。</param>
        /// <param name="param">参数实例。</param>
        /// <param name="valueFactory">实例化代理方法。</param>
        /// <returns>返回实例化后的对象。</returns>
        public static TValue EnsureInitialized<TParam, TValue>(
             ref TValue target,
             TParam param,
             Func<TParam, TValue> valueFactory) where TValue : class
        {
            if (Volatile.Read(ref target) != null)
            {
                return target;
            }

            Interlocked.CompareExchange(ref target, valueFactory(param), null);

            return target;
        }

        /// <summary>
        /// 确定对象已经实例化。
        /// </summary>
        /// <typeparam name="TValue">当前实例类型。</typeparam>
        /// <param name="target">目标对象。</param>
        /// <param name="value">实例化的值。</param>
        /// <returns>返回实例化后的对象。</returns>
        public static TValue EnsureInitialized<TValue>(
             ref TValue target,
             TValue value) where TValue : class
        {
            if (Volatile.Read(ref target) != null)
            {
                return target;
            }

            Interlocked.CompareExchange(ref target, value, null);

            return target;
        }
    }
}