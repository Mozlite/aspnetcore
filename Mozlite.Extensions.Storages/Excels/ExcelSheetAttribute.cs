using System;

namespace Mozlite.Extensions.Storages.Excels
{
    /// <summary>
    /// 工作表名称。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExcelSheetAttribute : Attribute
    {
        /// <summary>
        /// 初始化类<see cref="ExcelSheetAttribute"/>。
        /// </summary>
        /// <param name="sheetName">工作表名称。</param>
        public ExcelSheetAttribute(string sheetName = "sheet1")
        {
            SheetName = sheetName;
        }

        /// <summary>
        /// 工作表名称。
        /// </summary>
        public string SheetName { get; }
    }
}