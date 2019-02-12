using Mozlite.Data;

namespace Mozlite.Extensions.Messages.SMS
{
    /// <summary>
    /// 短信查询。
    /// </summary>
    public class NoteQuery : QueryBase<Note>
    {
        /// <summary>
        /// 状态。
        /// </summary>
        public NoteStatus? Status { get; set; }

        /// <summary>
        /// 电话号码。
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 初始化查询上下文。
        /// </summary>
        /// <param name="context">查询上下文。</param>
        protected override void Init(IQueryContext<Note> context)
        {
            context.WithNolock();
            if (Status != null)
                context.Where(x => x.Status == Status);
            if (!string.IsNullOrEmpty(No))
                context.Where(x => x.PhoneNumber == No);
        }
    }
}