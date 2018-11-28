using Mozlite.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Installers
{
    /// <summary>
    /// 安装管理实现类基类。
    /// </summary>
    public class InstallerManager : IInstallerManager
    {
        private readonly IDbContext<Lisence> _context;
        /// <summary>
        /// 初始化类<see cref="InstallerManager"/>。
        /// </summary>
        /// <param name="context">数据库操作接口。</param>
        public InstallerManager(IDbContext<Lisence> context)
        {
            _context = context;
        }

        /// <summary>
        /// 保存注册码。
        /// </summary>
        /// <param name="registration">注册码实例。</param>
        /// <returns>返回保存结果。</returns>
        public async Task<bool> SaveRegistrationAsync(Registration registration)
        {
            var lisence = new Lisence { Registration = Cores.Encrypto(registration.ToJsonString()) };
            if (await _context.AnyAsync())
                return await _context.UpdateAsync(lisence);
            return await _context.CreateAsync(lisence);
        }

        /// <summary>
        /// 获取注册码。
        /// </summary>
        /// <returns>返回注册码实例。</returns>
        public async Task<Registration> GetRegistrationAsync()
        {
            var registions = await _context.FetchAsync();
            if (registions.Any())
            {
                try
                {
                    var code = registions.First().Registration;
                    code = Cores.Decrypto(code.Trim());
                    return Cores.FromJsonString<Registration>(code);
                }
                catch
                {
                    // ignored
                }
            }

            var registration = new Registration();
            await SaveRegistrationAsync(registration);
            return registration;
        }

        /// <summary>
        /// 设置成功。
        /// </summary>
        /// <param name="status">状态。</param>
        /// <returns>返回保存结果。</returns>
        public async Task<bool> SuccessAsync(InstallerStatus status)
        {
            var registration = await GetRegistrationAsync();
            registration.Status = status;
            return await SaveRegistrationAsync(registration);
        }
    }
}