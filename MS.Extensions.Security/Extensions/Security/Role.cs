using System.Threading;
using System.Threading.Tasks;
using Mozlite.Data.Internal;
using Mozlite.Extensions.Security;
using Mozlite.Extensions.Security.Stores;

namespace MS.Extensions.Security
{
    /// <summary>
    /// 角色。
    /// </summary>
    public class Role : RoleBase, IRoleEventHandler<Role>
    {
        /// <summary>
        /// 当角色添加后触发得方法。
        /// </summary>
        /// <param name="context">数据库事务操作实例。</param>
        /// <returns>返回操作结果，返回<c>true</c>表示操作成功，将自动提交事务，如果<c>false</c>或发生错误，则回滚事务。</returns>
        public bool OnCreated(IDbTransactionContext<Role> context)
        {
            return true;
        }

        /// <summary>
        /// 当角色更新后触发得方法。
        /// </summary>
        /// <param name="context">数据库事务操作实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回操作结果，返回<c>true</c>表示操作成功，将自动提交事务，如果<c>false</c>或发生错误，则回滚事务。</returns>
        public Task<bool> OnCreatedAsync(IDbTransactionContext<Role> context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// 当角色更新前触发得方法。
        /// </summary>
        /// <param name="context">数据库事务操作实例。</param>
        /// <returns>返回操作结果，返回<c>true</c>表示操作成功，将自动提交事务，如果<c>false</c>或发生错误，则回滚事务。</returns>
        public bool OnUpdate(IDbTransactionContext<Role> context)
        {
            //更改用户显示的角色名称
            var dbRole = context.Find(RoleId);
            if (dbRole.Name != Name)
                return context.As<User>().Update(x => x.RoleId == RoleId,
                     new { RoleName = Name });
            return true;
        }

        /// <summary>
        /// 当角色更新前触发得方法。
        /// </summary>
        /// <param name="context">数据库事务操作实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回操作结果，返回<c>true</c>表示操作成功，将自动提交事务，如果<c>false</c>或发生错误，则回滚事务。</returns>
        public async Task<bool> OnUpdateAsync(IDbTransactionContext<Role> context, CancellationToken cancellationToken = default(CancellationToken))
        {
            //更改用户显示的角色名称
            var dbRole = await context.FindAsync(RoleId, cancellationToken);
            if (dbRole.Name != Name)
                return await context.As<User>().UpdateAsync(x => x.RoleId == RoleId,
                    new { RoleName = Name }, cancellationToken);
            return true;
        }

        /// <summary>
        /// 当角色删除前触发得方法。
        /// </summary>
        /// <param name="context">数据库事务操作实例。</param>
        /// <returns>返回操作结果，返回<c>true</c>表示操作成功，将自动提交事务，如果<c>false</c>或发生错误，则回滚事务。</returns>
        public bool OnDelete(IDbTransactionContext<Role> context)
        {
            //更改用户显示的角色名称
            var basic = context.Find(x => x.NormalizedName == DefaultRole.Member.NormalizedName);
            var dbRole = context.Find(RoleId);
            if (dbRole.Name != Name)
                return context.As<User>().Update(x => x.RoleId == basic.RoleId,
                    new { basic.RoleId, RoleName = basic.Name });
            return true;
        }

        /// <summary>
        /// 当角色删除前触发得方法。
        /// </summary>
        /// <param name="context">数据库事务操作实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回操作结果，返回<c>true</c>表示操作成功，将自动提交事务，如果<c>false</c>或发生错误，则回滚事务。</returns>
        public async Task<bool> OnDeleteAsync(IDbTransactionContext<Role> context, CancellationToken cancellationToken = default(CancellationToken))
        {
            //更改用户显示的角色名称
            var basic = await context.FindAsync(x => x.NormalizedName == DefaultRole.Member.NormalizedName, cancellationToken);
            var dbRole = await context.FindAsync(RoleId, cancellationToken);
            if (dbRole.Name != Name)
                return await context.As<User>().UpdateAsync(x => x.RoleId == basic.RoleId,
                    new { basic.RoleId, RoleName = basic.Name }, cancellationToken);
            return true;
        }
    }
}