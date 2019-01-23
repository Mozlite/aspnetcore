using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Messages.Notifications
{
    /// <summary>
    /// 通知管理接口。
    /// </summary>
    public interface INotificationManager : IObjectManager<Notification>, ISingletonService
    {
        /// <summary>
        /// 加载当前用户最新得通知。
        /// </summary>
        /// <param name="size">通知记录数。</param>
        /// <returns>返回通知列表。</returns>
        IEnumerable<Notification> Load(int size = 0);

        /// <summary>
        /// 加载当前用户最新得通知。
        /// </summary>
        /// <param name="size">通知记录数。</param>
        /// <returns>返回通知列表。</returns>
        Task<IEnumerable<Notification>> LoadAsync(int size = 0);

        /// <summary>
        /// 获取当前用户得通知数量。
        /// </summary>
        /// <returns>返回通知得数量。</returns>
        int GetSize();

        /// <summary>
        /// 获取当前用户得通知数量。
        /// </summary>
        /// <returns>返回通知得数量。</returns>
        Task<int> GetSizeAsync();

        /// <summary>
        /// 保存对象实例。
        /// </summary>
        /// <param name="userId">用户列表。</param>
        /// <param name="model">模型实例对象。</param>
        /// <returns>返回保存结果。</returns>
        DataResult Save(int[] userId, Notification model);

        /// <summary>
        /// 保存对象实例。
        /// </summary>
        /// <param name="userId">用户列表。</param>
        /// <param name="model">模型实例对象。</param>
        /// <returns>返回保存结果。</returns>
        Task<DataResult> SaveAsync(int[] userId, Notification model);

        /// <summary>
        /// 设置状态。
        /// </summary>
        /// <param name="id">通知id。</param>
        /// <param name="status">状态。</param>
        /// <returns>返回设置结果。</returns>
        bool SetStatus(int id, NotificationStatus status);

        /// <summary>
        /// 设置状态。
        /// </summary>
        /// <param name="id">通知id。</param>
        /// <param name="status">状态。</param>
        /// <returns>返回设置结果。</returns>
        Task<bool> SetStatusAsync(int id, NotificationStatus status);

        /// <summary>
        /// 设置状态。
        /// </summary>
        /// <param name="ids">通知id。</param>
        /// <param name="status">状态。</param>
        /// <returns>返回设置结果。</returns>
        bool SetStatus(int[] ids, NotificationStatus status);

        /// <summary>
        /// 设置状态。
        /// </summary>
        /// <param name="ids">通知id。</param>
        /// <param name="status">状态。</param>
        /// <returns>返回设置结果。</returns>
        Task<bool> SetStatusAsync(int[] ids, NotificationStatus status);

    }
}