using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Mozlite.Data.Migrations.Builders;
using Mozlite.Data.Migrations.Operations;
using Mozlite.Extensions;

namespace Mozlite.Data.Migrations
{
    /// <summary>
    /// 迁移构建实例。
    /// </summary>
    public class MigrationBuilder
    {
        /// <summary>
        /// 初始化类<see cref="MigrationBuilder"/>。
        /// </summary>
        /// <param name="activeProvider">数据库提供者。</param>
        public MigrationBuilder(string activeProvider)
        {
            ActiveProvider = activeProvider;
        }

        /// <summary>
        /// 激活的提供者。
        /// </summary>
        public virtual string ActiveProvider { get; }

        /// <summary>
        /// 操作列表。
        /// </summary>
        public virtual List<MigrationOperation> Operations { get; } = new List<MigrationOperation>();

        /// <summary>
        /// 添加列。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="column">列表达式。</param>
        /// <param name="type">字段类型。</param>
        /// <param name="unicode">是否为Unicode编码。</param>
        /// <param name="nullable">是否为空。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <param name="defaultValueSql">默认值SQL字符串。</param>
        /// <param name="computedColumnSql">计算列的SQL字符串。</param>
        /// <returns>返回操作实例。</returns>
        public virtual OperationBuilder<AddColumnOperation> AddColumn<TEntity>(
             Expression<Func<TEntity, object>> column,
             string type = null,
             bool? nullable = null,
             bool? unicode = null,
             object defaultValue = null,
             string defaultValueSql = null,
             string computedColumnSql = null)
        {
            Check.NotNull(column, nameof(column));

            var property = typeof(TEntity).GetEntityType().FindProperty(column.GetPropertyAccess().Name);
            var operation = new AddColumnOperation
            {
                Table = typeof(TEntity).GetTableName(),
                Name = property.Name,
                ClrType = property.ClrType,
                ColumnType = type,
                IsUnicode = unicode,
                IsIdentity = property.IsIdentity,
                MaxLength = property.MaxLength,
                IsRowVersion = property.IsRowVersion,
                IsNullable = nullable ?? property.IsNullable,
                DefaultValue = defaultValue,
                DefaultValueSql = defaultValueSql,
                ComputedColumnSql = computedColumnSql
            };
            Operations.Add(operation);

            return new OperationBuilder<AddColumnOperation>(operation);
        }

        /// <summary>
        /// 添加外键。
        /// </summary>
        /// <typeparam name="TEntity">实体类。</typeparam>
        /// <typeparam name="TPrincipal">主键类。</typeparam>
        /// <param name="columns">字段。</param>
        /// <param name="principalColumns">主键列。</param>
        /// <param name="onUpdate">更新时候对应的操作。</param>
        /// <param name="onDelete">删除时候对应的操作。</param>
        /// <returns>返回迁移构建实例。</returns>
        public virtual OperationBuilder<AddForeignKeyOperation> AddForeignKey<TEntity, TPrincipal>(
             Expression<Func<TEntity, object>> columns,
             Expression<Func<TPrincipal, object>> principalColumns = null,
            ReferentialAction onUpdate = ReferentialAction.NoAction,
            ReferentialAction onDelete = ReferentialAction.NoAction)
        {
            Check.NotNull(columns, nameof(columns));

            var operation = new AddForeignKeyOperation
            {
                Table = typeof(TEntity).GetTableName(),
                Columns = columns.GetPropertyNames(),
                PrincipalTable = typeof(TPrincipal).GetTableName(),
                OnUpdate = onUpdate,
                OnDelete = onDelete
            };
            if (principalColumns == null)
                operation.PrincipalColumns = operation.Columns;
            else
                operation.PrincipalColumns = principalColumns.GetPropertyNames();
            operation.Name = OperationHelper.GetName(NameType.ForeignKey, operation.Table, operation.Columns, operation.PrincipalTable);
            Operations.Add(operation);

            return new OperationBuilder<AddForeignKeyOperation>(operation);
        }

        /// <summary>
        /// 添加主键。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="columns">列表达式。</param>
        /// <param name="clustered">是否聚合。</param>
        /// <returns>返回构建实例对象。</returns>
        public virtual OperationBuilder<AddPrimaryKeyOperation> AddPrimaryKey<TEntity>(Expression<Func<TEntity, object>> columns, bool clustered = true)
        {
            Check.NotNull(columns, nameof(columns));

            var operation = new AddPrimaryKeyOperation
            {
                Table = typeof(TEntity).GetTableName(),
                Columns = columns.GetPropertyNames(),
                IsClustered = clustered
            };
            operation.Name = OperationHelper.GetName(NameType.PrimaryKey, operation.Table);
            Operations.Add(operation);

            return new OperationBuilder<AddPrimaryKeyOperation>(operation);
        }

