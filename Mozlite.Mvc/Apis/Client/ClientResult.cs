namespace Mozlite.Mvc.Apis.Client
{
    /// <summary>
    /// 客户端返回的结果。
    /// </summary>
    public class ClientResult
    {
        /// <summary>
        /// 错误码。
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 消息。
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 是否成功。
        /// </summary>
        public bool Succeeded => Code == 0;
    }

    /// <summary>
    /// 客户端返回的结果。
    /// </summary>
    /// <typeparam name="TData">返回数据类型。</typeparam>
    public class ClientResult<TData> : ClientResult
    {
        /// <summary>
        /// 返回的数据实例。
        /// </summary>
        public TData Data { get; set; }
    }
}