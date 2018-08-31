using System;
using System.Linq.Expressions;
using Mozlite.Data.Query.Translators;
using Mozlite.Data.MySql.Query.Expressions;

namespace Mozlite.Data.MySql.Query.Translators
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
                    return "%Y";
                case nameof(DateTime.Month):
                    return "%m";
                case nameof(DateTime.DayOfYear):
                    return "%j";
                case nameof(DateTime.Day):
                    return "%d";
                case nameof(DateTime.Hour):
                    return "%H";
                case nameof(DateTime.Minute):
                    return "%i";
                case nameof(DateTime.Second):
                    return "%s";
                case nameof(DateTime.Date):
                    return "%Y-%m-%d";
                case nameof(DateTime.TimeOfDay):
                    return "%H:%i:%s";
                default:
                    return null;
            }
        }
    }
}