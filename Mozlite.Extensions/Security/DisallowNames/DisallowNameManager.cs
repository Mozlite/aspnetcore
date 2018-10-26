using System;
using System.Linq;
using System.Threading.Tasks;
using Mozlite.Data;

namespace Mozlite.Extensions.Security.DisallowNames
{
    /// <summary>
    /// 非法名称管理实现类。
    /// </summary>
    public class DisallowNameManager : IDisallowNameManager
    {
        private readonly IDbContext<DisallowName> _context;
        /// <summary>
        /// 初始化类<see cref="DisallowNameManager"/>。
        /// </summary>
        /// <param name="context">数据库上下文接口。</param>
        public DisallowNameManager(IDbContext<DisallowName> context)
        {
            _context = context;
        }

        /// <summary>
        /// 保存非法名称。
        /// </summary>
        /// <param name="names">名称集合，多个非法名称以“,”或“\r\n”分割。</param>
        /// <returns>返回保存结果。</returns>
        public DataResult Save(string names)
        {
            var words = names.Split(new[] { ",", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim());
            foreach (var word in words)
            {
                if (IsDisallowed(word))
                    continue;
                _context.Create(new DisallowName { Name = word });
            }
            return DataAction.Created;
        }

        /// <summary>
        /// 删除非法名称。
        /// </summary>
        /// <param name="id">非法名称Id。</param>
        /// <returns>返回删除结果。</returns>
        public DataResult Delete(int id)
        {
            return DataResult.FromResult(_context.Delete(x=>x.Id == id), DataAction.Deleted);
        }

        /// <summary>
        /// 删除非法名称。
        /// </summary>
        /// <param name="ids">非法名称Id。</param>
        /// <returns>返回删除结果。</returns>
        public DataResult Delete(int[] ids)
        {
            return DataResult.FromResult(_context.Delete(x => x.Id.Included(ids)), DataAction.Deleted);
        }

        /// <summary>
        /// 判断当前名称是否为非法名称。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>返回判断结果。</returns>
        public bool IsDisallowed(string name)
        {
            return _context.Any(x => x.Name == name);
        }

        /// <summary>
        /// 判断当前名称是否为非法名称。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>返回判断结果。</returns>
        public async Task<bool> IsDisallowedAsync(string name)
        {
            return await _context.AnyAsync(x => x.Name == name);
        }

        /// <summary>
        /// 分页获取非法名称。
        /// </summary>
        /// <param name="query">非法名称查询实例。</param>
        /// <returns>返回非法名称列表。</returns>
        public DisallowNameQuery Load(DisallowNameQuery query)
        {
            return _context.Load(query);
        }
    }
}