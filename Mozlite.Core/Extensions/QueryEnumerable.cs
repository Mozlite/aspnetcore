using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 分页列表基类。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    public abstract class QueryEnumerable<TModel> : IPageEnumerable<TModel>
    {
        private int _page;
        /// <summary>
        /// 页码。
        /// </summary>
        public int PI
        {
            get
            {
                if (_page < 1)
                    _page = 1;
                return _page;
            }
            set => _page = Math.Max(1, value);
        }

        /// <summary>
        /// 每页显示记录数。
        /// </summary>
        public int PS { get; set; } = 20;

        /// <summary>
        /// 总记录数。
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// 总页数。
        /// </summary>
        public int Pages { get; private set; }

        /// <summary>
        /// 页码。
        /// </summary>
        public int Page
        {
            get => PI;
            set => PI = value;
        }

        /// <summary>
        /// 每页显示记录数。
        /// </summary>
        public int PageSize
        {
            get => PS;
            set => PS = value;
        }

        private readonly List<TModel> _models = new List<TModel>();

        /// <summary>返回一个循环访问集合的枚举器。</summary>
        /// <returns>用于循环访问集合的枚举数。</returns>
        public virtual IEnumerator<TModel> GetEnumerator()
        {
            return _models.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 通过条件过滤数据。
        /// </summary>
        /// <param name="models">模型列表。</param>
        /// <returns>返回过滤后的模型列表。</returns>
        protected abstract IEnumerable<TModel> Init(IEnumerable<TModel> models);

        /// <summary>
        /// 获取当前页面的数据。
        /// </summary>
        /// <param name="models">总数据列表。</param>
        /// <returns>返回当前数据列表。</returns>
        public TQuery Load<TQuery>(IEnumerable<TModel> models)
            where TQuery : QueryEnumerable<TModel>, new()
        {
            if (models == null || !models.Any())
                return (TQuery)this;
            models = Init(models);
            Size = models.Count();
            Pages = (int)Math.Ceiling(Size * 1.0 / PageSize);
            _models.AddRange(models.Skip((Page - 1) * PageSize).Take(PageSize).ToList());
            return (TQuery)this;
        }
    }
}