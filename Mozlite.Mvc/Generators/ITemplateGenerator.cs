using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Mozlite.Extensions;
using Mozlite.Mvc.TagHelpers.Templates;

namespace Mozlite.Mvc.Generators
{
    /// <summary>
    /// 模板生成器。
    /// </summary>
    public interface ITemplateGenerator : ISingletonService
    {
        /// <summary>
        /// 生成HTML字符串。
        /// </summary>
        /// <param name="path">路径。</param>
        /// <param name="model">当前模型对象。</param>
        /// <returns>返回生成的HTML字符串。</returns>
        Task<string> GenerateAsync(string path, object model);
    }

    public class TemplateGenerator : ITemplateGenerator
    {
        private readonly ITemplateExecutor _executor;

        public TemplateGenerator(ITemplateExecutor executor)
        {
            _executor = executor;
        }

        /// <summary>
        /// 生成HTML字符串。
        /// </summary>
        /// <param name="path">路径。</param>
        /// <param name="model">当前模型对象。</param>
        /// <returns>返回生成的HTML字符串。</returns>
        public async Task<string> GenerateAsync(string path, object model)
        {
            var source = await ReadSourceAsync(path);
            if (string.IsNullOrWhiteSpace(source))
                return source;
            var template = new TemplateDocument(source);
            return template.ToHtmlString(_executor, model, GetValue);
        }

        private object GetValue(object instance, string name)
        {
            if (instance is IDictionary<string, object> dic)
            {
                if (dic.TryGetValue(name, out var value))
                    return value;
                return null;
            }
            return instance.GetType().GetEntityType().FindProperty(name)?.Get(instance);
        }

        private async Task<string> ReadSourceAsync(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(fs, Encoding.UTF8))
                return await reader.ReadToEndAsync();
        }
    }
}