using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MozliteDemo
{
    /// <summary>
    /// 启动类。
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 启动方法。
        /// </summary>
        /// <param name="args">参数。</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// 启动网站服务。
        /// </summary>
        /// <param name="args">参数。</param>
        /// <returns>返回服务器容器实例。</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
