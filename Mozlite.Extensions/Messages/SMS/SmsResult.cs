namespace Mozlite.Extensions.Messages.SMS
{
    /// <summary>
    /// SMS发送返回的结果。
    /// </summary>
    public class SmsResult
    {
        /// <summary>
        /// 失败。
        /// </summary>
        public static readonly SmsResult Failured = new SmsResult { Status = NoteStatus.Failured };

        /// <summary>
        /// 成功。
        /// </summary>
        public static readonly SmsResult Succeed = new SmsResult { Status = NoteStatus.Completed };

        /// <summary>
        /// 编码。
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 消息。
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 状态。
        /// </summary>
        public NoteStatus Status { get; set; }

        /// <summary>
        /// 隐士转换为布尔类型。
        /// </summary>
        /// <param name="result">当前值。</param>
        public static implicit operator bool(SmsResult result) => result.Status == NoteStatus.Completed;

        /// <summary>
        /// 隐士转换为布尔类型。
        /// </summary>
        /// <param name="result">当前值。</param>
        public static implicit operator SmsResult(bool result) => result ? Succeed : Failured;
    }
}