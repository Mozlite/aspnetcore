using Microsoft.Extensions.Caching.Memory;
using Mozlite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 缓存对象管理基类。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    /// <typeparam name="TKey">模型主键类型。</typeparam>
    public abstract class CachableObjectManager<TModel, TKey> : ObjectManager<TModel, TKey>, ICachableObjectManager<TModel, TKey>
        where TModel : IIdObject<TKey>
    {
        /// <summary>
        /// 缓存实例。
        /// </summary>
        protected IMemoryCache Cache { get; }
        /// <summary>
        /// 初始化类<see cref="ObjectManager{TModel,TKey}"/>。
        /// </summary>
        /// <param name="context">数据库操作实例。</param>
        /// <param name="cache">缓存接口。</param>
        protected CachableObjectManager(IDbContext<TModel> context, IMemoryCache cache) : base(context)
        {
            Cache = cache;
        }

        /// <summary>
        /// 缓存键。
        /// </summary>
        protected virtual object CacheKey => typeof(TModel);

        /// <summary>
        /// 刷新缓存。
        /// </summary>
        public virtual void Refresh()
        {
            Cache.Remove(CacheKey);
        }

        /// <summary>
        /// 清空所有数据。
        /// </summary>
        public override void Clear()
        {
            Context.Delete();
            Refresh();
        }

        /// <summary>
        /// 清空所有数据。
        /// </summary>
        public override async Task ClearAsync(CancellationToken cancellationToken = default)
        {
            await Context.DeleteAsync(cancellationToken: cancellationToken);
            Refresh();
        }

        /// <summary>
        /// 如果操作结果成功刷新缓存并返回结果。
        /// </summary>
        /// <param name="result">数据库操作结果。</param>
        /// <returns>数据库操作结果。</returns>
        protected DataResult Refresh(DataResult result)
        {
            if (result) Refresh();
            return result;
        }

        /// <summary>
        /// 如果结果正确返回<paramref name="succeed"/>，否则返回失败项。
        /// </summary>
        /// <param name="result">执行结果。</param>
        /// <param name="succeed">执行成功返回的值。</param>
        /// <returns>返回执行结果实例对象。</returns>
        protected DataResult Refresh(bool result, DataAction succeed)
        {
            if (result)
            {
                Refresh();
                return succeed;
            }
            return -(int)succeed;
        }

        /// <summary>
        /// 通过唯一Id删除对象实例。
        /// </summary>
        /// <param name="ids">唯一Id集合。</param>
        /// <returns>返回删除结果。</returns>
        public override DataResult Delete(IEnumerable<TKey> ids)
        {
            return Refresh(base.Delete(ids));
        }

        /// <summary>
        /// 通过唯一Id删除对象实例。
        /// </summary>
        /// <param name="ids">唯一Id集合。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回删除结果。</returns>
        public override async Task<DataResult> DeleteAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        {
            return Refresh(await base.DeleteAsync(ids, cancellationToken));
        }

        /// <summary>
        /// 根据条件删除实例。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回删除结果。</returns>
        public override DataResult Delete(Expression<Predicate<TModel>> expression)
        {
            return Refresh(base.Delete(expression));
        }

        /// <summary>
        /// 通过唯一Id删除对象实例。
        /// </summary>
        /// <param name="id">唯一Id。</param>
        /// <returns>返回删除结果。</returns>
        public override DataResult Delete(TKey id)
        {
            return Refresh(base.Delete(id));
        }

        /// <summary>
        /// 根据条件删除实例。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回删除结果。</returns>
        public override async Task<DataResult> DeleteAsync(Expression<Predicate<TModel>> expression, CancellationToken cancellationToken = default)
        {
            return Refresh(await base.DeleteAsync(expression, cancellationToken));
        }

        /// <summary>
        /// 通过唯一Id删除对象实例。
        /// </summary>
        /// <param name="id">唯一Id。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回删除结果。</returns>
        public override async Task<DataResult> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
        {
            return Refresh(await base.DeleteAsync(id, cancellationToken));
        }

        /// <summary>
        /// 保存对象实例。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        /// <returns>返回保存结果。</returns>
        public override DataResult Save(TModel model)
        {
            return Refresh(base.Save(model));
        }

        /// <summary>
        /// 保存对象实例。
        /// </summary>
        /// <param name="model">模型实例对象。</param>
        /// <returns>返回保存结果。</returns>
        /// <param name="cancellationToken">取消标识。</param>
        public override async Task<DataResult> SaveAsync(TModel model, CancellationToken cancellationToken = default)
        {
            return Refresh(await base.SaveAsync(model, cancellationToken));
        }

        /// <summary>
        /// 根据条件更新特定的实例。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="satement">更新对象。</param>
        /// <returns>返回更新结果。</returns>
        public override DataResult Update(Expression<Predicate<TModel>> expression, object satement)
        {
            return Refresh(base.Update(expression, satement));
        }

        /// <summary>
        /// 根据条件更新特定的实例。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="satement">更新对象。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回更新结果。</returns>
        public override async Task<DataResult> UpdateAsync(Expression<Predicate<TModel>> expression, object satement, CancellationToken cancellationToken = default)
        {
            return Refresh(await base.UpdateAsync(expression, satement, cancellationToken));
        }

        /// <summary>
        /// 获取分类。
        /// </summary>
        /// <param name="id">分类Id。</param>
        /// <returns>返回分类实例。</returns>
        public override TModel Find(TKey id)
        {
            var categories = Fetch();
            return categories.FirstOrDefault(x => x.Id.Equals(id));
        }

        /// <summary>
        /// 获取分类。
        /// </summary>
        /// <param name="id">分类Id。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回分类实例。</returns>
        public override async Task<TModel> FindAsync(TKey id, CancellationToken cancellationToken = default)
        {
            var categories = await FetchAsync(cancellationToken: cancellationToken);
            return categories.FirstOrDefault(x => x.Id.Equals(id));
        }

        /// <summary>
        /// 通过唯一键获取当前值。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回当前模型实例。</returns>
        public override TModel Find(Expression<Predicate<TModel>> expression)
        {
            return Fetch(expression).SingleOrDefault();
        }

        /// <summary>
        /// 通过唯一键获取当前值。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回当前模型实例。</returns>
        public override async Task<TModel> FindAsync(Expression<Predicate<TModel>> expression, CancellationToken cancellationToken = default)
        {
            var categories = await FetchAsync(expression, cancellationToken);
            return categories.SingleOrDefault();
        }

        /// <summary>
        /// 根据条件更新特定的实例。
        /// </summary>
        /// <param name="id">唯一Id。</param>
        /// <param name="satement">更新对象。</param>
        /// <returns>返回更新结果。</returns>
        public override DataResult Update(TKey id, object satement)
        {
            return Refresh(base.Update(id, satement));
        }

        /// <summary>
        /// 根据条件更新特定的实例。
        /// </summary>
        /// <param name="id">唯一Id。</param>
        /// <param name="satement">更新对象。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回更新结果。</returns>
        public override async Task<DataResult> UpdateAsync(TKey id, object satement, CancellationToken cancellationToken = default)
        {
            return Refresh(await base.UpdateAsync(id, satement, cancellationToken));
        }

        /// <summary>
        /// 根据条件获取列表。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回模型实例列表。</returns>
        public override IEnumerable<TModel> Fetch(Expression<Predicate<TModel>> expression = null)
        {
            var models = Cache.GetOrCreate(CacheKey, ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                return Context.Fetch();
            });
            return models.Filter(expression);
        }

        /// <summary>
        /// 根据条件获取列表。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回模型实例列表。</returns>
        public override async Task<IEnumerable<TModel>> FetchAsync(Expression<Predicate<TModel>> expression = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var models = await Cache.GetOrCreateAsync(CacheKey, ctx =>
            {
                ctx.SetDefaultAbsoluteExpiration();
                return Context.FetchAsync(cancellationToken: cancellationToken);
            });
            return models.Filter(expression);
        }
    }

    /// <summary>
    /// 缓存对象管理基类。
    /// </summary>
    /// <typeparam name="TModel">模型类型。</typeparam>
    public abstract class CachableObjectManager<TModel> : CachableObjectManager<TModel, int>, ICachableObjectManager<TModel>
        where TModel : IIdObject
    {
        /// <summary>
        /// 初始化类<see cref="ObjectManager{TModel}"/>。
        /// </summary>
        /// <param name="context">数据库操作实例。</param>
        /// <param name="cache">缓存接口。</param>
        protected CachableObjectManager(IDbContext<TModel> context, IMemoryCache cache) : base(context, cache)
        {
        }
    }
}