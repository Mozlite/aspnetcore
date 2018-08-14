using System.Linq;
using Mozlite.Mvc.Templates;
using Mozlite.Mvc.Templates.Codings;
using Xunit;

namespace Mozlite.Mvc.Tests.Mvc.Templates
{
    public class SyntaxReaderTest
    {
        private readonly ISyntaxManager _syntaxManager;
        public SyntaxReaderTest()
        {
            _syntaxManager = Tests.GetRequiredService<ISyntaxManager>();
        }

        [Fact]
        public void Parse()
        {
            var document = _syntaxManager.Parse(@"!doctype
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
        div({style:""display:none;"", 'test-escape':""{转义符\\'dfder""}){
            @if(TEst == ""test )""){
                div(){
                    script({type:""text/javascript""}){
                        function test(){alert('xxx');}
                    }
                }
            }else if(Name==String.Empty){
                script({type:""text/javascript""}){}
            }else{
                !!注释测试
                alert();
            }
        }
    }
}");
            var html = document.FirstOrDefault();
            Assert.Equal("html", html?.Name);

            var render = _syntaxManager.Render(document, null);
        }

        [Fact]
        public void ReadParameters()
        {
            var reader = new CodeReader(@"TEst == ""test )""){");
            var parameters = reader.ReadParameters();
            Assert.NotNull(parameters);
            Assert.Equal(@"TEst == ""test )""", parameters[0]);
        }

        [Fact]
        public void IfSyntax()
        {
            var document = _syntaxManager.Parse(
                @"@if(TEst == ""test )""){div(){}}else if(Name==String.Empty){script({type:""text/javascript""}){}}else{
!!注释测试
alert();
}");
            var syntax = document.FirstOrDefault();
            Assert.IsType<IfSyntax>(syntax);
        }
    }
}
