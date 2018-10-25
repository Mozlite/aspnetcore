namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// API服务接口。
    /// </summary>
    public interface IApiService : IServices
    {
        /// <summary>
        /// 应用程序名称。
        /// </summary>
        string ApiName { get; }
    }
}