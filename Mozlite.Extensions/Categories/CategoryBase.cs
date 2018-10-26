namespace Mozlite.Extensions.Categories
{
    /// <summary>
    /// 分类基类。
    /// </summary>
    public abstract class CategoryBase : IIdObject
    {
        /// <summary>
        /// 获取或设置唯一Id。
        /// </summary>
        [Identity]
        public virtual int Id { get; set; }

        /// <summary>
        /// 分类名称。
        /// </summary>
        [Size(64)]
        public virtual string Name { get; set; }
    }
}