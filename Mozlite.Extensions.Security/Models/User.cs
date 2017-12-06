namespace Mozlite.Extensions.Security.Models
{
    /// <summary>
    /// 用户实例。
    /// </summary>
    public class User : IdentityUser
    {
        /// <summary>
        /// 积分。
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// 多线程更新列。
        /// </summary>
        [RowVersion]
        public byte[] ConcurrencyStamp { get; set; }
    }
}