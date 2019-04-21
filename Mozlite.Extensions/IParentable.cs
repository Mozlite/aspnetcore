using System;
using System.Collections.Generic;
using System.Linq;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 递归接口。
    /// </summary>
    public interface IParentable
    {
        /// <summary>
        /// 唯一Id。
        /// </summary>
        int Id { get; }

        /// <summary>
        /// 父级Id。
        /// </summary>
        int ParentId { get; }

        /// <summary>
        /// 父级实例。
        /// </summary>
        object Parent { get; }

        /// <summary>
        /// 获取子项。
        /// </summary>
        List<object> Children { get; }

        /// <summary>
        /// 层次等级。
        /// </summary>
        int Level { get; }

        /// <summary>
        /// 包含子项数量。
        /// </summary>
        int Count { get; }
    }

    /// <summary>
    /// 递归接口。
    /// </summary>
    public interface IParentable<TModel> : IParentable
        where TModel : IParentable<TModel>
    {
        /// <summary>
        /// 父级实例。
        /// </summary>
        new TModel Parent { get; }

        /// <summary>
        /// 获取子项。
        /// </summary>
        new List<TModel> Children { get; }

        /// <summary>
        /// 添加子集实例。
        /// </summary>
        /// <param name="model">子集实例。</param>
        void Add(TModel model);
    }

    /// <summary>
    /// 父子级操作辅助类。
    /// </summary>
    public static class ParentableExtensions
    {
        /// <summary>
        /// 具有上下级关系的模型进行封装，将对象添加到父级或子集对象中，从而可以访问父级或子集对象实例。
        /// </summary>
        /// <param name="models">当前从数据库中获取的模型列表。</param>
        /// <returns>返回当前ID实例。</returns>
        public static IDictionary<int, TModel> MakeDictionary<TModel>(this IEnumerable<TModel> models)
            where TModel : IParentable<TModel>
        {
            var dic = models.ToDictionary(c => c.Id);
            dic[0] = Activator.CreateInstance<TModel>();
            foreach (var model in models)
            {
                if (dic.TryGetValue(model.ParentId, out var temp))
                    temp.Add(model);
            }
            return dic;
        }
    }
}