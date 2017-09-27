using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Mozlite.Data.Query.Translators.Internal;
using System.Linq;

namespace Mozlite.Data.Query.Translators
{
    /// <summary>
    /// 方法调用转换实现基类。
    /// </summary>
    public abstract class RelationalCompositeMethodCallTranslator : IMethodCallTranslator
    {
        private readonly List<IMethodCallTranslator> _translators;
        /// <summary>
        /// 初始化类<see cref="RelationalCompositeMethodCallTranslator"/>。
        /// </summary>
        /// <param name="loggerFactory">日志工厂接口。</param>
        protected RelationalCompositeMethodCallTranslator( ILoggerFactory loggerFactory)
        {
            _translators = new List<IMethodCallTranslator>
            {
                new ContainsTranslator(),
                new EndsWithTranslator(),
                new EqualsTranslator(),
                new StartsWithTranslator(),
                new IsNullOrEmptyTranslator(),
                new InTranslator(),
            };
        }

        /// <summary>
        /// 转换表达式。
        /// </summary>
        /// <param name="methodCallExpression">方法调用表达式。</param>
        /// <returns>返回转换后的表达式。</returns>
        public virtual Expression Translate(MethodCallExpression methodCallExpression)
        {
            return
                _translators
                    .Select(translator => translator.Translate(methodCallExpression))
                    .FirstOrDefault(translatedMethodCall => translatedMethodCall != null);
        }

        /// <summary>
        /// 添加转换类型。
        /// </summary>
        /// <param name="translators">转换类型列表。</param>
        protected virtual void AddTranslators( IEnumerable<IMethodCallTranslator> translators)
        {
            _translators.InsertRange(0, translators);
        }
    }
}