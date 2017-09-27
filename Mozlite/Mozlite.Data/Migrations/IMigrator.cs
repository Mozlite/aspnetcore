using System.Threading.Tasks;

namespace Mozlite.Data.Migrations
{
    /// <summary>
    /// 数据迁移接口。
    /// </summary>
    public interface IMigrator
    {
        /// <summary>
        /// 迁移数据。
        /// </summary>
        /// <param name="version">版本：0表示安装到最新，-1卸载！</param>
        Task MigrateAsync(int version = 0);
    }
}