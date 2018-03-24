using System.Collections.Generic;
using Mozlite.Data.Query.Translators;

namespace Mozlite.Data.MySql.Query.Translators
{
    /// <summary>
    /// SQLServer的字段或属性表达式转换器。
    /// </summary>
    public class MySqlCompositeMemberTranslator : RelationalCompositeMemberTranslator
    {
        /// <summary>
        /// 初始化类<see cref="MySqlCompositeMemberTranslator"/>。
        /// </summary>
        public MySqlCompositeMemberTranslator()
        {
            var sqlServerTranslators = new List<IMemberTranslator>
            {
                new StringLengthTranslator(),
                new DateTimeNowTranslator(),
                new DateTimeDateComponentTranslator(),
                new DateTimeDatePartComponentTranslator(),
            };

            AddTranslators(sqlServerTranslators);
        }
    }
}