
namespace Mozlite.Extensions.Internal
{
    /// <summary>
    /// CLR属性的Getter方法接口。
    /// </summary>
    public interface IClrPropertyGetter
    {
        /// <summary>
        /// 获取当前对象中属性的值。
        /// </summary>
        /// <param name="instance">当前对象实例。</param>
        /// <returns>返回属性值。</returns>
        object GetClrValue( object instance);
    }
}