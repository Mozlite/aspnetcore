using Mozlite.Data.Migrations;
using Mozlite.Data.Migrations.Builders;
using Mozlite.Extensions.Security.Permissions;
using Mozlite.Extensions.Security.Stores;

namespace Mozlite.Extensions.Extensions.Security.Stores
{
    /// <summary>
    /// 数据库迁移。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登入类型。</typeparam>
    /// <typeparam name="TUserToken">用户标识类型。</typeparam>
    public abstract class StoreDataMigration<TUser, TUserClaim, TUserLogin, TUserToken> : Mozlite.Extensions.Security.Stores.StoreDataMigration<TUser, TUserClaim, TUserLogin, TUserToken>
        where TUser : UserBase, new()
        where TUserClaim : UserClaimBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
    {
        /// <summary>
        /// 添加用户定义列。
        /// </summary>
        /// <param name="builder">用户表格定义实例。</param>
        protected override void Create(CreateTableBuilder<TUser> builder)
        {
            builder.Column(x => x.SiteId);
        }

        /// <summary>
        /// 建索引。
        /// </summary>
        /// <param name="builder">构建实例。</param>
        protected override void CreateIndex(MigrationBuilder builder)
        {
            //索引。
            builder.CreateIndex<TUser>(x => new { x.NormalizedUserName, x.SiteId }, true);
            builder.CreateIndex<TUser>(x => new { x.NormalizedEmail, x.SiteId });
        }
    }

    /// <summary>
    /// 数据库迁移。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TRole">用户组类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登入类型。</typeparam>
    /// <typeparam name="TUserRole">用户所在组类型。</typeparam>
    /// <typeparam name="TRoleClaim">用户组声明类型。</typeparam>
    /// <typeparam name="TUserToken">用户标识类型。</typeparam>
    public abstract class StoreDataMigration<TUser, TRole, TUserClaim, TRoleClaim, TUserLogin, TUserRole, TUserToken> :
        StoreDataMigration<TUser, TUserClaim, TUserLogin, TUserToken>
        where TUser : UserBase, new()
        where TRole : RoleBase, new()
        where TUserClaim : UserClaimBase, new()
        where TRoleClaim : RoleClaimBase, new()
        where TUserRole : IUserRole, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
    {
        /// <summary>
        /// 创建操作。
        /// </summary>
        /// <param name="builder">迁移构建实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            base.Create(builder);

            builder.CreateTable<TRole>(table => table
                .Column(x => x.SiteId)
                .Column(x => x.RoleId)
                .Column(x => x.Name, nullable: false)
                .Column(x => x.NormalizedName, nullable: false)
                .Column(x => x.RoleLevel));

            //判断TUserRole是否单独一个表格，也可以把这个表格合并到TUser中，每一个用户只是应对一个用户组
            if (typeof(UserRoleBase).IsAssignableFrom(typeof(TUserRole)))
                builder.CreateTable<TUserRole>(table => table
                    .Column(x => x.UserId)
                    .Column(x => x.RoleId)
                    .ForeignKey<TUser>(x => x.UserId, onDelete: ReferentialAction.Cascade)
                    .ForeignKey<TRole>(x => x.RoleId, onDelete: ReferentialAction.Cascade));

            builder.CreateTable<TRoleClaim>(table => table
                .Column(x => x.Id)
                .Column(x => x.ClaimType, nullable: false)
                .Column(x => x.ClaimValue)
                .Column(x => x.RoleId)
                .ForeignKey<TRole>(x => x.RoleId, onDelete: ReferentialAction.Cascade));

            //权限
            builder.CreateTable<Permission>(table => table
                .Column(x => x.Id)
                .Column(x => x.Category)
                .Column(x => x.Name)
                .Column(x => x.Text)
                .Column(x => x.Description)
                .Column(x => x.Order)
                .UniqueConstraint(x => new { x.Category, x.Name }));

            builder.CreateTable<PermissionInRole>(table => table
                .Column(x => x.PermissionId)
                .Column(x => x.RoleId)
                .Column(x => x.Value)
                .ForeignKey<Permission>(x => x.PermissionId, x => x.Id, onDelete: ReferentialAction.Cascade));
        }

        /// <summary>
        /// 建索引。
        /// </summary>
        /// <param name="builder">构建实例。</param>
        protected override void CreateIndex(MigrationBuilder builder)
        {
            //索引。
            base.CreateIndex(builder);
            builder.CreateIndex<TRole>(x => new { x.NormalizedName, x.SiteId }, true);
        }
    }
}