using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 扩展基类。
    /// </summary>
    public abstract class ExtendBase
    {
        private IDictionary<string, string> _extendProperties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 扩展方法。
        /// </summary>
        [JsonIgnore]
        public string ExtendProperties
        {
            get => _extendProperties.ToJsonString();
            set => _extendProperties = Cores.FromJsonString<Dictionary<string, string>>(value);
        }

        /// <summary>
        /// 索引访问和设置扩展属性。
        /// </summary>
        /// <param name="name">索引值。</param>
        /// <returns>返回当前扩展属性值。</returns>
        [NotMapped]
        [JsonIgnore]
        public string this[string name]
        {
            get
            {
                if (!name.StartsWith("ex:"))
                    name = "ex:" + name;
                _extendProperties.TryGetValue(name, out var value);
                return value;
            }
            set
            {
                if (!name.StartsWith("ex:"))
                    name = "ex:" + name;
                _extendProperties[name] = value;
            }
        }

        /// <summary>
        /// 扩展属性列表。
        /// </summary>
        [JsonIgnore]
        public IEnumerable<string> ExtendKeys => _extendProperties.Keys;

        /// <summary>
        /// 从表单中读取扩展属性。
        /// </summary>
        /// <param name="form">表单集合。</param>
        public void Merge(IFormCollection form)
        {
            foreach (var key in form.Keys)
            {
                if (key.StartsWith("ex:"))
                    _extendProperties[key] = form[key];
            }
        }
    }
}