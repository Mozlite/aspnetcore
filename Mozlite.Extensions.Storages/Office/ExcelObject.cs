using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Mozlite.Extensions.Storages.Office
{
    /// <summary>
    /// Excel数据。
    /// </summary>
    /// <typeparam name="TModel">实例类型。</typeparam>
    public class ExcelObject<TModel> : IEnumerable<TModel>
        where TModel : class, new()
    {
        /// <summary>
        /// 工作表名称。
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// 当前实体实例接口。
        /// </summary>
        public IEntityType EntityType { get; }

        /// <summary>
        /// 初始化类<see cref="ExcelObject{TModel}"/>。
        /// </summary>
        public ExcelObject()
        {
            var excelAttribute = typeof(TModel).GetCustomAttribute<ExcelAttribute>();
            if (excelAttribute == null)
                throw new Exception($"当前类型{typeof(TModel)}没有{typeof(ExcelAttribute)}特性。");
            Types = excelAttribute.Types;
            Names = excelAttribute.Names;
            ColumnNames = (string[])Names.Clone();
            EntityType = typeof(TModel).GetEntityType();
            for (int i = 0; i < Names.Length; i++)
            {
                var property = EntityType.FindProperty(Names[i]).PropertyInfo;
                var column = property.GetCustomAttribute<ExcelColumnAttribute>();
                if (column != null && !string.IsNullOrWhiteSpace(column.ColumnName))
                {
                    ColumnNames[i] = column.ColumnName.Trim();
                }
            }
        }

        /// <summary>
        /// 获取定义的样式。
        /// </summary>
        /// <returns>返回定义的单元格样式。</returns>
        public Stylesheet GetDefinedStylesheet()
        {
            var style = new Stylesheet();
            style.Fonts = new Fonts();
            style.CellFormats = new CellFormats();
            style.NumberingFormats = new NumberingFormats();
            uint index = 0, fontIndex = 1, formatId = 0;
            foreach (var name in Names)
            {
                index++;
                var cellFormat = new CellFormat();
                cellFormat.FormatId = formatId++;
                style.CellFormats.AppendChild(cellFormat);
                var property = EntityType.FindProperty(name).PropertyInfo;
                var attribute = property.GetCustomAttribute<ExcelStyleAttribute>();
                if (attribute != null)
                {
                    Font font = attribute;
                    style.Fonts.AppendChild(font);
                    cellFormat.FontId = fontIndex++;
                    if (attribute.HorizontalAlignment != HorizontalAlignment.General || attribute.VerticalAlignment != VerticalAlignment.None)
                    {
                        var alignment = new Alignment();
                        if (attribute.HorizontalAlignment != HorizontalAlignment.General)
                            alignment.Horizontal = (HorizontalAlignmentValues)(int)attribute.HorizontalAlignment;
                        if (attribute.VerticalAlignment != VerticalAlignment.None)
                            alignment.Vertical = (VerticalAlignmentValues)(int)attribute.VerticalAlignment;
                        cellFormat.Alignment = alignment;
                        cellFormat.ApplyAlignment = true;
                    }
                    cellFormat.ApplyFont = true;
                }

                var column = property.GetCustomAttribute<ExcelColumnAttribute>();
                if (column != null)
                {
                    var format = new NumberingFormat();
                    format.FormatCode = column.Format;
                    format.NumberFormatId = index;
                    cellFormat.NumberFormatId = index;
                    cellFormat.ApplyNumberFormat = true;
                    style.NumberingFormats.AppendChild(format);
                }
            }

            var modelAttribute = typeof(TModel).GetCustomAttribute<ExcelStyleAttribute>();
            if (modelAttribute != null)
            {
                Font font = modelAttribute;
                style.Fonts.AppendChild(font);
                var headerCellFormat = new CellFormat();
                headerCellFormat.ApplyFont = true;
                style.CellFormats.AppendChild(headerCellFormat);
                headerCellFormat.FontId = fontIndex;
                headerCellFormat.FormatId = formatId;
            }

            return style;
        }

        /// <summary>
        /// 字段类型：i忽略，n数值，s字符串，f浮点值，d日期，m为Decimal, b布尔。
        /// </summary>
        public char[] Types { get; }

        /// <summary>
        /// 属性名称。
        /// </summary>
        public string[] Names { get; }

        /// <summary>
        /// 字段名称。
        /// </summary>
        public string[] ColumnNames { get; }

        private readonly IList<TModel> _models = new List<TModel>();

        /// <summary>
        /// 获取迭代器。
        /// </summary>
        /// <returns>返回当前迭代实例。</returns>
        public IEnumerator<TModel> GetEnumerator()
        {
            return _models.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 添加模型实例。
        /// </summary>
        /// <param name="model">当前模型实例。</param>
        public void Add(TModel model) => _models.Add(model);

        /// <summary>
        /// 行数。
        /// </summary>
        public int Rows => _models.Count;

        /// <summary>
        /// 列数。
        /// </summary>
        public int Columns => Names.Length;
    }
}