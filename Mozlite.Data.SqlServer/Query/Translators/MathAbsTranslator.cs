using System;
using Mozlite.Data.Query.Translators;

namespace Mozlite.Data.SqlServer.Query.Translators
{
    /// <summary>
    /// Math.Abs转换表达式。
    /// </summary>
    public class MathAbsTranslator : MultipleOverloadStaticMethodCallTranslator
    {
        /// <summary>
        /// 初始化类<see cref="MathAbsTranslator"/>。
        /// </summary>
        public MathAbsTranslator()
            : base(typeof(Math), nameof(Math.Abs), "ABS")
        {
        }
    }
}