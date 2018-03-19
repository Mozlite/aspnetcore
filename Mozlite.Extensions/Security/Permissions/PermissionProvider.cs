using System.Collections.Generic;

namespace Mozlite.Extensions.Security.Permissions
{
    /// <summary>
    /// 权限提供者基类。
    /// </summary>
    public abstract class PermissionProvider : IPermissionProvider
    {
        internal const string Core = "core";

        /// <summary>
        /// 分类。
        /// </summary>
        public virtual string Category => Core;

        /// <summary>
        /// 排序。
        /// </summary>
        public virtual int Order => 0;

        private readonly List<Permission> _permissions = new List<Permission>();

        /// <summary>
        /// 权限列表。
        /// </summary>
        /// <returns>返回权限列表。</returns>
        public IEnumerable<Permission> LoadPermissions()
        {
            Init();
            return _permissions;
        }

        /// <summary>
        /// 初始化权限实例。
        /// </summary>
        protected abstract void Init();

        /// <summary>
        /// 实例化一个权限。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="text">显示字符串。</param>
        /// <param name="description">描述。</param>
        /// <returns>返回权限实例。</returns>
        protected void Add(string name, string text, string description)
        {
            _permissions.Add(new Permission
            {
                Name = name,
                Text = text,
                Description = description
            });
        }
    }
}