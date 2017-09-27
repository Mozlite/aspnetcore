namespace Mozlite.Data
{
    /// <summary>
    /// SQL辅助接口。
    /// </summary>
    public interface ISqlHelper
    {
        /// <summary>
        /// 语句结束符。
        /// </summary>
        string StatementTerminator { get; }

        /// <summary>
        /// 参数化字符串。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <returns>返回参数化的字符串。</returns>
        string Parameterized( string name);
        
        /// <summary>
        /// 将对象转换为安全的SQL字符串。
        /// </summary>
        /// <param name="literal">值。</param>
        /// <returns>返回转换后的字符串。</returns>
        string EscapeLiteral(object literal);

        /// <summary>
        /// 将字符串的“'”替换为“''”。
        /// </summary>
        /// <param name="identifier">当前字符串。</param>
        /// <returns>返回替换后的结果。</returns>
        string EscapeIdentifier(string identifier);
        
        /// <summary>
        /// 将表格名称或列名称加上安全括弧。
        /// </summary>
        /// <param name="identifier">当前标识字符串。</param>
        /// <returns>返回格式化后的字符串。</returns>
        string DelimitIdentifier( string identifier);
        
        /// <summary>
        /// 将表格名称或列名称加上安全括弧。
        /// </summary>
        /// <param name="name">当前标识字符串。</param>
        /// <param name="schema">架构名称。</param>
        /// <returns>返回格式化后的字符串。</returns>
        string DelimitIdentifier( string name,  string schema);
    }
}