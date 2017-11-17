using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mozlite.Extensions.Categories
{
    /// <summary>
    /// 分组基类。
    /// </summary>
    public abstract class GroupBase<TGroup> : CategoryBase
        where TGroup : GroupBase<TGroup>
    {
        private readonly List<TGroup> _groups = new List<TGroup>();

        /// <summary>
        /// 父级Id。
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 父级分组。
        /// </summary>
        [JsonIgnore]
        public TGroup Parent { get; private set; }

        private int _level = -1;
        /// <summary>
        /// 层次等级。
        /// </summary>
        [NotUpdated]
        public int Level
        {
            get
            {
                if (_level == -1)
                {
                    var current = this;
                    while (current != null && current.Id > 0)
                    {
                        _level++;
                        current = current.Parent;
                    }
                }
                return _level;
            }
            set => _level = value;
        }

        /// <summary>
        /// 添加子集。
        /// </summary>
        /// <param name="group">分组实例。</param>
        public void Add(TGroup group)
        {
            group.ParentId = Id;
            group.Parent = (TGroup)this;
            _groups.Add(group);
        }

        /// <summary>
        /// 包含分组集合。
        /// </summary>
        public List<TGroup> Items => _groups;

        /// <summary>
        /// 子级数量。
        /// </summary>
        public int Count => _groups.Count;
    }
}