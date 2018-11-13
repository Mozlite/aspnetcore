using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mozlite.Data.Migrations;
using Mozlite.Data.Migrations.Operations;
using System.Linq.Expressions;
using Mozlite.Data.Query;
using Mozlite.Extensions;

namespace Mozlite.Data.SqlServer.Migrations
{
    /// <summary>
    /// SQLServer数据库迁移SQL生成类。
    /// </summary>
    public class SqlServerMigrationsSqlGenerator : MigrationsSqlGenerator
    {
        private readonly IExpressionVisitorFactory _visitorFactory;

        /// <summary>
        /// 初始化类<see cref="SqlServerMigrationsSqlGenerator"/>。
        /// </summary>
        /// <param name="sqlHelper">SQL辅助接口。</param>
        /// <param name="typeMapper">类型匹配接口。</param>
        /// <param name="visitorFactory">条件表达式访问器工厂。</param>
        public SqlServerMigrationsSqlGenerator(ISqlHelper sqlHelper, ITypeMapper typeMapper, IExpressionVisitorFactory visitorFactory)
            : base(sqlHelper, typeMapper)
        {
            _visitorFactory = visitorFactory;
        }

        /// <summary>
        /// 新建索引。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        /// <param name="terminate">是否结束语句。</param>
        protected override void Generate(
            CreateIndexOperation operation,
            MigrationCommandListBuilder builder,
            bool terminate)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            base.Generate(operation, builder, false);

            if (operation.IsUnique
                && !operation.IsClustered)
            {
                builder.Append(" WHERE ");
                builder.Append(string.Join(" AND ",
                    operation.Columns.Select(c => string.Format("{0} IS NOT NULL", SqlHelper.DelimitIdentifier(c)))));
            }

            if (terminate)
            {
                builder
                    .AppendLine(SqlHelper.StatementTerminator)
                    .EndCommand();
            }
        }

