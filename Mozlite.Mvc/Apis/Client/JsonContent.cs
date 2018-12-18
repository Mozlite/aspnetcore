using System.Net.Http;
using System.Text;

namespace Mozlite.Mvc.Apis.Client
{
    /// <summary>
    /// JSON内容。
    /// </summary>
    public class JsonContent : StringContent
    {
        /// <summary>
        /// 初始化类<see cref="JsonContent"/>。
        /// </summary>
        /// <param name="content">发送得JSON对象。</param>
        /// <param name="encoding">编码。</param>
        public JsonContent(object content, Encoding encoding) : base(content?.ToJsonString(), encoding, "application/json")
        {
        }

        /// <summary>
        /// 初始化类<see cref="JsonContent"/>。
        /// </summary>
        /// <param name="content">发送得JSON对象。</param>
        public JsonContent(object content) : this(content, Encoding.UTF8)
        {
        }
    }
}