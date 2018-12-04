using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using Mozlite.Extensions;
using System.Linq.Expressions;
using Mozlite.Data.Properties;
using System.Collections.Generic;
using Mozlite.Data.Migrations.Operations;

namespace Mozlite.Data.Migrations
{
    /// <summary>
    /// 数据迁移SQL生成类。
    /// </summary>
    public abstract class MigrationsSqlGenerator : IMigrationsSqlGenerator
    {
        private static readonly IReadOnlyDictionary<Type, Action<MigrationsSqlGenerator, MigrationOperation, MigrationCommandListBuilder>> _generateActions =
            new Dictionary<Type, Action<MigrationsSqlGenerator, MigrationOperation, MigrationCommandListBuilder>>
            {
                { typeof(AddColumnOperation), (g, o, b) => g.Generate((AddColumnOperation)o, b, true) },
                { typeof(AddForeignKeyOperation), (g, o, b) => g.Generate((AddForeignKeyOperation)o, b, true) },
                { typeof(AddPrimaryKeyOperation), (g, o, b) => g.Generate((AddPrimaryKeyOperation)o, b, true) },
                { typeof(AddUniqueConstraintOperation), (g, o, b) => g.Generate((AddUniqueConstraintOperation)o, b) },
                { typeof(AlterColumnOperation), (g, o, b) => g.Generate((AlterColumnOperation)o, b) },
                { typeof(AlterDatabaseOperation), (g, o, b) => g.Generate((AlterDatabaseOperation)o, b) },
                { typeof(AlterTableOperation), (g, o, b) => g.Generate((AlterTableOperation)o, b) },
                { typeof(CreateIndexOperation), (g, o, b) => g.Generate((CreateIndexOperation)o, b, true) },
                { typeof(CreateTableOperation), (g, o, b) => g.Generate((CreateTableOperation)o, b, true) },
                { typeof(DropColumnOperation), (g, o, b) => g.Generate((DropColumnOperation)o, b, true) },
                { typeof(DropForeignKeyOperation), (g, o, b) => g.Generate((DropForeignKeyOperation)o, b, true) },
                { typeof(DropIndexOperation), (g, o, b) => g.Generate((DropIndexOperation)o, b) },
                { typeof(DropPrimaryKeyOperation), (g, o, b) => g.Generate((DropPrimaryKeyOperation)o, b, true) },
                { typeof(DropTableOperation), (g, o, b) => g.Generate((DropTableOperation)o, b, true) },
                { typeof(DropUniqueConstraintOperation), (g, o, b) => g.Generate((DropUniqueConstraintOperation)o, b) },
                { typeof(RenameColumnOperation), (g, o, b) => g.Generate((RenameColumnOperation)o, b) },
                { typeof(RenameIndexOperation), (g, o, b) => g.Generate((RenameIndexOperation)o, b) },
                { typeof(RenameTableOperation), (g, o, b) => g.Generate((RenameTableOperation)o, b) },
                { typeof(SqlOperation), (g, o, b) => g.Generate((SqlOperation)o, b) }
            };

        /// <summary>
        /// 初始化类<see cref="MigrationsSqlGenerator"/>。
        /// </summary>
        /// <param name="sqlHelper">SQL辅助接口。</param>
        /// <param name="typeMapper">类型匹配接口。</param>
        protected MigrationsSqlGenerator(
             ISqlHelper sqlHelper,
             ITypeMapper typeMapper)
        {
            Check.NotNull(sqlHelper, nameof(sqlHelper));
            Check.NotNull(typeMapper, nameof(typeMapper));

            SqlHelper = sqlHelper;
            TypeMapper = typeMapper;
        }

        /// <summary>
        /// SQL辅助接口。
        /// </summary>
        protected ISqlHelper SqlHelper { get; }

        /// <summary>
        /// 类型匹配接口。
        /// </summary>
        protected ITypeMapper TypeMapper { get; }

