using Mozlite.Data;

namespace MozliteDemo.Extensions.ProjectManager.Issues
{
    /// <summary>
    /// 任务查询实例。
    /// </summary>
    public class IssueQuery : QueryBase<Issue>
    {
        /// <summary>
        /// 用户Id。
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 当前操作员。
        /// </summary>
        public int Operator { get; set; }

        /// <summary>
        /// 验收员。
        /// </summary>
        public int Moderator { get; set; }

        /// <summary>
        /// 状态。
        /// </summary>
        public IssueStatus? Status { get; set; }

        /// <summary>
        /// 类型。
        /// </summary>
        public IssueType? Type { get; set; }

        /// <summary>
        /// 标题。
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 优先级。
        /// </summary>
        public IssueLevel? Level { get; set; }

        /// <summary>
        /// 里程碑Id。
        /// </summary>
        public int Mid { get; set; }

        /// <summary>
        /// 项目Id。
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// 初始化查询上下文。
        /// </summary>
        /// <param name="context">查询上下文。</param>
        protected override void Init(IQueryContext<Issue> context)
        {
            context.WithNolock();
            if (ProjectId > 0)
                context.Where(x => x.ProjectId == ProjectId);
            if (Mid > 0)
                context.Where(x => x.MilestoneId == Mid);
            if (UserId > 0)
                context.Where(x => x.UserId == UserId);
            if (Operator > 0)
                context.Where(x => x.Operator == Operator);
            if (Moderator > 0)
                context.Where(x => x.Moderator == Moderator);
            if (Status != null)
                context.Where(x => x.Status == Status);
            if (Type != null)
                context.Where(x => x.Type == Type);
            if (Level != null)
                context.Where(x => x.Level == Level);
            if (!string.IsNullOrEmpty(Title))
                context.Where(x => x.Title.Contains(Title));
        }
    }
}