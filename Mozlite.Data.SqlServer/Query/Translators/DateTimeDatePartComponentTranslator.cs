using System;
using System.Linq.Expressions;
using Mozlite.Data.Query.Translators;
using Mozlite.Data.SqlServer.Query.Expressions;

namespace Mozlite.Data.SqlServer.Query.Translators
{
    /// <summary>
    /// 日期部分转换器。
    /// </summary>
    public class DateTimeDatePartComponentTranslator : IMemberTranslator
    {
        /// <summary>
        /// 转换字段或属性表达式。
        /// </summary>
        /// <param name="memberExpression">转换字段或属性表达式。</param>
        /// <returns>转换后的表达式。</returns>
        public virtual Expression Translate(MemberExpression memberExpression)
        {
            string datePart;
            if (memberExpression.Expression != null
                && memberExpression.Expression.Type == typeof(DateTime)
                && (datePart = GetDatePart(memberExpression.Member.Name)) != null)
            {
                return new DatePartExpression(datePart,
                    memberExpression.Type,
                    memberExpression.Expression);
            }
            return null;
        }

        private static string GetDatePart(string memberName)
        {
            switch (memberName)
            {
                case nameof(DateTime.Year):
                    return "year";
                case nameof(DateTime.Month):
                    return "month";
                case nameof(DateTime.DayOfYear):
                    return "dayofyear";
                case nameof(DateTime.Day):
                    return "day";
                case nameof(DateTime.Hour):
                    return "hour";
                case nameof(DateTime.Minute):
                    return "minute";
                case nameof(DateTime.Second):
                    return "second";
                case nameof(DateTime.Millisecond):
                    return "millisecond";
                default:
                    return null;
            }
        }
    }
}