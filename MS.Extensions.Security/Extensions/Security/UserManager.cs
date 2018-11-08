using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mozlite.Extensions.Security;
using Mozlite.Extensions.Storages;

namespace MS.Extensions.Security
{
    /// <summary>
    /// 用户管理。
    /// </summary>
    public class UserManager : UserManager<User, Role, UserClaim, UserRole, UserLogin, UserToken, RoleClaim>, IUserManager
    {
        private readonly IMediaDirectory _directory;

        /// <summary>
        /// 初始化类<see cref="UserManager"/>。
        /// </summary>
        /// <param name="store">用户存储接口。</param>
        /// <param name="optionsAccessor"><see cref="T:Microsoft.AspNetCore.Identity.IdentityOptions" />实例对象。</param>
        /// <param name="passwordHasher">密码加密器接口。</param>
        /// <param name="userValidators">用户验证接口。</param>
        /// <param name="passwordValidators">密码验证接口。</param>
        /// <param name="keyNormalizer">唯一键格式化字符串。</param>
        /// <param name="errors">错误实例。</param>
        /// <param name="serviceProvider">服务提供者接口。</param>
        public UserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider serviceProvider) 
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, serviceProvider)
        {
            _directory = serviceProvider.GetRequiredService<IMediaDirectory>();
        }
        
        /// <summary>
        /// 上传头像。
        /// </summary>
        /// <param name="file">上传的文件实例对象。</param>
        /// <param name="id">用户Id。</param>
        /// <returns>返回上传结果。</returns>
        public async Task<MediaResult> UploadAvatarAsync(IFormFile file, int id)
        {
            if (file.Length <= 0)
                return "不能上传空文件！";
            if (!file.FileName.IsPictureFile())
                return "头像只能为图片文件！";
            if (!await DbContext.UserContext.AnyAsync(id))
                return "用户不存在！";
            var result = await _directory.UploadAsync(file, SecuritySettings.ExtensionName, id);
            if (result.Succeeded)
            {
                if (await DbContext.UserContext.UpdateAsync(id, new { Avatar = result.Url, UpdatedDate = DateTimeOffset.Now }))
                    return result;
                return "更新用户头像失败！";
            }
            return result;
        }
    }
}