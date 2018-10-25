using Microsoft.Extensions.Logging;
using Mozlite.Mvc.Properties;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mozlite.Mvc.Apis.Client
{
    /// <summary>
    /// 客户端接口。
    /// </summary>
    public interface IClient
    {

    }

    /// <summary>
    /// 客户端基类。
    /// </summary>
    public abstract class ClientBase : IClient
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

        private async Task<TokenClientResult> GetTokenAsync()
        {
            var result = await ExecuteAsync<TokenClientResult>("token", $"appSecret={AppSecret}", (client, url) => client.GetStringAsync(url));
            if (result.Succeeded)
                Token = result.Data.Token;
            return result;
        }

        private Task<TResult> ExecuteAsync<TResult>(string api, string queryString, Func<HttpClient, string, Task<string>> action)
            where TResult : ClientResult, new() =>
            ExecuteAsync<TResult>(GetUrl(api, queryString), action, 0);

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
                    return JsonConvert.DeserializeObject<TResult>(result);
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
    }
}