        /// <summary>
        /// 添加唯一键。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="columns">列表达式。</param>
        /// <param name="clustered">是否聚合。</param>
        /// <returns>返回构建实例对象。</returns>
        public virtual OperationBuilder<AddUniqueConstraintOperation> AddUniqueConstraint<TEntity>(Expression<Func<TEntity, object>> columns, bool clustered = false)
        {
            Check.NotNull(columns, nameof(columns));

            var operation = new AddUniqueConstraintOperation
            {
                Table = typeof(TEntity).GetTableName(),
                Columns = columns.GetPropertyNames(),
                IsClustered = clustered
            };
            operation.Name = OperationHelper.GetName(NameType.UniqueKey, operation.Table, operation.Columns);
            Operations.Add(operation);

            return new OperationBuilder<AddUniqueConstraintOperation>(operation);
        }

        /// <summary>
        /// 修改列。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="type">字段类型。</param>
        /// <param name="unicode">是否为Unicode编码。</param>
        /// <param name="column">列表达式。</param>
        /// <param name="nullable">是否为空。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <param name="defaultValueSql">默认值SQL字符串。</param>
        /// <param name="computedColumnSql">计算列的SQL字符串。</param>
        /// <returns>返回操作实例。</returns>
        public virtual AlterOperationBuilder<AlterColumnOperation> AlterColumn<TEntity>(
             Expression<Func<TEntity, object>> column,
             string type = null,
             bool? unicode = null,
             bool? nullable = null,
             object defaultValue = null,
             string defaultValueSql = null,
             string computedColumnSql = null)
        {
            Check.NotNull(column, nameof(column));
            var property = typeof(TEntity).GetEntityType().FindProperty(column.GetPropertyAccess().Name);
            var operation = new AlterColumnOperation
            {
                Table = typeof(TEntity).GetTableName(),
                Name = property.Name,
                ClrType = property.ClrType,
                ColumnType = type,
                IsUnicode = unicode,
                IsIdentity = property.IsIdentity,
                MaxLength = property.MaxLength,
                IsRowVersion = property.IsRowVersion,
                IsNullable = nullable ?? property.IsNullable,
                DefaultValue = defaultValue,
                DefaultValueSql = defaultValueSql,
                ComputedColumnSql = computedColumnSql
            };

            Operations.Add(operation);

            return new AlterOperationBuilder<AlterColumnOperation>(operation);
        }

        /// <summary>
        /// 修改数据库。
        /// </summary>
        /// <returns>返回构建实例。</returns>
        public virtual AlterOperationBuilder<AlterDatabaseOperation> AlterDatabase()
        {
            var operation = new AlterDatabaseOperation();
            Operations.Add(operation);

            return new AlterOperationBuilder<AlterDatabaseOperation>(operation);
        }

        /// <summary>
        /// 修改序列号。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="schema">架构。</param>
        /// <param name="incrementBy">增量。</param>
        /// <param name="minValue">最小值。</param>
        /// <param name="maxValue">最大值。</param>
        /// <param name="cyclic">是否循环。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual AlterOperationBuilder<AlterSequenceOperation> AlterSequence(
            string name,
            string schema = null,
            int incrementBy = 1,
            long? minValue = null,
            long? maxValue = null,
            bool cyclic = false)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new AlterSequenceOperation
            {
                Schema = schema,
                Name = name,
                IncrementBy = incrementBy,
                MinValue = minValue,
                MaxValue = maxValue,
                IsCyclic = cyclic
            };
            Operations.Add(operation);

