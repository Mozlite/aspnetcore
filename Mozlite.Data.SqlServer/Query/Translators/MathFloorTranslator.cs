using System;
using Mozlite.Data.Query.Translators;

namespace Mozlite.Data.SqlServer.Query.Translators
{
    /// <summary>
    /// Math.Floor转换器。
    /// </summary>
    public class MathFloorTranslator : MultipleOverloadStaticMethodCallTranslator
    {
        /// <summary>
        /// 初始化类<see cref="MathFloorTranslator"/>。
        /// </summary>
        public MathFloorTranslator()
            : base(typeof(Math), nameof(Math.Floor), "FLOOR")
        {
        }
    }
}