using Mozlite.Data;
using Mozlite.Extensions;
using Mozlite.Mvc.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mozlite.Mvc.Logging
{
    /// <summary>
    /// 日志暂存对象，性能不高，一般用于实体修改。
    /// </summary>
    public class LogContext
    {
        private readonly IEntityType _entityType;
        private readonly ILocalizer _localizer;

        private readonly IDictionary<string, string> _stored = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private LogContext(object entity, ILocalizer localizer)
        {
            _entityType = entity.GetType().GetEntityType();
            foreach (var property in _entityType.GetProperties())
            {
                _stored[property.Name] = property.Get(entity)?.ToString();
            }
            _localizer = localizer;
        }

        /// <summary>
        /// 实例化一个日志上下文。
        /// </summary>
        /// <param name="entity">当前实体对象实例。</param>
        /// <param name="localizer">本地化接口。</param>
        /// <returns>返回日志上下文。</returns>
        public static LogContext Create(object entity, ILocalizer localizer = null) => new LogContext(entity, localizer);

        private IList<LogEntity> _entities;
        /// <summary>
        /// 判断是否更改。
        /// </summary>
        /// <param name="modified">更改后得对象。</param>
        /// <returns>返回判断结果。</returns>
        public bool Diff(object modified)
        {
            _entities = new List<LogEntity>();
            foreach (var property in _entityType.GetProperties())
            {
                if (!property.IsUpdatable())
                    continue;
                var source = _stored[property.Name];
                var value = property.Get(modified)?.ToString();
                if (string.IsNullOrEmpty(source))
                {
                    if (string.IsNullOrEmpty(value))
                        continue;
                    //新增
                    _entities.Add(new LogEntity { Action = LogAction.Add, Property = property, Value = value });
                    continue;
                }
                if (string.IsNullOrEmpty(value))
                {
                    //删除
                    _entities.Add(new LogEntity { Action = LogAction.Remove, Property = property, Source = source });
                    continue;
                }
                if (source.Equals(value, StringComparison.OrdinalIgnoreCase))
                    continue;
                //修改
                _entities.Add(new LogEntity { Action = LogAction.Modify, Property = property, Source = source, Value = value });
            }
            return _entities.Count > 0;
        }

        private string Local(IProperty property)
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
                    case LogAction.Add:
                        builder.Append(Resources.LogAction_Add);
                        foreach (var entity in group)
                        {
                            list.Add(string.Format(Resources.LogAction_AddFormat, Local(entity.Property), entity.Value));
                        }
                        break;
                    case LogAction.Modify:
                        builder.Append(Resources.LogAction_Modify);
                        foreach (var entity in group)
                        {
                            list.Add(string.Format(Resources.LogAction_ModifyFormat, Local(entity.Property), entity.Source,
                                entity.Value));
                        }
                        break;
                    case LogAction.Remove:
                        builder.Append(Resources.LogAction_Remove);
                        foreach (var entity in group)
                        {
                            list.Add(string.Format(Resources.LogAction_RemoveFormat, Local(entity.Property), entity.Source));
                        }
                        break;
                }
                builder.Append(string.Join(",", list));
            }
            return builder.ToString();
        }

        /// <summary>
        /// 日志实体。
        /// </summary>
        private class LogEntity
        {
            /// <summary>
            /// 日志操作实例。
            /// </summary>
            public LogAction Action { get; set; }

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