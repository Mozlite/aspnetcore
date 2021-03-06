﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Mozlite.Data;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Extensions.Security.Stores
{
    /// <summary>
    /// 数据库存储类。
    /// </summary>
    /// <typeparam name="TRole">用户组类型。</typeparam>
    /// <typeparam name="TUserRole">用户用户组类型。</typeparam>
    /// <typeparam name="TRoleClaim">用户组声明类型。</typeparam>
    public abstract class RoleStoreBase<TRole, TUserRole, TRoleClaim>
        : Mozlite.Extensions.Security.Stores.RoleStoreBase<TRole, TUserRole, TRoleClaim>,
        IRoleStoreBase<TRole, TUserRole, TRoleClaim>
        where TRole : RoleBase
        where TUserRole : IUserRole
        where TRoleClaim : RoleClaimBase, new()
    {
        /// <summary>
        /// 获取当前最大用户组等级。
        /// </summary>
        /// <param name="role">当前用户组实例。</param>
        /// <returns>返回最大用户组等级。</returns>
        protected override int GetMaxRoleLevel(TRole role)
        {
            return RoleContext.Max(x => x.RoleLevel, x => x.SiteId == role.SiteId && x.RoleLevel < int.MaxValue);
        }

        /// <summary>
        /// 获取当前最大用户组等级。
        /// </summary>
        /// <param name="role">当前用户组实例。</param>
        /// <returns>返回最大用户组等级。</returns>
        protected override Task<int> GetMaxRoleLevelAsync(TRole role)
        {
            return RoleContext.MaxAsync(x => x.RoleLevel, x => x.SiteId == role.SiteId && x.RoleLevel < int.MaxValue);
        }
        
        /// <summary>
        /// 下移用户组分组条件表达式。
        /// </summary>
        /// <param name="role">当前用户组。</param>
        /// <returns>返回条件表达式。</returns>
        protected override Expression<Predicate<TRole>> MoveExpression(TRole role)
        {
            return x => x.SiteId == role.SiteId && x.RoleLevel > 0 && x.RoleLevel < int.MaxValue;
        }

        /// <summary>
        /// 初始化类<see cref="RoleStoreBase{TRole,TUserRole,TRoleClaim}"/>。
        /// </summary>
        /// <param name="describer">错误描述<see cref="IdentityErrorDescriber"/>实例。</param>
        /// <param name="roleContext">用户组数据库操作接口。</param>
        /// <param name="userRoleContext">用户用户组数据库操作接口。</param>
        /// <param name="roleClaimContext">用户声明数据库操作接口。</param>
        protected RoleStoreBase(IdentityErrorDescriber describer, IDbContext<TRole> roleContext, IDbContext<TUserRole> userRoleContext, IDbContext<TRoleClaim> roleClaimContext)
            : base(describer, roleContext, userRoleContext, roleClaimContext)
        {
        }

        /// <summary>
        /// 获取所有用户组。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <returns>返回用户组列表。</returns>
        public virtual IEnumerable<TRole> LoadRoles(int siteId)
        {
            return RoleContext.Fetch(x => x.SiteId == siteId);
        }

        /// <summary>
        /// 获取所有用户组。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回用户组列表。</returns>
        public virtual Task<IEnumerable<TRole>> LoadRolesAsync(int siteId, CancellationToken cancellationToken = default)
        {
            return RoleContext.FetchAsync(x => x.SiteId == siteId, cancellationToken);
        }

        /// <summary>
        /// 通过用户组名称获取用户组实例。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="normalizedName">用户组名称。</param>
        /// <returns>返回当前用户组实例对象。</returns>
        public virtual TRole FindByName(int siteId, string normalizedName)
        {
            return RoleContext.Find(x => x.SiteId == siteId && x.NormalizedName == normalizedName);
        }

        /// <summary>
        /// 通过用户组名称获取用户组实例。
        /// </summary>
        /// <param name="siteId">网站Id。</param>
        /// <param name="normalizedName">用户组名称。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回当前用户组实例对象。</returns>
        public virtual Task<TRole> FindByNameAsync(int siteId, string normalizedName,
            CancellationToken cancellationToken = default)
        {
            return RoleContext.FindAsync(x => x.SiteId == siteId && x.NormalizedName == normalizedName, cancellationToken);
        }
    }
}