using Mozlite.Data;
using Mozlite.Extensions.Settings;
using Mozlite.Extensions.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Messages.Notifications
{
    /// <summary>
    /// 通知清理服务。
    /// </summary>
    public class NotificationTaskService : TaskService
    {
        private readonly IDbContext<Notification> _context;
        private readonly ISettingsManager _settingsManager;

        public NotificationTaskService(IDbContext<Notification> context, ISettingsManager settingsManager)
        {
            _context = context;
            _settingsManager = settingsManager;
        }

        /// <summary>
        /// 名称。
        /// </summary>
        public override string Name => "系统通知清理服务";

        /// <summary>
        /// 描述。
        /// </summary>
        public override string Description => "清理每个用户得系统通知，删除多余得系统通知";

        /// <summary>
        /// 执行间隔时间。
        /// </summary>
        public override TaskInterval Interval => new TimeSpan(2, 0, 0);

        private class Notifier
        {
            public int UserId { get; set; }

            public int Size { get; set; }
        }

        private async Task<IEnumerable<Notifier>> LoadAsync()
        {
            var notifiers = new List<Notifier>();
            using (var reader =
                await _context.ExecuteReaderAsync(
                    $"SELECT COUNT(1) as Size, UserId FROM {_context.EntityType.Table} GROUP BY UserId;"))
            {
                notifiers.Add(new Notifier { Size = reader.GetInt32(0), UserId = reader.GetInt32(1) });
            }

            return notifiers;
        }

        /// <summary>
        /// 执行方法。
        /// </summary>
        /// <param name="argument">参数。</param>
        public override async Task ExecuteAsync(Argument argument)
        {
            var notifiers = await LoadAsync();
            var settings = await _settingsManager.GetSettingsAsync<NotificationSettings>();
            notifiers = notifiers.Where(x => x.Size > settings.MaxSize).ToList();
            foreach (var notifier in notifiers)
            {
                await _context.ExecuteNonQueryAsync(
                    $"DELETE FROM {_context.EntityType.Table} WHERE Id IN(SELECT TOP({notifier.Size - settings.MaxSize}) Id FROM {_context.EntityType.Table} WHERE UserId = {notifier.UserId}) ORDER BY CreatedDate");
            }
        }
    }
}