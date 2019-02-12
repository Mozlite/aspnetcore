using Mozlite.Extensions.Messages;
using System;
using System.Collections.Generic;

namespace Mozlite.Extensions.Storages.Mail
{
    /// <summary>
    /// 附件。
    /// </summary>
    public static class MailAttachment
    {
        private const string ExtensionKey = "ex:MailAttachment_";

        /// <summary>
        /// 获取当前消息实例附件Id列表。
        /// </summary>
        /// <param name="message">消息实例。</param>
        /// <returns>返回附件列表。</returns>
        public static IEnumerable<Guid> GetAttachments(this Email message)
        {
            foreach (var extendKey in message.ExtendKeys)
            {
                if (extendKey.StartsWith(ExtensionKey))
                {
                    var id = extendKey.Substring(ExtensionKey.Length);
                    if (Guid.TryParse(id, out var result))
                        yield return result;
                }
            }
        }

        /// <summary>
        /// 添加附件。
        /// </summary>
        /// <param name="message">消息实例。</param>
        /// <param name="file">媒体文件实例。</param>
        public static void AddAttachment(this Email message, MediaFile file)
        {
            message[$"{ExtensionKey}{file.Id:N}"] = file.Name;
        }
    }
}