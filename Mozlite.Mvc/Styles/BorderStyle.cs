using System;
using System.Text.RegularExpressions;

namespace Mozlite.Mvc.Styles
{
    /// <summary>
    /// 边框样式。
    /// </summary>
    public class BorderStyle
    {
        /// <summary>
        /// 宽。
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// 样式。
        /// </summary>
        public BorderType Style { get; set; }

        /// <summary>
        /// 颜色。
        /// </summary>
        public string Color { get; set; }

        private static readonly Regex _regex = new Regex("^\\d+");
        /// <summary>
        /// 初始化类<see cref="BorderStyle"/>。
        /// </summary>
        /// <param name="borderStyle">样式字符串。</param>
        protected BorderStyle(string borderStyle)
        {
            if (string.IsNullOrEmpty(borderStyle))
                return;
            var styles = borderStyle.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (styles.Length != 3)
                throw new Exception("边框样式配置错误，必须为：宽度 样式 颜色！");
            var isStyled = false;
            for (var i = 0; i < 3; i++)
            {
                var style = styles[i];
                if (!isStyled && Enum.TryParse(style, true, out BorderType current))
                {
                    isStyled = true;
                    Style = current;
                    continue;
                }
                if (Width == null && _regex.IsMatch(style))
                {
                    Width = style;
                    continue;
                }
                Color = style;
            }
        }

        /// <summary>
        /// 隐式转换。
        /// </summary>
        /// <param name="border">样式字符串。</param>
        public static implicit operator BorderStyle(string border)=>new BorderStyle(border);

        /// <summary>
        /// 返回样式。
        /// </summary>
        public override string ToString()
        {
            return $"{Width} {Style.ToString().ToLower()} {Color};";
        }
    }
}