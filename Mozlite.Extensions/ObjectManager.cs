using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Mozlite.Data;

namespace Mozlite.Extensions
{
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
        // ReSharper disable once InconsistentNaming
        protected IDbContext<TModel> Context { get; }

        /// <summary>
        /// 初始化类<see cref="ObjectManager{TModel,TKey}"/>。
        /// </summary>
        /// <param name="context">数据库操作实例。</param>
        protected ObjectManager(IDbContext<TModel> context)
        {
            Context = context;
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
            if (Context.Any(model.Id))
                return DataResult.FromResult(Context.Update(model), DataAction.Updated);
            return DataResult.FromResult(Context.Create(model), DataAction.Created);
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
        /// <param name="id">唯一Id。</param>
        /// <param name="satement">更新对象。</param>
        /// <returns>返回更新结果。</returns>
        public virtual DataResult Update(TKey id, object satement)
        {
            return DataResult.FromResult(Context.Update(id, satement), DataAction.Updated);
        }

        /// <summary>
        /// 根据条件更新特定的实例。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="satement">更新对象。</param>
        /// <returns>返回更新结果。</returns>
        public virtual DataResult Update(Expression<Predicate<TModel>> expression, object satement)
        {
            return DataResult.FromResult(Context.Update(expression, satement), DataAction.Updated);
        }

        /// <summary>
        /// 根据条件删除实例。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回删除结果。</returns>
        public virtual DataResult Delete(Expression<Predicate<TModel>> expression)
        {
            return DataResult.FromResult(Context.Delete(expression), DataAction.Deleted);
        }

        /// <summary>
        /// 通过唯一Id删除对象实例。
        /// </summary>
        /// <param name="id">唯一Id。</param>
        /// <returns>返回删除结果。</returns>
        public virtual DataResult Delete(TKey id)
        {
            return DataResult.FromResult(Context.Delete(id), DataAction.Deleted);
        }

        /// <summary>
        /// 通过唯一键获取当前值。
        /// </summary>
        /// <param name="id">唯一Id。</param>
        /// <returns>返回当前模型实例。</returns>
        public virtual TModel Find(TKey id)
        {
            return Context.Find(id);
        }

        /// <summary>
        /// 通过唯一键获取当前值。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回当前模型实例。</returns>
        public virtual TModel Find(Expression<Predicate<TModel>> expression)
        {
            return Context.Find(expression);
        }

        /// <summary>
        /// 根据条件获取列表。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回模型实例列表。</returns>
        public virtual IEnumerable<TModel> Fetch(Expression<Predicate<TModel>> expression = null)
        {
            return Context.Fetch(expression);
        }

        /// <summary>
        /// 保存对象实例。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        /// <returns>返回保存结果。</returns>
        /// <param name="cancellationToken">取消标识。</param>
        public virtual async Task<DataResult> SaveAsync(TModel model, CancellationToken cancellationToken = default)
        {
            if (await IsDuplicatedAsync(model, cancellationToken))
                return DataAction.Duplicate;
            if (Context.Any(model.Id))
                return DataResult.FromResult(await Context.UpdateAsync(model, cancellationToken), DataAction.Updated);
            return DataResult.FromResult(await Context.CreateAsync(model, cancellationToken), DataAction.Created);
        }

        /// <summary>
        /// 判断是否重复。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        /// <returns>返回判断结果。</returns>
        /// <param name="cancellationToken">取消标识。</param>
        public virtual Task<bool> IsDuplicatedAsync(TModel model, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// 根据条件更新特定的实例。
        /// </summary>
        /// <param name="id">唯一Id。</param>
        /// <param name="satement">更新对象。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回更新结果。</returns>
        public virtual async Task<DataResult> UpdateAsync(TKey id, object satement, CancellationToken cancellationToken = default)
        {
            return DataResult.FromResult(await Context.UpdateAsync(id, satement, cancellationToken), DataAction.Updated);
        }

        /// <summary>
        /// 根据条件更新特定的实例。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="satement">更新对象。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回更新结果。</returns>
        public virtual async Task<DataResult> UpdateAsync(Expression<Predicate<TModel>> expression, object satement, CancellationToken cancellationToken = default)
        {
            return DataResult.FromResult(await Context.UpdateAsync(expression, satement, cancellationToken), DataAction.Updated);
        }

        /// <summary>
        /// 清空所有数据。
        /// </summary>
        public virtual void Clear()
        {
            Context.Delete();
        }

        /// <summary>
        /// 清空所有数据。
        /// </summary>
        public virtual Task ClearAsync(CancellationToken cancellationToken = default)
        {
            return Context.DeleteAsync(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 通过唯一Id删除对象实例。
        /// </summary>
        /// <param name="ids">唯一Id集合。</param>
        /// <returns>返回删除结果。</returns>
        public virtual DataResult Delete(IEnumerable<TKey> ids)
        {
            return DataResult.FromResult(Context.Delete(x => x.Id.Included(ids)), DataAction.Deleted);
        }

        /// <summary>
        /// 通过唯一Id删除对象实例。
        /// </summary>
        /// <param name="ids">唯一Id集合。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回删除结果。</returns>
        public virtual async Task<DataResult> DeleteAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        {
            return DataResult.FromResult(await Context.DeleteAsync(x => x.Id.Included(ids), cancellationToken), DataAction.Deleted);
        }

        /// <summary>
        /// 根据条件删除实例。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回删除结果。</returns>
        public virtual async Task<DataResult> DeleteAsync(Expression<Predicate<TModel>> expression, CancellationToken cancellationToken = default)
        {
            return DataResult.FromResult(await Context.DeleteAsync(expression, cancellationToken), DataAction.Deleted);
        }

        /// <summary>
        /// 通过唯一Id删除对象实例。
        /// </summary>
        /// <param name="id">唯一Id。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回删除结果。</returns>
        public virtual async Task<DataResult> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return DataResult.FromResult(await Context.DeleteAsync(id, cancellationToken), DataAction.Deleted);
        }

        /// <summary>
        /// 通过唯一键获取当前值。
        /// </summary>
        /// <param name="id">唯一Id。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回当前模型实例。</returns>
        public virtual Task<TModel> FindAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return Context.FindAsync(id, cancellationToken);
        }

        /// <summary>
        /// 通过唯一键获取当前值。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回当前模型实例。</returns>
        public virtual Task<TModel> FindAsync(Expression<Predicate<TModel>> expression, CancellationToken cancellationToken = default)
        {
            return Context.FindAsync(expression, cancellationToken);
        }

        /// <summary>
        /// 根据条件获取列表。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回模型实例列表。</returns>
        public virtual Task<IEnumerable<TModel>> FetchAsync(Expression<Predicate<TModel>> expression = null, CancellationToken cancellationToken = default)
        {
            return Context.FetchAsync(expression, cancellationToken);
        }

        /// <summary>
        /// 实例化一个查询实例，这个实例相当于实例化一个查询类，不能当作属性直接调用。
        /// </summary>
        /// <returns>返回模型的一个查询实例。</returns>
        public virtual IQueryable<TModel> AsQueryable()
        {
            return Context.AsQueryable();
        }

        /// <summary>
        /// 分页获取实例列表。
        /// </summary>
        /// <param name="query">查询实例。</param>
        /// <returns>返回分页实例列表。</returns>
        public virtual TQuery Load<TQuery>(TQuery query) where TQuery : QueryBase<TModel>
        {
            return Context.Load(query);
        }

        /// <summary>
        /// 分页获取实例列表。
        /// </summary>
        /// <param name="query">查询实例。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回分页实例列表。</returns>
        public virtual Task<TQuery> LoadAsync<TQuery>(TQuery query, CancellationToken cancellationToken = default) where TQuery : QueryBase<TModel>
        {
            return Context.LoadAsync(query, cancellationToken: cancellationToken);
        }
    }

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
        /// <param name="context">数据库操作实例。</param>
        protected ObjectManager(IDbContext<TModel> context) : base(context)
        {
        }
    }
}