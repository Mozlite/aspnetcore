using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Mozlite.Extensions.Storages.Apis
{
    /// <summary>
    /// 验证码。
    /// </summary>
    public class Verifiers
    {
        /// <summary>
        /// 加密验证码。
        /// </summary>
        /// <param name="code">验证码。</param>
        /// <returns>返回加密后的验证码。</returns>
        public static string Hashed(string code)
        {
            const string salt = "AmazingValidateCodeForMozlite!";
            return Cores.Md5(Cores.Sha1(salt + code.ToUpper()));
        }

        private static readonly Random _random = new Random();
        /// <summary>
        /// 随机生成六位数字。
        /// </summary>
        /// <returns>返回随机数。</returns>
        public static string RandomNumbers()
        {
            return _random.Next(100000, 999999).ToString();
        }

        private const string Codes = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,P,Q,R,S,T,U,V,W,X,Y,Z";
        /// <summary>  
        /// 该方法用于生成指定位数的随机数  
        /// </summary>  
        /// <param name="length">参数是随机数的位数</param>  
        /// <returns>返回一个随机数字符串</returns>  
        private static string Random(int length)
        {
            //验证码可以显示的字符集合  
            var codes = Codes.Split(',');//拆分成数组   
            var code = "";//产生的随机数  
            var temp = -1;//记录上次随机数值，尽量避避免生产几个一样的随机数  

            var rand = new Random();
            //采用一个简单的算法以保证生成随机数的不同  
            for (var i = 1; i < length + 1; i++)
            {
                if (temp != -1)
                    rand = new Random(i * temp * unchecked((int)DateTime.Now.Ticks));//初始化随机类 
                var index = rand.Next(codes.Length);//获取随机数  
                if (temp != -1 && temp == index)
                    return Random(length);//如果获取的随机数重复，则递归调用  
                temp = index;//把本次产生的随机数记录起来  
                code += codes[index];//随机数的位数加一  
            }
            return code;
        }

        /// <summary>  
        /// 该方法是将生成的随机数写入图像文件  
        /// </summary>  
        /// <param name="code">code是一个随机数</param>
        /// <param name="numbers">生成位数（默认4位）</param>
        /// <param name="fontSize">字体大小。</param>
        /// <param name="height">高度。</param>  
        public static MemoryStream Create(out string code, int numbers = 6, int fontSize = 16, int height = 32)
        {
            code = Random(numbers);
            var random = new Random();
            //验证码颜色集合  
            Color[] colors = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.DarkGoldenrod, Color.Brown, Color.DarkCyan, Color.Purple };

            //验证码字体集合
            string[] fonts = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };

            var width = fontSize + 2;
            //定义图像的大小，生成图像的实例  
            var bitmap = new Bitmap(code.Length * width + 20, height);
            var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);//背景设为白色  

            //在随机位置画背景点  
            for (var i = 0; i < 50; i++)
            {
                var x = random.Next(bitmap.Width);
                var y = random.Next(bitmap.Height);
                graphics.DrawRectangle(new Pen(colors[(x + y) % 8], 0), x, y, 1, 1);
            }
            //验证码绘制在g中  
            for (var i = 0; i < code.Length; i++)
            {
                var cindex = random.Next(colors.Length);//随机颜色索引值  
                var findex = random.Next(fonts.Length);//随机字体索引值  
                var f = new Font(fonts[findex], fontSize, FontStyle.Bold);//字体  
                var b = new SolidBrush(colors[cindex]);//颜色  
                var ii = 4;
                if ((i + 1) % 2 == 0)//控制验证码不在同一高度  
                {
                    ii = 2;
                }
                graphics.DrawString(code.Substring(i, 1), f, b, 3 + (i * width), ii);//绘制一个验证字符  
            }
            var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Jpeg);//将此图像以Png图像文件的格式保存到流中  

            //回收资源  
            graphics.Dispose();
            bitmap.Dispose();
            return ms;
        }
    }
}