using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mozlite.Mvc;
using Mozlite.Properties;

namespace Mozlite.Extensions.Logging
{
    /// <summary>
    /// 日志暂存对象，性能不高，一般用于实体修改。
    /// </summary>
    public class LogStorage
    {
        /// <summary>
        /// 实体类型。
        /// </summary>
        public IEntityType EntityType { get; }

        /// <summary>
        /// 本地化接口。
        /// </summary>
        public ILocalizer Localizer { get; set; }

        private readonly IDictionary<string, string> _stored = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// 初始化类<see cref="LogStorage"/>。
        /// </summary>
        /// <param name="entity">当前实体对象。</param>
        public LogStorage(object entity)
        {
            EntityType = entity.GetType().GetEntityType();
            foreach (var property in EntityType.GetProperties())
            {
                _stored[property.Name] = property.Get(entity)?.ToString();
            }
        }

        private IList<LogEntity> _entities;
        /// <summary>
        /// 判断是否更改。
        /// </summary>
        /// <param name="modified">更改后得对象。</param>
        /// <returns>返回判断结果。</returns>
        public bool Diff(object modified)
        {
            _entities = new List<LogEntity>();
            foreach (var property in EntityType.GetProperties())
            {
                var source = _stored[property.Name];
                var value = property.Get(modified)?.ToString();
                if (string.IsNullOrEmpty(source))
                {
                    if (string.IsNullOrEmpty(value))
                        continue;
                    //新增
                    _entities.Add(new LogEntity { Action = LogAction.Add, Name = property.Name, Value = value });
                    continue;
                }
                if (string.IsNullOrEmpty(value))
                {
                    //删除
                    _entities.Add(new LogEntity { Action = LogAction.Remove, Name = property.Name, Source = source });
                    continue;
                }
                if (source.Equals(value, StringComparison.OrdinalIgnoreCase))
                    continue;
                //修改
                _entities.Add(new LogEntity { Action = LogAction.Modify, Name = property.Name, Source = source, Value = value });
            }
            return _entities.Count > 0;
        }

        private string Local(string key)
        {
            return Localizer == null ? key : Localizer.GetString(EntityType.ClrType, key);
        }

        /// <summary>
        /// 格式化出字符串。
        /// </summary>
        /// <returns>返回日志字符串。</returns>
        public override string ToString()
        {
            if (_entities == null || _entities.Count == 0)
                return null;
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
                            list.Add(string.Format(Resources.LogAction_AddFormat, Local(entity.Name), entity.Value));
                        }
                        break;
                    case LogAction.Modify:
                        builder.Append(Resources.LogAction_Modify);
                        foreach (var entity in group)
                        {
                            list.Add(string.Format(Resources.LogAction_ModifyFormat, Local(entity.Name), entity.Source,
                                entity.Value));
                        }
                        break;
                    case LogAction.Remove:
                        builder.Append(Resources.LogAction_Remove);
                        foreach (var entity in group)
                        {
                            list.Add(string.Format(Resources.LogAction_RemoveFormat, Local(entity.Name), entity.Source));
                        }
                        break;
                }
                builder.Append(string.Join(",", list));
            }
            return builder.ToString();
        }

        /// <summary>
        /// 隐式转换为字符串。
        /// </summary>
        /// <param name="storage">日志存储实例。</param>
        public static implicit operator string(LogStorage storage) => storage.ToString();

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
            public string Name { get; set; }

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