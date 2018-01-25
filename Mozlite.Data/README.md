# Mozlite.Data

数据库应用模块，在Mozlite中数据库使用ADO.NET进行封装，有点类似于Linq的写法；这也许不是最好的做法，不过比EntityFramework简单，而且在应用当中很少使用多种数据库的项目。在`Mozlite.Data`中，主要包含了几个功能：CodeFirst代码迁移，`IDatabase`原始ADO.NET操作，以及`IRepository<TModel>`模型对象操作。

## CodeFirst代码迁移

在框架中，使用CodeFirst来建立数据库和更改数据表结构。在定一个实体类后，需要实现基类：`Mozlite.Data.Migrations.DataMigration`，在实现类里定义一个表格的相关操作。

实体类
```csharp
    /// <summary>
    /// 数据库实例。
    /// </summary>
    [Table("core_Migrations")]
    public class Migration
    {
        /// <summary>
        /// 迁移类型。
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// 版本。
        /// </summary>
        public int Version { get; set; }
    }
```

迁移类
```csharp
    /// <summary>
    /// 数据库迁移表格迁移类型。
    /// </summary>
    public class CoreDataMigration : DataMigration
    {
        /// <summary>
        /// 当模型建立时候构建的表格实例。
        /// </summary>
        /// <param name="builder">迁移实例对象。</param>
        public override void Create(MigrationBuilder builder)
        {
            builder.CreateTable<Migration>(table => table
                .Column(c => c.Id)
                .Column(c => c.Version)
            );
        }
    }
```
这样Mozlite.Data框架将根据迁移类进行数据表的操作。如果需要修改表格定义，则可以在数据库迁移类中定义升级方法`Up`，也可以进行降级方法`Down`跟着版本号，例如：

```csharp
        /// <summary>
        /// 添加默认角色。
        /// </summary>
        /// <param name="builder">数据迁移构建实例。</param>
        public void Up1(MigrationBuilder builder)
        {
            var role1 = new UserRole { RoleId = 1, UserId = 1 };
            builder.SqlCreate(role1);
            var role2 = new UserRole { RoleId = 2, UserId = 1 };
            builder.SqlCreate(role2);
        }
```

## IDatabase

这个接口中包含了数据库ADO.NET的基础操作，主要包含了三个方法：`ExecuteNonQuery`，`ExecuteReader`，`ExecuteScalar`。当然也对数据库事务操作进行了封装：`BeginTransaction`。

## IDbContext<TModel>

这个接口主要对相应的模型对象进行操作，包含了每个实体类的增加(Create)、读取查询(Retrieve)、更新(Update)和删除(Delete)，还封装了判断存在Any，分页查询等等。

> 注：这个Mozlite.Data作为Mozlite的数据库操作抽象框架，不同的数据库需要单独实现数据库具体操作，暂时提供SqlServer数据库的封装。
