using System;
using Microsoft.AspNetCore.Html;

namespace Mozlite.Mvc
{
    /// <summary>
    /// 本地化服务接口。
    /// </summary>
    public interface ILocalizer : ISingletonService
    {
        /// <summary>
        /// 获取当前键的本地化HTML字符串实例。
        /// </summary>
        /// <param name="key">枚举实例。</param>
        /// <returns>返回当前本地化HTML字符串。</returns>
        IHtmlContent this[Enum key] { get; }

        /// <summary>
        /// 获取当前键的本地化HTML字符串实例。
        /// </summary>
        /// <param name="key">枚举实例。</param>
        /// <param name="args">格式化参数。</param>
        /// <returns>返回当前本地化HTML字符串。</returns>
        IHtmlContent this[Enum key, params object[] args] { get; }

        /// <summary>
        /// 获取当前键的本地化字符串实例。
        /// </summary>
        /// <param name="key">资源键。</param>
        /// <returns>返回当前本地化字符串。</returns>
        string GetString(Enum key);

        /// <summary>
        /// 获取当前键的本地化字符串实例。
        /// </summary>
        /// <param name="key">资源键。</param>
        /// <param name="args">格式化参数。</param>
        /// <returns>返回当前本地化字符串。</returns>
        string GetString(Enum key, params object[] args);

        /// <summary>
        /// 获取当前键的本地化字符串实例。
        /// </summary>
        /// <typeparam name="TResource">包含当前资源包的所在程序集的类型。</typeparam>
        /// <param name="key">资源键。</param>
        /// <returns>返回当前本地化字符串。</returns>
        string GetString<TResource>(string key);

        /// <summary>
        /// 获取当前键的本地化字符串实例。
        /// </summary>
        /// <typeparam name="TResource">包含当前资源包的所在程序集的类型。</typeparam>
        /// <param name="key">资源键。</param>
        /// <param name="args">格式化参数。</param>
        /// <returns>返回当前本地化字符串。</returns>
        string GetString<TResource>(string key, params object[] args);
    }
}