using System;
using System.Linq.Expressions;
using Xunit;

namespace Mozlite.Core.Tests
{
    public class ExpressionExtensionsTest
    {
        public class TestClass
        {
            public string TestName { get; set; }
        }

        [Fact]
        public void GetPropertyName()
        {
            Expression<Func<TestClass, object>> expression = x => x.TestName;
            var name = expression.GetPropertyName();
            Assert.Equal("TestName", name);
        }
    }
}