using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mozlite.Data.Internal;
using Mozlite.Data.Migrations;
using Mozlite.Data.Migrations.Models;
using Mozlite.Data.Query;
using Mozlite.Data.Query.Translators;
using Mozlite.Data.MySql.Migrations;
using Mozlite.Data.MySql.Query;
using Mozlite.Data.MySql.Query.Translators;

namespace Mozlite.Data.MySql
{
    /// <summary>
    /// 数据库相关的服务器扩展。
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 添加SQLServer数据库服务。
        /// </summary>
        /// <param name="builder">服务集合。</param>
        /// <param name="configuration">配置实例。</param>
        /// <returns>返回服务集合实例。</returns>
        public static IMozliteBuilder AddMySql(this IMozliteBuilder builder, IConfiguration configuration)
        {
            return builder.AddMySql(options =>
            {
                var section = configuration.GetSection("Data");
                foreach(var current in section.GetChildren()){
                    switch(current.Key.ToLower()){
                        case "name":
                            options.ConnectionString =$"Server=localhost;Database={current.Value};Uid=root;Pwd=;";
                            break;
                        case "connectionstring":
                            options.ConnectionString = current.Value;
                            break;
                        case "prefix":
                            options.Prefix = current.Value;
                            break;
                        default:
                            options[current.Key] = current.Value;
                            break;
                    }
                }
            });
        }

        /// <summary>
        /// 添加SQLServer数据库服务。
        /// </summary>
        /// <param name="builder">构建服务实例。</param>
        /// <param name="options">数据源选项。</param>
        /// <returns>返回服务集合实例。</returns>
        public static IMozliteBuilder AddMySql(this IMozliteBuilder builder, Action<DatabaseOptions> options)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(options, nameof(options));
            var source = new DatabaseOptions();
            options(source);

            return builder.AddServices(services => services
                    .AddSingleton<IDatabase, MySqlDatabase>()
                    .Configure<DatabaseOptions>(o =>
                    {
                        o.ConnectionString = source.ConnectionString;
                        o.Prefix = source.Prefix?.Trim();
                        o.Provider = "MySql";
                    })
                    .AddSingleton(typeof(IDbContext<>), typeof(DbContext<>))
                    .AddTransient<IDataMigrator, DataMigrator>()
                    .AddTransient<IMigrationRepository, MySqlMigrationRepository>()
                    .AddTransient<IMigrationsSqlGenerator, MySqlMigrationsSqlGenerator>()
                    .AddSingleton<IQuerySqlGenerator, MySqlQuerySqlGenerator>()
                    .AddSingleton<ITypeMapper, MySqlTypeMapper>()
                    .AddSingleton<ISqlHelper, MySqlHelper>()
                    .AddSingleton<IMemberTranslator, MySqlCompositeMemberTranslator>()
                    .AddSingleton<IMethodCallTranslator, MySqlCompositeMethodCallTranslator>()
                    .AddSingleton<IExpressionFragmentTranslator, MySqlCompositeExpressionFragmentTranslator>()
                    .AddSingleton<IExpressionVisitorFactory, MySqlExpressionVisitorFactory>());
        }
    }
}