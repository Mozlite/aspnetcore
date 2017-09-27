using System.Collections.Generic;
using System.Linq.Expressions;
using Mozlite.Data.Query.Translators.Internal;

namespace Mozlite.Data.Query.Translators
{
    /// <summary>
    /// 代码段转换器。
    /// </summary>
    public class RelationalCompositeExpressionFragmentTranslator : IExpressionFragmentTranslator
    {
        private readonly List<IExpressionFragmentTranslator> _translators
            = new List<IExpressionFragmentTranslator>
            {
                new StringCompareTranslator(),
                new StringConcatTranslator()
            };

        /// <summary>
        /// 转换表达式。
        /// </summary>
        /// <param name="expression">当前表达式。</param>
        /// <returns>返回转换后的表达式。</returns>
        public virtual Expression Translate(Expression expression)
        {
            foreach (var translator in _translators)
            {
                var result = translator.Translate(expression);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// 添加转换器列表。
        /// </summary>
        /// <param name="translators">转换器列表。</param>
        protected virtual void AddTranslators( IEnumerable<IExpressionFragmentTranslator> translators)
        {
            _translators.InsertRange(0, translators);
        }
    }
}