            return new AlterOperationBuilder<AlterSequenceOperation>(operation);
        }

        /// <summary>
        /// 修改表格。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <returns>返回迁移实例。</returns>
        public virtual AlterOperationBuilder<AlterTableOperation> AlterTable<TEntity>()
        {
            var operation = new AlterTableOperation
            {
                Table = typeof(TEntity).GetTableName()
            };
            Operations.Add(operation);
            return new AlterOperationBuilder<AlterTableOperation>(operation);
        }

        /// <summary>
        /// 新建索引。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="columns">列表达式。</param>
        /// <param name="unique">是否唯一。</param>
        /// <param name="clustered">是否聚合。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<CreateIndexOperation> CreateIndex<TEntity>(
            Expression<Func<TEntity, object>> columns,
            bool unique = false,
            bool clustered = false)
        {
            Check.NotNull(columns, nameof(columns));

            var operation = new CreateIndexOperation
            {
                Table = typeof(TEntity).GetTableName(),
                Columns = columns.GetPropertyNames(),
                IsUnique = unique,
                IsClustered = clustered
            };
            operation.Name = OperationHelper.GetName(NameType.Index, operation.Table, operation.Columns);
            Operations.Add(operation);

            return new OperationBuilder<CreateIndexOperation>(operation);
        }

        /// <summary>
        /// 确认架构。
        /// </summary>
        /// <param name="name">架构名称。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<EnsureSchemaOperation> EnsureSchema(
             string name)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new EnsureSchemaOperation
            {
                Name = name
            };
            Operations.Add(operation);

            return new OperationBuilder<EnsureSchemaOperation>(operation);
        }

        /// <summary>
        /// 新建序列号。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="schema">架构。</param>
        /// <param name="startValue">起始值。</param>
        /// <param name="incrementBy">增量。</param>
        /// <param name="minValue">最小值。</param>
        /// <param name="maxValue">最大值。</param>
        /// <param name="cyclic">是否循环。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<CreateSequenceOperation> CreateSequence(
            string name,
            string schema = null,
            long startValue = 1L,
            int incrementBy = 1,
            long? minValue = null,
            long? maxValue = null,
            bool cyclic = false)
            => CreateSequence<long>(name, schema, startValue, incrementBy, minValue, maxValue, cyclic);

        /// <summary>
        /// 新建序列号。
        /// </summary>
        /// <typeparam name="T">列类型。</typeparam>
        /// <param name="name">名称。</param>
        /// <param name="schema">架构。</param>
        /// <param name="startValue">起始值。</param>
        /// <param name="incrementBy">增量。</param>
        /// <param name="minValue">最小值。</param>
        /// <param name="maxValue">最大值。</param>
        /// <param name="cyclic">是否循环。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<CreateSequenceOperation> CreateSequence<T>(
            string name,
            string schema = null,
            long startValue = 1L,
            int incrementBy = 1,
            long? minValue = null,
            long? maxValue = null,
            bool cyclic = false)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new CreateSequenceOperation
            {
                Schema = schema,
                Name = name,
                ClrType = typeof(T),
                StartValue = startValue,
                IncrementBy = incrementBy,
                MinValue = minValue,
                MaxValue = maxValue,
                IsCyclic = cyclic
            };
            Operations.Add(operation);

            return new OperationBuilder<CreateSequenceOperation>(operation);
        }

        /// <summary>
        /// 新建表格。
        /// </summary>
        /// <typeparam name="TEntity">表格实体类型。</typeparam>
        /// <param name="action">表格列定义表达式。</param>
        /// <returns>返回当前构建实例。</returns>
        public virtual CreateTableBuilder<TEntity> CreateTable<TEntity>(Action<CreateTableBuilder<TEntity>> action)
        {
            Check.NotNull(action, nameof(action));

            var createTableOperation = new CreateTableOperation
            {
                Table = typeof(TEntity).GetTableName()
            };

            var builder = new CreateTableBuilder<TEntity>(createTableOperation);
            action(builder);
            if (createTableOperation.PrimaryKey == null)
                builder.PrimaryKey();

            Operations.Add(createTableOperation);

            return builder;
        }

        /// <summary>
        /// 删除列。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="name">列名称。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropColumnOperation> DropColumn<TEntity>(Expression<Func<TEntity, object>> name)
        {
            return DropColumn<TEntity>(name.GetPropertyAccess().Name);
        }

        /// <summary>
        /// 删除列。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="name">列名称。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropColumnOperation> DropColumn<TEntity>(string name)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new DropColumnOperation
            {
                Table = typeof(TEntity).GetTableName(),
                Name = name
            };
            Operations.Add(operation);
            return new OperationBuilder<DropColumnOperation>(operation);
        }

        /// <summary>
        /// 删除外键。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="columns">列表达式。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropForeignKeyOperation> DropForeignKey<TEntity>(
             Expression<Func<TEntity, object>> columns)
        {
            Check.NotNull(columns, nameof(columns));

            var operation = new DropForeignKeyOperation
            {
                Table = typeof(TEntity).GetTableName()
            };
            operation.Name = OperationHelper.GetName(NameType.ForeignKey, operation.Table, columns.GetPropertyNames());
            Operations.Add(operation);

            return new OperationBuilder<DropForeignKeyOperation>(operation);
        }

        /// <summary>
        /// 删除外键。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="name">外键名称(全名，包含FK_)。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropForeignKeyOperation> DropForeignKey<TEntity>(string name)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new DropForeignKeyOperation
            {
                Table = typeof(TEntity).GetTableName(),
                Name = name
            };
            Operations.Add(operation);

            return new OperationBuilder<DropForeignKeyOperation>(operation);
        }

        /// <summary>
        /// 删除索引。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="columns">列表达式。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropIndexOperation> DropIndex<TEntity>(
             Expression<Func<TEntity, object>> columns)
        {
            Check.NotNull(columns, nameof(columns));

            var operation = new DropIndexOperation
            {
                Table = typeof(TEntity).GetTableName()
            };
            operation.Name = OperationHelper.GetName(NameType.Index, operation.Table, columns.GetPropertyNames());
            Operations.Add(operation);

            return new OperationBuilder<DropIndexOperation>(operation);
        }

        /// <summary>
        /// 删除索引。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="name">索引名称（全名，包含IX_）。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropIndexOperation> DropIndex<TEntity>(string name)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new DropIndexOperation
            {
                Table = typeof(TEntity).GetTableName(),
                Name = name
            };
            Operations.Add(operation);

            return new OperationBuilder<DropIndexOperation>(operation);
        }

        /// <summary>
        /// 删除主键。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropPrimaryKeyOperation> DropPrimaryKey<TEntity>()
        {
            var operation = new DropPrimaryKeyOperation
            {
                Table = typeof(TEntity).GetTableName()
            };
            operation.Name = OperationHelper.GetName(NameType.PrimaryKey, operation.Table);
            Operations.Add(operation);

            return new OperationBuilder<DropPrimaryKeyOperation>(operation);
        }

        /// <summary>
        /// 删除架构。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropSchemaOperation> DropSchema(
             string name)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new DropSchemaOperation
            {
                Name = name
            };
            Operations.Add(operation);

            return new OperationBuilder<DropSchemaOperation>(operation);
        }

        /// <summary>
        /// 删除序列号。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="schema">架构。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropSequenceOperation> DropSequence(
             string name,
             string schema = null)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new DropSequenceOperation
            {
                Schema = schema,
                Name = name
            };
            Operations.Add(operation);

            return new OperationBuilder<DropSequenceOperation>(operation);
        }

        /// <summary>
        /// 删除表格。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropTableOperation> DropTable<TEntity>()
        {
            var operation = new DropTableOperation
            {
                Table = typeof(TEntity).GetTableName()
            };
            Operations.Add(operation);

            return new OperationBuilder<DropTableOperation>(operation);
        }

        /// <summary>
        /// 删除表格。
        /// </summary>
        /// <param name="name">表格名称。</param>
        /// <param name="schema">架构。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropTableOperation> DropTable(
             string name,
             string schema = null)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new DropTableOperation
            {
                Table = GetName(schema, name)
            };
            Operations.Add(operation);

            return new OperationBuilder<DropTableOperation>(operation);
        }

        /// <summary>
        /// 删除唯一键。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="columns">列表达式。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropUniqueConstraintOperation> DropUniqueConstraint<TEntity>(Expression<Func<TEntity, object>> columns)
        {
            Check.NotNull(columns, nameof(columns));

            var operation = new DropUniqueConstraintOperation
            {
                Table = typeof(TEntity).GetTableName()
            };
            operation.Name = OperationHelper.GetName(NameType.UniqueKey, operation.Table, columns.GetPropertyNames());
            Operations.Add(operation);

            return new OperationBuilder<DropUniqueConstraintOperation>(operation);
        }

        /// <summary>
        /// 删除唯一键。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="name">名称（全名包含UK_）。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropUniqueConstraintOperation> DropUniqueConstraint<TEntity>(string name)
        {
            Check.NotNull(name, nameof(name));

            var operation = new DropUniqueConstraintOperation
            {
                Table = typeof(TEntity).GetTableName(),
                Name = name
            };
            Operations.Add(operation);

            return new OperationBuilder<DropUniqueConstraintOperation>(operation);
        }

        /// <summary>
        /// 修改列名。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="name">原名称。</param>
        /// <param name="column">新列名表达式。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<RenameColumnOperation> RenameColumn<TEntity>(
             string name,
             Expression<Func<TEntity, object>> column)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(column, nameof(column));

            var operation = new RenameColumnOperation
            {
                Name = name,
                Table = typeof(TEntity).GetTableName(),
                NewName = column.GetPropertyAccess().Name
            };
            Operations.Add(operation);

            return new OperationBuilder<RenameColumnOperation>(operation);
        }

        /// <summary>
        /// 修改列名。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="name">原名称。</param>
        /// <param name="newName">新列名。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<RenameColumnOperation> RenameColumn<TEntity>(
             string name,
             string newName)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotEmpty(newName, nameof(newName));

            var operation = new RenameColumnOperation
            {
                Name = name,
                Table = typeof(TEntity).GetTableName(),
                NewName = newName
            };
            Operations.Add(operation);

            return new OperationBuilder<RenameColumnOperation>(operation);
        }

        /// <summary>
        /// 修改列名。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="name">原名称（全名包含IX_）。</param>
        /// <param name="columns">新列名表达式。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<RenameIndexOperation> RenameIndex<TEntity>(
             string name,
             Expression<Func<TEntity, object>> columns)
        {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(columns, nameof(columns));

            var operation = new RenameIndexOperation
            {
                Table = typeof(TEntity).GetTableName(),
                Name = name,
            };
            operation.NewName = OperationHelper.GetName(NameType.Index, operation.Table, columns.GetPropertyNames());
            Operations.Add(operation);

            return new OperationBuilder<RenameIndexOperation>(operation);
        }

        /// <summary>
        /// 修改序列号名称。
        /// </summary>
        /// <param name="name">原名称。</param>
        /// <param name="schema">原架构。</param>
        /// <param name="newName">新名称。</param>
        /// <param name="newSchema">新架构。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<RenameSequenceOperation> RenameSequence(
             string name,
             string schema = null,
             string newName = null,
             string newSchema = null)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new RenameSequenceOperation
            {
                Name = name,
                Schema = schema,
                NewName = newName,
                NewSchema = newSchema
            };
            Operations.Add(operation);

            return new OperationBuilder<RenameSequenceOperation>(operation);
        }

        /// <summary>
        /// 拼接架构和名称。
        /// </summary>
        /// <param name="schema">架构。</param>
        /// <param name="name">名称。</param>
        /// <returns>返回拼接后的字符串。</returns>
        protected virtual string GetName(string schema, string name)
        {
            if (schema != null)
                schema += ".";
            return schema + name;
        }

        /// <summary>
        /// 修改表名称。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="name">原名称。</param>
        /// <param name="schema">原架构。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<RenameTableOperation> RenameTable<TEntity>(
             string name,
             string schema = null)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new RenameTableOperation
            {
                Table = GetName(schema, name),
                NewTable = typeof(TEntity).GetTableName(),
            };
            Operations.Add(operation);

            return new OperationBuilder<RenameTableOperation>(operation);
        }

        /// <summary>
        /// 重新计算序列号。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="startValue">开始值。</param>
        /// <param name="schema">架构。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<RestartSequenceOperation> RestartSequence(
             string name,
            long startValue = 1L,
             string schema = null)
        {
            Check.NotEmpty(name, nameof(name));

            var operation = new RestartSequenceOperation
            {
                Name = name,
                Schema = schema,
                StartValue = startValue
            };
            Operations.Add(operation);

            return new OperationBuilder<RestartSequenceOperation>(operation);
        }

        /// <summary>
        /// SQL语句。
        /// </summary>
        /// <param name="sql">SQL语句。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<SqlOperation> Sql(
             string sql)
        {
            Check.NotEmpty(sql, nameof(sql));

            var operation = new SqlOperation
            {
                Sql = sql
            };
            Operations.Add(operation);

            return new OperationBuilder<SqlOperation>(operation);
        }

        /// <summary>
        /// 新建语句。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="instance">对象实例。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<SqlOperation> SqlCreate<TEntity>(TEntity instance)
        {
            Check.NotNull(instance, nameof(instance));
            var operation = new SqlOperation
            {
                Instance = instance,
                EntityType = typeof(TEntity)
            };
            Operations.Add(operation);

            return new OperationBuilder<SqlOperation>(operation);
        }

        /// <summary>
        /// 更新语句。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="expression">条件表达式。</param>
        /// <param name="instance">匿名对象。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<SqlOperation> SqlUpdate<TEntity>(Expression<Predicate<TEntity>> expression, object instance)
        {
            Check.NotNull(expression, nameof(expression));
            Check.NotNull(instance, nameof(instance));
            var operation = new SqlOperation
            {
                Instance = instance,
                EntityType = typeof(TEntity),
                Expression = expression
            };
            Operations.Add(operation);

            return new OperationBuilder<SqlOperation>(operation);
        }

        /// <summary>
        /// 删除语句。
        /// </summary>
        /// <typeparam name="TEntity">模型类型。</typeparam>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<SqlOperation> SqlDelete<TEntity>(Expression<Predicate<TEntity>> expression)
        {
            Check.NotNull(expression, nameof(expression));
            var operation = new SqlOperation
            {
                EntityType = typeof(TEntity),
                Expression = expression
            };
            Operations.Add(operation);

            return new OperationBuilder<SqlOperation>(operation);
        }
    }

    /// <summary>
    /// 模型迁移实例类型。
    /// </summary>
    /// <typeparam name="TEntity">实体类型。</typeparam>
    public class MigrationBuilder<TEntity>
    {
        private readonly MigrationBuilder _builder;
        /// <summary>
        /// 初始化类<see cref="MigrationBuilder{TEntity}"/>。
        /// </summary>
        /// <param name="builder">迁移实例。</param>
        public MigrationBuilder(MigrationBuilder builder)
        {
            _builder = builder;
        }

        /// <summary>
        /// 添加列。
        /// </summary>
        /// <param name="column">列表达式。</param>
        /// <param name="type">字段类型。</param>
        /// <param name="unicode">是否为Unicode编码。</param>
        /// <param name="nullable">是否为空。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <param name="defaultValueSql">默认值SQL字符串。</param>
        /// <param name="computedColumnSql">计算列的SQL字符串。</param>
        /// <returns>返回操作实例。</returns>
        public virtual OperationBuilder<AddColumnOperation> AddColumn(Expression<Func<TEntity, object>> column, string type = null, bool? nullable = null, bool? unicode = null, object defaultValue = null, string defaultValueSql = null, string computedColumnSql = null)
            => _builder.AddColumn(column, type, nullable, unicode, defaultValue, defaultValueSql, computedColumnSql);

        /// <summary>
        /// 添加外键。
        /// </summary>
        /// <typeparam name="TPrincipal">主键类。</typeparam>
        /// <param name="columns">字段。</param>
        /// <param name="principalColumns">主键列。</param>
        /// <param name="onUpdate">更新时候对应的操作。</param>
        /// <param name="onDelete">删除时候对应的操作。</param>
        /// <returns>返回迁移构建实例。</returns>
        public virtual OperationBuilder<AddForeignKeyOperation> AddForeignKey<TPrincipal>(Expression<Func<TEntity, object>> columns, Expression<Func<TPrincipal, object>> principalColumns = null, ReferentialAction onUpdate = ReferentialAction.NoAction, ReferentialAction onDelete = ReferentialAction.NoAction)
            => _builder.AddForeignKey(columns, principalColumns, onUpdate, onDelete);

        /// <summary>
        /// 添加主键。
        /// </summary>
        /// <param name="columns">列表达式。</param>
        /// <param name="clustered">是否聚合。</param>
        /// <returns>返回构建实例对象。</returns>
        public virtual OperationBuilder<AddPrimaryKeyOperation> AddPrimaryKey(Expression<Func<TEntity, object>> columns, bool clustered = true)
            => _builder.AddPrimaryKey(columns, clustered);

        /// <summary>
        /// 添加唯一键。
        /// </summary>
        /// <param name="columns">列表达式。</param>
        /// <param name="clustered">是否聚合。</param>
        /// <returns>返回构建实例对象。</returns>
        public virtual OperationBuilder<AddUniqueConstraintOperation> AddUniqueConstraint(Expression<Func<TEntity, object>> columns, bool clustered = false)
            => _builder.AddUniqueConstraint(columns, clustered);

        /// <summary>
        /// 修改列。
        /// </summary>
        /// <param name="type">字段类型。</param>
        /// <param name="unicode">是否为Unicode编码。</param>
        /// <param name="column">列表达式。</param>
        /// <param name="nullable">是否为空。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <param name="defaultValueSql">默认值SQL字符串。</param>
        /// <param name="computedColumnSql">计算列的SQL字符串。</param>
        /// <returns>返回操作实例。</returns>
        public virtual AlterOperationBuilder<AlterColumnOperation> AlterColumn(Expression<Func<TEntity, object>> column, string type = null, bool? unicode = null, bool? nullable = null, object defaultValue = null, string defaultValueSql = null, string computedColumnSql = null)
            => _builder.AlterColumn(column, type, unicode, nullable, defaultValue, defaultValueSql, computedColumnSql);

        /// <summary>
        /// 修改数据库。
        /// </summary>
        /// <returns>返回构建实例。</returns>
        public virtual AlterOperationBuilder<AlterDatabaseOperation> AlterDatabase()
            => _builder.AlterDatabase();

        /// <summary>
        /// 修改序列号。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="schema">架构。</param>
        /// <param name="incrementBy">增量。</param>
        /// <param name="minValue">最小值。</param>
        /// <param name="maxValue">最大值。</param>
        /// <param name="cyclic">是否循环。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual AlterOperationBuilder<AlterSequenceOperation> AlterSequence(string name, string schema = null, int incrementBy = 1, long? minValue = null, long? maxValue = null, bool cyclic = false)
            => _builder.AlterSequence(name, schema, incrementBy, minValue, maxValue, cyclic);

        /// <summary>
        /// 修改表格。
        /// </summary>
        /// <returns>返回迁移实例。</returns>
        public virtual AlterOperationBuilder<AlterTableOperation> AlterTable()
            => _builder.AlterTable<TEntity>();

        /// <summary>
        /// 新建索引。
        /// </summary>
        /// <param name="columns">列表达式。</param>
        /// <param name="unique">是否唯一。</param>
        /// <param name="clustered">是否聚合。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<CreateIndexOperation> CreateIndex(Expression<Func<TEntity, object>> columns, bool unique = false, bool clustered = false)
            => _builder.CreateIndex(columns, unique, clustered);

        /// <summary>
        /// 新建索引。
        /// </summary>
        /// <typeparam name="TModel">模型类型。</typeparam>
        /// <param name="columns">列表达式。</param>
        /// <param name="unique">是否唯一。</param>
        /// <param name="clustered">是否聚合。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<CreateIndexOperation> CreateIndex<TModel>(Expression<Func<TModel, object>> columns, bool unique = false, bool clustered = false)
            => _builder.CreateIndex(columns, unique, clustered);

        /// <summary>
        /// 确认架构。
        /// </summary>
        /// <param name="name">架构名称。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<EnsureSchemaOperation> EnsureSchema(
                 string name)
            => _builder.EnsureSchema(name);

        /// <summary>
        /// 新建序列号。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="schema">架构。</param>
        /// <param name="startValue">起始值。</param>
        /// <param name="incrementBy">增量。</param>
        /// <param name="minValue">最小值。</param>
        /// <param name="maxValue">最大值。</param>
        /// <param name="cyclic">是否循环。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<CreateSequenceOperation> CreateSequence(string name, string schema = null, long startValue = 1L, int incrementBy = 1, long? minValue = null, long? maxValue = null, bool cyclic = false)
            => CreateSequence<long>(name, schema, startValue, incrementBy, minValue, maxValue, cyclic);

        /// <summary>
        /// 新建序列号。
        /// </summary>
        /// <typeparam name="T">列类型。</typeparam>
        /// <param name="name">名称。</param>
        /// <param name="schema">架构。</param>
        /// <param name="startValue">起始值。</param>
        /// <param name="incrementBy">增量。</param>
        /// <param name="minValue">最小值。</param>
        /// <param name="maxValue">最大值。</param>
        /// <param name="cyclic">是否循环。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<CreateSequenceOperation> CreateSequence<T>(string name, string schema = null, long startValue = 1L, int incrementBy = 1, long? minValue = null, long? maxValue = null, bool cyclic = false)
            => _builder.CreateSequence<T>(name, schema, startValue, incrementBy, minValue, maxValue, cyclic);

        /// <summary>
        /// 新建表格。
        /// </summary>
        /// <param name="action">表格列定义表达式。</param>
        /// <returns>返回当前构建实例。</returns>
        public CreateTableBuilder<TEntity> CreateTable(Action<CreateTableBuilder<TEntity>> action)
            => CreateTable<TEntity>(action);

        /// <summary>
        /// 新建表格。
        /// </summary>
        /// <param name="action">表格列定义表达式。</param>
        /// <returns>返回当前构建实例。</returns>
        public virtual CreateTableBuilder<TModel> CreateTable<TModel>(Action<CreateTableBuilder<TModel>> action)
            => _builder.CreateTable(action);

        /// <summary>
        /// 删除列。
        /// </summary>
        /// <param name="name">列名称。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropColumnOperation> DropColumn(Expression<Func<TEntity, object>> name)
            => _builder.DropColumn(name);

        /// <summary>
        /// 删除列。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="name">列名称。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropColumnOperation> DropColumn(string name)
            => _builder.DropColumn<TEntity>(name);

        /// <summary>
        /// 删除外键。
        /// </summary>
        /// <param name="columns">列表达式。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropForeignKeyOperation> DropForeignKey(
                 Expression<Func<TEntity, object>> columns)
            => _builder.DropForeignKey(columns);

        /// <summary>
        /// 删除外键。
        /// </summary>
        /// <param name="name">外键名称(全名，包含FK_)。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropForeignKeyOperation> DropForeignKey(string name)
            => _builder.DropForeignKey<TEntity>(name);

        /// <summary>
        /// 删除索引。
        /// </summary>
        /// <param name="columns">列表达式。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropIndexOperation> DropIndex(Expression<Func<TEntity, object>> columns) => _builder.DropIndex(columns);

        /// <summary>
        /// 删除索引。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="name">索引名称（全名，包含IX_）。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropIndexOperation> DropIndex(string name) => _builder.DropIndex<TEntity>(name);

        /// <summary>
        /// 删除主键。
        /// </summary>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropPrimaryKeyOperation> DropPrimaryKey() => _builder.DropPrimaryKey<TEntity>();

        /// <summary>
        /// 删除架构。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropSchemaOperation> DropSchema(string name) => _builder.DropSchema(name);

        /// <summary>
        /// 删除序列号。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="schema">架构。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropSequenceOperation> DropSequence(string name, string schema = null) => _builder.DropSequence(name, schema);

        /// <summary>
        /// 删除表格。
        /// </summary>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropTableOperation> DropTable() => _builder.DropTable<TEntity>();

        /// <summary>
        /// 删除表格。
        /// </summary>
        /// <param name="name">表格名称。</param>
        /// <param name="schema">架构。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropTableOperation> DropTable(string name, string schema = null) => _builder.DropTable(name, schema);

        /// <summary>
        /// 删除唯一键。
        /// </summary>
        /// <param name="columns">列表达式。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropUniqueConstraintOperation> DropUniqueConstraint(Expression<Func<TEntity, object>> columns)
            => _builder.DropUniqueConstraint(columns);

        /// <summary>
        /// 删除唯一键。
        /// </summary>
        /// <param name="name">名称（全名包含UK_）。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<DropUniqueConstraintOperation> DropUniqueConstraint(string name)
            => _builder.DropUniqueConstraint<TEntity>(name);

        /// <summary>
        /// 修改列名。
        /// </summary>
        /// <param name="name">原名称。</param>
        /// <param name="column">新列名表达式。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<RenameColumnOperation> RenameColumn(string name, Expression<Func<TEntity, object>> column) => _builder.RenameColumn(name, column);

        /// <summary>
        /// 修改列名。
        /// </summary>
        /// <param name="name">原名称。</param>
        /// <param name="newName">新列名。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<RenameColumnOperation> RenameColumn(string name, string newName) => _builder.RenameColumn<TEntity>(name, newName);

        /// <summary>
        /// 修改列名。
        /// </summary>
        /// <param name="name">原名称（全名包含IX_）。</param>
        /// <param name="columns">新列名表达式。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<RenameIndexOperation> RenameIndex(string name, Expression<Func<TEntity, object>> columns) => _builder.RenameIndex(name, columns);

        /// <summary>
        /// 修改序列号名称。
        /// </summary>
        /// <param name="name">原名称。</param>
        /// <param name="schema">原架构。</param>
        /// <param name="newName">新名称。</param>
        /// <param name="newSchema">新架构。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<RenameSequenceOperation> RenameSequence(
             string name,
             string schema = null,
             string newName = null,
             string newSchema = null) => _builder.RenameSequence(name, schema, newName, newSchema);

        /// <summary>
        /// 修改表名称。
        /// </summary>
        /// <param name="name">原名称。</param>
        /// <param name="schema">原架构。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<RenameTableOperation> RenameTable(
             string name,
             string schema = null) => _builder.RenameTable<TEntity>(name, schema);

        /// <summary>
        /// 重新计算序列号。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="startValue">开始值。</param>
        /// <param name="schema">架构。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<RestartSequenceOperation> RestartSequence(
             string name,
            long startValue = 1L,
             string schema = null) => _builder.RestartSequence(name, startValue, schema);

        /// <summary>
        /// SQL语句。
        /// </summary>
        /// <param name="sql">SQL语句。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<SqlOperation> Sql(
             string sql) => _builder.Sql(sql);

        /// <summary>
        /// 新建语句。
        /// </summary>
        /// <param name="instance">对象实例。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<SqlOperation> SqlCreate(TEntity instance)
            => _builder.SqlCreate(instance);

        /// <summary>
        /// 新建语句。
        /// </summary>
        /// <typeparam name="TModel">模型类型。</typeparam>
        /// <param name="instance">对象实例。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<SqlOperation> SqlCreate<TModel>(TModel instance)
            => _builder.SqlCreate(instance);

        /// <summary>
        /// 更新语句。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <param name="instance">匿名对象。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<SqlOperation> SqlUpdate(
                 Expression<Predicate<TEntity>> expression, object instance)
            => _builder.SqlUpdate(expression, instance);

        /// <summary>
        /// 更新语句。
        /// </summary>
        /// <typeparam name="TModel">模型类型。</typeparam>
        /// <param name="expression">条件表达式。</param>
        /// <param name="instance">匿名对象。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<SqlOperation> SqlUpdate<TModel>(
                 Expression<Predicate<TModel>> expression, object instance)
            => _builder.SqlUpdate(expression, instance);

        /// <summary>
        /// 删除语句。
        /// </summary>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<SqlOperation> SqlDelete(
                 Expression<Predicate<TEntity>> expression)
            => _builder.SqlDelete(expression);

        /// <summary>
        /// 删除语句。
        /// </summary>
        /// <typeparam name="TModel">模型类型。</typeparam>
        /// <param name="expression">条件表达式。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<SqlOperation> SqlDelete<TModel>(
                 Expression<Predicate<TModel>> expression)
            => _builder.SqlDelete(expression);
    }
}