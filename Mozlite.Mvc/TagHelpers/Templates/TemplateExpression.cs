using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Mozlite.Mvc.TagHelpers.Templates
{
    /// <summary>
    /// 模板表达式。
    /// </summary>
    public class TemplateExpression
    {
        /// <summary>
        /// 读取一个操作符。
        /// </summary>
        /// <returns>返回当前操作符类型。</returns>
        private ExpressionType ReadOperator()
        {
            if (!_expression.CanRead)
                return ExpressionType.Default;
            _expression.PassOptionalWhitespace();
            switch (_expression.Current)
            {
                case '>':
                    if (_expression.Next == '=')
                    {
                        _expression.Skip();
                        return ExpressionType.GreaterThanOrEqual;
                    }
                    else if (_expression.Next == '>')
                    {
                        _expression.Skip();
                        return ExpressionType.RightShift;
                    }
                    return ExpressionType.GreaterThan;
                case '<':
                    if (_expression.Next == '=')
                    {
                        _expression.Skip();
                        return ExpressionType.LessThanOrEqual;
                    }
                    else if (_expression.Next == '<')
                    {
                        _expression.Skip();
                        return ExpressionType.LeftShift;
                    }
                    return ExpressionType.LessThan;
                case '!':
                    if (_expression.Next == '=')
                    {
                        _expression.Skip();
                        return ExpressionType.NotEqual;
                    }
                    throw new Exception("表达式不支持赋值操作，如果是等于请使用“!=”!");
                case '=':
                    if (_expression.Next == '=')
                    {
                        _expression.Skip();
                        return ExpressionType.Equal;
                    }
                    throw new Exception("表达式不支持赋值操作，如果是等于请使用“==”!");
                case '&':
                    if (_expression.Next == '&')
                    {
                        _expression.Skip();
                        return ExpressionType.AndAlso;
                    }
                    return ExpressionType.And;
                case '|':
                    if (_expression.Next == '|')
                    {
                        _expression.Skip();
                        return ExpressionType.OrElse;
                    }
                    return ExpressionType.Or;
                case '+':
                    if (_expression.Next == '+')
                    {
                        _expression.Skip();
                        return ExpressionType.PostIncrementAssign;
                    }
                    return ExpressionType.Add;
                case '-':
                    if (_expression.Next == '-')
                    {
                        _expression.Skip();
                        return ExpressionType.PostDecrementAssign;
                    }
                    return ExpressionType.Subtract;
                case '*':
                    return ExpressionType.Multiply;
                case '/':
                    return ExpressionType.Divide;
                case '%':
                    return ExpressionType.Modulo;
                case '^':
                    return ExpressionType.ExclusiveOr;
            }
            return ExpressionType.Default;
        }

        private const string NumberIntString = "0123456789";
        private const string NumberHexString = "0123456789abcdefABCDEF";
        private const string NumberDoubleString = "0123456789.";
        private const string KeywordString = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";

        private Expression ReadToken(ParameterExpression parameter)
        {
            _expression.PassOptionalWhitespace();
            Func<Expression, Expression> convert = expr => expr;
            switch (_expression.Current)
            {
                case '+':
                    if (_expression.Next == '+')
                    {
                        convert = Expression.PreIncrementAssign;
                        _expression.Skip();
                    }
                    else
                    {
                        convert = Expression.UnaryPlus;
                    }
                    _expression.Skip();
                    _expression.PassOptionalWhitespace();
                    break;
                case '-':
                    if (_expression.Next == '-')
                    {
                        convert = Expression.PreDecrementAssign;
                        _expression.Skip();
                    }
                    else
                    {
                        convert = Expression.Negate;
                    }
                    _expression.Skip();
                    _expression.PassOptionalWhitespace();
                    break;
                case '!':
                    convert = Expression.Not;
                    _expression.Skip();
                    _expression.PassOptionalWhitespace();
                    break;
                case '~':
                    convert = Expression.OnesComplement;
                    _expression.Skip();
                    _expression.PassOptionalWhitespace();
                    break;
            }

            Expression expression = null;
            if (_expression.Current == '0' && (_expression.Next == 'x' || _expression.Next == 'X'))
            {
                _expression.Skip(2);
                var current = _expression.Read(NumberHexString);
                if (int.TryParse(current, NumberStyles.AllowHexSpecifier, NumberFormatInfo.CurrentInfo, out int value))
                    expression = Expression.Constant(value);
                else if (long.TryParse(current, NumberStyles.AllowHexSpecifier, NumberFormatInfo.CurrentInfo,
                    out long cvalue))
                    expression = Expression.Constant(cvalue);
                else
                    throw new Exception($"十六进制数值：0x{current}不能够转换为int或者long类型！");
            }
            else if (NumberIntString.Contains(_expression.Current))
            {
                var current = _expression.Read(NumberDoubleString);
                if (current.IndexOf('.') != -1)
                {
                    if (double.TryParse(current, out var value))
                        expression = Expression.Constant(value);
                    else
                        throw new Exception($"字符串：{current}不能够转换为double类型！");
                }
                else if (int.TryParse(current, out var value))
                    expression = Expression.Constant(value);
                else if (long.TryParse(current, out var cvalue))
                    expression = Expression.Constant(cvalue);
                else
                    throw new Exception($"字符串：{current}不能够转换为int或者long类型！");
            }
            else if (_expression.Current == '$')
            {
                _expression.Skip();
                var current = _expression.Read(KeywordString);
                if (current == ParameterName)
                {
                    _expression.PassOptionalWhitespace();
                    expression = parameter;
                    while (_expression.Current == '.')
                    {
                        _expression.Skip();
                        current = _expression.Read(KeywordString);
                        expression = Expression.PropertyOrField(expression, current);
                        _expression.PassOptionalWhitespace();
                    }
                }
                else
                    throw new Exception("未知参数！");
            }
            else if (_expression.Current == '\'' || _expression.Current == '"')
            {
                var chr = _expression.Current;
                var current = _expression.ReadQuote(chr).ToString().Trim(chr);
                expression = Expression.Constant(current);
            }

            return convert(expression);
        }

        private readonly TemplateString _expression;

        /// <summary>
        /// 初始化类<see cref="TemplateExpression"/>。
        /// </summary>
        /// <param name="expression">表达式字符串。</param>
        public TemplateExpression(string expression)
        {
            _expression = new TemplateString(expression.Trim());
        }

        private Expression Transfer(ParameterExpression parameter)
        {
            var tokens = new Stack<Token>();
            _expression.Begin();
            while (_expression.CanRead)
            {
                if (_expression.Current == '(')
                {
                    _expression.Skip();
                    tokens.Push(new Token());
                    continue;
                }
                Expression expression;
                if (_expression.Current == ')')
                {
                    _expression.Skip();
                    var blocks = new List<Token>();
                    while (tokens.Count > 0)
                    {
                        var token = tokens.Pop();
                        if (token.Type == TokenType.Block)
                            break;
                        blocks.Add(token);
                    }
                    blocks.Reverse();
                    expression = TransferCondition(blocks, 0);
                    if (!_expression.CanRead)
                    {
                        tokens.Push(new Token(expression));
                        break;
                    }
                }
                else
                    expression = ReadToken(parameter);
                var expressionType = ReadOperator();
                if (expressionType == ExpressionType.PostIncrementAssign)
                {
                    _expression.Skip();
                    expression = Expression.PostIncrementAssign(expression);
                    expressionType = ReadOperator();
                }
                else if (expressionType == ExpressionType.PostDecrementAssign)
                {
                    _expression.Skip();
                    expression = Expression.PostDecrementAssign(expression);
                    expressionType = ReadOperator();
                }

                tokens.Push(new Token(expression));
                if (expressionType != ExpressionType.Default)
                {
                    tokens.Push(new Token(expressionType));
                    _expression.Skip();
                    _expression.PassOptionalWhitespace();
                }
            }
            if (tokens.Count % 2 != 1)
                throw new Exception("表达式解析错误！");
            var list = tokens.ToList();
            list.Reverse();
            return Transfer(list);
        }

        /// <summary>
        /// 将多个标识拼接在一个标识中，以括号分割。
        /// </summary>
        /// <example>
        /// (1==1||$model.count>0&&1==2||3+1==4)
        /// </example>
        private Token Transfer(List<Token> tokens)
        {
            var expression = TransferCondition(tokens, 0);
            return new Token(expression);
        }

        private static readonly Dictionary<ExpressionType, Func<Expression, Expression, Expression>> _expressionTypes = new Dictionary<ExpressionType, Func<Expression, Expression, Expression>>
        {
            [ExpressionType.OrElse] = Expression.OrElse,
            [ExpressionType.AndAlso] = Expression.AndAlso,
            [ExpressionType.GreaterThan] = Expression.GreaterThan,
            [ExpressionType.GreaterThanOrEqual] = Expression.GreaterThanOrEqual,
            [ExpressionType.LessThan] = Expression.LessThan,
            [ExpressionType.LessThanOrEqual] = Expression.LessThanOrEqual,
            [ExpressionType.Equal] = Expression.Equal,
            [ExpressionType.NotEqual] = Expression.NotEqual,
            [ExpressionType.Multiply] = Expression.Multiply,
            [ExpressionType.Divide] = Expression.Divide,
            [ExpressionType.Modulo] = Expression.Modulo,
            [ExpressionType.Add] = Expression.Add,
            [ExpressionType.Subtract] = Expression.Subtract
        };

        //条件优先级
        private static readonly List<ExpressionType[]> _conditionTypes = new List<ExpressionType[]>
        {
            new []{ ExpressionType.OrElse },
            new []{ ExpressionType.AndAlso },
            new []{ ExpressionType.GreaterThan,ExpressionType.GreaterThanOrEqual, ExpressionType.LessThan, ExpressionType.LessThanOrEqual, ExpressionType.Equal, ExpressionType.NotEqual }
        };

        /// <summary>
        /// 将多个标识拼接在一个标识中，条件表达式。
        /// </summary>
        /// <example>
        /// (1==1||$model.count>0&&1==2||3+1==4)
        /// </example>
        private Expression TransferCondition(List<Token> tokens, int index)
        {
            if (tokens.Count == 1)
                return tokens[0];
            if (index >= _conditionTypes.Count)
                return TransferCaculation(tokens);
            var types = _conditionTypes[index];
            var buffer = new List<Token>();
            var queue = new Queue<Token>(tokens);
            while (queue.Count > 0)
            {
                var token = queue.Dequeue();
                if (token.Type == TokenType.Operator && types.Contains(token.ExpressionType) && _expressionTypes.TryGetValue(token.ExpressionType, out var func))
                {
                    var left = TransferCondition(buffer, index + 1);
                    var right = TransferCondition(queue.ToList(), index);
                    return func(left, right);
                }
                buffer.Add(token);
            }
            return TransferCondition(tokens, index + 1);
        }

        //计算优先级
        private static readonly ExpressionType[] _caculateTypes = { ExpressionType.Multiply, ExpressionType.Divide, ExpressionType.Modulo };

        /// <summary>
        /// 将多个标识拼接在一个标识中，计算表达式。
        /// </summary>
        /// <example>
        /// (3+1*8/2)
        /// </example>
        private Expression TransferCaculation(List<Token> tokens)
        {
            if (tokens.Count == 1)
                return tokens[0];
            var buffer = new List<Token>();
            Expression expression = null;
            var hasRighted = false;
            for (var i = 1; i < tokens.Count - 1; i += 2)
            {
                if (expression == null)
                    expression = tokens[i - 1];
                var current = tokens[i];
                var right = tokens[i + 1];
                if (_caculateTypes.Contains(current.ExpressionType) && _expressionTypes.TryGetValue(current.ExpressionType, out var func))
                {
                    hasRighted = true;
                    expression = func(expression, right.Expression);
                    continue;
                }
                if (current.Type == TokenType.Operator)
                {
                    buffer.Add(new Token(expression));
                    expression = null;
                }
                hasRighted = false;
                buffer.Add(current);
            }

            if (hasRighted)
                buffer.Add(new Token(expression));
            else
                buffer.Add(tokens[tokens.Count - 1]);
            return TransferCaculate(buffer);
        }

        /// <summary>
        /// 加减。
        /// </summary>
        private Expression TransferCaculate(List<Token> tokens)
        {
            if (tokens.Count == 1)
                return tokens[0];
            Expression expression = null;
            Func<Expression, Expression, Expression> convert = null;
            foreach (var token in tokens)
            {
                if (expression == null)
                    expression = token.Expression;
                else if (token.Type == TokenType.Operator)
                    _expressionTypes.TryGetValue(token.ExpressionType, out convert);
                else
                {
                    expression = convert(expression, token.Expression);
                    convert = null;
                }
            }
            return expression;
        }

        private const string ParameterName = "model";
        /// <summary>
        /// 计算表达式。
        /// </summary>
        /// <param name="instance">当前实例对象。</param>
        /// <param name="func">获取当前实例对象的值。</param>
        /// <returns>返回表达式值。</returns>
        public object Invoke(object instance)
        {
            var func = Compile(instance.GetType());
            return func.DynamicInvoke(instance);
        }

        /// <summary>
        /// 将表达式编译成代理方法。
        /// </summary>
        /// <param name="parameterType">参数类型。</param>
        /// <returns>返回代理方法。</returns>
        public Delegate Compile(Type parameterType)
        {
            var lambda = Lambda(parameterType);
            return lambda.Compile();
        }

        /// <summary>
        /// 将表达式转换为<see cref="LambdaExpression"/>实例。
        /// </summary>
        /// <param name="parameterType">参数类型。</param>
        /// <returns>返回Lambda表达式。</returns>
        public LambdaExpression Lambda(Type parameterType)
        {
            var parameter = Expression.Parameter(parameterType, ParameterName);
            var expression = Transfer(parameter);
            return Expression.Lambda(expression, parameter);
        }

        private enum TokenType
        {
            /// <summary>
            /// 表达式
            /// </summary>
            Expression,

            /// <summary>
            /// ()。
            /// </summary>
            Block,

            /// <summary>
            /// +-*/%
            /// </summary>
            Operator
        }

        /// <summary>
        /// 辅助标识。
        /// </summary>
        private class Token
        {
            public ExpressionType ExpressionType { get; protected set; }

            public Expression Expression { get; }

            public TokenType Type { get; }

            public Token()
            {
                Type = TokenType.Block;
                ExpressionType = ExpressionType.Default;
            }

            public Token(Expression expression)
            {
                Expression = expression;
                ExpressionType = expression.NodeType;
                Type = TokenType.Expression;
            }

            public Token(ExpressionType expressionType)
            {
                ExpressionType = expressionType;
                Type = TokenType.Operator;
            }

            /// <summary>返回表示当前对象的字符串。</summary>
            /// <returns>表示当前对象的字符串。</returns>
            public override string ToString()
            {
                if (Type == TokenType.Operator)
                {
                    switch (ExpressionType)
                    {
                        case ExpressionType.Add:
                        case ExpressionType.UnaryPlus:
                            return "+";
                        case ExpressionType.Subtract:
                        case ExpressionType.Negate:
                            return "-";
                        case ExpressionType.Multiply:
                            return "*";
                        case ExpressionType.Divide:
                            return "/";
                        case ExpressionType.Modulo:
                            return "%";
                        case ExpressionType.GreaterThan:
                            return ">";
                        case ExpressionType.GreaterThanOrEqual:
                            return ">=";
                        case ExpressionType.LessThan:
                            return "<";
                        case ExpressionType.LessThanOrEqual:
                            return "<=";
                        case ExpressionType.Equal:
                            return "==";
                        case ExpressionType.Not:
                            return "!";
                        case ExpressionType.NotEqual:
                            return "!=";
                        case ExpressionType.OnesComplement:
                            return "~";
                        case ExpressionType.PostIncrementAssign:
                        case ExpressionType.PreIncrementAssign:
                            return "++";
                        case ExpressionType.PreDecrementAssign:
                        case ExpressionType.PostDecrementAssign:
                            return "--";
                        case ExpressionType.AndAlso:
                            return "&&";
                        case ExpressionType.OrElse:
                            return "||";
                        default:
                            return ExpressionType.ToString();
                    }
                }
                if (Type == TokenType.Block)
                {
                    return "(";
                }
                return Expression?.ToString();
            }

            public static implicit operator Expression(Token token) => token.Expression;

            public static implicit operator Token(ExpressionType type) => new Token(type);
        }

        /// <summary>
        /// 获取当前表达式对应的动态执行方法。
        /// </summary>
        /// <param name="expression">当前条件表达式。</param>
        /// <param name="instance">通过对象执行当前表达式。</param>
        /// <returns>返回表达式代理方法。</returns>
        public static object Execute(string expression, object instance)
        {
            var type = instance.GetType();
            var key = expression + ":" + type.FullName;
            var lambda = _invokers.GetOrAdd(key, k =>
            {
                var templateExpression = new TemplateExpression(expression);
                return templateExpression.Lambda(type);
            });
            var func = lambda.Compile();
            return func.DynamicInvoke(instance);
        }

        private static readonly ConcurrentDictionary<string, LambdaExpression> _invokers = new ConcurrentDictionary<string, LambdaExpression>();
    }
}