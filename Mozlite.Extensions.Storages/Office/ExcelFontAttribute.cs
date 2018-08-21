using DocumentFormat.OpenXml.Spreadsheet;
using System;

namespace Mozlite.Extensions.Storages.Office
{
    /// <summary>
    /// Excel导出字体特性，如果是Class则表示第一行头部样式。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class ExcelStyleAttribute : Attribute
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
        public VerticalTextAlignment VerticalTextAlignment { get; set; }

        /// <summary>
        /// 字体名称。
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// 横向对齐方式。
        /// </summary>
        public HorizontalAlignment HorizontalAlignment { get; set; }

        /// <summary>
        /// 垂直对齐方式。
        /// </summary>
        public VerticalAlignment VerticalAlignment { get; set; }

        /// <summary>
        /// 隐式转换为Excel字体。
        /// </summary>
        /// <param name="current">当前特性实例。</param>

        public static implicit operator Font(ExcelStyleAttribute current)
        {
            var font = new Font();
            if (current.FontSize > 0)
                font.FontSize = new FontSize { Val = current.FontSize };
            if (!string.IsNullOrWhiteSpace(current.Color))
                font.Color = new Color { Rgb = current.Color.TrimStart('#', ' ') };
            if (current.Bold)
                font.Bold = new Bold();
            if (current.Italic)
                font.Italic = new Italic();
            if (current.Outline)
                font.Outline = new Outline();
            if (current.Underline)
                font.Underline = new Underline();
            if (current.Shadow)
                font.Shadow = new Shadow();
            if (current.Strike)
                font.Strike = new Strike();
            if (current.VerticalTextAlignment != VerticalTextAlignment.None)
                font.VerticalTextAlignment = new DocumentFormat.OpenXml.Spreadsheet.VerticalTextAlignment { Val = (VerticalAlignmentRunValues)(int)current.VerticalTextAlignment };
            if (!string.IsNullOrWhiteSpace(current.FontName))
                font.FontName = new FontName { Val = current.FontName.Trim() };
            return font;
        }
    }
}