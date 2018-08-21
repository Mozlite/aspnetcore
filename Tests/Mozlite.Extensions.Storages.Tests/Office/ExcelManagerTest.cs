using Mozlite.Extensions.Storages.Office;
using System;
using Xunit;

namespace Mozlite.Extensions.Storages.Tests.Office
{
    public class ExcelManagerTest
    {
        private readonly IExcelManager _excelManager;
        public ExcelManagerTest()
        {
            _excelManager = Tests.GetRequiredService<IExcelManager>();
        }

        [Fact]
        public void Load()
        {
            var path = "Office/imports.xlsx";
            var models = _excelManager.Load<TestModel>(path);
            Assert.Equal(2, models.Rows);
            Assert.Equal(6, models.Columns);
            path = "exports.xlsx";
            _excelManager.Save(models, path);
        }
    }

    [ExcelStyle(Bold = true, HorizontalAlignment = HorizontalAlignment.Center)]
    [Excel("nsdmbf", "Id", "Name", "CreateDate", "Money", "IsPayed", "Cost")]
    public class TestModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [ExcelColumn("添加日期", "yyyy年MM月dd日")]
        public DateTime CreateDate { get; set; }

        [ExcelColumn("工资", "\"￥\"#,##0.00;\"￥\"\\-#,##0.00")]
        public decimal Money { get; set; }

        public bool IsPayed { get; set; }

        public double Cost { get; set; }
    }
}