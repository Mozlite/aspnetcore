using System;
using System.Threading.Tasks;
using Mozlite.Extensions.Properties;

namespace Mozlite.Extensions
{
    /// <summary>
    /// 数据库操作结果。
    /// </summary>
    public class DataResult
    {
        private readonly string _desc;
        /// <summary>
        /// 初始化类<see cref="DataResult"/>。
        /// </summary>
        /// <param name="code">编码。</param>
        /// <param name="desc">描述信息。</param>
        private DataResult(int code, string desc)
        {
            _desc = desc;
            Code = code;
        }

        /// <summary>
        /// 编码。
        /// </summary>
        public int Code { get; }

        /// <summary>
        /// 隐式转换操作结果。
        /// </summary>
        /// <param name="action">数据操作结果。</param>
        public static implicit operator DataResult(DataAction action)
        {
            return (int)action;
        }

        /// <summary>
        /// 隐式转换为布尔型。
        /// </summary>
        /// <param name="result">数据操作结果。</param>
        public static implicit operator bool(DataResult result)
        {
            return result.Succeed();
        }

        /// <summary>
        /// 隐式转换操作结果。
        /// </summary>
        /// <param name="action">数据操作结果。</param>
        public static implicit operator DataResult(int action)
        {
            return new DataResult(action, Resources.ResourceManager.GetString($"DataAction_{(DataAction)action}"));
        }

        /// <summary>
        /// 隐式转换操作结果。
        /// </summary>
        /// <param name="msg">数据操作描述结果。</param>
        public static implicit operator DataResult(string msg)
        {
            return new DataResult((int)DataAction.UnknownError, msg);
        }

        /// <summary>
        /// 返回错误消息。
        /// </summary>
        /// <returns>错误消息。</returns>
        public override string ToString() => _desc;

        /// <summary>
        /// 格式化参数并返回描述信息。
        /// </summary>
        /// <param name="args">参数列表。</param>
        /// <returns>返回格式化后的描述信息。</returns>
        public string ToString(params object[] args)
        {
            return string.Format(_desc, args);
        }

        /// <summary>
        /// 是否成功。
        /// </summary>
        /// <returns>返回判断结果。</returns>
        public bool Succeed() => Code <= 0;

        /// <summary>
        /// 如果结果正确返回<paramref name="succeed"/>，否则返回失败项。
        /// </summary>
        /// <param name="result">执行结果。</param>
        /// <param name="succeed">执行成功返回的值。</param>
        /// <returns>返回执行结果实例对象。</returns>
        public static DataResult FromResult(bool result, DataAction succeed)
        {
            return result ? succeed : (DataAction)(-(int)succeed);
        }

        /// <summary>
        /// 如果结果正确返回<paramref name="succeed"/>，否则返回失败项。
        /// </summary>
        /// <param name="result">执行方法。</param>
        /// <param name="succeed">执行成功返回的值。</param>
        /// <returns>返回执行结果实例对象。</returns>
        public static DataResult FromResult(Func<bool> result, DataAction succeed)
        {
            return result() ? succeed : (DataAction)(-(int)succeed);
        }

        /// <summary>
        /// 如果结果正确返回<paramref name="succeed"/>，否则返回失败项。
        /// </summary>
        /// <param name="result">执行结果。</param>
        /// <param name="succeed">执行成功返回的值。</param>
        /// <param name="success">执行成功后执行的方法。</param>
        /// <returns>返回执行结果实例对象。</returns>
        public static DataResult FromResult(bool result, DataAction succeed, Action success)
        {
            if (result)
            {
                success();
                return succeed;
            }
            return (DataAction)(-(int)succeed);
        }

        /// <summary>
        /// 如果结果正确返回<paramref name="succeed"/>，否则返回失败项。
        /// </summary>
        /// <param name="result">执行方法。</param>
        /// <param name="succeed">执行成功返回的值。</param>
        /// <param name="success">执行成功后执行的方法。</param>
        /// <returns>返回执行结果实例对象。</returns>
        public static DataResult FromResult(Func<bool> result, DataAction succeed, Action success)
        {
            if (result())
            {
                success();
                return succeed;
            }
            return (DataAction)(-(int)succeed);
        }

        /// <summary>
        /// 如果结果正确返回<paramref name="succeed"/>，否则返回失败项。
        /// </summary>
        /// <param name="result">执行结果。</param>
        /// <param name="succeed">执行成功返回的值。</param>
        /// <param name="success">执行成功后执行的方法。</param>
        /// <returns>返回执行结果实例对象。</returns>
        public static async Task<DataResult> FromResult(bool result, DataAction succeed, Func<Task> success)
        {
            if (result)
            {
                await success();
                return succeed;
            }
            return (DataAction)(-(int)succeed);
        }

        /// <summary>
        /// 如果结果正确返回<paramref name="succeed"/>，否则返回失败项。
        /// </summary>
        /// <param name="result">执行方法。</param>
        /// <param name="succeed">执行成功返回的值。</param>
        /// <param name="success">执行成功后执行的方法。</param>
        /// <returns>返回执行结果实例对象。</returns>
        public static async Task<DataResult> FromResult(Func<bool> result, DataAction succeed, Func<Task> success)
        {
            if (result())
            {
                await success();
                return succeed;
            }
            return (DataAction)(-(int)succeed);
        }
    }
}