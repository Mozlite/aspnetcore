using Microsoft.AspNetCore.Mvc.Razor;

namespace Mozlite.Mvc.Templates
{
    public abstract class SyntaxPage<TModel> : RazorPage<TModel>
    {
        /// <summary>
        /// Writes the specified <paramref name="value" /> with HTML encoding to <see cref="P:Microsoft.AspNetCore.Mvc.Razor.RazorPageBase.Output" />.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object" /> to write.</param>
        public override void Write(object value)
        {
            base.Write(value);
        }
    }
}