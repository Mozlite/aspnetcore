using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Resources;
using Microsoft.AspNetCore.Html;

namespace Mozlite.Mvc
{
    /// <summary>
    /// 本地化资源实现类。
    /// </summary>
    public class Localizer : ILocalizer
    {
        private readonly ConcurrentDictionary<Type, ResourceManager> _localizers = new ConcurrentDictionary<Type, ResourceManager>();
        /// <summary>
        /// 获取当前键的本地化HTML字符串实例。
        /// </summary>
        /// <param name="key">枚举实例。</param>
        /// <returns>返回当前本地化HTML字符串。</returns>
        IHtmlContent ILocalizer.this[Enum key] => new HtmlString(GetString(key));

        /// <summary>
        /// 获取当前键的本地化HTML字符串实例。
        /// </summary>
        /// <param name="key">枚举实例。</param>
        /// <param name="args">格式化参数。</param>
        /// <returns>返回当前本地化HTML字符串。</returns>
        IHtmlContent ILocalizer.this[Enum key, params object[] args] => new HtmlString(GetString(key, args));

        /// <summary>
        /// 获取当前键的本地化字符串实例。
        /// </summary>
        /// <param name="key">资源键。</param>
        /// <returns>返回当前本地化字符串。</returns>
        public virtual string GetString(Enum key)
        {
            var type = key.GetType();
            var resource = GetString(type, $"{type.Name}_{key}");
            if (resource == null)
                return key.ToString();
            return resource;
        }

        /// <summary>
        /// 获取当前键的本地化字符串实例。
        /// </summary>
        /// <param name="key">资源键。</param>
        /// <param name="args">格式化参数。</param>
        /// <returns>返回当前本地化字符串。</returns>
        public virtual string GetString(Enum key, params object[] args)
        {
            var type = key.GetType();
            var resource = GetString(type, $"{type.Name}_{key}");
            if (resource == null)
                return key.ToString();
            return string.Format(resource, args);
        }

        /// <summary>
        /// 获取当前键的本地化字符串实例。
        /// </summary>
        /// <typeparam name="TResource">包含当前资源包的所在程序集的类型。</typeparam>
        /// <param name="key">资源键。</param>
        /// <returns>返回当前本地化字符串。</returns>
        public virtual string GetString<TResource>(string key)
        {
            return GetString(typeof(TResource), key);
        }

        /// <summary>
        /// 获取当前键的本地化字符串实例。
        /// </summary>
        /// <typeparam name="TResource">包含当前资源包的所在程序集的类型。</typeparam>
        /// <param name="key">资源键。</param>
        /// <param name="args">格式化参数。</param>
        /// <returns>返回当前本地化字符串。</returns>
        public virtual string GetString<TResource>(string key, params object[] args)
        {
            var resource = GetString(typeof(TResource), key);
            if (resource == null)
                return null;
            return string.Format(resource, args);
        }

        /// <summary>
        /// 获取当前键的本地化字符串实例。
        /// </summary>
        /// <param name="type">资源所在程序集的类型。</param>
        /// <param name="key">资源键。</param>
        /// <returns>返回当前本地化字符串。</returns>
        protected virtual string GetString(Type type, string key)
        {
            var resourceManager = _localizers.GetOrAdd(type, x =>
            {
                var assembly = x.GetTypeInfo().Assembly;
                var baseName = assembly.GetManifestResourceNames()
                    .SingleOrDefault();
                baseName = baseName.Substring(0, baseName.Length - 10);
                return new ResourceManager(baseName, assembly);
            });
            return resourceManager.GetString(key);
        }
    }
}