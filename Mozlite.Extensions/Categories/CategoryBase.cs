namespace Mozlite.Extensions.Categories
{
    /// <summary>
    /// 分类Id。
    /// </summary>
    public abstract class CategoryBase : IIdObject
    {
        /// <summary>
        /// 获取或设置唯一Id。
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 分类名称。
        /// </summary>
        [Size(64)]
        public string Name { get; set; }
    }
}