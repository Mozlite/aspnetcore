using Mozlite.Data.Migrations.Builders;
using Mozlite.Data.Migrations.Operations;
using Mozlite.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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
        /// <param name="table">表格名称。</param>
        /// <param name="name">列名称。</param>
        /// <param name="type">字段类型。</param>
        /// <param name="identity">是否为自增长列。</param>
        /// <param name="unicode">是否为Unicode编码。</param>
        /// <param name="nullable">是否为空。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <param name="defaultValueSql">默认值SQL字符串。</param>
        /// <param name="computedColumnSql">计算列的SQL字符串。</param>
        /// <returns>返回操作实例。</returns>
        public virtual OperationBuilder<AddColumnOperation> AddColumn(
            string table,
            string name,
            string type,
            bool nullable = true,
            bool identity = false,
            bool? unicode = null,
            object defaultValue = null,
            string defaultValueSql = null,
            string computedColumnSql = null)
        {
            Check.NotNull(table, nameof(table));
            Check.NotNull(name, nameof(name));
            Check.NotNull(type, nameof(type));

            var operation = new AddColumnOperation
            {
                Table = table,
                Name = name,
                ColumnType = type,
                IsUnicode = unicode,
                IsIdentity = identity,
                IsNullable = nullable,
                DefaultValue = defaultValue,
                DefaultValueSql = defaultValueSql,
                ComputedColumnSql = computedColumnSql
            };
            Operations.Add(operation);

            return new OperationBuilder<AddColumnOperation>(operation);
        }

        /// <summary>
        /// 添加列。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <param name="name">列名称。</param>
        /// <param name="type">字段类型。</param>
        /// <param name="identity">是否为自增长列。</param>
        /// <param name="unicode">是否为Unicode编码。</param>
        /// <param name="nullable">是否为空。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <param name="defaultValueSql">默认值SQL字符串。</param>
        /// <param name="computedColumnSql">计算列的SQL字符串。</param>
        /// <returns>返回操作实例。</returns>
        public virtual OperationBuilder<AddColumnOperation> AddColumn<TEntity>(
            string name,
            string type,
            bool nullable = true,
            bool identity = false,
            bool? unicode = null,
            object defaultValue = null,
            string defaultValueSql = null,
            string computedColumnSql = null)
        {
            return AddColumn(typeof(TEntity).GetTableName(), name, type, nullable, identity, unicode, defaultValue,
                defaultValueSql, computedColumnSql);
        }

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
        /// <param name="table">当前外键所在得表格。</param>
        /// <param name="columns">字段列表。</param>
        /// <param name="principal">主键所在得表格。</param>
        /// <param name="principalColumns">主键字段列表。</param>
        /// <param name="onUpdate">更新时候对应的操作。</param>
        /// <param name="onDelete">删除时候对应的操作。</param>
        /// <returns>返回迁移构建实例。</returns>
        public virtual OperationBuilder<AddForeignKeyOperation> AddForeignKey(
            string table,
            string[] columns,
            string principal,
            string[] principalColumns = null,
            ReferentialAction onUpdate = ReferentialAction.NoAction,
            ReferentialAction onDelete = ReferentialAction.NoAction)
        {
            Check.NotNull(columns, nameof(columns));

            var operation = new AddForeignKeyOperation
            {
                Table = table,
                Columns = columns,
                PrincipalTable = principal,
                OnUpdate = onUpdate,
                OnDelete = onDelete
            };
            operation.PrincipalColumns = principalColumns ?? operation.Columns;
            operation.Name = OperationHelper.GetName(NameType.ForeignKey, operation.Table, operation.Columns, operation.PrincipalTable);
            Operations.Add(operation);

            return new OperationBuilder<AddForeignKeyOperation>(operation);
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
        /// <param name="table">表格名称。</param>
        /// <param name="columns">字段列表。</param>
        /// <param name="clustered">是否聚合。</param>
        /// <returns>返回构建实例对象。</returns>
        public virtual OperationBuilder<AddPrimaryKeyOperation> AddPrimaryKey(string table, string[] columns, bool clustered = true)
        {
            Check.NotNull(columns, nameof(columns));

            var operation = new AddPrimaryKeyOperation
            {
                Table = table,
                Columns = columns,
                IsClustered = clustered
            };
            operation.Name = OperationHelper.GetName(NameType.PrimaryKey, operation.Table);
            Operations.Add(operation);

            return new OperationBuilder<AddPrimaryKeyOperation>(operation);
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
        /// <param name="table">表格名称。</param>
        /// <param name="columns">字段列表。</param>
        /// <param name="clustered">是否聚合。</param>
        /// <returns>返回构建实例对象。</returns>
        public virtual OperationBuilder<AddUniqueConstraintOperation> AddUniqueConstraint(string table, string[] columns, bool clustered = false)
        {
            Check.NotNull(columns, nameof(columns));

            var operation = new AddUniqueConstraintOperation
            {
                Table = table,
                Columns = columns,
                IsClustered = clustered
            };
            operation.Name = OperationHelper.GetName(NameType.UniqueKey, operation.Table, operation.Columns);
            Operations.Add(operation);

            return new OperationBuilder<AddUniqueConstraintOperation>(operation);
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
        /// 修改列。
        /// </summary>
        /// <param name="type">字段类型。</param>
        /// <param name="unicode">是否为Unicode编码。</param>
        /// <param name="table">表格名称。</param>
        /// <param name="name">字段。</param>
        /// <param name="nullable">是否为空。</param>
        /// <param name="identity">是否为自增长。</param>
        /// <param name="defaultValue">默认值。</param>
        /// <param name="defaultValueSql">默认值SQL字符串。</param>
        /// <param name="computedColumnSql">计算列的SQL字符串。</param>
        /// <returns>返回操作实例。</returns>
        public virtual AlterOperationBuilder<AlterColumnOperation> AlterColumn(
            string table,
            string name,
            string type = null,
            bool? unicode = null,
            bool? nullable = true,
            bool identity = false,
            object defaultValue = null,
            string defaultValueSql = null,
            string computedColumnSql = null)
        {
            Check.NotNull(name, nameof(name));
            var operation = new AlterColumnOperation
            {
                Table = table,
                Name = name,
                ColumnType = type,
                IsUnicode = unicode,
                IsIdentity = identity,
                IsNullable = nullable,
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
        /// 修改表格。
        /// </summary>
        /// <typeparam name="TEntity">实体类型。</typeparam>
        /// <returns>返回迁移实例。</returns>
        public virtual AlterOperationBuilder<AlterTableOperation> AlterTable<TEntity>()
            => AlterTable(typeof(TEntity).GetTableName());

        /// <summary>
        /// 修改表格。
        /// </summary>
        /// <param name="table">表格名称。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual AlterOperationBuilder<AlterTableOperation> AlterTable(string table)
        {
            var operation = new AlterTableOperation
            {
                Table = table
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
            => CreateIndex(typeof(TEntity).GetTableName(), columns.GetPropertyNames(), unique, clustered);

        /// <summary>
        /// 新建索引。
        /// </summary>
        /// <param name="table">表格名称。</param>
        /// <param name="columns">字段列表。</param>
        /// <param name="unique">是否唯一。</param>
        /// <param name="clustered">是否聚合。</param>
        /// <returns>返回迁移实例。</returns>
        public virtual OperationBuilder<CreateIndexOperation> CreateIndex(
            string table,
            string[] columns,
            bool unique = false,
            bool clustered = false)
        {
            Check.NotNull(columns, nameof(columns));

            var operation = new CreateIndexOperation
            {
                Table = table,
                Columns = columns,
                IsUnique = unique,
                IsClustered = clustered
            };
            operation.Name = OperationHelper.GetName(NameType.Index, operation.Table, operation.Columns);
            Operations.Add(operation);

            return new OperationBuilder<CreateIndexOperation>(operation);
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
}