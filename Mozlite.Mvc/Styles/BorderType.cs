namespace Mozlite.Mvc.Styles
{
    /// <summary>
    /// 边框样式枚举。
    /// </summary>
    public enum BorderType
    {
        /// <summary>
        /// 无。
        /// </summary>
        None,
        /// <summary>
        /// 与 "none" 相同。不过应用于表时除外，对于表，hidden 用于解决边框冲突。
        /// </summary>
        Hidden,
        /// <summary>
        /// 实体边框。
        /// </summary>
        Solid,
        /// <summary>
        /// 点边框。
        /// </summary>
        Dotted,
        /// <summary>
        /// 虚线。
        /// </summary>
        Dashed,
        /// <summary>
        /// 双线。
        /// </summary>
        Double,
        /// <summary>
        /// 定义 3D 凹槽边框。其效果取决于 border-color 的值。
        /// </summary>
        Groove,
        /// <summary>
        /// 定义 3D 垄状边框。其效果取决于 border-color 的值。
        /// </summary>
        Ridge,
        /// <summary>
        /// 定义 3D inset 边框。其效果取决于 border-color 的值。
        /// </summary>
        Inset,
        /// <summary>
        /// 定义 3D outset 边框。其效果取决于 border-color 的值。
        /// </summary>
        Outset,
        /// <summary>
        /// 规定应该从父元素继承边框样式。
        /// </summary>
        Inherit,
    }
}