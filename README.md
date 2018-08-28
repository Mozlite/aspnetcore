# Mozlite(aspnetcore 2.x)

Mozlite框架基于AspNetCore框架进行快速的开发，包括UI，数据库等等功能模块，本框架的目的是为了更高效，更友好，更简单的进行项目开发，实现价值最大化，完全开放源代码，如有技术支持可以加QQ群：`88794325`,`8781765`。

# 项目构建目录

基于Mozlite开发的项目结构，建议使用如下的项目结构:

├──Project(项目文件夹)

│&nbsp;&nbsp;&nbsp;&nbsp;├── Project

│&nbsp;&nbsp;&nbsp;&nbsp;├── Project.Extensions

│&nbsp;&nbsp;&nbsp;&nbsp;└── Project.Extensions.*

├──Mozlite(框架文件夹)

│&nbsp;&nbsp;&nbsp;&nbsp;├── [Mozlite.Core](https://github.com/Mozlite/aspnetcore/blob/master/Mozlite.Core/README.md)

│&nbsp;&nbsp;&nbsp;&nbsp;├── [Mozlite.Data](https://github.com/Mozlite/aspnetcore/blob/master/Mozlite.Data/README.md)

│&nbsp;&nbsp;&nbsp;&nbsp;├── Mozlite.Data.SqlServer

│&nbsp;&nbsp;&nbsp;&nbsp;├── Mozlite.Mvc

│&nbsp;&nbsp;&nbsp;&nbsp;├── Mozlite.Extensions

│&nbsp;&nbsp;&nbsp;&nbsp;├── [Mozlite.Extensions.Storages](https://github.com/Mozlite/aspnetcore/blob/master/Mozlite.Extensions.Storages/README.md)

│&nbsp;&nbsp;&nbsp;&nbsp;└── Mozlite.Extensions.*

├──Tests(测试文件夹)

│&nbsp;&nbsp;&nbsp;&nbsp;├── Project.Tests

│&nbsp;&nbsp;&nbsp;&nbsp;├── Project.Extensions.Tests

│&nbsp;&nbsp;&nbsp;&nbsp;└── Project.Extensions.*.Tests

└── Others(其他文件夹)

## Mozlite

用git从`https://github.com/mozlite/aspnetcore`获取最新版本，从而保证架构得统一性，如果发现Mozlite相关BUG可以到github上进行BUG提交等。

## Project

能源管理项目目录，设计到业务逻辑相关得所有项目，可以放到这个目录下面，TFS项目中保存当前项目得代码。

# 相关链接

* https://www.mozlite.com
