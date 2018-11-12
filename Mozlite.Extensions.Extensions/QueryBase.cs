using Mozlite.Data;

namespace Mozlite.Extensions.Extensions
{
    /// <summary>
    /// 分页查询实例。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    public abstract class QueryBase<TModel> : Mozlite.Data.QueryBase<TModel>, ISitable
        where TModel : ISitable
    {
        /// <summary>
        /// 获取当前站Id。
        /// </summary>
        public int SiteId { get; set; }

        /// <summary>
        /// 初始化查询上下文。
        /// </summary>
        /// <param name="context">查询上下文。</param>
        protected override void Init(IQueryContext<TModel> context)
        {
            if (SiteId > 0)
                context.Where(x => x.SiteId == SiteId);
        }
    }
}