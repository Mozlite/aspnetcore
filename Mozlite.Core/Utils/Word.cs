using System;
using System.Collections.Generic;

namespace Mozlite.Utils
{
    /// <summary>
    /// 字符。
    /// </summary>
    public class Word
    {
        internal Word(string code, string[] pinyins)
        {
            Code = (char)Convert.ToInt32(code);
            foreach (var pinyin in pinyins)
            {
                Pinyins.Add(new Pinyin(pinyin.Split('.')));
            }
        }

        /// <summary>
        /// Unicode字符串。
        /// </summary>
        public char Code { get; }

        /// <summary>
        /// 拼音。
        /// </summary>
        public List<Pinyin> Pinyins { get; } = new List<Pinyin>();

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            return Code.ToString();
        }

        /// <summary>
        /// 隐式转换为字符串。
        /// </summary>
        /// <param name="word">字符实例对象。</param>
        public static implicit operator string(Word word) => word.ToString();
    }
}