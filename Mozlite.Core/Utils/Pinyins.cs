using System;
using System.Collections.Generic;
using System.Linq;

namespace Mozlite.Utils
{
    /// <summary>
    /// 拼音管理类型。
    /// </summary>
    public static class Pinyins
    {
        /// <summary>
        /// 获取字符串的拼音。
        /// </summary>
        /// <param name="words">当前汉字字符串。</param>
        /// <param name="seperator">拼接字符分隔符。</param>
        /// <param name="multiSeperator">多音字拼接分隔符。</param>
        /// <returns>返回当前字符串对应的拼音。</returns>
        public static string GetLetters(this string words, string seperator = "-", string multiSeperator = ".")
        {
            if (string.IsNullOrWhiteSpace(words))
                return null;
            string letters = null;
            var list = new List<string>();
            foreach (var word in words)
            {
                if ((word >= 'a' && word <= 'z') || (word >= 'A' && word <= 'Z'))//英文字母
                {
                    letters += word;
                    continue;
                }
                if (letters != null)//添加英文字母
                {
                    list.Add(letters);
                    letters = null;
                }
                Word pingyin;
                if (WordDictionary.TryGet(word, out pingyin))
                {
                    var names = pingyin.Pinyins
                        .Select(x => x.Name)
                        .Distinct(StringComparer.OrdinalIgnoreCase);
                    list.Add(string.Join(multiSeperator, names));
                }
            }
            return string.Join(seperator, list).ToLower();
        }

        /// <summary>
        /// 获取字符串的拼音首字符。
        /// </summary>
        /// <param name="words">当前汉字字符串。</param>
        /// <param name="seperator">多音字拼接分隔符。</param>
        /// <returns>返回当前字符串对应的拼音。</returns>
        public static string GetFirstLetters(this string words, string seperator = ".")
        {
            if (string.IsNullOrWhiteSpace(words))
                return null;
            var list = new List<string>();
            foreach (var word in words)
            {
                if (word >= 'A' && word <= 'Z')//英文字母
                {
                    list.Add(word.ToString());
                    continue;
                }
                Word pingyin;
                if (WordDictionary.TryGet(word, out pingyin))
                {
                    var names = pingyin.Pinyins
                        .Select(x => x.Name[0].ToString())
                        .Distinct(StringComparer.OrdinalIgnoreCase);
                    list.Add(string.Join(seperator, names));
                }
            }
            return string.Join("", list).ToUpper();
        }
    }
}