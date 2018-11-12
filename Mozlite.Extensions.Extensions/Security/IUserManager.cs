using Mozlite.Extensions.Extensions.Security.Stores;

namespace Mozlite.Extensions.Extensions.Security
{
    /// <summary>
    /// 用户管理接口。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    public interface IUserManager<TUser>
        : Mozlite.Extensions.Security.IUserManager<TUser>
        where TUser : UserBase, new()
    {

    }

    /// <summary>
    /// 用户管理接口。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TRole">用户组类型。</typeparam>
    public interface IUserManager<TUser, TRole>
        : Mozlite.Extensions.Security.IUserManager<TUser, TRole>
        where TUser : UserBase, new()
    {

    }
}