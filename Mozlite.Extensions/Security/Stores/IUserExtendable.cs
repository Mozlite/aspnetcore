using System.Threading.Tasks;
using Mozlite.Data.Internal;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 用户扩展接口，主要是在添加用户时候进行得添加操作。
    /// </summary>
    public interface IUserExtendable<TUser>
        where TUser : UserBase
    {
        /// <summary>
        /// 当用户添加后触发得方法。
        /// </summary>
        /// <param name="context">数据库事务操作实例。</param>
        /// <returns>返回操作结果，返回<c>true</c>表示操作成功，将自动提交事务，如果<c>false</c>或发生错误，则回滚事务。</returns>
        Task<bool> OnCreateAsync(IDbTransactionContext<TUser> context);
    }
}