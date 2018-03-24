using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Mozlite.Data.Query.Translators;

namespace Mozlite.Data.MySql.Query.Translators
{
    /// <summary>
    /// SQLServer方法调用转换类。
    /// </summary>
    public class MySqlCompositeMethodCallTranslator : RelationalCompositeMethodCallTranslator
    {
        /// <summary>
        /// 初始化类<see cref="MySqlCompositeMethodCallTranslator"/>。
        /// </summary>
        /// <param name="loggerFactory">日志工厂接口。</param>
        public MySqlCompositeMethodCallTranslator( ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            var sqlServerTranslators = new List<IMethodCallTranslator>
            {
                new NewGuidTranslator(),
                new StringSubstringTranslator(),
                new MathAbsTranslator(),
                new MathCeilingTranslator(),
                new MathFloorTranslator(),
                new MathPowerTranslator(),
                new MathRoundTranslator(),
                new MathTruncateTranslator(),
                new StringReplaceTranslator(),
                new StringToLowerTranslator(),
                new StringToUpperTranslator(),
                new ConvertTranslator(),
                new ContainsOptimizedTranslator(),
                new EndsWithOptimizedTranslator(),
                new StartsWithOptimizedTranslator(),
                new StringIsNullOrWhiteSpaceTranslator(),
                new StringTrimStartTranslator(),
                new StringTrimEndTranslator(),
                new StringTrimTranslator(),
            };

            AddTranslators(sqlServerTranslators);
        }
    }
}