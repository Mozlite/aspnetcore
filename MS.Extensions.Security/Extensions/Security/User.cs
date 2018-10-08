using System.Threading;
using System.Threading.Tasks;
using Mozlite.Data.Internal;
using Mozlite.Extensions.Security;
using Mozlite.Extensions.Security.Stores;

namespace MS.Extensions.Security
{
    /// <summary>
    /// 用户。
    /// </summary>
    public class User : UserBase, IUserEventHandler<User>
    { /// <summary>
      /// 当用户添加后触发得方法。
      /// </summary>
      /// <param name="context">数据库事务操作实例。</param>
      /// <returns>返回操作结果，返回<c>true</c>表示操作成功，将自动提交事务，如果<c>false</c>或发生错误，则回滚事务。</returns>
        public bool OnCreated(IDbTransactionContext<User> context)
        {
            //添加用户角色
            var role = context.As<Role>()
                .Find(x => x.NormalizedName == DefaultRole.Member.NormalizedName);
            var userRole = new UserRole { RoleId = role.RoleId, UserId = UserId };
            return context.As<UserRole>().Create(userRole);
        }

        /// <summary>
        /// 当用户添加后触发得方法。
        /// </summary>
        /// <param name="context">数据库事务操作实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回操作结果，返回<c>true</c>表示操作成功，将自动提交事务，如果<c>false</c>或发生错误，则回滚事务。</returns>
        public async Task<bool> OnCreatedAsync(IDbTransactionContext<User> context, CancellationToken cancellationToken = default(CancellationToken))
        {
            //添加用户角色
            var role = await context.As<Role>()
                .FindAsync(x => x.NormalizedName == DefaultRole.Member.NormalizedName, cancellationToken);
            var userRole = new UserRole { RoleId = role.RoleId, UserId = UserId };
            return await context.As<UserRole>().CreateAsync(userRole, cancellationToken);
        }

        /// <summary>
        /// 当用户更新前触发得方法。
        /// </summary>
        /// <param name="context">数据库事务操作实例。</param>
        /// <returns>返回操作结果，返回<c>true</c>表示操作成功，将自动提交事务，如果<c>false</c>或发生错误，则回滚事务。</returns>
        public bool OnUpdate(IDbTransactionContext<User> context)
        {
            return true;
        }

        /// <summary>
        /// 当用户更新前触发得方法。
        /// </summary>
        /// <param name="context">数据库事务操作实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回操作结果，返回<c>true</c>表示操作成功，将自动提交事务，如果<c>false</c>或发生错误，则回滚事务。</returns>
        public Task<bool> OnUpdateAsync(IDbTransactionContext<User> context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// 当用户删除前触发得方法。
        /// </summary>
        /// <param name="context">数据库事务操作实例。</param>
        /// <returns>返回操作结果，返回<c>true</c>表示操作成功，将自动提交事务，如果<c>false</c>或发生错误，则回滚事务。</returns>
        public bool OnDelete(IDbTransactionContext<User> context)
        {
            return true;
        }

        /// <summary>
        /// 当用户删除前触发得方法。
        /// </summary>
        /// <param name="context">数据库事务操作实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回操作结果，返回<c>true</c>表示操作成功，将自动提交事务，如果<c>false</c>或发生错误，则回滚事务。</returns>
        public Task<bool> OnDeleteAsync(IDbTransactionContext<User> context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(true);
        }
    }
}