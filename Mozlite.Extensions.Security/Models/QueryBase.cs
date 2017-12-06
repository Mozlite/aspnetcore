using System;
using Mozlite.Data;

namespace Mozlite.Extensions.Security.Models
{
    /// <summary>
    /// 分页查询基类。
    /// </summary>
    public abstract class QueryBase : QueryBase<User>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 注册开始时间。
        /// </summary>
        public DateTime? RegisterStart { get; set; }

        /// <summary>
        /// 注册结束时间。
        /// </summary>
        public DateTime? RegisterEnd { get; set; }

        /// <summary>
        /// 初始化查询上下文。
        /// </summary>
        /// <param name="context">查询上下文。</param>
        protected override void Init(IQueryContext<User> context)
        {
            if (!string.IsNullOrWhiteSpace(Name))
                context.Where(x => x.NormalizedUserName.Contains(Name) || x.NickName.Contains(Name));
            if (RegisterStart != null)
                context.Where(x => x.CreatedDate >= RegisterStart);
            if (RegisterEnd != null)
                context.Where(x => x.CreatedDate <= RegisterEnd);
        }
    }
}