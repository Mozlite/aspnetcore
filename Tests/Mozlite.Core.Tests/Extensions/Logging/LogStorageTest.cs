using System;
using Mozlite.Extensions.Logging;
using Xunit;

namespace Mozlite.Core.Tests.Extensions.Logging
{
    public class LogStorageTest
    {
        public class Storage
        {
            public string Name { get; set; }

            public int Id { get; set; }

            public bool IsChecked { get; set; }

            public static Storage Create() => new Storage { Id = 1, IsChecked = true, Name = "测试名称" };
        }

        [Fact]
        public void Diff()
        {
            var storage = Storage.Create();
            var log = new LogStorage(storage);
            storage.Name = "正式名称";
            storage.IsChecked = false;
            var diff = log.Diff(storage);
            Assert.True(diff);
            Console.WriteLine(log);
            storage.Name = null;
            diff = log.Diff(storage);
            Assert.True(diff);
            Console.WriteLine(log);
        }
    }
}