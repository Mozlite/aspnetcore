using System.Threading.Tasks;

namespace Mozlite.Extensions.Security.DisallowNames
{
    /// <summary>
    /// 非法名称管理接口。
    /// </summary>
    public interface IDisallowNameManager : ISingletonService
    {
        /// <summary>
        /// 保存非法名称。
        /// </summary>
        /// <param name="names">名称集合，多个非法名称以“,”或“\r\n”分割。</param>
        /// <returns>返回保存结果。</returns>
        DataResult Save(string names);

        /// <summary>
        /// 删除非法名称。
        /// </summary>
        /// <param name="id">非法名称Id。</param>
        /// <returns>返回删除结果。</returns>
        DataResult Delete(int id);

        /// <summary>
        /// 删除非法名称。
        /// </summary>
        /// <param name="ids">非法名称Id。</param>
        /// <returns>返回删除结果。</returns>
        DataResult Delete(int[] ids);

        /// <summary>
        /// 判断当前名称是否为非法名称。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>返回判断结果。</returns>
        bool IsDisallowed(string name);

        /// <summary>
        /// 判断当前名称是否为非法名称。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>返回判断结果。</returns>
        Task<bool> IsDisallowedAsync(string name);

        /// <summary>
        /// 分页获取非法名称。
        /// </summary>
        /// <param name="query">非法名称查询实例。</param>
        /// <returns>返回非法名称列表。</returns>
        DisallowNameQuery Load(DisallowNameQuery query);
    }
}