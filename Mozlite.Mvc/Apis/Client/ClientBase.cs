using Microsoft.Extensions.Logging;
using Mozlite.Mvc.Properties;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mozlite.Mvc.Apis.Client
{
    /// <summary>
    /// 客户端基类。
    /// </summary>
    public abstract class ClientBase : IClientBase
    {
        private readonly ILogger _logger;
        private string _url;
        /// <summary>
        /// 初始化类<see cref="ClientBase"/>。
        /// </summary>
        /// <param name="logger">日志接口。</param>
        protected ClientBase(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// API域名地址。
        /// </summary>
        protected abstract string Domain { get; }

        /// <summary>
        /// 是否使用SSL。
        /// </summary>
        protected virtual bool UseSSL { get; }

        private string GetUrl(string api, string queryString)
        {
            if (_url == null)
            {
                _url = UseSSL ? "https://" : "http://";
                _url += Domain;
                _url += "/api/";
            }

            if (queryString == null)
                return _url + api;
            return $"{_url}{api}?{queryString}";
        }

        /// <summary>
        /// 应用程序Id，和服务端的<see cref="Application.AppId"/>对应。
        /// </summary>
        protected abstract string AppId { get; }

        /// <summary>
        /// 应用程序密钥，和服务端的<see cref="Application.AppSecret"/>对应。
        /// </summary>
        protected abstract string AppSecret { get; }

        /// <summary>
        /// 客户端类型，和服务端的<see cref="Application.ExtensionName"/>对应。
        /// </summary>
        protected virtual string ClientName { get; }

        /// <summary>
        /// 令牌，和服务端的<see cref="Application.Token"/>对应。
        /// </summary>
        protected string Token { get; private set; }

        /// <summary>
        /// 请求失败后延迟的秒数。
        /// </summary>
        protected virtual int Delay { get; } = 1;

        private async Task<TResult> ExecuteAsync<TResult>(string api, string queryString,
            Func<HttpClient, string, Task<string>> action)
            where TResult : ClientResult, new()
        {
            var result = await ExecuteAsync<TResult>(GetUrl(api, queryString), action, 0);
            if (result.Code == (int)ErrorCode.InvalidToken)
            {
                var token = await ExecuteAsync<TokenClientResult>(GetUrl("token", $"appSecret={AppSecret}"), (client, url) => client.GetStringAsync(url), 0);
                if (token.Succeeded)
                {
                    Token = token.Data.Token;
                    return await ExecuteAsync<TResult>(api, queryString, action);
                }
                return new TResult { Code = token.Code, Msg = token.Msg };
            }

            return result;
        }

        private async Task<TResult> ExecuteAsync<TResult>(string url, Func<HttpClient, string, Task<string>> action, int times)
            where TResult : ClientResult, new()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-client", ClientName);
                client.DefaultRequestHeaders.Add("x-appid", AppId);
                if (Token != null)
                    client.DefaultRequestHeaders.Add("x-token", Token);
                string result = null;
                try
                {
                    result = await action(client, url);
                    if (string.IsNullOrEmpty(result))
                        return new TResult { Code = (int)ErrorCode.Failured, Msg = Resources.ErrorCode_Failured };
                    return Cores.FromJsonString<TResult>(result);
                }
                catch (Exception exception)
                {
                    if (times < 3)
                    {
                        await Task.Delay(Delay * 1000);
                        return await ExecuteAsync<TResult>(url, action, times + 1);
                    }
                    _logger.LogError(exception, @"请求API失败：{1}
--------------------------------------------------
地址：{2}
--------------------------------------------------
结果：{3}
==================================================
错误：{4}", exception.Message, url, result, exception.StackTrace);
                }
            }
            return new TResult { Code = (int)ErrorCode.UnknownError, Msg = Resources.ErrorCode_UnknownError };
        }

        /// <summary>
        /// 发送DELETE请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="queryString">参数表达式。</param>
        /// <returns>返回DELETE结果。</returns>
        public virtual Task<ClientResult> DeleteAsync(string api, string queryString = null)
        {
            return DeleteAsync<ClientResult>(api, queryString);
        }

        /// <summary>
        /// 发送DELETE请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="queryString">参数表达式。</param>
        /// <returns>返回DELETE结果。</returns>
        public virtual Task<TResult> DeleteAsync<TResult>(string api, string queryString = null)
            where TResult : ClientResult, new()
        {
            return ExecuteAsync<TResult>(api, queryString, async (client, url) => await ReadAsStringAsync(await client.DeleteAsync(url)));
        }

        /// <summary>
        /// 发送POST请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="queryString">参数表达式。</param>
        /// <param name="content">请求内容。</param>
        /// <returns>返回POST结果。</returns>
        public virtual Task<ClientResult> PostAsync(string api, string queryString, HttpContent content)
        {
            return PostAsync<ClientResult>(api, queryString, content);
        }

        /// <summary>
        /// 发送POST请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="queryString">参数表达式。</param>
        /// <param name="content">请求内容。</param>
        /// <returns>返回POST结果。</returns>
        public virtual Task<TResult> PostAsync<TResult>(string api, string queryString, HttpContent content) where TResult : ClientResult, new()
        {
            return ExecuteAsync<TResult>(api, queryString, async (client, url) => await ReadAsStringAsync(await client.PostAsync(url, content)));
        }

        /// <summary>
        /// 发送POST请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="content">请求内容。</param>
        /// <returns>返回POST结果。</returns>
        public virtual Task<ClientResult> PostAsync(string api, HttpContent content)
        {
            return PostAsync<ClientResult>(api, content);
        }

        /// <summary>
        /// 发送POST请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="content">请求内容。</param>
        /// <returns>返回POST结果。</returns>
        public virtual Task<TResult> PostAsync<TResult>(string api, HttpContent content) where TResult : ClientResult, new()
        {
            return PostAsync<TResult>(api, null, content);
        }

        /// <summary>
        /// 发送PUT请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="content">请求内容。</param>
        /// <returns>返回PUT结果。</returns>
        public virtual Task<TResult> PutAsync<TResult>(string api, HttpContent content) where TResult : ClientResult, new()
        {
            return PutAsync<TResult>(api, null, content);
        }

        /// <summary>
        /// 发送PUT请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="content">请求内容。</param>
        /// <returns>返回PUT结果。</returns>
        public virtual Task<ClientResult> PutAsync(string api, HttpContent content)
        {
            return PutAsync<ClientResult>(api, content);
        }

        /// <summary>
        /// 发送PUT请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="queryString">参数表达式。</param>
        /// <param name="content">请求内容。</param>
        /// <returns>返回PUT结果。</returns>
        public virtual Task<TResult> PutAsync<TResult>(string api, string queryString, HttpContent content) where TResult : ClientResult, new()
        {
            return ExecuteAsync<TResult>(api, queryString, async (client, url) => await ReadAsStringAsync(await client.PutAsync(url, content)));
        }

        /// <summary>
        /// 发送PUT请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="queryString">参数表达式。</param>
        /// <param name="content">请求内容。</param>
        /// <returns>返回PUT结果。</returns>
        public virtual Task<ClientResult> PutAsync(string api, string queryString, HttpContent content)
        {
            return PutAsync<ClientResult>(api, queryString, content);
        }

        /// <summary>
        /// 发送GET请求。
        /// </summary>
        /// <param name="api">API名称。</param>
        /// <param name="queryString">参数表达式。</param>
        /// <returns>返回GET结果。</returns>
        public virtual Task<TResult> GetAsync<TResult>(string api, string queryString = null) where TResult : ClientResult, new()
        {
            return ExecuteAsync<TResult>(api, queryString, (client, url) => client.GetStringAsync(url));
        }

        private Task<string> ReadAsStringAsync(HttpResponseMessage response)
        {
            var message = response.EnsureSuccessStatusCode();
            return message.Content.ReadAsStringAsync();
        }
    }
}