using Mozlite.Data.Internal;
using Mozlite.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mozlite.Data.Query
{
    /// <summary>
    /// 查询构建实例上下文。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    public class QueryContext<TModel> : IQueryable<TModel>, IQuerySql
    {
        #region init
        private readonly IExpressionVisitorFactory _visitorFactory;
        private readonly IQuerySqlGenerator _sqlGenerator;
        private readonly IDbExecutor _db;

        /// <summary>
        /// 初始化类<see cref="QueryContext{TModel}"/>。
        /// </summary>
        /// <param name="sqlHelper">SQL辅助接口。</param>
        /// <param name="visitorFactory">表达式解析工厂接口。</param>
        /// <param name="sqlGenerator">SQL脚本生成接口。</param>
        /// <param name="db">数据库接口。</param>
        public QueryContext(ISqlHelper sqlHelper, IExpressionVisitorFactory visitorFactory, IQuerySqlGenerator sqlGenerator, IDbExecutor db)
        {
            _visitorFactory = visitorFactory;
            _sqlGenerator = sqlGenerator;
            _db = db;
            SqlHelper = sqlHelper;
            Entity = typeof(TModel).GetEntityType();
        }

        /// <summary>
        /// 当前模型类型。
        /// </summary>
        protected IEntityType Entity { get; }

        /// <summary>
        /// SQL辅助接口。
        /// </summary>
        protected ISqlHelper SqlHelper { get; }

        /// <summary>
        /// 解析表达式代码。
        /// </summary>
        /// <param name="expression">表达式。</param>
        /// <param name="getAliasFunc">获取别名方法。</param>
        /// <returns>返回解析后的SQL字符串。</returns>
        protected string Visit(Expression expression, Func<Type, string> getAliasFunc)
        {
            var visitor = _visitorFactory.Create(getAliasFunc);
            visitor.Visit(expression);
            return visitor.ToString();
        }

        /// <summary>
        /// 格式化SQL查询字符串。
        /// </summary>
        /// <param name="value">值对象。</param>
        /// <returns>返回格式化后的字符串。</returns>
        protected string Escape(object value) => SqlHelper.EscapeLiteral(value);

        /// <summary>
        /// 格式化标识符，如列名称加上“[]”。
        /// </summary>
        /// <param name="name">标识字符串。</param>
        /// <returns>返回格式化后的字符串。</returns>
        protected virtual string Delimit(string name) => SqlHelper.DelimitIdentifier(name, GetAlias(typeof(TModel)));

        /// <summary>
        /// 格式化列名称，如列名称加上“[]”和前缀。
        /// </summary>
        /// <typeparam name="TAlias">别名类型。</typeparam>
        /// <param name="property">属性实例对象。</param>
        /// <returns>返回格式化后的字符串。</returns>
        protected virtual string Delimit<TAlias>(PropertyInfo property)
        {
            return SqlHelper.DelimitIdentifier(property.Name, GetAlias(typeof(TAlias)));
        }

        private int _current = 'a';
        private readonly Dictionary<Type, string> _alias = new Dictionary<Type, string>
        {
            {typeof(TModel), "a" }
        };

        /// <summary>
        /// 获取表格前缀。
        /// </summary>
        /// <param name="type">当前模型类型。</param>
        /// <returns>返回表格前缀。</returns>
        protected string GetAlias(Type type)
        {
            if (!_alias.TryGetValue(type, out var alias))
            {
                _current++;
                alias = ((char)_current).ToString();
                _alias[type] = alias;
            }
            return alias;
        }

        private Func<Type, string> GetExpressionAlias(params Type[] suggestions)
        {
            return type =>
            {
                foreach (var suggestion in suggestions)
                {
                    if (type.IsAssignableFrom(suggestion))
                        return GetAlias(suggestion);
                }
                return GetAlias(type);
            };
        }
        #endregion

        #region froms
        private string _fromSql;
        /// <summary>
        /// FROM语句。
        /// </summary>
        public virtual string FromSql
        {
            get
            {
                if (_fromSql == null)
                {
                    var builder = new StringBuilder();
                    builder.Append("FROM ")
                        .Append(typeof(TModel).GetTableName())
                        .Append(" AS a ");
                    if (_nolock)
                    {
                        builder.Append(_sqlGenerator.WithNolock())
                            .Append(" ");
                    }
                    if (_joins.Count > 0)
                        builder.Append(string.Join(" ", _joins.Select(x => x())));
                    _fromSql = builder.ToString();
                }
                return _fromSql;
            }
        }

        private readonly List<Func<string>> _joins = new List<Func<string>>();
        /// <summary>
        /// 设置表格关联。
        /// </summary>
        /// <typeparam name="TForeign">拼接类型。</typeparam>
        /// <param name="onExpression">关联条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        public IQueryable<TModel> InnerJoin<TForeign>(Expression<Func<TModel, TForeign, bool>> onExpression)
        {
            return InnerJoin<TModel, TForeign>(onExpression);
        }

        /// <summary>
        /// 设置表格关联。
        /// </summary>
        /// <typeparam name="TPrimary">主键所在的模型类型。</typeparam>
        /// <typeparam name="TForeign">外键所在的模型类型。</typeparam>
        /// <param name="onExpression">关联条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        public IQueryable<TModel> InnerJoin<TPrimary, TForeign>(Expression<Func<TPrimary, TForeign, bool>> onExpression)
        {
            return Join("INNER", onExpression);
        }

        private IQueryable<TModel> Join<TPrimary, TForeign>(string key, Expression<Func<TPrimary, TForeign, bool>> onExpression)
        {
            _joins.Add(() =>
            {
                var type = typeof(TForeign);
                var builder = new StringBuilder();
                builder.Append(key)
                    .Append(" JOIN ")
                    .Append(type.GetTableName())
                    .Append(" AS ")
                    .Append(GetAlias(type));
                if (_nolock)
                {
                    builder.Append(" ")
                        .Append(_sqlGenerator.WithNolock());
                }
                builder.Append(" ON ");
                builder.Append(Visit(onExpression, GetExpressionAlias(typeof(TPrimary), typeof(TForeign))));
                return builder.ToString();
            });
            return this;
        }

        IQueryContext<TModel> IQueryContext<TModel>.InnerJoin<TForeign>(
                Expression<Func<TModel, TForeign, bool>> onExpression)
            => InnerJoin<TForeign>(onExpression);

        IQueryContext<TModel> IQueryContext<TModel>.InnerJoin<TPrimary, TForeign>(
                Expression<Func<TPrimary, TForeign, bool>> onExpression)
            => InnerJoin(onExpression);

        /// <summary>
        /// 设置表格左关联。
        /// </summary>
        /// <typeparam name="TForeign">拼接类型。</typeparam>
        /// <param name="onExpression">关联条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        public IQueryable<TModel> LeftJoin<TForeign>(Expression<Func<TModel, TForeign, bool>> onExpression)
        {
            return LeftJoin<TModel, TForeign>(onExpression);
        }

        /// <summary>
        /// 设置表格左关联。
        /// </summary>
        /// <typeparam name="TPrimary">主键所在的模型类型。</typeparam>
        /// <typeparam name="TForeign">外键所在的模型类型。</typeparam>
        /// <param name="onExpression">关联条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        public IQueryable<TModel> LeftJoin<TPrimary, TForeign>(Expression<Func<TPrimary, TForeign, bool>> onExpression)
        {
            return Join("LEFT", onExpression);
        }

        /// <summary>
        /// 设置表格右关联。
        /// </summary>
        /// <typeparam name="TForeign">拼接类型。</typeparam>
        /// <param name="onExpression">关联条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        public IQueryable<TModel> RightJoin<TForeign>(Expression<Func<TModel, TForeign, bool>> onExpression)
        {
            return RightJoin<TModel, TForeign>(onExpression);
        }

        /// <summary>
        /// 设置表格右关联。
        /// </summary>
        /// <typeparam name="TPrimary">主键所在的模型类型。</typeparam>
        /// <typeparam name="TForeign">外键所在的模型类型。</typeparam>
        /// <param name="onExpression">关联条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        public IQueryable<TModel> RightJoin<TPrimary, TForeign>(Expression<Func<TPrimary, TForeign, bool>> onExpression)
        {
            return Join("RIGHT", onExpression);
        }

        IQueryContext<TModel> IQueryContext<TModel>.LeftJoin<TForeign>(Expression<Func<TModel, TForeign, bool>> onExpression)
            => LeftJoin<TForeign>(onExpression);

        IQueryContext<TModel> IQueryContext<TModel>.LeftJoin<TPrimary, TForeign>(Expression<Func<TPrimary, TForeign, bool>> onExpression)
            => LeftJoin(onExpression);

        IQueryContext<TModel> IQueryContext<TModel>.RightJoin<TForeign>(Expression<Func<TModel, TForeign, bool>> onExpression)
            => RightJoin<TForeign>(onExpression);

        IQueryContext<TModel> IQueryContext<TModel>.RightJoin<TPrimary, TForeign>(Expression<Func<TPrimary, TForeign, bool>> onExpression)
            => RightJoin(onExpression);

        private bool _nolock;
        /// <summary>
        /// 忽略锁（脏查询）。
        /// </summary>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryable<TModel> IQueryable<TModel>.WithNolock()
        {
            _nolock = true;
            return this;
        }

        /// <summary>
        /// 忽略锁（脏查询）。
        /// </summary>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> IQueryContext<TModel>.WithNolock()
        {
            _nolock = true;
            return this;
        }
        #endregion

        #region fields
        private string _fieldSql;
        /// <summary>
        /// 选择列。
        /// </summary>
        public string FieldSql
        {
            get
            {
                if (_fieldSql == null)
                {
                    if (_fields.Count == 0)
                    {
                        var fields = Entity.GetProperties()
                            .Where(x => !x.PropertyInfo.IsDefined(typeof(NotMappedAttribute)))
                            .Select(p => Delimit(p.Name));
                        _fields.AddRange(fields);
                    }
                    _fieldSql = string.Join(",", _fields.Distinct(StringComparer.OrdinalIgnoreCase));
                }
                return _fieldSql;
            }
        }

        private readonly List<string> _fields = new List<string>();

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="field">列表达式。</param>
        /// <param name="alias">别名。</param>
        /// <returns>返回当前查询实例对象。</returns>
        public virtual IQueryable<TModel> Select<TEntity>(Expression<Func<TEntity, object>> field, string alias)
        {
            var column = Delimit<TEntity>(field.GetPropertyAccess());
            alias = SqlHelper.DelimitIdentifier(alias);
            _fields.Add($"{column} AS {alias}");
            return this;
        }

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="fields">列表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        public virtual IQueryable<TModel> Select<TEntity>(Expression<Func<TEntity, object>> fields)
        {
            if (fields == null)
                _fields.Add($"{GetAlias(typeof(TEntity))}.*");
            else
                _fields.AddRange(fields.GetPropertyAccessList().Select(Delimit<TEntity>));
            return this;
        }

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <param name="fields">列表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        public virtual IQueryable<TModel> Select(Expression<Func<TModel, object>> fields)
        {
            return Select<TModel>(fields);
        }

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> IQueryContext<TModel>.Select()
            => Select();

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="fields">不包含的列表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        public virtual IQueryable<TModel> Exclude<TEntity>(Expression<Func<TEntity, object>> fields)
        {
            var excludes = fields.GetPropertyNames();
            var includes = Entity.GetProperties()
                .Where(x => !x.PropertyInfo.IsDefined(typeof(NotMappedAttribute)) && !excludes.Contains(x.Name, StringComparer.OrdinalIgnoreCase))
                .Select(p => Delimit(p.Name));
            _fields.AddRange(includes);
            return this;
        }

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <param name="fields">不包含的列表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        public virtual IQueryable<TModel> Exclude(Expression<Func<TModel, object>> fields) => Exclude<TModel>(fields);

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="fields">不包含的列表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> IQueryContext<TModel>.Exclude<TEntity>(Expression<Func<TEntity, object>> fields) => Exclude(fields);

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <param name="fields">不包含的列表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> IQueryContext<TModel>.Exclude(Expression<Func<TModel, object>> fields) => Exclude(fields);

        /// <summary>
        /// 设置选择列。
        /// </summary>
        /// <returns>返回当前查询实例对象。</returns>
        public IQueryable<TModel> Select()
        {
            _fields.Add("a.*");
            return this;
        }


        /// <summary>
        /// 设置选择列(不重复)。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="fields">列表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        public IQueryable<TModel> Distinct<TEntity>(Expression<Func<TEntity, object>> fields)
        {
            IsDistinct = true;
            return Select(fields);
        }

        /// <summary>
        /// 设置选择列(不重复)。
        /// </summary>
        /// <param name="fields">列表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        public IQueryable<TModel> Distinct(Expression<Func<TModel, object>> fields)
        {
            return Distinct<TModel>(fields);
        }

        IQueryContext<TModel> IQueryContext<TModel>.Distinct<TEntity>(Expression<Func<TEntity, object>> fields)
            => Distinct(fields);

        IQueryContext<TModel> IQueryContext<TModel>.Distinct(Expression<Func<TModel, object>> fields)
            => Distinct(fields);

        IQueryContext<TModel> IQueryContext<TModel>.Select<TEntity>(Expression<Func<TEntity, object>> fields, string alias)
            => Select(fields, alias);

        IQueryContext<TModel> IQueryContext<TModel>.Select<TEntity>(Expression<Func<TEntity, object>> fields)
            => Select(fields);

        IQueryContext<TModel> IQueryContext<TModel>.Select(Expression<Func<TModel, object>> fields)
            => Select(fields);
        #endregion

        #region wheres
        private string _whereSql;
        /// <summary>
        /// WHERE语句。
        /// </summary>
        public string WhereSql
        {
            get
            {
                if (_whereSql == null)
                {
                    if (_wheres.Count > 0)
                        _whereSql = $"WHERE {string.Join(" AND ", _wheres)}";
                }
                return _whereSql;
            }
        }

        private readonly List<string> _wheres = new List<string>();
        /// <summary>
        /// 添加条件表达式。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        public virtual IQueryable<TModel> Where<TEntity>(Expression<Predicate<TEntity>> expression)
        {
            return Where(Visit(expression, type => GetAlias(typeof(TEntity))));
        }

        /// <summary>
        /// 添加条件表达式。
        /// </summary>
        /// <param name="where">条件语句。</param>
        /// <returns>返回当前查询上下文。</returns>
        public IQueryable<TModel> Where(string where)
        {
            _wheres.Add(where);
            return this;
        }

        /// <summary>
        /// 添加条件表达式。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回当前查询实例对象。</returns>
        public IQueryable<TModel> Where(Expression<Predicate<TModel>> expression)
        {
            return Where<TModel>(expression);
        }

        IQueryContext<TModel> IQueryContext<TModel>.Where<TEntity>(Expression<Predicate<TEntity>> expression)
            => Where(expression);

        IQueryContext<TModel> IQueryContext<TModel>.Where(string where)
            => Where(where);

        IQueryContext<TModel> IQueryContext<TModel>.Where(Expression<Predicate<TModel>> expression)
            => Where(expression);
        #endregion

        #region orderbys
        private string _orderbySql;
        private bool _requiredOrderby;
        /// <summary>
        /// ORDER BY语句。
        /// </summary>
        public string OrderBySql
        {
            get
            {
                if (_orderbySql == null)
                {
                    if (_orderbys.Count > 0)
                        _orderbySql = $"ORDER BY {string.Join(", ", _orderbys)}";
                    else if (_requiredOrderby)
                    {
                        _orderbySql = string.Join(",", Entity.PrimaryKey.Properties.Select(k => Delimit(k.Name)));
                        _orderbySql = $"ORDER BY {_orderbySql}";
                    }
                }
                return _orderbySql;
            }
        }

        private readonly List<string> _orderbys = new List<string>();
        /// <summary>
        /// 添加排序规则。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="expression">列名称表达式。</param>
        public virtual IQueryable<TModel> OrderBy<TEntity>(Expression<Func<TEntity, object>> expression)
        {
            return OrderBy(expression, false);
        }

        /// <summary>
        /// 添加排序规则。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="expression">列名称表达式。</param>
        public virtual IQueryable<TModel> OrderByDescending<TEntity>(Expression<Func<TEntity, object>> expression)
        {
            return OrderBy(expression, true);
        }

        /// <summary>
        /// 添加排序规则。
        /// </summary>
        /// <param name="expression">列名称表达式。</param>
        public virtual IQueryable<TModel> OrderBy(Expression<Func<TModel, object>> expression)
        {
            return OrderBy<TModel>(expression);
        }

        /// <summary>
        /// 添加排序规则。
        /// </summary>
        /// <param name="expression">列名称表达式。</param>
        public virtual IQueryable<TModel> OrderByDescending(Expression<Func<TModel, object>> expression)
        {
            return OrderByDescending<TModel>(expression);
        }

        /// <summary>
        /// 添加排序规则。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="expression">列名称表达式。</param>
        /// <param name="isDesc">是否为降序。</param>
        /// <returns>返回当前查询实例对象。</returns>
        public virtual IQueryable<TModel> OrderBy<TEntity>(Expression<Func<TEntity, object>> expression, bool isDesc)
        {
            var properties = expression.GetPropertyAccessList();
            if (isDesc)
                _orderbys.AddRange(properties.Select(field => Delimit<TEntity>(field) + " DESC"));
            else
                _orderbys.AddRange(properties.Select(Delimit<TEntity>));
            return this;
        }

        /// <summary>
        /// 添加排序规则。
        /// </summary>
        /// <param name="expression">列名称表达式。</param>
        /// <param name="isDesc">是否为降序。</param>
        /// <returns>返回当前查询实例对象。</returns>
        public virtual IQueryable<TModel> OrderBy(Expression<Func<TModel, object>> expression, bool isDesc)
        {
            return OrderBy<TModel>(expression, isDesc);
        }

        /// <summary>
        /// 添加排序规则。
        /// </summary>
        /// <param name="expression">列名称表达式。</param>
        /// <param name="isDesc">是否为降序。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> IQueryContext<TModel>.OrderBy<TEntity>(Expression<Func<TEntity, object>> expression, bool isDesc)
            => OrderBy(expression, isDesc);

        /// <summary>
        /// 添加排序规则。
        /// </summary>
        /// <param name="expression">列名称表达式。</param>
        /// <param name="isDesc">是否为降序。</param>
        /// <returns>返回当前查询实例对象。</returns>
        IQueryContext<TModel> IQueryContext<TModel>.OrderBy(Expression<Func<TModel, object>> expression, bool isDesc)
            => OrderBy(expression, isDesc);

        IQueryContext<TModel> IQueryContext<TModel>.OrderBy<TEntity>(Expression<Func<TEntity, object>> expression)
            => OrderBy(expression);

        IQueryContext<TModel> IQueryContext<TModel>.OrderByDescending<TEntity>(Expression<Func<TEntity, object>> expression)
            => OrderByDescending(expression);

        IQueryContext<TModel> IQueryContext<TModel>.OrderBy(Expression<Func<TModel, object>> expression)
            => OrderBy(expression);

        IQueryContext<TModel> IQueryContext<TModel>.OrderByDescending(Expression<Func<TModel, object>> expression)
            => OrderByDescending(expression);
        #endregion

        #region size or page
        /// <summary>
        /// 获取记录数。
        /// </summary>
        public int? Size { get; private set; }

        /// <summary>
        /// 是否多表
        /// </summary>
        public bool IsDistinct { get; private set; }

        /// <summary>
        /// 聚合列或表达式。
        /// </summary>
        public string Aggregation { get; private set; }

        /// <summary>
        /// 页码。
        /// </summary>
        public int? PageIndex { get; private set; }
        #endregion

        #region database
        /// <summary>
        /// 查询数据库返回<paramref name="size"/>项结果。
        /// </summary>
        /// <param name="size">返回的记录数。</param>
        /// <returns>返回数据列表。</returns>
        public IEnumerable<TModel> AsEnumerable(int size)
        {
            Size = size;
            return Load(_sqlGenerator.Query(this));
        }

        /// <summary>
        /// 查询数据库返回结果。
        /// </summary>
        /// <returns>返回数据列表。</returns>
        public TModel FirstOrDefault()
        {
            Size = 1;
            if (_fields.Count == 0)
                _fields.Add($"{GetAlias(typeof(TModel))}.*");
            return Get(_sqlGenerator.Query(this));
        }

        /// <summary>
        /// 查询数据库返回结果。
        /// </summary>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回数据列表。</returns>
        public Task<TModel> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
        {
            Size = 1;
            if (_fields.Count == 0)
                _fields.Add($"{GetAlias(typeof(TModel))}.*");
            return GetAsync(_sqlGenerator.Query(this), cancellationToken);
        }

        /// <summary>
        /// 查询数据库返回结果。
        /// </summary>
        /// <param name="converter">对象转换器。</param>
        /// <returns>返回数据列表。</returns>
        public TValue FirstOrDefault<TValue>(Func<DbDataReader, TValue> converter)
        {
            Size = 1;
            if (_fields.Count == 0)
                _fields.Add($"{GetAlias(typeof(TModel))}.*");
            using (var reader = _db.ExecuteReader(_sqlGenerator.Query(this).ToString()))
            {
                if (reader.Read())
                    return converter(reader);
            }
            return default;
        }

        /// <summary>
        /// 查询数据库返回结果。
        /// </summary>
        /// <param name="converter">对象转换器。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回数据列表。</returns>
        public async Task<TValue> FirstOrDefaultAsync<TValue>(Func<DbDataReader, TValue> converter, CancellationToken cancellationToken = default)
        {
            Size = 1;
            if (_fields.Count == 0)
                _fields.Add($"{GetAlias(typeof(TModel))}.*");
            using (var reader = await _db.ExecuteReaderAsync(_sqlGenerator.Query(this).ToString(), cancellationToken: cancellationToken))
            {
                if (reader.Read())
                    return converter(reader);
            }
            return default;
        }

        /// <summary>
        /// 查询数据库返回结果。
        /// </summary>
        /// <param name="pageIndex">页码。</param>
        /// <param name="pageSize">每页显示的记录数。</param>
        /// <param name="count">分页总记录数计算列。</param>
        /// <returns>返回数据列表。</returns>
        public IPageEnumerable<TModel> AsEnumerable(int pageIndex, int pageSize, Expression<Func<TModel, object>> count = null)
        {
            //必须添加ORDER BY，如果没添加都自动附加主键排序
            _requiredOrderby = true;
            Size = pageSize;
            PageIndex = pageIndex;
            if (count != null)
                Aggregation = Delimit<TModel>(count.GetPropertyAccess());
            else
                Aggregation = "1";
            return LoadPage();
        }

        /// <summary>
        /// 查询数据库返回结果。
        /// </summary>
        /// <returns>返回数据列表。</returns>
        public IEnumerable<TModel> AsEnumerable() => Load(_sqlGenerator.Query(this));

        /// <summary>
        /// 查询数据库返回结果。
        /// </summary>
        /// <param name="converter">对象转换器。</param>
        /// <returns>返回数据列表。</returns>
        public IEnumerable<TValue> AsEnumerable<TValue>(Func<DbDataReader, TValue> converter)
        {
            var models = new List<TValue>();
            using (var reader = _db.ExecuteReader(_sqlGenerator.Query(this).ToString()))
            {
                while (reader.Read())
                    models.Add(converter(reader));
            }
            return models;
        }


        /// <summary>
        /// 查询数据库返回<paramref name="size"/>项结果。
        /// </summary>
        /// <param name="size">返回的记录数。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回数据列表。</returns>
        public Task<IEnumerable<TModel>> AsEnumerableAsync(int size, CancellationToken cancellationToken = default)
        {
            Size = size;
            return LoadAsync(_sqlGenerator.Query(this), cancellationToken);
        }

        /// <summary>
        /// 查询数据库返回结果。
        /// </summary>
        /// <param name="pageIndex">页码。</param>
        /// <param name="pageSize">每页显示的记录数。</param>
        /// <param name="count">分页总记录数计算列。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回数据列表。</returns>
        public Task<IPageEnumerable<TModel>> AsEnumerableAsync(int pageIndex, int pageSize, Expression<Func<TModel, object>> count = null,
            CancellationToken cancellationToken = default)
        {
            //必须添加ORDER BY，如果没添加都自动附加主键排序
            _requiredOrderby = true;
            Size = pageSize;
            PageIndex = pageIndex;
            if (count != null)
                Aggregation = Delimit<TModel>(count.GetPropertyAccess());
            else
                Aggregation = "1";
            return LoadPageAsync(cancellationToken);
        }

        /// <summary>
        /// 查询数据库返回结果。
        /// </summary>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回数据列表。</returns>
        public Task<IEnumerable<TModel>> AsEnumerableAsync(CancellationToken cancellationToken = default)
        {
            return LoadAsync(_sqlGenerator.Query(this), cancellationToken);
        }

        /// <summary>
        /// 查询数据库返回结果，主要配合查询特定列时候使用。
        /// </summary>
        /// <param name="converter">对象转换器。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回数据列表。</returns>
        public async Task<IEnumerable<TValue>> AsEnumerableAsync<TValue>(Func<DbDataReader, TValue> converter, CancellationToken cancellationToken = default)
        {
            var models = new List<TValue>();
            using (var reader = await _db.ExecuteReaderAsync(_sqlGenerator.Query(this).ToString(), cancellationToken: cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                    models.Add(converter(reader));
            }
            return models;
        }

        /// <summary>
        /// 读取模型实例。
        /// </summary>
        /// <param name="sql">SQL脚本。</param>
        /// <returns>返回模型实例。</returns>
        protected TModel Get(SqlIndentedStringBuilder sql)
        {
            using (var reader = _db.ExecuteReader(sql.ToString()))
            {
                if (reader.Read())
                    return Entity.Read<TModel>(reader);
            }
            return default;
        }

        /// <summary>
        /// 读取模型实例。
        /// </summary>
        /// <param name="sql">SQL脚本。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回模型实例。</returns>
        protected async Task<TModel> GetAsync(SqlIndentedStringBuilder sql, CancellationToken cancellationToken = default)
        {
            using (var reader = await _db.ExecuteReaderAsync(sql.ToString(), cancellationToken: cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                    return Entity.Read<TModel>(reader);
            }
            return default;
        }

        /// <summary>
        /// 读取模型实例列表。
        /// </summary>
        /// <param name="sql">SQL脚本。</param>
        /// <returns>返回模型实例列表。</returns>
        protected IEnumerable<TModel> Load(SqlIndentedStringBuilder sql)
        {
            var models = new List<TModel>();
            using (var reader = _db.ExecuteReader(sql.ToString()))
            {
                while (reader.Read())
                    models.Add(Entity.Read<TModel>(reader));
            }
            return models;
        }

        /// <summary>
        /// 读取模型实例列表。
        /// </summary>
        /// <param name="sql">SQL脚本。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回模型实例列表。</returns>
        protected async Task<IEnumerable<TModel>> LoadAsync(SqlIndentedStringBuilder sql, CancellationToken cancellationToken = default)
        {
            var models = new List<TModel>();
            using (var reader = await _db.ExecuteReaderAsync(sql.ToString(), cancellationToken: cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                    models.Add(Entity.Read<TModel>(reader));
            }
            return models;
        }

        /// <summary>
        /// 读取模型实例列表。
        /// </summary>
        /// <returns>返回模型实例列表。</returns>
        protected IPageEnumerable<TModel> LoadPage()
        {
            var models = new PageEnumerable<TModel>();
            models.PI = PageIndex ?? 1;
            models.PS = Size ?? 20;
            using (var reader = _db.ExecuteReader(_sqlGenerator.Query(this).ToString()))
            {
                while (reader.Read())
                    models.Add(Entity.Read<TModel>(reader));
                if (reader.NextResult() && reader.Read())
                    models.Size = reader.GetInt32(0);
            }
            return models;
        }

        /// <summary>
        /// 读取模型实例列表。
        /// </summary>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回模型实例列表。</returns>
        protected async Task<IPageEnumerable<TModel>> LoadPageAsync(CancellationToken cancellationToken = default)
        {
            var models = new PageEnumerable<TModel>();
            models.PI = PageIndex ?? 1;
            models.PS = Size ?? 20;
            using (var reader = await _db.ExecuteReaderAsync(_sqlGenerator.Query(this).ToString(), cancellationToken: cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                    models.Add(Entity.Read<TModel>(reader));
                if (await reader.NextResultAsync(cancellationToken) && await reader.ReadAsync(cancellationToken))
                    models.Size = reader.GetInt32(0);
            }
            return models;
        }
        #endregion
    }
}