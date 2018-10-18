using System.Threading;
using System.Threading.Tasks;
using Mozlite.Data.Internal;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 角色事件处理器接口。
    /// </summary>
    public interface IRoleEventHandler<TRole>
        where TRole : RoleBase
    {
        /// <summary>
        /// 当角色添加后触发得方法。
        /// </summary>
        /// <param name="context">数据库事务操作实例。</param>
        /// <returns>返回操作结果，返回<c>true</c>表示操作成功，将自动提交事务，如果<c>false</c>或发生错误，则回滚事务。</returns>
        bool OnCreated(IDbTransactionContext<TRole> context);

        /// <summary>
        /// 当角色添加后触发得方法。
        /// </summary>
        /// <param name="context">数据库事务操作实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回操作结果，返回<c>true</c>表示操作成功，将自动提交事务，如果<c>false</c>或发生错误，则回滚事务。</returns>
        Task<bool> OnCreatedAsync(IDbTransactionContext<TRole> context, CancellationToken cancellationToken = default);

        /// <summary>
        /// 当角色更新前触发得方法。
        /// </summary>
        /// <param name="context">数据库事务操作实例。</param>
        /// <returns>返回操作结果，返回<c>true</c>表示操作成功，将自动提交事务，如果<c>false</c>或发生错误，则回滚事务。</returns>
        bool OnUpdate(IDbTransactionContext<TRole> context);

        /// <summary>
        /// 当角色更新前触发得方法。
        /// </summary>
        /// <param name="context">数据库事务操作实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回操作结果，返回<c>true</c>表示操作成功，将自动提交事务，如果<c>false</c>或发生错误，则回滚事务。</returns>
        Task<bool> OnUpdateAsync(IDbTransactionContext<TRole> context, CancellationToken cancellationToken = default);

        /// <summary>
        /// 当角色删除前触发得方法。
        /// </summary>
        /// <param name="context">数据库事务操作实例。</param>
        /// <returns>返回操作结果，返回<c>true</c>表示操作成功，将自动提交事务，如果<c>false</c>或发生错误，则回滚事务。</returns>
        bool OnDelete(IDbTransactionContext<TRole> context);

        /// <summary>
        /// 当角色删除前触发得方法。
        /// </summary>
        /// <param name="context">数据库事务操作实例。</param>
        /// <param name="cancellationToken">取消标志。</param>
        /// <returns>返回操作结果，返回<c>true</c>表示操作成功，将自动提交事务，如果<c>false</c>或发生错误，则回滚事务。</returns>
        Task<bool> OnDeleteAsync(IDbTransactionContext<TRole> context, CancellationToken cancellationToken = default);
    }
}