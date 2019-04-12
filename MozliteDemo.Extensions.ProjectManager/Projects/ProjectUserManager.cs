using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Mozlite;
using Mozlite.Data;
using Mozlite.Extensions;
using Mozlite.Extensions.Security;
using MozliteDemo.Extensions.Security;

namespace MozliteDemo.Extensions.ProjectManager.Projects
{
    /// <summary>
    /// 项目管理接口。
    /// </summary>
    public class ProjectUserManager : CachableObjectManager<ProjectUser>, IProjectUserManager
    {
        public ProjectUserManager(IDbContext<ProjectUser> context, IMemoryCache cache)
            : base(context, cache)
        {
        }

        /// <summary>
        /// 保存用户.
        /// </summary>
        /// <param name="ids">用户Id列表.</param>
        /// <returns>返回保存结果.</returns>
        public bool SaveUsers(int[] ids)
        {
            var result = true;
            if (ids == null || ids.Length == 0)
                result = Context.Delete();
            else
                result = Context.BeginTransaction(db =>
             {
                 db.Delete(x => x.Id.Included(ids));
                 foreach (var id in ids)
                 {
                     if (db.Any(id)) continue;
                     db.Create(new ProjectUser { Id = id });
                 }

                 return true;
             });
            if (result) Refresh();
            return result;
        }

        /// <summary>
        /// 根据条件获取列表。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回模型实例列表。</returns>
        public override IEnumerable<ProjectUser> Fetch(Expression<Predicate<ProjectUser>> expression = null)
        {
            var models = Cache.GetOrCreate(CacheKey, ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                return Context.AsQueryable()
                    .WithNolock()
                    .Select()
                    .JoinSelect<ProjectUser, User>((p, u) => p.Id == u.UserId)
                    .AsEnumerable();
            });
            return models.Filter(expression);
        }

        /// <summary>
        /// 根据条件获取列表。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回模型实例列表。</returns>
        public override async Task<IEnumerable<ProjectUser>> FetchAsync(Expression<Predicate<ProjectUser>> expression = null, CancellationToken cancellationToken = default)
        {
            var models = await Cache.GetOrCreateAsync(CacheKey, ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                return Context.AsQueryable()
                    .WithNolock()
                    .Select()
                    .JoinSelect<ProjectUser, User>((p, u) => p.Id == u.UserId)
                    .AsEnumerableAsync(cancellationToken);
            });
            return models.Filter(expression);
        }
    }
}