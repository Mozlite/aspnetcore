using Mozlite.Data;
using Mozlite.Extensions.Properties;
using Mozlite.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mozlite.Extensions.Security.Events
{
    /// <summary>
    /// 对象变更对比实现类。
    /// </summary>
    public class ObjectDiffer : IObjectDiffer
    {
        private readonly ILocalizer _localizer;
        private bool _initialized;
        private bool _differed;
        private IEntityType _entityType;
        private readonly IDictionary<string, string> _stored = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 初始化类<see cref="ObjectDiffer"/>。
        /// </summary>
        /// <param name="localizer">本地化接口。</param>
        public ObjectDiffer(ILocalizer localizer)
        {
            _localizer = localizer;
        }

        /// <summary>
        /// 实例化一个对象，一般为原有对象实例。
        /// </summary>
        /// <param name="oldInstance">原有对象实例。</param>
        public virtual void Init(object oldInstance)
        {
            if (_initialized)
                throw new Exception(Resources.Differ_Duplicated_Initialized);
            _initialized = true;
            _entityType = oldInstance.GetType().GetEntityType();
            foreach (var property in _entityType.GetProperties())
            {
                _stored[property.Name] = GetValue(property, oldInstance);
            }
        }

        /// <summary>
        /// 实例化一个对象，一般为原有对象实例。
        /// </summary>
        /// <param name="oldInstance">原有对象实例。</param>
        /// <returns>返回当前实例。</returns>
        public virtual T GetAndInit<T>(T oldInstance)
        {
            Init(oldInstance);
            return oldInstance;
        }

        private string GetValue(IProperty property, object instance)
        {
            var value = property.Get(instance);
            if (value == null)
                return null;
            return value.GetType().IsEnum ? _localizer?.GetString((Enum)value) : value.ToString();
        }

        private IList<Differ> _entities;
        /// <summary>
        /// 对象新的对象，判断是否已经变更。
        /// </summary>
        /// <param name="newInstance">新对象实例。</param>
        /// <returns>返回对比结果。</returns>
        public virtual bool IsDifference(object newInstance)
        {
            if (!_initialized) throw new Exception();
            if (_differed)
                throw new Exception(Resources.Differ_Duplicated_Differed);
            _differed = true;
            _entities = new List<Differ>();
            foreach (var property in _entityType.GetProperties())
            {
                if (!property.IsUpdatable())
                    continue;
                var source = _stored[property.Name];
                var value = GetValue(property, newInstance);
                if (string.IsNullOrEmpty(source))
                {
                    if (string.IsNullOrEmpty(value))
                        continue;
                    //新增
                    _entities.Add(new Differ { Action = DifferAction.Add, Property = property, Value = value });
                    continue;
                }
                if (string.IsNullOrEmpty(value))
                {
                    //删除
                    _entities.Add(new Differ { Action = DifferAction.Remove, Property = property, Source = source });
                    continue;
                }
                if (source.Equals(value, StringComparison.OrdinalIgnoreCase))
                    continue;
                //修改
                _entities.Add(new Differ { Action = DifferAction.Modify, Property = property, Source = source, Value = value });
            }
            return _entities.Count > 0;
        }

        private string GetName(IProperty property)
        {
            if (property.DisplayName != null)
                return property.DisplayName;
            return _localizer == null ? property.Name : _localizer.GetString(property.DeclaringType.ClrType, property.Name);
        }

        /// <summary>
        /// 格式化出字符串。
        /// </summary>
        /// <returns>返回日志字符串。</returns>
        public override string ToString()
        {
            if (_entities == null || _entities.Count == 0)
                return string.Empty;
            var builder = new StringBuilder();
            foreach (var group in _entities.GroupBy(x => x.Action))
            {
                if (builder.Length > 0)
                    builder.Append("; ");
                var list = new List<string>();
                switch (group.Key)
                {
                    case DifferAction.Add:
                        builder.Append(Resources.DifferAction_Add);
                        foreach (var entity in group)
                        {
                            list.Add(string.Format(Resources.DifferAction_AddFormat, GetName(entity.Property), entity.Value));
                        }
                        break;
                    case DifferAction.Modify:
                        builder.Append(Resources.DifferAction_Modify);
                        foreach (var entity in group)
                        {
                            list.Add(string.Format(Resources.DifferAction_ModifyFormat, GetName(entity.Property), entity.Source,
                                entity.Value));
                        }
                        break;
                    case DifferAction.Remove:
                        builder.Append(Resources.DifferAction_Remove);
                        foreach (var entity in group)
                        {
                            list.Add(string.Format(Resources.DifferAction_RemoveFormat, GetName(entity.Property), entity.Source));
                        }
                        break;
                }
                builder.Append(string.Join(",", list));
            }
            return builder.ToString();
        }

        /// <summary>
        /// 变更实体。
        /// </summary>
        private class Differ
        {
            /// <summary>
            /// 日志操作实例。
            /// </summary>
            public DifferAction Action { get; set; }

            /// <summary>
            /// 属性名称。
            /// </summary>
            public IProperty Property { get; set; }

            /// <summary>
            /// 原始数据。
            /// </summary>
            public string Source { get; set; }

            /// <summary>
            /// 修改后得值。
            /// </summary>
            public string Value { get; set; }
        }
    }
}