        /// <summary>
        /// 修改列。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        protected override void Generate(
            AlterColumnOperation operation,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.ComputedColumnSql != null)
            {
                var dropColumnOperation = new DropColumnOperation
                {
                    Table = operation.Table,
                    Name = operation.Name
                };
                var addColumnOperation = new AddColumnOperation
                {
                    Table = operation.Table,
                    Name = operation.Name,
                    ClrType = operation.ClrType,
                    ColumnType = operation.ColumnType,
                    IsUnicode = operation.IsUnicode,
                    MaxLength = operation.MaxLength,
                    IsRowVersion = operation.IsRowVersion,
                    IsNullable = operation.IsNullable,
                    DefaultValue = operation.DefaultValue,
                    DefaultValueSql = operation.DefaultValueSql,
                    ComputedColumnSql = operation.ComputedColumnSql
                };
                Generate(dropColumnOperation, builder, false);
                builder.AppendLine(SqlHelper.StatementTerminator);
                Generate(addColumnOperation, builder, false);
                builder.AppendLine(SqlHelper.StatementTerminator);
                builder.EndCommand();
                return;
            }

            DropDefaultConstraint(operation.Table, operation.Name, builder);

            builder
                .Append("ALTER TABLE ")
                .Append(operation.Table)
                .Append(" ALTER COLUMN ");

            ColumnDefinition(
                operation.Table,
                operation.Name,
                operation.ClrType,
                operation.ColumnType,
                operation.IsUnicode,
                operation.MaxLength,
                operation.IsRowVersion,
                operation.IsIdentity,
                operation.IsNullable,
                /*defaultValue:*/ null,
                /*defaultValueSql:*/ null,
                operation.ComputedColumnSql,
                builder);

            builder.AppendLine(SqlHelper.StatementTerminator);

            if ((operation.DefaultValue != null)
                || (operation.DefaultValueSql != null))
            {
                builder
                    .Append("ALTER TABLE ")
                    .Append(operation.Table)
                    .Append(" ADD");
                DefaultValue(operation.DefaultValue, operation.DefaultValueSql, builder);
                builder
                    .Append(" FOR ")
                    .Append(SqlHelper.DelimitIdentifier(operation.Name))
                    .AppendLine(SqlHelper.StatementTerminator);
            }

            builder.EndCommand();
        }

        /// <summary>
        /// 删除索引。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        protected override void Generate(
            DropIndexOperation operation,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("DROP INDEX ")
                .Append(SqlHelper.DelimitIdentifier(operation.Name))
                .Append(" ON ")
                .Append(operation.Table);
        }

        /// <summary>
        /// 删除列。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        /// <param name="terminate">是否结束语句。</param>
        protected override void Generate(
            DropColumnOperation operation,
            MigrationCommandListBuilder builder,
            bool terminate)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            DropDefaultConstraint(operation.Table, operation.Name, builder);
            base.Generate(operation, builder, false);

            if (terminate)
            {
                builder
                    .AppendLine(SqlHelper.StatementTerminator)
                    .EndCommand();
            }
        }


        /// <summary>
        /// 添加索引的相关定义。
        /// </summary>
        /// <param name="clustered">是否聚合。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例。</param>
        protected override void IndexTraits(bool clustered, MigrationCommandListBuilder builder)
        {
            builder.Append(clustered ? "CLUSTERED " : "NONCLUSTERED ");
        }

        /// <summary>
        /// 添加外键操作的相关定义。
        /// </summary>
        /// <param name="referentialAction">外键操作。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例。</param>
        protected override void ForeignKeyAction(ReferentialAction referentialAction, MigrationCommandListBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            if (referentialAction == ReferentialAction.Restrict)
            {
                builder.Append("NO ACTION");
            }
            else
            {
                base.ForeignKeyAction(referentialAction, builder);
            }
        }

        /// <summary>
        /// 重命名列。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        protected override void Generate(
            RenameColumnOperation operation,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var qualifiedName = new StringBuilder();
            qualifiedName
                .Append(operation.Table)
                .Append(".")
                .Append(operation.Name);

            Rename(SqlHelper.DelimitIdentifier(qualifiedName.ToString()), SqlHelper.DelimitIdentifier(operation.NewName), "COLUMN", builder);
            builder.EndCommand();
        }

        /// <summary>
        /// 列定义。
        /// </summary>
        /// <param name="table">表格。</param>
        /// <param name="name">名称。</param>
        /// <param name="clrType">CLR类型。</param>
        /// <param name="type">字段类型。</param>
        /// <param name="unicode">是否Unicode字符集。</param>
        /// <param name="maxLength">大小。</param>
        /// <param name="rowVersion">是否为RowVersion列。</param>
        /// <param name="identity">是否自增长。</param>
        /// <param name="nullable">是否可空。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <param name="defaultValueSql">默认SQL字符串。</param>
        /// <param name="computedColumnSql">计算列的SQL字符串。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例。</param>
        protected override void ColumnDefinition(string table, string name, Type clrType, string type, bool? unicode, int? maxLength,
            bool rowVersion, bool identity, bool? nullable, object defaultValue, string defaultValueSql,
            string computedColumnSql, MigrationCommandListBuilder builder)
        {
            if (computedColumnSql != null)
            {
                builder
                    .Append(SqlHelper.DelimitIdentifier(name))
                    .Append(" AS ")
                    .Append(computedColumnSql);

                return;
            }

            base.ColumnDefinition(table, name, clrType, type, unicode, maxLength, rowVersion, identity, nullable, defaultValue, defaultValueSql, computedColumnSql, builder);

            if (identity)
            {
                builder.Append(" IDENTITY");
            }
        }

        /// <summary>
        /// 生成插入语句。
        /// </summary>
        /// <param name="entityType">当前实体。</param>
        /// <param name="instance">当前实例。</param>
        /// <returns>返回生成的SQL语句。</returns>
        protected override string GenerateSqlCreate(IEntityType entityType, object instance)
        {
            var builder = new IndentedStringBuilder();
            var items = new List<string>();
            var values = new List<string>();
            ForEachProperty(instance, (k, v) =>
            {
                items.Add(SqlHelper.DelimitIdentifier(k.Name));
                values.Add(SqlHelper.EscapeLiteral(v));
            });
            builder.Append("INSERT INTO ").Append(entityType.Table).Append("(");
            builder.JoinAppend(items);
            builder.Append(")VALUES(");
            builder.JoinAppend(values);
            builder.Append(")");
            return builder.ToString();
        }

        /// <summary>
        /// 生成更新语句。
        /// </summary>
        /// <param name="entityType">当前实体。</param>
        /// <param name="instance">当前实例。</param>
        /// <param name="where">条件表达式。</param>
        /// <returns>返回生成的SQL语句。</returns>
        protected override string GenerateSqlUpdate(IEntityType entityType, object instance, Expression @where)
        {
            var builder = new IndentedStringBuilder();
            builder.Append("UPDATE ").Append(entityType.Table).Append(" SET ");
            var items = new List<string>();
            ForEachProperty(instance, (k, v) =>
            {
                if (k.IsDefined(typeof(NotUpdatedAttribute), true))
                    return;
                items.Add($"{SqlHelper.DelimitIdentifier(k.Name)}={SqlHelper.EscapeLiteral(v)}");
            });
            builder.JoinAppend(items);
            var visitor = _visitorFactory.Create();
            visitor.Visit(where);
            builder.AppendEx(visitor.ToString(), " WHERE {0}");
            return builder.ToString();
        }

        /// <summary>
        /// 生成更新语句。
        /// </summary>
        /// <param name="entityType">当前实体。</param>
        /// <param name="where">条件表达式。</param>
        /// <returns>返回生成的SQL语句。</returns>
        protected override string GenerateSqlDelete(IEntityType entityType, Expression @where)
        {
            var builder = new IndentedStringBuilder();
            builder.Append("DELETE FROM ").Append(entityType.Table);
            var visitor = _visitorFactory.Create();
            visitor.Visit(where);
            builder.AppendEx(visitor.ToString(), " WHERE {0}");
            return builder.ToString();
        }

        /// <summary>
        /// 修改名称。
        /// </summary>
        /// <param name="name">原名称。</param>
        /// <param name="newName">新名称。</param>
        /// <param name="type">类型。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例。</param>
        protected virtual void Rename(
            string name,
            string newName,
            string type,
            MigrationCommandListBuilder builder)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(newName, nameof(newName));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("EXEC sp_rename ")
                .Append(SqlHelper.EscapeIdentifier(name))
                .Append(", ")
                .Append(SqlHelper.EscapeIdentifier(newName));

            if (type != null)
            {
                builder
                    .Append(", ")
                    .Append(SqlHelper.EscapeLiteral(type));
            }

            builder.AppendLine(SqlHelper.StatementTerminator);
        }

        /// <summary>
        /// 转换架构。
        /// </summary>
        /// <param name="newSchema">新架构。</param>
        /// <param name="schema">原有架构。</param>
        /// <param name="name">名称。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例。</param>
        protected virtual void Transfer(
            string newSchema,
            string schema,
            string name,
            MigrationCommandListBuilder builder)
        {
            Check.NotEmpty(newSchema, nameof(newSchema));
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER SCHEMA ")
                .Append(SqlHelper.DelimitIdentifier(newSchema))
                .Append(" TRANSFER ")
                .Append(SqlHelper.DelimitIdentifier(name, schema))
                .AppendLine(SqlHelper.StatementTerminator);
        }

        private int _variableCounter = 0;
        /// <summary>
        /// 删除默认值。
        /// </summary>
        /// <param name="table">表格。</param>
        /// <param name="columnName">列名。</param>
        /// <param name="builder">命令构建实例。</param>
        protected virtual void DropDefaultConstraint(
            string table,
            string columnName,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(table, nameof(table));
            Check.NotEmpty(columnName, nameof(columnName));
            Check.NotNull(builder, nameof(builder));

            var variable = "@var" + _variableCounter++;

            builder
                .Append("DECLARE ")
                .Append(variable)
                .AppendLine(" sysname;")
                .Append("SELECT ")
                .Append(variable)
                .AppendLine(" = [d].[name]")
                .AppendLine("FROM [sys].[default_constraints] [d]")
                .AppendLine("INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]")
                .Append("WHERE ([d].[parent_object_id] = OBJECT_ID(N'");

            builder
                .Append(SqlHelper.EscapeIdentifier(table))
                .Append("') AND [c].[name] = N'")
                .Append(SqlHelper.EscapeIdentifier(columnName))
                .AppendLine("');")
                .Append("IF ")
                .Append(variable)
                .Append(" IS NOT NULL EXEC(N'ALTER TABLE ")
                .Append(table)
                .Append(" DROP CONSTRAINT [' + ")
                .Append(variable)
                .Append(" + ']")
                .Append(SqlHelper.StatementTerminator)
                .Append("')")
                .AppendLine(SqlHelper.StatementTerminator);
        }
    }
}