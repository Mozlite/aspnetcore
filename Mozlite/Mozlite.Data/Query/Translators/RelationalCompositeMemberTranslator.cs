using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mozlite.Data.Query.Translators
{
    /// <summary>
    /// 字段或属性表达式转换器。
    /// </summary>
    public abstract class RelationalCompositeMemberTranslator : IMemberTranslator
    {
        private readonly List<IMemberTranslator> _translators = new List<IMemberTranslator>();

        /// <summary>
        /// 转换字段或属性表达式。
        /// </summary>
        /// <param name="expression">转换字段或属性表达式。</param>
        /// <returns>转换后的表达式。</returns>
        public virtual Expression Translate(MemberExpression expression)
        {
            foreach (var translator in _translators)
            {
                var translatedMember = translator.Translate(expression);
                if (translatedMember != null)
                {
                    return translatedMember;
                }
            }

            return null;
        }

        /// <summary>
        /// 添加转换器。
        /// </summary>
        /// <param name="translators">转换器列表。</param>
        protected virtual void AddTranslators( IEnumerable<IMemberTranslator> translators)
        {
            _translators.InsertRange(0, translators);
        }
    }
}