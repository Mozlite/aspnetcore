using System.Linq;
using System.Threading.Tasks;
using Mozlite.Data;
using Newtonsoft.Json;

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
        /// 是否是新站。
        /// </summary>
        /// <returns>返回判断结果。</returns>
        public async Task<bool> IsNewAsync()
        {
            return !await _context.AnyAsync();
        }

        /// <summary>
        /// 保存注册码。
        /// </summary>
        /// <param name="registration">注册码实例。</param>
        /// <returns>返回保存结果。</returns>
        public async Task<bool> SaveLisenceAsync(Registration registration)
        {
            var lisence = new Lisence { Registration = Cores.Encrypto(JsonConvert.SerializeObject(registration)) };
            if (await IsNewAsync())
                return await _context.CreateAsync(lisence);
            return await _context.UpdateAsync(lisence);
        }

        /// <summary>
        /// 获取注册码。
        /// </summary>
        /// <returns>返回注册码实例。</returns>
        public async Task<Registration> GetLisenceAsync()
        {
            try
            {
                var registration = (await _context.FetchAsync()).FirstOrDefault()?.Registration;
                if (string.IsNullOrWhiteSpace(registration))
                    return null;
                registration = Cores.Decrypto(registration.Trim());
                return JsonConvert.DeserializeObject<Registration>(registration);
            }
            catch
            {
                return null;
            }
        }
    }
}