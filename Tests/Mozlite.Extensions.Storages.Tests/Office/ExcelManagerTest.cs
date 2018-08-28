using System;
using System.Collections.Generic;
using System.Linq;
using Mozlite.Extensions.Storages.Excels;
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
            var path = "office.xlsx";
            var models = new List<TestModel>
            {
                new TestModel
                {
                    Id = 1,
                    Name = "张三",
                    Money = 10000,
                    Cost = 200,
                    IsPaid = true,
                },
                new TestModel
                {
                    Id = 2,
                    Name = "李四",
                    Money = 20000,
                    Cost = 2100
                },
                new TestModel
                {
                    Id = 3,
                    Name = "王五",
                    Money = 8000,
                    Cost = 100
                },
            };
            _excelManager.Save(models, path);
            var data = _excelManager.Load<TestModel>(path);
            Assert.Equal(3, data.Count());
            var paid = data.SingleOrDefault(x => x.IsPaid);
            Assert.Equal(1, paid.Id);
        }
    }

    [ExcelSheet("test")]
    public class TestModel
    {
        [Excel("编码", Index = 1)]
        [ExcelColumn(Horizontal = HorizontalAlignment.Center)]
        public int Id { get; set; }

        [Excel("名称", Index = 2)]
        [ExcelColumn(Color = "#00ffff")]
        public string Name { get; set; }

        [ExcelColumn(Format = "yyyy年MM月dd日 HH:mm:ss")]
        [Excel("添加日期", Index = 6)]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [ExcelColumn(Format = "￥#,##0.00;￥\\-#,##0.00")]
        [Excel("金额", Index = 3)]
        public decimal Money { get; set; }

        [Excel("是否付款", Index = 5)]
        [ExcelColumn(Horizontal = HorizontalAlignment.Right)]
        public bool IsPaid { get; set; }

        [ExcelColumn(Format = "#,##0.00;\\-#,##0.00")]
        [Excel("人工成本", Index = 4)]
        public double Cost { get; set; }
    }
}