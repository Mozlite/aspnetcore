using System.Collections.Concurrent;

namespace Mozlite.Utils
{
    /// <summary>
    /// Unicode字符字典。
    /// </summary>
    public static class WordDictionary
    {
        private static readonly ConcurrentDictionary<char, Word> _tables = new ConcurrentDictionary<char, Word>();
        static WordDictionary()
        {
            var words = Unicodes.Defines.Split('|');
            foreach (var wordStr in words)
            {
                var codes = wordStr.Split(':');
                var word = new Word(codes[0], codes[1].Split(','));
                _tables[word.Code] = word;
            }
        }

        /// <summary>
        /// 获取字符集数量。
        /// </summary>
        public static int Count => _tables.Count;

        /// <summary>
        /// 尝试获取字符实例对象。
        /// </summary>
        /// <param name="code">Unicode对应的十进制编码。</param>
        /// <param name="word">字符实例对象。</param>
        /// <returns>返回获取结果。</returns>
        public static bool TryGet(char code, out Word word)
        {
            return _tables.TryGetValue(code, out word);
        }
    }
}