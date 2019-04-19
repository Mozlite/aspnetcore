using Mozlite.Extensions;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MozliteDemo.Extensions.ProjectManager.Projects
{
    /// <summary>
    /// 项目。
    /// </summary>
    [Table("pm_Projects")]
    public class Project : IIdObject
    {
        /// <summary>
        /// 获取或设置唯一Id。
        /// </summary>
        [Identity]
        public int Id { get; set; }

        /// <summary>
        /// 项目名称。
        /// </summary>
        [Size(64)]
        public string Name { get; set; }

        /// <summary>
        /// 备注。
        /// </summary>
        [Size(256)]
        public string Summary { get; set; }

        /// <summary>
        /// 项目负责人。
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 激活。
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 添加时间。
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;
    }
}