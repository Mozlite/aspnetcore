# Mozlite.Extensions

通用扩展模块，主要对功能应用的扩展模块，主要包含了常用的分类，配置，用户等等基础模块。在Mozlite中，如果扩展一个模块的模型实例，先定义一个模型实体类`Entity`，然后就是数据库迁移类`EntityDataMigration`，接着需要定义一个`IEntityManager`接口，然后实现这个接口`EntityManager`。

## 功能模块配置

在Mozlite中，功能模块配置进行统一得存储，在模块扩展时候不需要考虑数据库保存，统一保存在如下实体表中。使用`Mozlite.Extensions.Settings.ISettingsManager`进行保存和检索操作即可，主要是应对各个扩展功能模块数据存储的方式。

## 后台任务服务

此后台任务服务启动线程为`Mozlite.Core`中的`Mozlite.Tasks.TaskHostedService`。所有后台服务需要继承`Mozlite.Extensions.Tasks.TaskService`类。

## 数据库管理操作

由于使用比较普遍，对拥有主键的CRUD以及分页等进行封装。

## 分类基类CategoryBase

分类在功能扩展模块中经常都会使用到，所以在Mozlite中进行封装，其中包含了缓存等，扩展一个分类时只要继承相应的对象即可。

## 分组基类GroupBase

分组时对分类进行扩展，这个主要对分类进行子类进行扩展，其中包含了子类和父类的缓存扩展等等。

## 简易搜索引擎

在Mozlite中建立了建议的搜索引擎，不过搜索的结果以及分词需要自己写服务进行操作。

## 非法名称

这个功能模块被用于限定名称是否合法，主要用于过滤和验证非法字符串。
