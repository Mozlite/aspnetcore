# Mozlite(aspnetcore 2.0)

随着ASPNETCore更新到2.0，我们得框架也升级到2.0，但是还没有应用得实际得项目中。ASPNETCore1.1已经在实际商业项目中，正常使用了，这里主要介绍一下这个框架得主要内容。

* [核心框架Mozltie.Core](https://github.com/Mozlite/aspnetcore/blob/master/Mozlite.Core/README.md)
* [数据库框架Mozlite.Data](https://github.com/Mozlite/aspnetcore/blob/master/Mozlite.Data/README.md)
   数据库框架需要引用特定数据库，如：`Mozlite.Data.SqlServer`，并且在Startup中进行注册：`builder.AddSqlServer(Configuration)`
* [文件存储Mozltie.Extensions.Storages](https://github.com/Mozlite/aspnetcore/blob/master/Mozlite.Extensions.Storages/README.md)

# 项目构建目录

根目录

|--Project

|&nbsp;&nbsp;&nbsp;&nbsp;|-- Project

|&nbsp;&nbsp;&nbsp;&nbsp;|-- Project.Extensions

|&nbsp;&nbsp;&nbsp;&nbsp;|-- Project.Extensions.*

|--Mozlite

|&nbsp;&nbsp;&nbsp;&nbsp;|-- Mozlite.*

|--Tests

|&nbsp;&nbsp;&nbsp;&nbsp;|-- Project.Tests

|&nbsp;&nbsp;&nbsp;&nbsp;|-- Project.Extensions.Tests

|&nbsp;&nbsp;&nbsp;&nbsp;|-- Project.Extensions.*.Tests

|--Others

## Mozlite

用git从`https://github.com/mozlite/aspnetcore`获取最新版本，从而保证架构得统一性，如果发现Mozlite相关BUG可以到github上进行BUG提交等。

## Project

能源管理项目目录，设计到业务逻辑相关得所有项目，可以放到这个目录下面，TFS项目中保存当前项目得代码。

