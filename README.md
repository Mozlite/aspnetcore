# Mozlite(aspnetcore 2.0)

随着ASPNETCore更新到2.0，我们得框架也升级到2.0，但是还没有应用得实际得项目中。ASPNETCore1.1已经在实际商业项目中，正常使用了，这里主要介绍一下这个框架得主要内容。

* [核心框架Mozltie.Core](https://github.com/Mozlite/aspnetcore/blob/master/Mozlite.Core/README.md)
* [数据库框架Mozlite.Data](https://github.com/Mozlite/aspnetcore/blob/master/Mozlite.Data/README.md)
   数据库框架需要引用特定数据库，如：`Mozlite.Data.SqlServer`，并且在Startup中进行注册：`builder.AddSqlServer(Configuration)`
* [文件存储Mozltie.Extensions.Storages](https://github.com/Mozlite/aspnetcore/blob/master/Mozlite.Extensions.Storages/README.md)
