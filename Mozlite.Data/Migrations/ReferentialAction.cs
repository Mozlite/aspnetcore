namespace Mozlite.Data.Migrations
{
    /// <summary>
    /// 外键相关操作。
    /// </summary>
    public enum ReferentialAction
    {
        /// <summary>
        /// 无操作。
        /// </summary>
        NoAction,
        /// <summary>
        /// 约束。
        /// </summary>
        Restrict,
        /// <summary>
        /// 级联。
        /// </summary>
        Cascade,
        /// <summary>
        /// 设为空。
        /// </summary>
        SetNull,
        /// <summary>
        /// 设为默认值。
        /// </summary>
        SetDefault
    }
}