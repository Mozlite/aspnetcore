using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 角色声明类。
    /// </summary>
    [Table("core_Roles_Claims")]
    public abstract class RoleClaimBase
    {
        /// <summary>
        /// 标识ID。
        /// </summary>
        [Identity]
        public int Id { get; set; }

        /// <summary>
        /// 角色ID。
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 声明类型。
        /// </summary>
        [Size(256)]
        public string ClaimType { get; set; }

        /// <summary>
        /// 声明值。
        /// </summary>
        public string ClaimValue { get; set; }

        /// <summary>
        /// 将角色声明转换为<see cref="Claim"/>对象。
        /// </summary>
        /// <returns>返回<see cref="Claim"/>实例对象。</returns>
        public virtual Claim ToClaim()
        {
            return new Claim(ClaimType, ClaimValue);
        }
        
        /// <summary>
        /// 实例化属性。
        /// </summary>
        /// <param name="other">当前<see cref="Claim"/>对象实例。</param>
        public virtual void InitializeFromClaim(Claim other)
        {
            ClaimType = other?.Type;
            ClaimValue = other?.Value;
        }
    }
}