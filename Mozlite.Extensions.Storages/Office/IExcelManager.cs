using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Mozlite.Extensions.Storages.Office
{
    /// <summary>
    /// EXCEL管理接口。
    /// </summary>
    public interface IExcelManager : ISingletonService
    {
        /// <summary>
        /// 加载Excel文件。
        /// </summary>
        /// <param name="path">Excel文件物理路径。</param>
        /// <returns>返回Excel内容。</returns>
        ExcelObject<TModel> Load<TModel>(string path)
            where TModel : class, new();

        /// <summary>
        /// 将对象保存到文件中。
        /// </summary>
        /// <typeparam name="TModel">模型列表类型。</typeparam>
        /// <param name="models">模型列表实例。</param>
        /// <param name="path">路径。</param>
        void Save<TModel>(ExcelObject<TModel> models, string path)
            where TModel : class, new();
    }

    /// <summary>
    /// EXCEL管理类。
    /// </summary>
    public class ExcelManager : IExcelManager
    {
        /// <summary>
        /// 加载Excel文件。
        /// </summary>
        /// <param name="path">Excel文件物理路径。</param>
        /// <returns>返回Excel内容。</returns>
        public ExcelObject<TModel> Load<TModel>(string path) where TModel : class, new()
        {
            var model = new ExcelObject<TModel>();
            using (var document = SpreadsheetDocument.Open(path, false))
            {
                if (1 != document.WorkbookPart.WorksheetParts.Count())
                    throw new Exception("Excel中只能有一个工作表。");
                var shared = document.WorkbookPart.SharedStringTablePart.SharedStringTable;
                var sheet = document.WorkbookPart.WorksheetParts.Single().Worksheet;
                var data = sheet.GetFirstChild<SheetData>();
                model.SheetName = sheet.LocalName;
                Load(model, shared, data);
                return model;
            }
        }

        private List<OpenXmlAttribute> NewRowOpenXmlAttribute(ref int index, int spans)
        {
            index++;
            var attributes = new List<OpenXmlAttribute>();
            attributes.Add(new OpenXmlAttribute("r", null, index.ToString()));
            attributes.Add(new OpenXmlAttribute("spans", null, $"1:{spans}"));
            return attributes;
        }

        private List<OpenXmlAttribute> NewCellOpenXmlAttribute(int index, int column, Action<List<OpenXmlAttribute>> action = null)
        {
            var attributes = new List<OpenXmlAttribute>();
            attributes.Add(new OpenXmlAttribute("r", null, $"{(char)('A' + column)}{index}"));
            action?.Invoke(attributes);
            return attributes;
        }

        /// <summary>
        /// 将对象保存到文件中。
        /// </summary>
        /// <typeparam name="TModel">模型列表类型。</typeparam>
        /// <param name="models">模型列表实例。</param>
        /// <param name="path">路径。</param>
        public void Save<TModel>(ExcelObject<TModel> models, string path) where TModel : class, new()
        {
            using (var document = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
            {
                var index = 0;
                var workbookPart = document.AddWorkbookPart();
                var workSheetPart = workbookPart.AddNewPart<WorksheetPart>();
                //工作表
                var writer = OpenXmlWriter.Create(workSheetPart);
                writer.WriteStartElement(new Worksheet());
                writer.WriteStartElement(new SheetData());
                //第一行标题
                var getters = new Dictionary<int, Func<object, object>>();
                writer.WriteStartElement(new Row(), NewRowOpenXmlAttribute(ref index, models.Columns));
                for (int i = 0; i < models.Columns; i++)
                {
                    getters.Add(i, models.EntityType.FindProperty(models.Names[i]).Get);
                    writer.WriteStartElement(new Cell { StyleIndex = (uint)models.Columns + 1 }, NewCellOpenXmlAttribute(index, i, x => x.Add(new OpenXmlAttribute("t", null, "str"))));
                    writer.WriteElement(new CellValue(models.ColumnNames[i]));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                //写入数据
                foreach (var model in models)
                {
                    writer.WriteStartElement(new Row(), NewRowOpenXmlAttribute(ref index, models.Columns));
                    for (int i = 0; i < models.Columns; i++)
                    {
                        var value = getters[i](model);
                        var type = "";
                        if (value is DateTime date)
                            value = date.ToOADate();
                        else if (value is DateTimeOffset dateTimeOffset)
                            value = dateTimeOffset.DateTime.ToOADate();
                        else if (value is bool bValue)
                        {
                            value = bValue ? 1 : 0;
                            type = "b";
                        }
                        else if (!value.GetType().IsValueType)
                            type = "str";

                        writer.WriteStartElement(new Cell { StyleIndex = UInt32Value.FromUInt32((uint)i) }, NewCellOpenXmlAttribute(index, i, x =>
                          {
                              x.Add(new OpenXmlAttribute("s", null, i.ToString()));
                              if (type.Length > 0)
                                  x.Add(new OpenXmlAttribute("t", null, type));
                          }));
                        writer.WriteElement(new CellValue(value.ToString()));
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.Close();
                //工作区
                writer = OpenXmlWriter.Create(document.WorkbookPart);
                writer.WriteStartElement(new Workbook());
                writer.WriteStartElement(new Sheets());
                writer.WriteElement(new Sheet
                {
                    Name = models.SheetName ?? "sheet1",
                    SheetId = 1,
                    Id = document.WorkbookPart.GetIdOfPart(workSheetPart)
                });
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.Close();
                //样式
                var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                var stylesheet = models.GetDefinedStylesheet();
                stylesheet.Save(stylesPart);
            }
        }

        private void Load<TModel>(ExcelObject<TModel> model, SharedStringTable shared, SheetData sheet) where TModel : class, new()
        {
            var isFirst = true;
            foreach (var row in sheet.Descendants<Row>())
            {
                if (isFirst)
                {//第一行，描述
                    LoadColumnNames(model, row, shared);
                    isFirst = false;
                }
                else
                {
                    LoadModel(model, row, shared);
                }
            }
        }

        private static void LoadColumnNames<TModel>(ExcelObject<TModel> model, Row row, SharedStringTable shared) where TModel : class, new()
        {
            var cells = row.Descendants<Cell>().ToList();
            if (cells.Count != model.Types.Length)
                throw new Exception("当前列数和配置的类型数量不一致。");
            var index = 0;//获取名称配置
            for (int i = 0; i < cells.Count; i++)
            {
                if (model.Types[i] == 'i')
                    continue;

                var value = GetCellValue(cells[i], shared);
                if (!string.IsNullOrEmpty(value))
                    model.ColumnNames[index] = value;
                index++;
            }
        }

        private static void LoadModel<TModel>(ExcelObject<TModel> model, Row row, SharedStringTable shared) where TModel : class, new()
        {
            var cells = row.Descendants<Cell>().ToList();
            var current = new TModel();
            var index = 0;//获取名称配置
            for (int i = 0; i < cells.Count; i++)
            {
                if (model.Types[i] == 'i')
                    continue;

                var value = GetCellValue(model.Types[i], cells[i], shared);
                if (value != null)
                    model.EntityType.FindProperty(model.Names[index]).Set(current, value);
                index++;
            }
            model.Add(current);
        }

        /// <summary>
        /// 获取当前值。
        /// </summary>
        /// <param name="type">配置类型。</param>
        /// <param name="cell">当前单元格。</param>
        /// <param name="shared">共享字符串表。</param>
        /// <returns>返回当前值。</returns>
        private static object GetCellValue(char type, Cell cell, SharedStringTable shared)
        {
            var value = GetCellValue(cell, shared);
            return _converters[type](value);
        }

        private static readonly ConcurrentDictionary<char, Func<string, object>> _converters = new ConcurrentDictionary<char, Func<string, object>>(new Dictionary<char, Func<string, object>>
        {
            ['n'] = v => Convert.ToInt32(v),
            ['f'] = v => Convert.ToDouble(v),
            ['d'] = v => DateTime.FromOADate(Convert.ToDouble(v)),
            ['m'] = v => Convert.ToDecimal(v),
            ['b'] = v =>
            {
                if (v == "1") return true;
                if (v == "0") return false;
                return Convert.ToBoolean(v);
            },
            ['s'] = v => v.ToString(),
        });

        /// <summary>
        /// 获取当前值。
        /// </summary>
        /// <param name="cell">当前单元格。</param>
        /// <param name="shared">共享字符串表。</param>
        /// <returns>返回当前值。</returns>
        private static string GetCellValue(Cell cell, SharedStringTable shared)
        {
            if (cell.ChildElements.Count == 0)
                return null;

            var value = cell.CellValue.InnerText;
            if (cell.DataType != null && cell.DataType == CellValues.SharedString)
            {
                if (int.TryParse(value, out var index))
                    value = shared.ChildElements[index].InnerText;
                else
                    value = null;
            }

            return value;
        }
    }
}