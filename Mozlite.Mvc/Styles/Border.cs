using System.Text;

namespace Mozlite.Mvc.Styles
{
    /// <summary>
    /// 边框样式。
    /// </summary>
    public class Border : BorderStyle
    {
        private readonly bool _global;
        /// <summary>
        /// 初始化类<see cref="Border"/>。
        /// </summary>
        /// <param name="borderStyle">样式字符串。</param>
        private Border(string borderStyle) : base(borderStyle)
        {
            _global = true;
        }

        /// <summary>
        /// 隐式转换。
        /// </summary>
        /// <param name="border">样式字符串。</param>
        public static implicit operator Border(string border) => new Border(border);
        
        /// <summary>
        /// 顶部边框。
        /// </summary>
        public BorderStyle Top { get; set; }

        /// <summary>
        /// 底部边框。
        /// </summary>
        public BorderStyle Bottom { get; set; }

        /// <summary>
        /// 左边边框。
        /// </summary>
        public BorderStyle Left { get; set; }

        /// <summary>
        /// 右边边框。
        /// </summary>
        public BorderStyle Right { get; set; }

        /// <summary>
        /// 返回样式。
        /// </summary>
        public override string ToString()
        {
            var builder = new StringBuilder();
            if (_global)
                builder.AppendFormat("border:{0}", base.ToString());
            if (Top != null)
                builder.AppendFormat("border-top:{0}", Top);
            if (Bottom != null)
                builder.AppendFormat("border-bottom:{0}", Bottom);
            if (Left != null)
                builder.AppendFormat("border-left:{0}", Left);
            if (Right != null)
                builder.AppendFormat("border-right:{0}", Right);
            return builder.ToString();
        }
    }
}