using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Mozlite.Extensions;

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
        DataResult GenerateToken(CacheApplication application);

        /// <summary>
        /// 重新生产令牌，并且保存。
        /// </summary>
        /// <param name="application">当前应用程序。</param>
        /// <returns>返回数据库操作结果。</returns>
        Task<DataResult> GenerateTokenAsync(CacheApplication application);
        
        /// <summary>
        /// 保存应用程序。
        /// </summary>
        /// <param name="application">当前应用程序。</param>
        /// <returns>返回数据库操作结果。</returns>
        DataResult Save(Application application);

        /// <summary>
        /// 保存应用程序。
        /// </summary>
        /// <param name="application">当前应用程序。</param>
        /// <returns>返回数据库操作结果。</returns>
        Task<DataResult> SaveAsync(Application application);

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
        CacheApplication Find(Guid appId);

        /// <summary>
        /// 获取应用程序。
        /// </summary>
        /// <param name="appId">应用程序Id。</param>
        /// <returns>返回应用程序实例对象。</returns>
        Task<CacheApplication> FindAsync(Guid appId);

        /// <summary>
        /// 获取应用程序列表。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回应用程序实例对象列表。</returns>
        IEnumerable<Application> Load(Expression<Predicate<Application>> expression = null);

        /// <summary>
        /// 获取应用程序列表。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回应用程序实例对象列表。</returns>
        Task<IEnumerable<Application>> LoadAsync(Expression<Predicate<Application>> expression = null);

        /// <summary>
        /// 加载API。
        /// </summary>
        /// <param name="categoryId">分类Id。</param>
        /// <returns>返回当前类型的API列表。</returns>
        IEnumerable<ApiDescriptor> LoadApis(int categoryId = 0);

        /// <summary>
        /// 加载API。
        /// </summary>
        /// <param name="categoryId">分类Id。</param>
        /// <returns>返回当前类型的API列表。</returns>
        Task<IEnumerable<ApiDescriptor>> LoadApisAsync(int categoryId = 0);

        /// <summary>
        /// 加载API。
        /// </summary>
        /// <param name="applicationId">应用程序Id。</param>
        /// <returns>返回当前应用程序的API列表。</returns>
        IEnumerable<ApiDescriptor> LoadApplicationApis(int applicationId);

        /// <summary>
        /// 加载API。
        /// </summary>
        /// <param name="applicationId">应用程序Id。</param>
        /// <returns>返回当前应用程序的API列表。</returns>
        Task<IEnumerable<ApiDescriptor>> LoadApplicationApisAsync(int applicationId);

        /// <summary>
        /// 删除应用程序。
        /// </summary>
        /// <param name="ids">应用程序Id。</param>
        /// <returns>返回删除结果。</returns>
        DataResult DeleteApplications(int[] ids);

        /// <summary>
        /// 删除应用程序。
        /// </summary>
        /// <param name="ids">应用程序Id。</param>
        /// <returns>返回删除结果。</returns>
        Task<DataResult> DeleteApplicationsAsync(int[] ids);

        /// <summary>
        /// 将API设置到应用中。
        /// </summary>
        /// <param name="appid">应用程序Id。</param>
        /// <param name="apis">API的Id列表。</param>
        /// <returns>返回保存结果。</returns>
        DataResult AddApis(int appid, int[] apis);

        /// <summary>
        /// 将API设置到应用中。
        /// </summary>
        /// <param name="appid">应用程序Id。</param>
        /// <param name="apis">API的Id列表。</param>
        /// <returns>返回保存结果。</returns>
        Task<DataResult> AddApisAsync(int appid, int[] apis);
    }
}