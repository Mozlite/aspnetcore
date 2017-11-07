using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security.Models
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
        /// 活动时间。
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 扩展名称。
        /// </summary>
        [Size(64)]
        public string ExtensionName { get; set; }

        /// <summary>
        /// 操作日志。
        /// </summary>
        [Size(512)]
        public string Activity { get; set; }
    }
}