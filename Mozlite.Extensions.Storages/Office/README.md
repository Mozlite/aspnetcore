# Excel操作

> 使用`IExcelManager`实现Excel文档的简易导入导出操作。

## Excel工作表格式

Excel文档格式第一行为标题，其他行为数据，并且不允许有空行，否则会出现错误！

```
	标题1	标题2	标题3...
	数据1...
	数据2...
	数据3...
	...
```

## 关联实体类

要进行Excel导入导出，需要对实体类进行标记，使用特性：`ExcelAttribute`(定义标题), `ExcelColumnAttribute`(定义数据字段),`ExcelSheet`(定义工作表名)对相应的属性和类进行标记。关联的属性必须要有`Getter`和`Setter`公有权限。

```csharp
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
```

定义好实体类的特性后，就可以使用API进行简单的Excel操作（导入，导出），主要用于配置参数功能模块。