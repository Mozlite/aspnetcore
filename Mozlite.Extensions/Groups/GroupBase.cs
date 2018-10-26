using Mozlite.Extensions.Categories;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Mozlite.Extensions.Groups
{
    /// <summary>
    /// 分组基类。
    /// </summary>
    public abstract class GroupBase<TGroup> : CategoryBase, IParentable<TGroup>
        where TGroup : GroupBase<TGroup>
    {
        private readonly List<TGroup> _children = new List<TGroup>();

        /// <summary>
        /// 父级Id。
        /// </summary>
        public int ParentId { get; set; }

        object IParentable.Parent => Parent;

        List<object> IParentable.Children => Children.OfType<object>().ToList();

        /// <summary>
        /// 父级分组。
        /// </summary>
        [JsonIgnore]
        public TGroup Parent { get; private set; }

        /// <summary>
        /// 层次等级。
        /// </summary>
        public int Level
        {
            get
            {
                var level = -1;
                var current = this;
                while (current != null && current.Id > 0)
                {
                    level++;
                    current = current.Parent;
                }
                return level;
            }
        }

        /// <summary>
        /// 添加子集。
        /// </summary>
        /// <param name="group">分组实例。</param>
        public void Add(TGroup group)
        {
            group.ParentId = Id;
            group.Parent = (TGroup)this;
            _children.Add(group);
        }

        /// <summary>
        /// 索引获取当前模型实例对象。
        /// </summary>
        /// <param name="index">索引值。</param>
        /// <returns>返回当前模型实例。</returns>
        public TGroup this[int index] => _children[index];

        /// <summary>
        /// 包含分组集合。
        /// </summary>
        public List<TGroup> Children => _children;

        /// <summary>
        /// 子级数量。 
        /// </summary>
        public int Count => _children.Count;

        object IParentable.this[int index] => this[index];
    }
}