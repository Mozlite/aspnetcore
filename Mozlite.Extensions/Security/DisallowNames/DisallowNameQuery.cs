using Mozlite.Data;

namespace Mozlite.Extensions.Security.DisallowNames
{
    /// <summary>
    /// 非法名称分页查询实例。
    /// </summary>
    public class DisallowNameQuery : QueryBase<DisallowName>
    {
        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 初始化查询上下文。
        /// </summary>
        /// <param name="context">查询上下文。</param>
        protected override void Init(IQueryContext<DisallowName> context)
        {
            if (!string.IsNullOrWhiteSpace(Name))
                context.Where(x => x.Name.Contains(Name));
        }
    }
}