using System.Collections.Generic;
using Mozlite.Data.Query.Translators;

namespace Mozlite.Data.SqlServer.Query.Translators
{
    /// <summary>
    /// SQLServer的字段或属性表达式转换器。
    /// </summary>
    public class SqlServerCompositeMemberTranslator : RelationalCompositeMemberTranslator
    {
        /// <summary>
        /// 初始化类<see cref="SqlServerCompositeMemberTranslator"/>。
        /// </summary>
        public SqlServerCompositeMemberTranslator()
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