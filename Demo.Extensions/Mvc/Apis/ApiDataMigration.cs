using Mozlite.Data.Migrations;
using Mozlite.Mvc.Apis;

namespace Demo.Extensions.Mvc
{
    /// <summary>
    /// API数据迁移类。
    /// </summary>
    public class ApiDataMigration : Mozlite.Mvc.Apis.ApiDataMigration
    {
        public void Up1(MigrationBuilder builder)
        {
            builder.SqlCreate(new Application { Name = "我得应用程序" });
        }
    }
}