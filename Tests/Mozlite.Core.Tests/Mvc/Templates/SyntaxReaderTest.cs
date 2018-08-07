using System.Text;
using Mozlite.Mvc.Templates;
using Xunit;

namespace Mozlite.Core.Tests.Mvc.Templates
{
    public class SyntaxReaderTest
    {
        [Fact]
        public void Parse()
        {
            var syntax = new SyntaxManager();
            var document = syntax.Parse(@"!doctype
html(){
    head(){
        meta({http-equiv:""X-UA-Compatible"", content:""IE=edge""});
        meta({name:""viewport"", content:""width=device-width, initial-scale=1""});
        title(){@SiteName}
        meta({name:""keywords"", content:""@Keywords""});
        meta({name:""description"", content:""@Description""});
        link({href:""/dist/css/index.min.css"", rel:""stylesheet""});
        link({href:""/css/theme.css"", rel:""stylesheet""});
        script({type:""text/javascript"", src:""/js/jquery.min.js""}){}
    }
    body({class:""home""}){
        div({style:""display:none;"", 'test-escape':""\""转义符\\'dfder""}){}
    }
}");
            Assert.Equal("html", document.Name);
        }
    }
}
