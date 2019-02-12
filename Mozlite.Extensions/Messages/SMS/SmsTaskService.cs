using Mozlite.Extensions.Tasks;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Messages.SMS
{
    /// <summary>
    /// 短信发送后台服务。
    /// </summary>
    public class SmsTaskService : TaskService
    {
        private readonly ISmsManager _smsManager;
        /// <summary>
        /// 初始化类<see cref="SmsTaskService"/>。
        /// </summary>
        /// <param name="smsManager">短信管理接口。</param>
        public SmsTaskService(ISmsManager smsManager)
        {
            _smsManager = smsManager;
        }

        /// <summary>
        /// 名称。
        /// </summary>
        public override string Name => "短信发送服务";

        /// <summary>
        /// 描述。
        /// </summary>
        public override string Description => "发送短信相关服务";

        /// <summary>
        /// 执行间隔时间。
        /// </summary>
        public override TaskInterval Interval => 30;

        /// <summary>
        /// 执行方法。
        /// </summary>
        /// <param name="argument">参数。</param>
        public override async Task ExecuteAsync(Argument argument)
        {
            var notes = await _smsManager.LoadAsync();
            foreach (var note in notes)
            {
                await _smsManager.SendAsync(note);
            }
        }
    }
}