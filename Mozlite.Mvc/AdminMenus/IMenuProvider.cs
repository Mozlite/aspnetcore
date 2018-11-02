namespace Mozlite.Mvc.AdminMenus
{
    /// <summary>
    /// 菜单提供者接口。
    /// </summary>
    public interface IMenuProvider : IServices
    {
        /// <summary>
        /// 提供者名称，同一个名称归为同一个菜单。
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// 初始化菜单实例。
        /// </summary>
        /// <param name="root">根目录菜单。</param>
        void Init(MenuItem root);
    }
}