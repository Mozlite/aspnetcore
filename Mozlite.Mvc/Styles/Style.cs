using System.Text;

namespace Mozlite.Mvc.Styles
{
    /// <summary>
    /// 通用样式。
    /// </summary>
    public class Style
    {
        /// <summary>
        /// 边框样式。
        /// </summary>
        public Border Border { get; set; }

        /// <summary>
        /// 顶部坐标。
        /// </summary>
        public string Top { get; set; }

        /// <summary>
        /// 底部坐标。
        /// </summary>
        public string Bottom { get; set; }

        /// <summary>
        /// 左边坐标。
        /// </summary>
        public string Left { get; set; }

        /// <summary>
        /// 右边坐标。
        /// </summary>
        public string Right { get; set; }

        /// <summary>
        /// 返回样式表。
        /// </summary>
        public override string ToString()
        {
            var builder = new StringBuilder();
            if (Border != null)
                builder.AppendFormat("border:{0}", Border);
            if (Left != null)
                builder.AppendFormat("left:{0}", Left);
            if (Top != null)
                builder.AppendFormat("top:{0}", Top);
            if (Bottom != null)
                builder.AppendFormat("bottom:{0}", Bottom);
            if (Right != null)
                builder.AppendFormat("right:{0}", Right);
            return builder.ToString();
        }
    }
}