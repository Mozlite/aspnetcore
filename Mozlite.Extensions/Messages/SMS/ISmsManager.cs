using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mozlite.Extensions.Messages.SMS
{
    /// <summary>
    /// 短信管理接口。
    /// </summary>
    public interface ISmsManager : IObjectManager<Note>, ISingletonService
    {
        /// <summary>
        /// 保存短信。
        /// </summary>
        /// <param name="client">客户端发送器名称<see cref="ISmsClient.Name"/>。</param>
        /// <param name="phoneNumbers">电话号码，多个电话号码使用“,”分开。</param>
        /// <param name="message">消息字符串。</param>
        /// <returns>返回保存结果。</returns>
        DataResult Save(string client, string phoneNumbers, string message);

        /// <summary>
        /// 保存短信。
        /// </summary>
        /// <param name="client">客户端发送器名称<see cref="ISmsClient.Name"/>。</param>
        /// <param name="phoneNumbers">电话号码，多个电话号码使用“,”分开。</param>
        /// <param name="message">消息字符串。</param>
        /// <returns>返回保存结果。</returns>
        Task<DataResult> SaveAsync(string client, string phoneNumbers, string message);

        /// <summary>
        /// 发送并保存短信。
        /// </summary>
        /// <param name="note">短信实例对象。</param>
        /// <returns>返回发送结果。</returns>
        Task<SmsResult> SendAsync(Note note);

        /// <summary>
        /// 发送并保存短信。
        /// </summary>
        /// <param name="client">客户端发送器名称<see cref="ISmsClient.Name"/>。</param>
        /// <param name="phoneNumber">电话号码。</param>
        /// <param name="message">消息字符串。</param>
        /// <returns>返回发送结果。</returns>
        Task<SmsResult> SendAsync(string client, string phoneNumber, string message);

        /// <summary>
        /// 加载未发送的短信列表。
        /// </summary>
        /// <returns>未发送的短信列表。</returns>
        Task<IEnumerable<Note>> LoadAsync();
    }
}