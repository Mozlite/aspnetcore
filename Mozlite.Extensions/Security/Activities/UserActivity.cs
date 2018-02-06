using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 用户活动状态。
    /// </summary>
    [Table("core_Users_Activities")]
    public class UserActivity
    {
        /// <summary>
        /// ID。
        /// </summary>
        [Identity]
        public int Id { get; set; }

        /// <summary>
        /// 当前用户Id。
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 分类Id。
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 活动时间。
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// IP地址。
        /// </summary>
        [Size(32)]
        // ReSharper disable once InconsistentNaming
        public string IPAdress { get; set; }

        /// <summary>
        /// 操作日志。
        /// </summary>
        [Size(512)]
        public string Activity { get; set; }
        
        /// <summary>
        /// 用户名。
        /// </summary>
        [NotMapped]
        public string UserName { get; set; }

        /// <summary>
        /// 昵称。
        /// </summary>
        [NotMapped]
        public string NickName { get; set; }
    }
}