using System.Net.Http;
using System.Threading.Tasks;

namespace Mozlite.Mvc.Apis.Client
{
    /// <summary>
    /// 客户端接口。
    /// </summary>
    public interface IClientBase
    {
        /// <summary>
        /// 发送DELETE请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="queryString">参数表达式。</param>
        /// <returns>返回DELETE结果。</returns>
        Task<ClientResult> DeleteAsync(string api, string queryString = null);

        /// <summary>
        /// 发送DELETE请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="queryString">参数表达式。</param>
        /// <returns>返回DELETE结果。</returns>
        Task<TResult> DeleteAsync<TResult>(string api, string queryString = null)
            where TResult : ClientResult, new();

        /// <summary>
        /// 发送POST请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="queryString">参数表达式。</param>
        /// <param name="content">请求内容。</param>
        /// <returns>返回POST结果。</returns>
        Task<ClientResult> PostAsync(string api, string queryString, HttpContent content);

        /// <summary>
        /// 发送POST请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="queryString">参数表达式。</param>
        /// <param name="content">请求内容。</param>
        /// <returns>返回POST结果。</returns>
        Task<TResult> PostAsync<TResult>(string api, string queryString, HttpContent content)
            where TResult : ClientResult, new();

        /// <summary>
        /// 发送POST请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="content">请求内容。</param>
        /// <returns>返回POST结果。</returns>
        Task<ClientResult> PostAsync(string api, HttpContent content);

        /// <summary>
        /// 发送POST请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="content">请求内容。</param>
        /// <returns>返回POST结果。</returns>
        Task<TResult> PostAsync<TResult>(string api, HttpContent content)
            where TResult : ClientResult, new();

        /// <summary>
        /// 发送PUT请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="content">请求内容。</param>
        /// <returns>返回PUT结果。</returns>
        Task<TResult> PutAsync<TResult>(string api, HttpContent content)
            where TResult : ClientResult, new();

        /// <summary>
        /// 发送PUT请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="content">请求内容。</param>
        /// <returns>返回PUT结果。</returns>
        Task<ClientResult> PutAsync(string api, HttpContent content);

        /// <summary>
        /// 发送PUT请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="queryString">参数表达式。</param>
        /// <param name="content">请求内容。</param>
        /// <returns>返回PUT结果。</returns>
        Task<TResult> PutAsync<TResult>(string api, string queryString, HttpContent content)
            where TResult : ClientResult, new();

        /// <summary>
        /// 发送PUT请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="queryString">参数表达式。</param>
        /// <param name="content">请求内容。</param>
        /// <returns>返回PUT结果。</returns>
        Task<ClientResult> PutAsync(string api, string queryString, HttpContent content);

        /// <summary>
        /// 发送GET请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="queryString">参数表达式。</param>
        /// <returns>返回GET结果。</returns>
        Task<TResult> GetAsync<TResult>(string api, string queryString = null)
            where TResult : ClientResult, new();
    }
}