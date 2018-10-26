namespace Mozlite.Extensions
{
    /// <summary>
    /// 数据库操作结果，负值表示成功，正直表示失败。
    /// </summary>
    /// <remarks>
    /// 成功和失败绝对值相等。
    /// </remarks>
    public enum DataAction
    {
        /// <summary>
        /// 成功添加。
        /// </summary>
        Created = -4,

        /// <summary>
        /// 成功更新。
        /// </summary>
        Updated = -3,

        /// <summary>
        /// 成功删除。
        /// </summary>
        Deleted = -2,

        /// <summary>
        /// 成功。
        /// </summary>
        Success = 0,

        /// <summary>
        /// 重复。
        /// </summary>
        Duplicate = 1,

        /// <summary>
        /// 删除失败。
        /// </summary>
        DeletedFailured = 2,

        /// <summary>
        /// 更新失败。
        /// </summary>
        UpdatedFailured = 3,

        /// <summary>
        /// 添加失败。
        /// </summary>
        CreatedFailured = 4,

        /// <summary>
        /// 包含子项不为空，不能删除！
        /// </summary>
        DeletedFailuredItemsNotEmpty = 5,

        /// <summary>
        /// 未知错误。
        /// </summary>
        UnknownError = 999,
    }
}