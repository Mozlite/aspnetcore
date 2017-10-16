using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Mozlite.Data;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 对象管理接口。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    /// <typeparam name="TKey">唯一键类型。</typeparam>
    public interface IObjectManager<TModel, TKey>
        where TModel : IIdObject<TKey>
    {
        /// <summary>
        /// 保存对象实例。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        /// <returns>返回保存结果。</returns>
        DataResult Save(TModel model);

        /// <summary>
        /// 判断是否重复。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        /// <returns>返回判断结果。</returns>
        bool IsDuplicated(TModel model);

        /// <summary>
        /// 根据条件更新特定的实例。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="satement">更新对象。</param>
        /// <returns>返回更新结果。</returns>
        DataResult Update(Expression<Predicate<TModel>> expression, object satement);

        /// <summary>
        /// 根据条件删除实例。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回删除结果。</returns>
        DataResult Delete(Expression<Predicate<TModel>> expression);

        /// <summary>
        /// 通过唯一Id删除对象实例。
        /// </summary>
        /// <param name="id">唯一Id。</param>
        /// <returns>返回删除结果。</returns>
        DataResult Delete(TKey id);

        /// <summary>
        /// 通过唯一键获取当前值。
        /// </summary>
        /// <param name="id">唯一Id。</param>
        /// <returns>返回当前模型实例。</returns>
        TModel Find(TKey id);

        /// <summary>
        /// 通过唯一键获取当前值。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回当前模型实例。</returns>
        TModel Find(Expression<Predicate<TModel>> expression);

        /// <summary>
        /// 根据条件获取列表。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回模型实例列表。</returns>
        IEnumerable<TModel> Fetch(Expression<Predicate<TModel>> expression = null);

        /// <summary>
        /// 实例化一个查询实例，这个实例相当于实例化一个查询类，不能当作属性直接调用。
        /// </summary>
        /// <returns>返回模型的一个查询实例。</returns>
        IQueryable<TModel> AsQueryable();

        /// <summary>
        /// 分页获取实例列表。
        /// </summary>
        /// <param name="query">查询实例。</param>
        /// <returns>返回分页实例列表。</returns>
        TQuery Load<TQuery>(TQuery query) where TQuery : QueryBase<TModel>;

        /// <summary>
        /// 分页获取实例列表。
        /// </summary>
        /// <param name="query">查询实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回分页实例列表。</returns>
        Task<TQuery> LoadAsync<TQuery>(TQuery query, CancellationToken cancellationToken = default) where TQuery : QueryBase<TModel>;
    }

    /// <summary>
    /// 对象管理基类。
    /// </summary>
    /// <typeparam name="TModel">当前模型实例。</typeparam>
    /// <typeparam name="TKey">唯一键类型。</typeparam>
    public abstract class ObjectManager<TModel, TKey> : IObjectManager<TModel, TKey>
        where TModel : IIdObject<TKey>
    {
        /// <summary>
        /// 数据库操作实例。
        /// </summary>
        protected IRepository<TModel> Repository { get; }

        /// <summary>
        /// 初始化类<see cref="ObjectManager{TModel,TKey}"/>。
        /// </summary>
        /// <param name="repository">数据库操作实例。</param>
        protected ObjectManager(IRepository<TModel> repository)
        {
            Repository = repository;
        }

        /// <summary>
        /// 保存对象实例。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        /// <returns>返回保存结果。</returns>
        public virtual DataResult Save(TModel model)
        {
            if (IsDuplicated(model))
                return DataAction.Duplicate;
            if (Repository.Any(model.Id))
                return DataResult.FromResult(Repository.Update(model), DataAction.Updated);
            return DataResult.FromResult(Repository.Create(model), DataAction.Created);
        }

        /// <summary>
        /// 判断是否重复。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        /// <returns>返回判断结果。</returns>
        public virtual bool IsDuplicated(TModel model)
        {
            return false;
        }

        /// <summary>
        /// 根据条件更新特定的实例。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="satement">更新对象。</param>
        /// <returns>返回更新结果。</returns>
        public virtual DataResult Update(Expression<Predicate<TModel>> expression, object satement)
        {
            return DataResult.FromResult(Repository.Update(expression, satement), DataAction.Updated);
        }

        /// <summary>
        /// 根据条件删除实例。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回删除结果。</returns>
        public virtual DataResult Delete(Expression<Predicate<TModel>> expression)
        {
            return DataResult.FromResult(Repository.Delete(expression), DataAction.Deleted);
        }

        /// <summary>
        /// 通过唯一Id删除对象实例。
        /// </summary>
        /// <param name="id">唯一Id。</param>
        /// <returns>返回删除结果。</returns>
        public virtual DataResult Delete(TKey id)
        {
            return DataResult.FromResult(Repository.Delete(id), DataAction.Deleted);
        }

        /// <summary>
        /// 通过唯一键获取当前值。
        /// </summary>
        /// <param name="id">唯一Id。</param>
        /// <returns>返回当前模型实例。</returns>
        public virtual TModel Find(TKey id)
        {
            return Repository.Find(id);
        }

        /// <summary>
        /// 通过唯一键获取当前值。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回当前模型实例。</returns>
        public virtual TModel Find(Expression<Predicate<TModel>> expression)
        {
            return Repository.Find(expression);
        }

        /// <summary>
        /// 根据条件获取列表。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回模型实例列表。</returns>
        public virtual IEnumerable<TModel> Fetch(Expression<Predicate<TModel>> expression = null)
        {
            return Repository.Fetch(expression);
        }

        /// <summary>
        /// 实例化一个查询实例，这个实例相当于实例化一个查询类，不能当作属性直接调用。
        /// </summary>
        /// <returns>返回模型的一个查询实例。</returns>
        public virtual IQueryable<TModel> AsQueryable()
        {
            return Repository.AsQueryable();
        }

        /// <summary>
        /// 分页获取实例列表。
        /// </summary>
        /// <param name="query">查询实例。</param>
        /// <returns>返回分页实例列表。</returns>
        public virtual TQuery Load<TQuery>(TQuery query) where TQuery : QueryBase<TModel>
        {
            return Repository.Load(query);
        }

        /// <summary>
        /// 分页获取实例列表。
        /// </summary>
        /// <param name="query">查询实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回分页实例列表。</returns>
        public virtual Task<TQuery> LoadAsync<TQuery>(TQuery query, CancellationToken cancellationToken = default) where TQuery : QueryBase<TModel>
        {
            return Repository.LoadAsync(query, cancellationToken: cancellationToken);
        }
    }

    /// <summary>
    /// 对象管理接口。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    public interface IObjectManager<TModel> : IObjectManager<TModel, int>
        where TModel : IIdObject
    { }

    /// <summary>
    /// 对象管理实现基类。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    public abstract class ObjectManager<TModel> : ObjectManager<TModel, int>, IObjectManager<TModel>
        where TModel : IIdObject
    {
        /// <summary>
        /// 初始化类<see cref="ObjectManager{TModel}"/>。
        /// </summary>
        /// <param name="repository">数据库操作实例。</param>
        protected ObjectManager(IRepository<TModel> repository) : base(repository)
        {
        }
    }
}
