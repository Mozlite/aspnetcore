using System;
using Mozlite.Extensions.Groups;

namespace Mozlite.Extensions.Documents
{
    /// <summary>
    /// 分类管理。
    /// </summary>
    public interface ICategoryManager:IGroupManager<Category>,ISingletonService
    {
    }
}
