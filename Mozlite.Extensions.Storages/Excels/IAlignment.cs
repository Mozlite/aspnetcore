namespace Mozlite.Extensions.Storages.Excels
{
    /// <summary>
    /// 对齐接口。
    /// </summary>
    public interface IAlignment
    {
        /// <summary>
        /// 横向对齐方式。
        /// </summary>
        HorizontalAlignment Horizontal { get; set; }

        /// <summary>
        /// 垂直对齐方式。
        /// </summary>
        VerticalAlignment Vertical { get; set; }
    }
}