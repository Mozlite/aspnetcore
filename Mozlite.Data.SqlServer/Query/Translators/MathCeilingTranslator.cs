using System;
using Mozlite.Data.Query.Translators;

namespace Mozlite.Data.SqlServer.Query.Translators
{
    /// <summary>
    /// Math.Ceiling转换器。
    /// </summary>
    public class MathCeilingTranslator : MultipleOverloadStaticMethodCallTranslator
    {
        /// <summary>
        /// 初始化类<see cref="MathCeilingTranslator"/>。
        /// </summary>
        public MathCeilingTranslator()
            : base(typeof(Math), nameof(Math.Ceiling), "CEILING")
        {
        }
    }
}