using Mozlite.Data.Migrations;
using Mozlite.Data.Migrations.Builders;
using Mozlite.Extensions.Security.DisallowNames;
using Mozlite.Extensions.Security.Permissions;

namespace Mozlite.Extensions.Security.Stores
{
    /// <summary>
    /// 数据库迁移。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登入类型。</typeparam>
    /// <typeparam name="TUserToken">用户标识类型。</typeparam>
    public abstract class StoreDataMigration<TUser, TUserClaim, TUserLogin, TUserToken> : DataMigration
        where TUser : UserBase, new()
        where TUserClaim : UserClaimBase, new()
        where TUserLogin : UserLoginBase, new()
        where TUserToken : UserTokenBase, new()
    {
        /// <summary>
        /// 创建操作。
        /// </summary>
        /// <param name="builder">迁移构建实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            //禁用名称。
            builder.CreateTable<DisallowName>(table => table
                .Column(x => x.Id)
                .Column(x => x.Name)
                .UniqueConstraint(x => x.Name));
            //用户
            builder.CreateTable<TUser>(table =>
            {
                table.Column(x => x.UserId)
                    .Column(x => x.UserName, nullable: false)
                    .Column(x => x.NormalizedUserName, nullable: false)
                    .Column(x => x.PasswordHash, nullable: false)
                    .Column(x => x.Email)
                    .Column(x => x.NormalizedEmail)
                    .Column(x => x.EmailConfirmed)
                    .Column(x => x.SecurityStamp)
                    .Column(x => x.PhoneNumber)
                    .Column(x => x.PhoneNumberConfirmed)
                    .Column(x => x.TwoFactorEnabled)
                    .Column(x => x.LockoutEnd)
                    .Column(x => x.LockoutEnabled)
                    .Column(x => x.AccessFailedCount)
                    .Column(x => x.ConcurrencyStamp)
                    .Column(x => x.CreatedIP)
                    .Column(x => x.CreatedDate)
                    .Column(x => x.UpdatedDate)
                    .Column(x => x.Avatar)
                    .Column(x => x.RoleId, defaultValue: 0)
                    .Column(x => x.RoleName)
                    .Column(x => x.LoginIP)
                    .Column(x => x.LastLoginDate)
                    .Column(x => x.Action);
                Create(table);
            });

            builder.CreateTable<TUserClaim>(table => table
                .Column(x => x.Id)
                .Column(x => x.ClaimType, nullable: false)
                .Column(x => x.ClaimValue)
                .Column(x => x.UserId)
                .ForeignKey<TUser>(x => x.UserId, onDelete: ReferentialAction.Cascade));

            builder.CreateTable<TUserLogin>(table => table
                .Column(x => x.LoginProvider, nullable: false)
                .Column(x => x.ProviderKey, nullable: false)
                .Column(x => x.ProviderDisplayName)
                .Column(x => x.UserId)
                .ForeignKey<TUser>(x => x.UserId, onDelete: ReferentialAction.Cascade));

            builder.CreateTable<TUserToken>(table => table
                .Column(x => x.LoginProvider, nullable: false)
                .Column(x => x.Name, nullable: false)
                .Column(x => x.Value)
                .Column(x => x.UserId)
                .ForeignKey<TUser>(x => x.UserId, onDelete: ReferentialAction.Cascade));
        }

        /// <summary>
        /// 建索引。
        /// </summary>
        /// <param name="builder">构建实例。</param>
        protected virtual void CreateIndex(MigrationBuilder builder)
        {
            //索引。
            builder.CreateIndex<TUser>(x => x.NormalizedUserName, true);
            builder.CreateIndex<TUser>(x => x.NormalizedEmail);
            builder.CreateIndex<TUser>(x => x.RoleId);
        }

        /// <summary>
        /// 建立索引。
        /// </summary>
        /// <param name="builder">数据迁移构建实例。</param>
        public void Up1(MigrationBuilder builder)
        {
            CreateIndex(builder);
        }

        /// <summary>
        /// 添加用户定义列。
        /// </summary>
        /// <param name="builder">用户表格定义实例。</param>
        protected virtual void Create(CreateTableBuilder<TUser> builder) { }
    }

    /// <summary>
    /// 数据库迁移。
    /// </summary>
    /// <typeparam name="TUser">用户类型。</typeparam>
    /// <typeparam name="TRole">角色类型。</typeparam>
    /// <typeparam name="TUserClaim">用户声明类型。</typeparam>
    /// <typeparam name="TUserLogin">用户登入类型。</typeparam>
    /// <typeparam name="TUserRole">用户所在组类型。</typeparam>
    /// <typeparam name="TRoleClaim">角色声明类型。</typeparam>
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
            //角色
            builder.CreateTable<TRole>(table => table
                .Column(x => x.RoleId)
                .Column(x => x.Name, nullable: false)
                .Column(x => x.NormalizedName, nullable: false)
                .Column(x => x.RoleLevel));

            //判断TUserRole是否单独一个表格，也可以把这个表格合并到TUser中，每一个用户只是应对一个角色
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
            base.CreateIndex(builder);
            builder.CreateIndex<TRole>(x => x.NormalizedName, true);
        }
    }
}