        /// <summary>
        /// 将操作生成为SQL语句。
        /// </summary>
        /// <param name="operations">操作实例列表。</param>
        /// <returns>返回对应的SQL语句。</returns>
        public virtual MigrationCommandListBuilder Generate(IReadOnlyList<MigrationOperation> operations)
        {
            Check.NotNull(operations, nameof(operations));

            var builder = new MigrationCommandListBuilder();
            foreach (var operation in operations)
            {
                Generate(operation, builder);
            }

            return builder;
        }

        /// <summary>
        /// 解析SQL语句。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder">字符串构建实例。</param>
        protected virtual void Generate(
             MigrationOperation operation,
             MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            var operationType = operation.GetType();
            if (!_generateActions.TryGetValue(operationType, out var generateAction))
            {
                throw new InvalidOperationException(string.Format(Resources.UnknownOperation, operationType, GetType().DisplayName(false)));
            }

            generateAction(this, operation, builder);
        }

        /// <summary>
        /// 添加列。
        /// </summary>
        /// <param name="operation">添加列操作。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        /// <param name="terminate">是否添加结束符。</param>
        protected virtual void Generate(
             AddColumnOperation operation,
             MigrationCommandListBuilder builder,
            bool terminate)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(operation.Table)
                .Append(" ADD ");

            ColumnDefinition(operation, builder);

            if (terminate)
            {
                builder.AppendLine(SqlHelper.StatementTerminator);
                EndStatement(builder);
            }
        }

        /// <summary>
        /// 添加外键。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        /// <param name="terminate">是否添加结束符。</param>
        protected virtual void Generate(
             AddForeignKeyOperation operation,
             MigrationCommandListBuilder builder,
            bool terminate)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(operation.Table)
                .Append(" ADD ");

            ForeignKeyConstraint(operation, builder);

