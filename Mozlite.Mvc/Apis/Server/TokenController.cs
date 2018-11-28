using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Mozlite.Mvc.Apis.Server
{
    /// <summary>
    /// API控制器。
    /// </summary>
    [ApiController(Anonymousable = true)]
    public class TokenController : ApiController
    {
        /// <summary>
        /// 验证API，生成令牌。
        /// </summary>
        /// <param name="appSecret">应用程序密钥。</param>
        /// <returns>返回令牌验证结果。</returns>
        [Result]
        public async Task<ApiResult> Index([ApiParameter("密钥")]string appSecret)
        {
            if (string.IsNullOrEmpty(appSecret))
                return NullParameter(nameof(appSecret));
            if (!Application.AppSecret.Equals(appSecret, StringComparison.OrdinalIgnoreCase))
                return Error(ErrorCode.AuthorizeFailure);
            if (Application.ExpiredDate <= DateTimeOffset.Now || string.IsNullOrEmpty(Application.Token))
            {
                var result = await GetRequiredService<IApiManager>().GenerateTokenAsync(Application);
                if (!result)
                {
                    Logger.LogError($"更新令牌失败：appid({Application.AppId}), token({Application.Token}), expiredDate({Application.ExpiredDate})。");
                    return UnknownError();
                }
            }
            return Data(new { Application.ExpiredDate, Application.Token });
        }

        private class ResultAttribute : ApiDataResultAttribute
        {
            public ResultAttribute() : base(
                new { ExpiredDate = DateTime.Now.AddDays(72), Token = Cores.GeneralKey(128) },
                "获取令牌API，通过验证后可以获得请求API令牌。"
                )
            {
            }
        }
    }
}