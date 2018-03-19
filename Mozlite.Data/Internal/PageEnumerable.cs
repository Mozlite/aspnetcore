using System;
using System.Collections;
using System.Collections.Generic;
using Mozlite.Extensions;

namespace Mozlite.Data.Internal
{
    /// <summary>
    /// 分页迭代实现类。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    public class PageEnumerable<TModel> : IPageEnumerable<TModel>
    {
        private readonly List<TModel> _models = new List<TModel>();

        /// <summary>返回一个循环访问集合的枚举器。</summary>
        /// <returns>用于循环访问集合的枚举数。</returns>
        public IEnumerator<TModel> GetEnumerator()
        {
            return _models.GetEnumerator();
        }

        /// <summary>返回循环访问集合的枚举数。</summary>
        /// <returns>可用于循环访问集合的 <see cref="T:System.Collections.IEnumerator" /> 对象。</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 页码。
        /// </summary>
        public int PI { get; set; }

        /// <summary>
        /// 每页显示记录数。
        /// </summary>
        public int PS { get; set; }

        /// <summary>
        /// 总记录数。
        /// </summary>
        public int Size { get; set; }

        private int _page = -1;
        /// <summary>
        /// 总页数。
        /// </summary>
        public int Pages
        {
            get
            {
                if (_page == -1)
                {
                    _page = (int)Math.Ceiling(Size * 1.0 / PS);
                }
                return _page;
            }
        }

        /// <summary>
        /// 添加模型。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        public void Add(TModel model) => _models.Add(model);
    }
}