            if (terminate)
            {
                builder.AppendLine(SqlHelper.StatementTerminator);
                EndStatement(builder);
            }
        }

        /// <summary>
        /// 添加主键。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        /// <param name="terminate">是否添加结束符。</param>
        protected virtual void Generate(
             AddPrimaryKeyOperation operation,
             MigrationCommandListBuilder builder,
            bool terminate)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(operation.Table)
                .Append(" ADD ");
            PrimaryKeyConstraint(operation, builder);

            if (terminate)
            {
                builder.AppendLine(SqlHelper.StatementTerminator);
                EndStatement(builder);
            }
        }

        /// <summary>
        /// 添加唯一键。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        protected virtual void Generate(
             AddUniqueConstraintOperation operation,
             MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(operation.Table)
                .Append(" ADD ");
            UniqueConstraint(operation, builder);
            builder.AppendLine(SqlHelper.StatementTerminator);
            EndStatement(builder);
        }

        /// <summary>
        /// 修改列。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        protected virtual void Generate(
             AlterColumnOperation operation,
             MigrationCommandListBuilder builder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 修数据库。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        protected virtual void Generate(
             AlterDatabaseOperation operation,
             MigrationCommandListBuilder builder)
        {
        }

        /// <summary>
        /// 重命名索引。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        protected virtual void Generate(
             RenameIndexOperation operation,
             MigrationCommandListBuilder builder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 修改表格。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        protected virtual void Generate(
             AlterTableOperation operation,
             MigrationCommandListBuilder builder)
        {
        }

        /// <summary>
        /// 重命名表格。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        protected virtual void Generate(
             RenameTableOperation operation,
             MigrationCommandListBuilder builder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 新建索引。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        /// <param name="terminate">是否结束语句。</param>
        protected virtual void Generate(
             CreateIndexOperation operation,
             MigrationCommandListBuilder builder,
            bool terminate)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder.Append("CREATE ");

            if (operation.IsUnique)
            {
                builder.Append("UNIQUE ");
            }

            IndexTraits(operation.IsClustered, builder);

            builder
                .Append("INDEX ")
                .Append(operation.Name)
                .Append(" ON ")
                .Append(operation.Table)
                .Append(" (")
                .Append(ColumnList(operation.Columns))
                .Append(")");

            if (terminate)
            {
                builder.AppendLine(SqlHelper.StatementTerminator);
                EndStatement(builder);
            }
        }

        /// <summary>
        /// 新建表格。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        /// <param name="terminate">是否结束语句。</param>
        protected virtual void Generate(
             CreateTableOperation operation,
             MigrationCommandListBuilder builder,
            bool terminate)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("CREATE TABLE ")
                .Append(operation.Table)
                .AppendLine(" (");

            using (builder.Indent())
            {
                for (var i = 0; i < operation.Columns.Count; i++)
                {
                    var column = operation.Columns[i];
                    ColumnDefinition(column, builder);

                    if (i != operation.Columns.Count - 1)
                    {
                        builder.AppendLine(",");
                    }
                }

                if (operation.PrimaryKey != null)
                {
                    builder.AppendLine(",");
                    PrimaryKeyConstraint(operation.PrimaryKey, builder);
                }

                foreach (var uniqueConstraint in operation.UniqueConstraints)
                {
                    builder.AppendLine(",");
                    UniqueConstraint(uniqueConstraint, builder);
                }

                foreach (var foreignKey in operation.ForeignKeys)
                {
                    builder.AppendLine(",");
                    ForeignKeyConstraint(foreignKey, builder);
                }

                builder.AppendLine();
            }

            builder.Append(")");
            CreateTableAppender(builder);
            if (terminate)
            {
                builder.AppendLine(SqlHelper.StatementTerminator);
                EndStatement(builder);
            }
        }

        /// <summary>
        /// 添加表格后字符集得附加方法，主要用于MySQL表格格式，如：InnoDB。
        /// </summary>
        /// <param name="builder">构建实例。</param>
        protected virtual void CreateTableAppender(MigrationCommandListBuilder builder)
        {
            
        }

        /// <summary>
        /// 删除列。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        /// <param name="terminate">是否结束语句。</param>
        protected virtual void Generate(
             DropColumnOperation operation,
             MigrationCommandListBuilder builder,
            bool terminate)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(operation.Table)
                .Append(" DROP COLUMN ")
                .Append(SqlHelper.DelimitIdentifier(operation.Name));

            if (terminate)
            {
                builder.AppendLine(SqlHelper.StatementTerminator);
                EndStatement(builder);
            }
        }

        /// <summary>
        /// 删除外键。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        /// <param name="terminate">是否结束语句。</param>
        protected virtual void Generate(
             DropForeignKeyOperation operation,
             MigrationCommandListBuilder builder,
            bool terminate)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(operation.Table)
                .Append(" DROP CONSTRAINT ")
                .Append(operation.Name);

            if (terminate)
            {
                builder.AppendLine(SqlHelper.StatementTerminator);
                EndStatement(builder);
            }
        }

        /// <summary>
        /// 删除索引。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        protected virtual void Generate(
             DropIndexOperation operation,
             MigrationCommandListBuilder builder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 新建主键。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        /// <param name="terminate">是否结束语句。</param>
        protected virtual void Generate(
             DropPrimaryKeyOperation operation,
             MigrationCommandListBuilder builder,
            bool terminate)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(operation.Table)
                .Append(" DROP CONSTRAINT ")
                .Append(operation.Name);

            if (terminate)
            {
                builder.AppendLine(SqlHelper.StatementTerminator);
                EndStatement(builder);
            }
        }

        /// <summary>
        /// 删除表格。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        /// <param name="terminate">是否结束语句。</param>
        protected virtual void Generate(
             DropTableOperation operation,
             MigrationCommandListBuilder builder,
             bool terminate)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("DROP TABLE ")
                .Append(operation.Table);

            if (terminate)
            {
                builder.AppendLine(SqlHelper.StatementTerminator);
                EndStatement(builder);
            }
        }

        /// <summary>
        /// 删除唯一键。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        protected virtual void Generate(
             DropUniqueConstraintOperation operation,
             MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append("ALTER TABLE ")
                .Append(operation.Table)
                .Append(" DROP CONSTRAINT ")
                .Append(operation.Name)
                .AppendLine(SqlHelper.StatementTerminator);

            EndStatement(builder);
        }

        /// <summary>
        /// 重命名列。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        protected virtual void Generate(
             RenameColumnOperation operation,
             MigrationCommandListBuilder builder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SQL语句。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例对象。</param>
        protected virtual void Generate(
             SqlOperation operation,
             MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.EntityType != null)
            {
                var entity = operation.EntityType.GetEntityType();
                if (operation.Instance != null)
                {
                    if (operation.Expression == null)
                        operation.Sql = GenerateSqlCreate(entity, operation.Instance);
                    else
                        operation.Sql = GenerateSqlUpdate(entity, operation.Instance, operation.Expression);
                }
                else
                    operation.Sql = GenerateSqlDelete(entity, operation.Expression);
            }
            builder
                .Append(operation.Sql)
                .AppendLine(SqlHelper.StatementTerminator);

            EndStatement(builder);
        }

        /// <summary>
        /// 循环访问每个属性。
        /// </summary>
        /// <param name="statement">匿名对象。</param>
        /// <param name="action">执行每一项。</param>
        protected virtual void ForEachProperty(object statement, Action<PropertyInfo, object> action)
        {
            var type = statement.GetType();
            var isAnonymous = type.IsAnonymous();
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (isAnonymous || property.IsCreatable())
                {
                    var value = property.GetValue(statement);
                    action(property, value);
                }
            }
        }

        /// <summary>
        /// 生成插入语句。
        /// </summary>
        /// <param name="entityType">当前实体。</param>
        /// <param name="instance">当前实例。</param>
        /// <returns>返回生成的SQL语句。</returns>
        protected abstract string GenerateSqlCreate(IEntityType entityType, object instance);

        /// <summary>
        /// 生成更新语句。
        /// </summary>
        /// <param name="entityType">当前实体。</param>
        /// <param name="instance">当前实例。</param>
        /// <param name="where">条件表达式。</param>
        /// <returns>返回生成的SQL语句。</returns>
        protected abstract string GenerateSqlUpdate(IEntityType entityType, object instance, Expression where);

        /// <summary>
        /// 生成更新语句。
        /// </summary>
        /// <param name="entityType">当前实体。</param>
        /// <param name="where">条件表达式。</param>
        /// <returns>返回生成的SQL语句。</returns>
        protected abstract string GenerateSqlDelete(IEntityType entityType, Expression where);
        
        /// <summary>
        /// 添加列的相关定义。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例。</param>
        protected virtual void ColumnDefinition(
                 AddColumnOperation operation,
                 MigrationCommandListBuilder builder)
            => ColumnDefinition(
                operation.Table,
                operation.Name,
                operation.ClrType,
                operation.ColumnType,
                operation.IsUnicode,
                operation.MaxLength,
                operation.IsRowVersion,
                operation.IsIdentity,
                operation.IsNullable,
                operation.DefaultValue,
                operation.DefaultValueSql,
                operation.ComputedColumnSql,
                builder);

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
        protected virtual void ColumnDefinition(
            string table,
            string name,
            Type clrType,
            string type,
            bool? unicode,
            int? maxLength,
            bool rowVersion,
            bool identity,
            bool? nullable,
            object defaultValue,
            string defaultValueSql,
            string computedColumnSql,
            MigrationCommandListBuilder builder)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(builder, nameof(builder));

            builder
                .Append(SqlHelper.DelimitIdentifier(name))
                .Append(" ")
                .Append(type ?? TypeMapper.GetMapping(clrType, maxLength, rowVersion, unicode));

            if (nullable == false)
            {
                builder.Append(" NOT NULL");
            }

            if (!identity)
                DefaultValue(defaultValue, defaultValueSql, builder);
        }

        /// <summary>
        /// 添加默认值。
        /// </summary>
        /// <param name="defaultValue">默认值。</param>
        /// <param name="defaultValueSql">默认值SQL字符串。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例。</param>
        protected virtual void DefaultValue(
             object defaultValue,
             string defaultValueSql,
             MigrationCommandListBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            if (defaultValueSql != null)
            {
                builder
                    .Append(" DEFAULT (")
                    .Append(defaultValueSql)
                    .Append(")");
            }
            else if (defaultValue != null)
            {
                builder
                    .Append(" DEFAULT ")
                    .Append(SqlHelper.EscapeLiteral(defaultValue));
            }
        }

        /// <summary>
        /// 添加外键定义。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例。</param>
        protected virtual void ForeignKeyConstraint(
             AddForeignKeyOperation operation,
             MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.Name != null)
            {
                builder
                    .Append("CONSTRAINT ")
                    .Append(SqlHelper.DelimitIdentifier(operation.Name))
                    .Append(" ");
            }

            builder
                .Append("FOREIGN KEY (")
                .Append(ColumnList(operation.Columns))
                .Append(") REFERENCES ")
                .Append(operation.PrincipalTable);

            if (operation.PrincipalColumns != null)
            {
                builder
                    .Append(" (")
                    .Append(ColumnList(operation.PrincipalColumns))
                    .Append(")");
            }

            if (operation.OnUpdate != ReferentialAction.NoAction)
            {
                builder.Append(" ON UPDATE ");
                ForeignKeyAction(operation.OnUpdate, builder);
            }

            if (operation.OnDelete != ReferentialAction.NoAction)
            {
                builder.Append(" ON DELETE ");
                ForeignKeyAction(operation.OnDelete, builder);
            }
        }

        /// <summary>
        /// 添加主键的相关定义。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例。</param>
        protected virtual void PrimaryKeyConstraint(
             AddPrimaryKeyOperation operation,
             MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.Name != null)
            {
                builder
                    .Append("CONSTRAINT ")
                    .Append(SqlHelper.DelimitIdentifier(operation.Name))
                    .Append(" ");
            }

            builder
                .Append("PRIMARY KEY ");

            IndexTraits(operation.IsClustered, builder);

            builder.Append("(")
                .Append(ColumnList(operation.Columns))
                .Append(")");
        }

        /// <summary>
        /// 添加唯一键的相关定义。
        /// </summary>
        /// <param name="operation">操作实例。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例。</param>
        protected virtual void UniqueConstraint(
             AddUniqueConstraintOperation operation,
             MigrationCommandListBuilder builder)
        {
            Check.NotNull(operation, nameof(operation));
            Check.NotNull(builder, nameof(builder));

            if (operation.Name != null)
            {
                builder
                    .Append("CONSTRAINT ")
                    .Append(SqlHelper.DelimitIdentifier(operation.Name))
                    .Append(" ");
            }

            builder
                .Append("UNIQUE ");

            IndexTraits(operation.IsClustered, builder);

            builder.Append("(")
                .Append(ColumnList(operation.Columns))
                .Append(")");
        }

        /// <summary>
        /// 添加索引的相关定义。
        /// </summary>
        /// <param name="clustered">是否聚合。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例。</param>
        protected virtual void IndexTraits(bool clustered, MigrationCommandListBuilder builder)
        {
        }

        /// <summary>
        /// 添加外键操作的相关定义。
        /// </summary>
        /// <param name="referentialAction">外键操作。</param>
        /// <param name="builder"><see cref="MigrationCommandListBuilder"/>实例。</param>
        protected virtual void ForeignKeyAction(
            ReferentialAction referentialAction,
            MigrationCommandListBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            switch (referentialAction)
            {
                case ReferentialAction.Restrict:
                    builder.Append("RESTRICT");
                    break;
                case ReferentialAction.Cascade:
                    builder.Append("CASCADE");
                    break;
                case ReferentialAction.SetNull:
                    builder.Append("SET NULL");
                    break;
                case ReferentialAction.SetDefault:
                    builder.Append("SET DEFAULT");
                    break;
                default:
                    Debug.Assert(
                        referentialAction == ReferentialAction.NoAction,
                        "Unexpected value: " + referentialAction);
                    break;
            }
        }

        protected virtual void EndStatement(MigrationCommandListBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));
            builder.EndCommand();
        }

        /// <summary>
        /// 拼接列。
        /// </summary>
        /// <param name="columns">列集合。</param>
        /// <returns>返回拼接后的字符串。</returns>
        protected virtual string ColumnList(string[] columns)
            => string.Join(", ", columns.Select(SqlHelper.DelimitIdentifier));
    }
}