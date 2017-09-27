namespace Mozlite.Data.Internal
{
    internal static class PrefixExtensions
    {
        internal static string ReplacePrefix(this string commandText, string prefix)
        {
            return commandText.Replace("$pre:$", string.Empty).Replace("$pre:", prefix);
        }
    }
}