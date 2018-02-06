# 用户活动状态日志

用户活动状态日志，主要用于记录用户在网站中操作相关信息。在Mozlite中将用户活动状态记录集成到`ILogger`日志中，只要在`EventId`中设置`Name`为"`{:user<->activity:}`"，日志将会自动记录到数据库中。

## 定义分类

在用户活动状态日志中，分类可以在扩展项目中，定义一个枚举类型，这样在编写代码的时候可以指定特定的事件实例。

## 定义用户活动实例UserActivity

如果不需要扩展其他属性，可以直接使用`UserActivity`类型，不过推荐扩展得项目定义一个类并继承`UserActivity`，这样命名空间将会和扩展得项目一样。

同样按照Mozlite规则，定义`IUserActivityManager`，`UserActivityManager`，以及`UserActivityDataMigration`继承自各自得接口或者实现类。并且泛型类型使用`UserActivity`。

## 分页查询支持

分页查询实体继承自`UserActivityQuery{TUser, TUserActivity}`。

## 日志扩展

为了能够更灵活的使用用户活动日志，扩展了两个方法。

```csharp
		/// <summary>
        /// 记录用户日志。
        /// </summary>
        /// <param name="logger">当前日志接口。</param>
        /// <param name="message">用户操作信息。</param>
        /// <param name="args">格式化参数。</param>
        public static void Info(this ILogger logger, string message, params object[] args);
		
        /// <summary>
        /// 记录用户日志。
        /// </summary>
        /// <param name="logger">当前日志接口。</param>
        /// <param name="categoryId">分类Id。</param>
        /// <param name="message">用户操作信息。</param>
        /// <param name="args">格式化参数。</param>
        public static void Info(this ILogger logger, int categoryId, string message, params object[] args);
```

如果直接使用`ILogger`的扩展方法记录用户活动日志，需要将EventId设置为分类的ID（可以使用`CategoryBase.Create`方法实例化）。

## 集成系统日志

添加IOC，只需要实现类`ServiceConfigurer{TActivityManager, TActivityManagerImplementation, TUserActivity}`即可。

需要在Startup配置时候，使用用户活动日志。

```csharp
		/// <summary>
        /// 使用用户状态日志记录。
        /// </summary>
        /// <param name="app">应用程序构建实例。</param>
        /// <returns>返回应用程序构建实例。</returns>
        public static IApplicationBuilder UseUserActivity(this IApplicationBuilder app)
```