using Xunit;

namespace Mozlite.Core.Tests
{
    public class CoresTest
    {
        [Fact]
        public void GeneralKey()
        {
            var key = Cores.GeneralKey(8);
            Assert.Equal(2, key.Length);
            key = Cores.GeneralKey(128);
            Assert.Equal(32, key.Length);
        }

        [Fact]
        public void Included(){
            var result = 1.Included(new[] { 1, 2, 34 });
            Assert.True(result);
            result = "11".Included(new[] { "12", "13" });
            Assert.False(result);
        }

        [Fact]
        public void Join(){
            var result = new[] { '1', '3', '3' }.Join();
            Assert.Equal("1,3,3", result);
            result = new[] { 1, 3, 3 }.Join();
            Assert.Equal("1,3,3", result);
            result = new[] { "1", "3", "3"}.Join();
            Assert.Equal("1,3,3", result);
        }

        [Fact]
        public void ToHtmlCase(){
            var result = Cores.ToHtmlCase("SomeThing");
            Assert.Equal("some-thing", result);
            result = Cores.ToHtmlCase("capsONInside");
            Assert.Equal("caps-on-inside", result);
            result = Cores.ToHtmlCase("CAPSOnOUTSIDE");
            Assert.Equal("caps-on-outside", result);
            result = Cores.ToHtmlCase("ALLCAPS");
            Assert.Equal("allcaps", result);
            result = Cores.ToHtmlCase("One1Two2Three3");
            Assert.Equal("one1-two2-three3", result);
            result = Cores.ToHtmlCase("ONE1TWO2THREE3");
            Assert.Equal("one1two2three3", result);
            result = Cores.ToHtmlCase("First_Second_ThirdHi");
            Assert.Equal("first_second_third-hi", result);
        }

        [Fact]
        public void EncryptoOrDecrypt(){
            const string text = "234#$23sdfsdfs$%$d4234234";
            var encryptor = Cores.Encrypto(text);
            encryptor = Cores.Decrypto(encryptor);
            Assert.Equal(text, encryptor);
        }
    }
}
