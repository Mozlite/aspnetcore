using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// API控制器。
    /// </summary>
    public class OpenController : ApiControllerBase
    {
        /// <summary>
        /// 验证API，生成令牌。
        /// </summary>
        /// <param name="appSecret">应用程序密钥。</param>
        /// <returns>返回令牌验证结果。</returns>
        [Api("token", Anonymousable = true)]
        public async Task<ApiResult> Index(string appSecret)
        {
            if (string.IsNullOrEmpty(appSecret))
                return NullParameter(nameof(appSecret));
            if (!Application.AppSecret.Equals(appSecret, StringComparison.OrdinalIgnoreCase))
                return Error(ErrorCode.AuthorizeFailure);
            if (Application.ExpiredDate <= DateTime.Now)
            {
                var result = await GetRequiredService<IApiManager>().SaveGenerateTokenAsync(Application);
                if (!result)
                {
                    Logger.LogError($"更新令牌失败：appid({Application.AppId}), token({Application.Token}), expiredDate({Application.ExpiredDate})。");
                    return UnknownError();
                }
            }
            return Data(new { Application.ExpiredDate, Application.Token });
        }
    }
}