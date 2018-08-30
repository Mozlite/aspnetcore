namespace Mozlite.Mvc.AdminMenus
{
    /// <summary>
    /// 菜单提供者。
    /// </summary>
    public abstract class MenuProvider : IMenuProvider
    {
        /// <summary>
        /// 提供者名称，同一个名称归为同一个菜单。
        /// </summary>
        public virtual string Name => "admin";

        /// <summary>
        /// 初始化菜单实例。
        /// </summary>
        /// <param name="root">根目录菜单。</param>
        public abstract void Init(MenuItem root);
    }
}