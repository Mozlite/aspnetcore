using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Mozlite.DataService.SqlServer
{
    /// <summary>
    /// 数据库相关的服务器扩展。
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 添加SQLServer数据库服务。
        /// </summary>
        /// <param name="services">服务集合。</param>
        /// <param name="configuration">配置实例。</param>
        /// <returns>返回服务集合实例。</returns>
        public static IServiceCollection AddSqlServer(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddSqlServer(options =>
            {
                var section = configuration.GetSection("Data");
                foreach (var current in section.GetChildren())
                {
                    switch (current.Key.ToLower())
                    {
                        case "name":
                            options.ConnectionString = $"Data Source=.;Initial Catalog={current.Value};Integrated Security=True;";
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
        /// <param name="services">构建服务实例。</param>
        /// <param name="options">数据源选项。</param>
        /// <returns>返回服务集合实例。</returns>
        public static IServiceCollection AddSqlServer(this IServiceCollection services, Action<DatabaseOptions> options)
        {
            var source = new DatabaseOptions();
            options(source);

            return services.AddSingleton<IDatabase, SqlServerDatabase>()
                .Configure<DatabaseOptions>(o =>
                {
                    o.ConnectionString = source.ConnectionString;
                    o.Prefix = source.Prefix?.Trim();
                    o.Provider = "SqlServer";
                })
                .AddSingleton<ISqlHelper, SqlServerHelper>();
        }
    }
}