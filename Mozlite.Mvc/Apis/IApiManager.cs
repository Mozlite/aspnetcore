using System;
using System.Threading.Tasks;
using Mozlite.Extensions.Data;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// 应用程序管理接口。
    /// </summary>
    public interface IApiManager : ICachableObjectManager<Application, Guid>, ISingletonService
    {
        /// <summary>
        /// 重新生产令牌。
        /// </summary>
        /// <param name="application">当前应用程序。</param>
        void GenerateToken(Application application);

        /// <summary>
        /// 重新生产令牌，并且保存。
        /// </summary>
        /// <param name="application">当前应用程序。</param>
        /// <returns>返回数据库操作结果。</returns>
        DataResult SaveGenerateToken(Application application);

        /// <summary>
        /// 重新生产令牌，并且保存。
        /// </summary>
        /// <param name="application">当前应用程序。</param>
        /// <returns>返回数据库操作结果。</returns>
        Task<DataResult> SaveGenerateTokenAsync(Application application);
    }
}