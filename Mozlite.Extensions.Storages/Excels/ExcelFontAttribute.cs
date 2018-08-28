using System;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Mozlite.Extensions.Storages.Excels
{
    /// <summary>
    /// Excel导出字体特性。
    /// </summary>
    public abstract class ExcelFontAttribute : Attribute
    {
        /// <summary>
        /// 字体大小。
        /// </summary>
        public int FontSize { get; set; }

        /// <summary>
        /// 字体颜色。
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// 是否加粗。
        /// </summary>
        public bool Bold { get; set; }

        /// <summary>
        /// 斜体。
        /// </summary>
        public bool Italic { get; set; }

        /// <summary>
        /// 删除线。
        /// </summary>
        public bool Strike { get; set; }

        /// <summary>
        /// 轮廓线。
        /// </summary>
        public bool Outline { get; set; }

        /// <summary>
        /// 阴影。
        /// </summary>
        public bool Shadow { get; set; }

        /// <summary>
        /// 下划线。
        /// </summary>
        public bool Underline { get; set; }

        /// <summary>
        /// 文本垂直对齐样式。
        /// </summary>
        public VerticalTextAlignment VerticalTextAlignment { get; set; } = VerticalTextAlignment.None;

        /// <summary>
        /// 字体名称。
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// 是否为空。
        /// </summary>
        private bool IsEmpty => FontSize == 0 && string.IsNullOrEmpty(Color) && !Bold && !Italic && !Strike &&
                               !Outline && !Shadow && !Underline && string.IsNullOrEmpty(FontName) &&
                               VerticalTextAlignment == VerticalTextAlignment.None;

        /// <summary>
        /// 转换为Excel的字体实例。
        /// </summary>
        /// <returns>返回字体实例。</returns>
        public Font ToExcelFont()
        {
            if (IsEmpty) return null;
            var font = new Font();
            if (FontSize > 0)
                font.FontSize = new FontSize { Val = FontSize };
            if (!string.IsNullOrWhiteSpace(Color))
                font.Color = new Color { Rgb = Color.TrimStart('#', ' ') };
            if (Bold)
                font.Bold = new Bold();
            if (Italic)
                font.Italic = new Italic();
            if (Outline)
                font.Outline = new Outline();
            if (Underline)
                font.Underline = new Underline();
            if (Shadow)
                font.Shadow = new Shadow();
            if (Strike)
                font.Strike = new Strike();
            if (VerticalTextAlignment != VerticalTextAlignment.None)
                font.VerticalTextAlignment = new DocumentFormat.OpenXml.Spreadsheet.VerticalTextAlignment { Val = (VerticalAlignmentRunValues)(int)VerticalTextAlignment };
            if (!string.IsNullOrWhiteSpace(FontName))
                font.FontName = new FontName { Val = FontName.Trim() };
            return font;
        }
    }
}