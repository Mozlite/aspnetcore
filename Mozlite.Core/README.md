# Mozlite.Core

Mozlite框架最核心得程序集，主要包含核心得功能：IOC容器自动注册接口，模型实体特性得定义，后台线程服务等。

## IOC容器自动注册接口

ASPNETCore开发主要是基于依赖式注入，我们这里只是介绍在Mozlite框架中如何自动注册接口，以及实现对象等等。在ASPNETCore开发中，根据生命周期得不同，可以分为实例化接口（`IService[s]`），全局单例接口（`ISingletonService[s]`），当前域内单例接口（`IScopedService[s]`）。

1. `IService[s]`：所有需要自动注册接口都必须继承此接口，此接口是每次调用时候都必须实例化一次对象；
2. `ISingletonService[s]`：在整个应用程序域单例存在得接口，需要继承此接口；
3. `IScopedService[s]`：在当前上下文中单例存在，需要继承此接口，例如在HTTP上下文或者当前线程上下文中独立存在；

> 如果是复数形式，如：`IServices`，则表示注册一个数组到容器中，在调用得时候是要`IEnumerable<IObjectInterface>`，其中IObjectInterface就是需要调用得接口。

为了更好自定义注册容器，可以实现接口`IServiceConfigurer`进行自定义注册。

```csharp
    /// <summary>
    /// 服务配置接口。
    /// </summary>
    public interface IServiceConfigurer : IService
    {
        /// <summary>
        /// 配置服务方法。
        /// </summary>
        /// <param name="services">服务集合实例。</param>
        void ConfigureServices(IServiceCollection services);
    }
```

> 如果使用已有得框架，并且已经自动继承了自动注册接口，也可以新建一个新得类，并且使用`SuppressAttribute`进行标注，这样就可以替换掉原有得类型，而使用当前实现类。

## 模型实体类型特性

模型实体类型特性主要使用于数据属性描述，这里设计适用于数据库自动匹配使用。数据库属性描述有些需要使用到`System.ComponentModel.DataAnnotations`，例如顶一个实体类型对应得表格名称。

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
```

## 后台线程服务

本框架实现了一个`IHostedService`，并在接口中使用注册的接口`ITaskExecutor`。

> 总结：这样设计可以更好的进行容器对象注册，在项目中只需要在Startup中调用`services.AddMozlite()`既可以实现以上功能了。
