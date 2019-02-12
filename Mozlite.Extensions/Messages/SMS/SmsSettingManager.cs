using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Messages.SMS
{
    /// <summary>
    /// 短信配置管理实现类。
    /// </summary>
    public class SmsSettingManager : CachableObjectManager<SmsSettings>, ISmsSettingManager
    {
        /// <summary>
        /// 初始化类<see cref="SmsSettingManager"/>。
        /// </summary>
        /// <param name="context">数据库操作实例。</param>
        /// <param name="cache">缓存接口。</param>
        public SmsSettingManager(IDbContext<SmsSettings> context, IMemoryCache cache) : base(context, cache)
        {
        }

        /// <summary>
        /// 通过名称查询配置。
        /// </summary>
        /// <param name="name">客户端名称。</param>
        /// <returns>返回配置实例。</returns>
        public virtual SmsSettings Find(string name)
        {
            var settings = Fetch();
            var setting = settings.SingleOrDefault(x => x.Client.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (setting == null)
            {
                setting = new SmsSettings { Client = name };
                Save(setting);
            }

            return setting;
        }

        /// <summary>
        /// 通过名称查询配置。
        /// </summary>
        /// <param name="name">客户端名称。</param>
        /// <returns>返回配置实例。</returns>
        public virtual async Task<SmsSettings> FindAsync(string name)
        {
            var settings = await FetchAsync();
            var setting = settings.SingleOrDefault(x => x.Client.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (setting == null)
            {
                setting = new SmsSettings { Client = name };
                await SaveAsync(setting);
            }

            return setting;
        }

        /// <summary>
        /// 判断是否重复。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        /// <returns>返回判断结果。</returns>
        public override bool IsDuplicated(SmsSettings model)
        {
            return Context.Any(x => x.Client == model.Client && x.Id != model.Id);
        }

        /// <summary>
        /// 判断是否重复。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        /// <returns>返回判断结果。</returns>
        /// <param name="cancellationToken">取消标识。</param>
        public override Task<bool> IsDuplicatedAsync(SmsSettings model, CancellationToken cancellationToken = default)
        {
            return Context.AnyAsync(x => x.Client == model.Client && x.Id != model.Id, cancellationToken);
        }
    }
}