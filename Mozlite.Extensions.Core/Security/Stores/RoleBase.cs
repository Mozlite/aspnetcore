using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 角色基类。
    /// </summary>
    [Table("core_Roles")]
    public abstract class RoleBase
    {
        /// <summary>
        /// 初始化类<see cref="RoleBase"/>。
        /// </summary>
        protected RoleBase() { }

        /// <summary>
        /// 初始化类<see cref="RoleBase"/>。
        /// </summary>
        /// <param name="roleName">角色名称。</param>
        protected RoleBase(string roleName)
        {
            Name = roleName;
        }

        /// <summary>
        /// 角色Id。
        /// </summary>
        [Identity]
        public int RoleId { get; set; }

        /// <summary>
        /// 角色名称。
        /// </summary>
        [Size(64)]
        public string Name { get; set; }

        /// <summary>
        /// 用于比对的角色名称。
        /// </summary>
        [Size(64)]
        public string NormalizedName { get; set; }

        /// <summary>
        /// 用于多线程更新附加随机条件。
        /// </summary>
        [Size(36)]
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 返回角色名称。
        /// </summary>
        public override string ToString()
        {
            return Name;
        }
    }
}