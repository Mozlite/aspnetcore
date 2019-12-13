namespace Mozlite
{
    /// <summary>
    /// 服务配置接口。
    /// </summary>
    public interface IServiceConfigurer : IService
    {
        /// <summary>
        /// 配置服务方法。
        /// </summary>
        /// <param name="builder">容器构建实例。</param>
        void ConfigureServices(IMozliteBuilder builder);
    }
}
