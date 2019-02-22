using Mozlite.Extensions.Messages.SMS;

namespace Mozlite.Mvc.RazorUI.Extensions.Messages
{
    /// <summary>
    /// 短信发送服务。
    /// </summary>
    public class SmsTaskService : Mozlite.Extensions.Messages.SMS.SmsTaskService
    {
        /// <summary>
        /// 初始化类<see cref="SmsTaskService"/>。
        /// </summary>
        /// <param name="smsManager">短信管理接口。</param>
        public SmsTaskService(ISmsManager smsManager) : base(smsManager)
        {
        }
    }
}