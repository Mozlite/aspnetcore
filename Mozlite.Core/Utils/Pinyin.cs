using System;

namespace Mozlite.Utils
{
    /// <summary>
    /// 拼音。
    /// </summary>
    public class Pinyin
    {
        internal Pinyin(string[] pinyins)
        {
            DisplayName = pinyins[0];
            var name = pinyins[1];
            Name = name.Substring(0, name.Length - 1);
            Tone = Convert.ToInt16(name[name.Length - 1].ToString());
        }

        /// <summary>
        /// 含音标的显示名称。
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// 英文字母。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 声调。
        /// </summary>
        public short Tone { get; }

        /// <summary>返回表示当前对象的字符串。</summary>
        /// <returns>表示当前对象的字符串。</returns>
        public override string ToString()
        {
            return DisplayName;
        }
    }
}