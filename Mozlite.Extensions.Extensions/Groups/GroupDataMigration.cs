﻿using Mozlite.Data.Migrations.Builders;
using Mozlite.Extensions.Extensions.Categories;

namespace Mozlite.Extensions.Extensions.Groups
{
    /// <summary>
    /// 分组数据迁移基类。
    /// </summary>
    /// <typeparam name="TGroup">分组类型。</typeparam>
    public abstract class GroupDataMigration<TGroup> : CategoryDataMigration<TGroup> where TGroup : GroupBase<TGroup>
    {
        /// <summary>
        /// 添加列。
        /// </summary>
        /// <param name="table">表格构建实例。</param>
        protected override void Create(CreateTableBuilder<TGroup> table)
        {
            table.Column(x => x.SiteId)
                .Column(x => x.ParentId)
                .UniqueConstraint(x => new { x.SiteId, x.ParentId, x.Name });
        }
    }
}