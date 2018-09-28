using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Mozlite.Extensions.Storages.Excels
{
    /// <summary>
    /// Excel列描述信息。
    /// </summary>
    public class ExcelColumnDescriptor
    {
        /// <summary>
        /// 初始化类<see cref="ExcelColumnDescriptor"/>。
        /// </summary>
        /// <param name="property">属性接口。</param>
        public ExcelColumnDescriptor(IProperty property)
        {
            Get = property.Get;
            var type = property.ClrType.UnwrapNullableType();
            if (_converters.TryGetValue(type, out var convert))
                Set = (instance, value) => property.Set(instance, convert(value));
            else if (type.IsEnum)
                Set = (instance, value) => property.Set(instance, EnumConverter(value, type));
            else
                throw new Exception($"暂时不支持导入属性{property.Name}的类型：{property.ClrType}！");
            var info = property.PropertyInfo;
            var headAttribute = info.GetCustomAttribute<ExcelAttribute>();
            HeadCellFormat = new CellFormat();
            HeadFont = headAttribute.ToExcelFont();
            AppendAlignment(HeadCellFormat, headAttribute);
            ColumnName = headAttribute.Name?.Trim() ?? property.Name;
            Index = headAttribute.Index;
            Reference = headAttribute.Reference;

            CellFormat = new CellFormat();
            var attribute = info.GetCustomAttribute<ExcelColumnAttribute>();
            if (attribute != null)
            {
                Font = attribute.ToExcelFont();
                AppendAlignment(CellFormat, attribute);
                if (!string.IsNullOrEmpty(attribute.Format))
                {
                    NumberingFormat = new NumberingFormat { FormatCode = attribute.Format };
                }
            }
        }

        /// <summary>
        /// 导出排序，从小到大。
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 字段名称。
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 列引用，如：A,B,C,D,E...，不包含行号。
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// 字体样式。
        /// </summary>
        public Font Font { get; }

        /// <summary>
        /// 头部字体样式。
        /// </summary>
        public Font HeadFont { get; }

        /// <summary>
        /// 获取当前属性值方法。
        /// </summary>
        public Func<object, object> Get { get; }

        /// <summary>
        /// 设置当前属性值方法。
        /// </summary>
        public Action<object, string> Set { get; }

        /// <summary>
        /// 字段样式。
        /// </summary>
        public CellFormat CellFormat { get; }

        /// <summary>
        /// 头部字段样式。
        /// </summary>
        public CellFormat HeadCellFormat { get; }

        /// <summary>
        /// 数值格式化。
        /// </summary>
        public NumberingFormat NumberingFormat { get; }

        private void AppendAlignment(CellFormat format, IAlignment alignment)
        {
            if (alignment.Horizontal != HorizontalAlignment.General || alignment.Vertical != VerticalAlignment.None)
            {
                var currentAlignment = new Alignment();
                if (alignment.Horizontal != HorizontalAlignment.General)
                    currentAlignment.Horizontal = (HorizontalAlignmentValues)(int)alignment.Horizontal;
                if (alignment.Vertical != VerticalAlignment.None)
                    currentAlignment.Vertical = (VerticalAlignmentValues)(int)alignment.Vertical;
                format.Alignment = currentAlignment;
                format.ApplyAlignment = true;
            }
        }

        private static readonly ConcurrentDictionary<Type, Func<string, object>> _converters = new ConcurrentDictionary<Type, Func<string, object>>(new Dictionary<Type, Func<string, object>>
        {
            [typeof(short)] = v => Convert.ToInt16(v),
            [typeof(int)] = v => Convert.ToInt32(v),
            [typeof(long)] = v => Convert.ToInt64(v),
            [typeof(float)] = v => Convert.ToDouble(v),
            [typeof(double)] = v => Convert.ToDouble(v),
            [typeof(Guid)] = v => Guid.Parse(v),
            [typeof(DateTime)] = v => DateTime.FromOADate(Convert.ToDouble(v)),
            [typeof(DateTimeOffset)] = v => DateTime.FromOADate(Convert.ToDouble(v)),
            [typeof(decimal)] = v => Convert.ToDecimal(v),
            [typeof(bool)] = v =>
            {
                if (v == "1") return true;
                if (v == "0") return false;
                return Convert.ToBoolean(v);
            },
            [typeof(string)] = v => v,
        });

        private static object EnumConverter(string value, Type type)
        {
            try
            {
                return Enum.Parse(type, value, true);
            }
            catch
            {
                return 0;
            }
        }
    }
}