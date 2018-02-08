using System;
using System.Linq;
using System.Threading.Tasks;
using Mozlite.Data;
using Newtonsoft.Json;

namespace Mozlite.Extensions.Installers
{
    /// <summary>
    /// 安装管理实现类基类。
    /// </summary>
    /// <typeparam name="TRegistration">注册类型。</typeparam>
    public abstract class InstallerManager<TRegistration> : IInstallerManager<TRegistration>
        where TRegistration : Registration, new()
    {
        private readonly IDbContext<Lisence> _context;
        /// <summary>
        /// 初始化类<see cref="InstallerManager{TRegistration}"/>。
        /// </summary>
        /// <param name="context">数据库操作接口。</param>
        protected InstallerManager(IDbContext<Lisence> context)
        {
            _context = context;
        }

        private readonly object _locker = new object();
        /// <summary>
        /// 设置当前安装步骤。
        /// </summary>
        /// <param name="current">当前步骤。</param>
        /// <returns>返回当前安装步骤。</returns>
        private InstallerStatus SetCurrent(InstallerStatus current)
        {
            lock (_locker)
            {
                Installer.Current = current;
                return current;
            }
        }

        /// <summary>
        /// 是否已经安装。
        /// </summary>
        /// <returns>返回判断结果。</returns>
        public async Task<InstallerStatus> InstalledAsync()
        {
            //等待数据迁移
            var counter = 0;
            while (true)
            {
                if (Installer.Current == InstallerStatus.DataSuccess)
                    break;
                if (counter++ > 100)
                    return SetCurrent(InstallerStatus.DataError);
                await Task.Delay(100);
                await InstalledAsync();
            }

            var registration = await GetLisenceAsync();
            if (registration.Current == InstallerStatus.New)
            {
                registration = await SetupAsync();
                if (await SaveLisenceAsync(registration))
                    return SetCurrent(InstallerStatus.Config);//配置
            }
            else if (registration.Current == InstallerStatus.Success)
                return SetCurrent(await IsValidAsync(registration));//验证
            return SetCurrent(registration.Current);//保存的步骤
        }

        /// <summary>
        /// 保存注册码。
        /// </summary>
        /// <param name="registration">注册码实例。</param>
        /// <returns>返回保存结果。</returns>
        public async Task<bool> SaveLisenceAsync(TRegistration registration)
        {
            var lisence = new Lisence { Registration = Cores.Encrypto(JsonConvert.SerializeObject(registration)) };
            if (!await _context.AnyAsync())
                return await _context.CreateAsync(lisence);
            if (await _context.UpdateAsync(lisence))
            {//验证
                SetCurrent(await IsValidAsync(registration));
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取注册码。
        /// </summary>
        /// <returns>返回注册码实例。</returns>
        public async Task<TRegistration> GetLisenceAsync()
        {
            var lisence = (await _context.FetchAsync()).FirstOrDefault();
            var registration = lisence?.Registration;
            if (string.IsNullOrWhiteSpace(registration))
                return new TRegistration { Current = InstallerStatus.New };
            try
            {
                registration = Cores.Decrypto(registration.Trim());
                return JsonConvert.DeserializeObject<TRegistration>(registration);
            }
            catch
            {
                return new TRegistration { Current = InstallerStatus.Failured };
            }
        }

        /// <summary>
        /// 验证是否合法。
        /// </summary>
        /// <param name="registration">注册码。</param>
        /// <returns>返回验证结果。</returns>
        protected virtual Task<InstallerStatus> IsValidAsync(TRegistration registration)
        {
            if (registration.Expired <= DateTimeOffset.Now)
                return Task.FromResult(InstallerStatus.Expired);
            return Task.FromResult(InstallerStatus.Success);
        }

        /// <summary>
        /// 执行方法。
        /// </summary>
        /// <returns>返回执行后的注册码实例。</returns>
        protected abstract Task<TRegistration> SetupAsync();
    }
}