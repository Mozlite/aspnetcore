using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Mozlite.Data;

namespace Mozlite.Extensions.Security.Activities
{
    /// <summary>
    /// 活动状态管理类。
    /// </summary>
    public abstract class ActivityManager<TActivity> : IActivityManager<TActivity>
        where TActivity : UserActivity, new()
    {
        /// <summary>
        /// 数据库操作上下文。
        /// </summary>
        protected IDbContext<TActivity> Context { get; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        /// <summary>
        /// 初始化<see cref="ActivityManager{TActivity}"/>。
        /// </summary>
        /// <param name="db">数据库操作接口。</param>
        /// <param name="httpContextAccessor">HTTP上下文访问器。</param>
        protected ActivityManager(IDbContext<TActivity> db, IHttpContextAccessor httpContextAccessor)
        {
            Context = db;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <returns>返回添加结果。</returns>
        public virtual async Task<bool> CreateAsync(TActivity activity)
        {
            if (Init(activity))
                return await Context.CreateAsync(activity);
            return false;
        }

        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <returns>返回添加结果。</returns>
        public virtual bool Create(TActivity activity)
        {
            if (Init(activity))
                return Context.Create(activity);
            return false;
        }

        /// <summary>
        /// 查询活动状态。
        /// </summary>
        /// <typeparam name="TQuery">查询实例类型。</typeparam>
        /// <param name="query">当前查询实例对象。</param>
        /// <returns>查询实例对象。</returns>
        public virtual TQuery Load<TQuery>(TQuery query) where TQuery : QueryBase<TActivity>
        {
            return Context.Load(query);
        }

        /// <summary>
        /// 查询活动状态。
        /// </summary>
        /// <typeparam name="TQuery">查询实例类型。</typeparam>
        /// <param name="query">当前查询实例对象。</param>
        /// <returns>查询实例对象。</returns>
        public virtual Task<TQuery> LoadAsync<TQuery>(TQuery query) where TQuery : QueryBase<TActivity>
        {
            return Context.LoadAsync(query);
        }

        /// <summary>
        /// 实例化类<see cref="TActivity"/>。
        /// </summary>
        /// <param name="activity">用户活动实例。</param>
        /// <returns>返回是否实例化成功。</returns>
        protected virtual bool Init(TActivity activity)
        {
            var context = _httpContextAccessor.HttpContext;
            activity.IPAdress = context.GetUserAddress();
            activity.UserId = context.User.GetUserId();
            return activity.UserId > 0;
        }

        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <param name="userId">当前用户。</param>
        /// <returns>返回添加结果。</returns>
        public virtual Task<bool> CreateAsync(string activity, int userId = 0)
        {
            var current = new TActivity();
            if (!Init(current))
                current.UserId = userId;
            current.Activity = activity;
            if (current.UserId > 0)
                return Context.CreateAsync(current);
            return Task.FromResult(false);
        }
        
        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="activity">活动状态实例。</param>
        /// <param name="userId">当前用户。</param>
        /// <returns>返回添加结果。</returns>
        public virtual bool Create(string activity, int userId)
        {
            var current = new TActivity();
            if (!Init(current))
                current.UserId = userId;
            current.Activity = activity;
            if (current.UserId > 0)
                return Context.Create(current);
            return false;
        }

        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="categoryId">分类Id。</param>
        /// <param name="activity">活动状态实例。</param>
        /// <param name="userId">当前用户。</param>
        /// <returns>返回添加结果。</returns>
        public virtual Task<bool> CreateAsync(int categoryId, string activity, int userId = 0)
        {
            var current = new TActivity();
            if (!Init(current))
                current.UserId = userId;
            current.CategoryId = categoryId;
            current.Activity = activity;
            if (current.UserId > 0)
                return Context.CreateAsync(current);
            return Task.FromResult(false);
        }

        /// <summary>
        /// 添加活动状态。
        /// </summary>
        /// <param name="categoryId">分类Id。</param>
        /// <param name="activity">活动状态实例。</param>
        /// <param name="userId">当前用户。</param>
        /// <returns>返回添加结果。</returns>
        public virtual bool Create(int categoryId, string activity, int userId = 0)
        {
            var current = new TActivity();
            if (!Init(current))
                current.UserId = userId;
            current.CategoryId = categoryId;
            current.Activity = activity;
            if (current.UserId > 0)
                return Context.Create(current);
            return false;
        }
    }
}