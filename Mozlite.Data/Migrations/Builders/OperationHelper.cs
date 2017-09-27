using System.Text;

namespace Mozlite.Data.Migrations.Builders
{
    internal class OperationHelper
    {
        public static string GetName(NameType type, string table, string[] columns = null, string alt = null)
        {
            var name = new StringBuilder();
            switch (type)
            {
                case NameType.PrimaryKey:
                    name.Append("PK_");
                    break;
                case NameType.UniqueKey:
                    name.Append("UK_");
                    break;
                case NameType.Index:
                    name.Append("IX_");
                    break;
                case NameType.ForeignKey:
                    name.Append("FK_");
                    break;
            }
            name.Append(table.Replace('.', '_'));
            if (columns != null)
                name.Append("_").Append(string.Join("_", columns));
            if (alt != null)
                name.Append("_").Append(alt.Replace('.', '_'));
            return name.ToString();
        }
    }
}