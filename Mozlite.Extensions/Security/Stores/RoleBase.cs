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
        /// 角色等级。
        /// </summary>
        public int RoleLevel { get; set; }

        /// <summary>
        /// 返回角色名称。
        /// </summary>
        public override string ToString()
        {
            return Name;
        }
    }
}