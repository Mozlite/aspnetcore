using Mozlite.Extensions.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// 应用程序管理接口。
    /// </summary>
    public interface IApiManager : ISingletonService
    {
        /// <summary>
        /// 重新生产令牌，并且保存。
        /// </summary>
        /// <param name="application">当前应用程序。</param>
        /// <returns>返回数据库操作结果。</returns>
        DataResult GenerateToken(Application application);

        /// <summary>
        /// 重新生产令牌，并且保存。
        /// </summary>
        /// <param name="application">当前应用程序。</param>
        /// <returns>返回数据库操作结果。</returns>
        Task<DataResult> GenerateTokenAsync(Application application);

        /// <summary>
        /// 获取应用程序。
        /// </summary>
        /// <param name="id">Id。</param>
        /// <returns>返回应用程序实例对象。</returns>
        Application Find(int id);

        /// <summary>
        /// 获取应用程序。
        /// </summary>
        /// <param name="id">Id。</param>
        /// <returns>返回应用程序实例对象。</returns>
        Task<Application> FindAsync(int id);

        /// <summary>
        /// 获取应用程序。
        /// </summary>
        /// <param name="appId">应用程序Id。</param>
        /// <returns>返回应用程序实例对象。</returns>
        Application Find(Guid appId);

        /// <summary>
        /// 获取应用程序。
        /// </summary>
        /// <param name="appId">应用程序Id。</param>
        /// <returns>返回应用程序实例对象。</returns>
        Task<Application> FindAsync(Guid appId);

        /// <summary>
        /// 获取应用程序列表。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回应用程序实例对象列表。</returns>
        IEnumerable<Application> Load(Predicate<Application> expression = null);

        /// <summary>
        /// 获取应用程序列表。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回应用程序实例对象列表。</returns>
        Task<IEnumerable<Application>> LoadAsync(Predicate<Application> expression = null);
    }
}