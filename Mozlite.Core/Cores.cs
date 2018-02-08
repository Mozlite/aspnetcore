using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Mozlite.Properties;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Mozlite
{
    /// <summary>
    /// 核心辅助类。
    /// </summary>
    public static class Cores
    {
        /// <summary>
        /// 生成固定字节的随机数的十六进制字符串。
        /// </summary>
        /// <param name="size">字节数。</param>
        /// <returns>返回固定字节的随机数的十六进制字符串。</returns>
        public static string GeneralKey(int size)
        {
            return RandomOAuthStateGenerator.Generate(size);
        }

        private static class RandomOAuthStateGenerator
        {
            private static readonly RandomNumberGenerator _random = RandomNumberGenerator.Create();

            public static string Generate(int size)
            {
                const int bitsPerByte = 8;

                if (size % bitsPerByte != 0)
                {
                    throw new ArgumentException(Resources.RandomNumberGenerator_SizeInvalid, nameof(size));
                }

                var strengthInBytes = size / bitsPerByte;

                var data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return data.ToHexString();
            }
        }

        /// <summary>
        /// 将数组转换为十六进制字符串。
        /// </summary>
        /// <param name="bytes">当前数组。</param>
        /// <returns>返回十六进制字符串。</returns>
        public static string ToHexString(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }

        /// <summary>
        /// 将十六进制字符串转换为二进制数组。
        /// </summary>
        /// <param name="hexString">十六进制字符串。</param>
        /// <returns>返回二进制数组。</returns>
        public static byte[] GetBytes(string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                var hex = hexString.Substring(i * 2, 2);
                bytes[i] = (byte)int.Parse(hex, NumberStyles.HexNumber);
            }
            return bytes;
        }

        /// <summary>
        /// 使用HMACMD5进行加密。
        /// </summary>
        /// <param name="text">当前字符串。</param>
        /// <param name="key">哈希算法密钥128位密钥，十六进制字符串。</param>
        /// <returns>返回加密后的16进制的字符串。</returns>
        // ReSharper disable once InconsistentNaming
        public static string HMACMD5(string text, string key)
        {
            var md5 = new HMACMD5(GetBytes(key));
            var hashed = md5.ComputeHash(Encoding.UTF8.GetBytes(text));
            return hashed.ToHexString();
        }

        /// <summary>
        /// 使用HMACSHA1进行加密。
        /// </summary>
        /// <param name="text">当前字符串。</param>
        /// <param name="key">哈希算法密钥128位密钥，十六进制字符串。</param>
        /// <returns>返回加密后的16进制的字符串。</returns>
        // ReSharper disable once InconsistentNaming
        public static string HMACSHA1(string text, string key)
        {
            var sha = new HMACSHA1(GetBytes(key));
            var hashed = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
            return hashed.ToHexString();
        }

        /// <summary>
        /// 使用HMACSHA256进行加密。
        /// </summary>
        /// <param name="text">当前字符串。</param>
        /// <param name="key">哈希算法密钥128位密钥，十六进制字符串。</param>
        /// <returns>返回加密后的16进制的字符串。</returns>
        // ReSharper disable once InconsistentNaming
        public static string HMACSHA256(string text, string key)
        {
            var sha = new HMACSHA256(GetBytes(key));
            var hashed = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
            return hashed.ToHexString();
        }

        /// <summary>
        /// 使用HMACSHA384进行加密。
        /// </summary>
        /// <param name="text">当前字符串。</param>
        /// <param name="key">哈希算法密钥128位密钥，十六进制字符串。</param>
        /// <returns>返回加密后的16进制的字符串。</returns>
        // ReSharper disable once InconsistentNaming
        public static string HMACSHA384(string text, string key)
        {
            var sha = new HMACSHA384(GetBytes(key));
            var hashed = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
            return hashed.ToHexString();
        }

        /// <summary>
        /// 使用HMACSHA512进行加密。
        /// </summary>
        /// <param name="text">当前字符串。</param>
        /// <param name="key">哈希算法密钥128位密钥，十六进制字符串。</param>
        /// <returns>返回加密后的16进制的字符串。</returns>
        // ReSharper disable once InconsistentNaming
        public static string HMACSHA512(string text, string key)
        {
            var sha = new HMACSHA512(GetBytes(key));
            var hashed = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
            return hashed.ToHexString();
        }

        /// <summary>
        /// MD5加密。
        /// </summary>
        /// <param name="text">当前字符串。</param>
        /// <returns>加密后的字符串。</returns>
        public static string Md5(string text)
        {
            var md5 = MD5.Create();
            var hashed = md5.ComputeHash(Encoding.UTF8.GetBytes(text));
            return hashed.ToHexString();
        }

        /// <summary>
        /// SHA1加密。
        /// </summary>
        /// <param name="text">当前字符串。</param>
        /// <returns>加密后的字符串。</returns>
        public static string Sha1(string text)
        {
            var sha1 = SHA1.Create();
            var hashed = sha1.ComputeHash(Encoding.UTF8.GetBytes(text));
            return hashed.ToHexString();
        }

        /// <summary>
        /// SHA256加密。
        /// </summary>
        /// <param name="text">当前字符串。</param>
        /// <returns>加密后的字符串。</returns>
        public static string Sha256(string text)
        {
            var sha = SHA256.Create();
            var hashed = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
            return hashed.ToHexString();
        }

        /// <summary>
        /// SHA384加密。
        /// </summary>
        /// <param name="text">当前字符串。</param>
        /// <returns>加密后的字符串。</returns>
        public static string Sha384(string text)
        {
            var sha = SHA384.Create();
            var hashed = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
            return hashed.ToHexString();
        }

        /// <summary>
        /// SHA512加密。
        /// </summary>
        /// <param name="text">当前字符串。</param>
        /// <returns>加密后的字符串。</returns>
        public static string Sha512(string text)
        {
            var sha = SHA512.Create();
            var hashed = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
            return hashed.ToHexString();
        }

        /// <summary>
        /// 获取页面区间。
        /// </summary>
        /// <param name="pageIndex">页码。</param>
        /// <param name="pages">总页数。</param>
        /// <param name="factor">显示项数。</param>
        /// <param name="end">返回结束索引。</param>
        /// <returns>返回开始索引。</returns>
        public static int GetRange(int pageIndex, int pages, int factor, out int end)
        {
            var item = factor / 2;
            var start = pageIndex - item;
            end = pageIndex + item;
            if (start < 1)
            {
                end += 1 - start;
                start = 1;
            }
            if (end > pages)
            {
                start -= (end - pages);
                end = pages;
            }
            if (end < 1)
                end = 1;
            if (start < 1)
                return 1;
            return start;
        }

        private static readonly DateTime _unixDate = new DateTime(1970, 1, 1);
        /// <summary>
        /// 获取当前时间对应的UNIX时间的毫秒数。
        /// </summary>
        public static long UnixNow => (long)(DateTime.Now - _unixDate).TotalMilliseconds;

        private const string HtmlCaseRegexReplacement = "-$1$2";
        private static readonly Regex _htmlCaseRegex =
            new Regex(
                "(?<!^)((?<=[a-zA-Z0-9])[A-Z][a-z])|((?<=[a-z])[A-Z])",
                RegexOptions.None,
                TimeSpan.FromMilliseconds(500));
        /// <summary>
        /// 将pascal/camel格式的名称转换为小写并且以“-”分隔的字符串名称。
        /// </summary>
        /// <example>
        /// SomeThing => some-thing
        /// capsONInside => caps-on-inside
        /// CAPSOnOUTSIDE => caps-on-outside
        /// ALLCAPS => allcaps
        /// One1Two2Three3 => one1-two2-three3
        /// ONE1TWO2THREE3 => one1two2three3
        /// First_Second_ThirdHi => first_second_third-hi
        /// </example>
        public static string ToHtmlCase(string name)
        {
            return _htmlCaseRegex.Replace(name, HtmlCaseRegexReplacement).ToLowerInvariant();
        }

        /// <summary>
        /// 加密字符串。
        /// </summary>
        /// <param name="text">当前字符串。</param>
        /// <returns>返回加密后的字符串。</returns>
        public static string Encrypto(string text)
        {
            var rijndael = Rijndael.Create();
            rijndael.GenerateIV();
            rijndael.GenerateKey();
            var buffer = Encoding.UTF8.GetBytes(text);
            byte[] result;
            using (var ms = new MemoryStream())
            {
                var encrypto = rijndael.CreateEncryptor();
                var cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
                cs.Write(buffer, 0, buffer.Length);
                cs.FlushFinalBlock();
                result = ms.ToArray();
            }
            buffer = new byte[48 + result.Length];
            rijndael.IV.CopyTo(buffer, 0);
            result.CopyTo(buffer, 16);
            rijndael.Key.CopyTo(buffer, 16 + result.Length);
            return Convert.ToBase64String(buffer);
        }

        /// <summary>
        /// 解密字符串。
        /// </summary>
        /// <param name="text">当前字符串。</param>
        /// <returns>返回解密后的字符串。</returns>
        public static string Decrypto(string text)
        {
            var buffer = Convert.FromBase64String(text);
            var rijndael = Rijndael.Create();
            var iv = new byte[16];
            var key = new byte[32];
            using (var ms = new MemoryStream(buffer, 0, buffer.Length))
            {
                ms.Read(iv, 0, 16);
                buffer = new byte[buffer.Length - 48];
                ms.Read(buffer, 0, buffer.Length);
                ms.Read(key, 0, 32);
            }
            rijndael.IV = iv;
            rijndael.Key = key;
            using (var ms = new MemoryStream())
            {
                var encrypto = rijndael.CreateDecryptor();
                var cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
                using (var sr = new StreamReader(cs, Encoding.UTF8))
                    return sr.ReadToEnd();
            }
        }
    }
}