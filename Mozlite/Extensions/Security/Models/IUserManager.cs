using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Mozlite.Extensions.Storages;

namespace Mozlite.Extensions.Security.Models
{
    /// <summary>
    /// 用户管理接口。
    /// </summary>
    public interface IUserManager : IIdentityUserManager<User>, IScopedService
    {
        /// <summary>
        /// 上传头像。
        /// </summary>
        /// <param name="file">上传的文件实例对象。</param>
        /// <param name="id">用户Id。</param>
        /// <returns>返回上传结果。</returns>
        Task<MediaResult> UploadAvatarAsync(IFormFile file, int id);

        /// <summary>
        /// 判断用户名是否已经存在。
        /// </summary>
        /// <param name="userName">用户名称。</param>
        /// <returns>返回判断结果。</returns>
        Task<bool> CheckUserNameAsync(string userName);

        /// <summary>
        /// 获取用户档案，缓存上下文。
        /// </summary>
        /// <param name="userId">用户实例。</param>
        /// <returns>返回用户档案实例。</returns>
        UserProfile GetProfile(int userId);

        /// <summary>
        /// 获取当前用户档案。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <returns>返回用户档案实例。</returns>
        UserProfile GetProfile(User user);

        /// <summary>
        /// 获取当前用户档案。
        /// </summary>
        /// <param name="user">用户实例。</param>
        /// <returns>返回用户档案实例。</returns>
        Task<UserProfile> GetProfileAsync(User user);

        /// <summary>
        /// 获取当前用户档案。
        /// </summary>
        /// <param name="userName">用户名称。</param>
        /// <returns>返回用户档案实例。</returns>
        Task<UserProfile> GetProfileAsync(string userName);

        /// <summary>
        /// 获取当前用户档案。
        /// </summary>
        /// <param name="userName">用户名称。</param>
        /// <returns>返回用户档案实例。</returns>
        Task<UserProfile> FindAndViewAsync(string userName);

        /// <summary>
        /// 修改个人简介。
        /// </summary>
        /// <param name="userId">用户Id。</param>
        /// <param name="intro">用户介绍。</param>
        /// <returns>返回更新结果。</returns>
        Task<DataResult> ChangeIntroAsync(int userId, string intro);
    }
}