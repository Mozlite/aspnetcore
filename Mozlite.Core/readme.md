# Mozlite.Core

Mozlite框架最核心得程序集，主要包含核心得功能：IOC容器自动注册接口，模型实体特性得定义等。

## IOC容器自动注册接口

ASPNETCore开发主要是基于依赖式注入，我们这里只是介绍在Mozlite框架中如何自动注册接口，以及实现对象等等。在ASPNETCore开发中，根据生命周期得不同，可以分为实例化接口（`IService[s]`），全局单例接口（`ISingletonService[s]`），当前域内单例接口（`IScopedService[s]`）。

1. `IService[s]`：所有需要自动注册接口都必须继承此接口，此接口是每次调用时候都必须实例化一次对象；
2. `ISingletonService[s]`：在整个应用程序域单例存在得接口，需要继承此接口；
3. `IScopedService[s]`：在当前上下文中单例存在，需要继承此接口，例如在HTTP上下文或者当前线程上下文中独立存在；

> 如果是复数形式，如：`IServices`，则表示注册一个数组到容器中，在调用得时候是要`IEnumerable<IObjectInterface>`，其中IObjectInterface就是需要调用得接口。
