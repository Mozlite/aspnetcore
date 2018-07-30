using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using Mozlite.Extensions.Data;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// 应用程序管理。
    /// </summary>
    public class ApiManager : CachableObjectManager<Application, Guid>, IApiManager
    {
        /// <summary>
        /// 初始化类<see cref="ApiManager"/>。
        /// </summary>
        /// <param name="context">数据库操作实例。</param>
        /// <param name="cache">缓存接口。</param>
        public ApiManager(IDbContext<Application> context, IMemoryCache cache)
            : base(context, cache)
        {
        }

        private const int ExpiredDays = 72;

        /// <summary>
        /// 重新生产令牌。
        /// </summary>
        /// <param name="application">当前应用程序。</param>
        public virtual void GenerateToken(Application application)
        {
            application.Token = Cores.GeneralKey(128);
            application.ExpiredDate = DateTime.Now.AddDays(ExpiredDays);
        }

        /// <summary>
        /// 重新生产令牌，并且保存。
        /// </summary>
        /// <param name="application">当前应用程序。</param>
        /// <returns>返回数据库操作结果。</returns>
        public virtual DataResult SaveGenerateToken(Application application)
        {
            GenerateToken(application);
            return DataResult.FromResult(Context.Update(application.Id, new { application.Token, application.ExpiredDate }), DataAction.Updated);
        }

        /// <summary>
        /// 重新生产令牌，并且保存。
        /// </summary>
        /// <param name="application">当前应用程序。</param>
        /// <returns>返回数据库操作结果。</returns>
        public virtual async Task<DataResult> SaveGenerateTokenAsync(Application application)
        {
            GenerateToken(application);
            return DataResult.FromResult(await Context.UpdateAsync(application.Id, new { application.Token, application.ExpiredDate }), DataAction.Updated);
        }
    }
}