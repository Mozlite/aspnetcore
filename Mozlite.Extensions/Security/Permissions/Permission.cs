using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security.Permissions
{
    /// <summary>
    /// 权限实体。
    /// </summary>
    [Table("core_Permissions")]
    public class Permission
    {
        /// <summary>
        /// 初始化类<see cref="Permission"/>。
        /// </summary>
        public Permission() { }

        /// <summary>
        /// 初始化类<see cref="Permission"/>。
        /// </summary>
        /// <param name="permissionName">权限名称。</param>
        public Permission(string permissionName)
        {
            var parts = permissionName.Split('.');
            if (parts.Length == 2)
            {
                Category = parts[0];
                Name = parts[1];
            }
            else
            {
                Category = PermissionProvider.Core;
                Name = permissionName;
            }
        }

        /// <summary>
        /// 唯一Id。
        /// </summary>
        [Identity]
        public int Id { get; set; }

        /// <summary>
        /// 分类。
        /// </summary>
        [Size(64)]
        public string Category { get; set; }

        /// <summary>
        /// 名称。
        /// </summary>
        [Size(64)]
        public string Name { get; set; }

        /// <summary>
        /// 显示字符串。
        /// </summary>
        [Size(64)]
        public string Text { get; set; }

        /// <summary>
        /// 排序。
        /// </summary>
        [NotUpdated]
        public int Order { get; set; }

        /// <summary>
        /// 描述。
        /// </summary>
        [Size(256)]
        public string Description { get; set; }

        /// <summary>
        /// 唯一键。
        /// </summary>
        public string Key => $"{Category}.{Name}".ToLower();
    }
}