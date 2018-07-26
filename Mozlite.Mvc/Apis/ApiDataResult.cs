namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// API数据结果。
    /// </summary>
    public class ApiDataResult : ApiResult
    {
        /// <summary>
        /// 返回的数据实例。
        /// </summary>
        public object Data { get; }

        /// <summary>
        /// 初始化类<see cref="ApiDataResult"/>。
        /// </summary>
        /// <param name="data">返回的数据实例。</param>
        public ApiDataResult(object data) : base(ErrorCode.Succeeded)
        {
            Data = data;
        }
    }
}