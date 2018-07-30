using System.Text;

namespace Mozlite.Mvc.TagHelpers.Templates
{
    internal static class AttributeExtensions
    {
        public static StringBuilder SetJsAttribute(this StringBuilder sb, string name, string value)
        {
            sb.Append("html+=' ").Append(name).Append("=\"'+").Append(value).Append("+'\"';");
            return sb;
        }

        public static StringBuilder MergeJsAttribute(this StringBuilder sb, string name, string value, string merger, string seperator = " ")
        {
            sb.Append("html+=' ").Append(name).Append("=\"").Append(value);
            sb.Append(seperator).Append(merger);
            sb.Append("\"';");
            return sb;
        }
    }
}