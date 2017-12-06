using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Extensions.Security.Models
{
    /// <summary>
    /// 用户档案。
    /// </summary>
    [Table("core_Users_Profiles")]
    public class UserProfile
    {
        /// <summary>
        /// 初始化类<see cref="UserProfile"/>。
        /// </summary>
        public UserProfile() { }

        /// <summary>
        /// 初始化类<see cref="UserProfile"/>。
        /// </summary>
        /// <param name="user">用户实例。</param>
        public void SetUser(User user)
        {
            Id = user.UserId;
            User = user;
        }

        /// <summary>
        /// 用户实例。
        /// </summary>
        [NotMapped]
        public User User { get; private set; }

        /// <summary>
        /// 用户Id。
        /// </summary>
        [Key]
        public int Id { get; set; }
        
        /// <summary>
        /// 个人简介。
        /// </summary>
        [Size(256)]
        public string Intro { get; set; } = "这个家伙很懒，还没有留下任何足迹...";

        /// <summary>
        /// 评论Id。
        /// </summary>
        public int CommentId { get; set; }

        /// <summary>
        /// 评论数量。
        /// </summary>
        public int Comments { get; set; }

        /// <summary>
        /// 是否允许评论。
        /// </summary>
        public bool EnabledComment { get; set; }

        /// <summary>
        /// 粉丝数量。
        /// </summary>
        public int Follows { get; set; }

        /// <summary>
        /// 关注数量。
        /// </summary>
        public int Followeds { get; set; }

        /// <summary>
        /// 微信名称。
        /// </summary>
        [Size(64)]
        public string Weixin { get; set; }

        /// <summary>
        /// QQ号码。
        /// </summary>
        [Size(12)]
        public string QQ { get; set; }

        /// <summary>
        /// 微博地址。
        /// </summary>
        [Size(64)]
        public string Weibo { get; set; }
    }
}