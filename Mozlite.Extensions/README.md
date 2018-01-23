# Mozlite.Extensions

通用扩展模块，主要对功能应用的扩展模块，主要包含了常用的分类，配置，用户等等基础模块。

## 配置

在2.0中配置包含了两个部分，一个是网站相关信息的配置，一个是功能模块信息的配置。

__1.网站相关信息配置__

这个配置包含了网站域名，以及网站信息配置，这样做法主要是可以将多个二级网站的数据库放在一起进行管理。这样的做法在现有很多项目中经常用到，这个网站信息配置需要继承`SiteBase`类，位于Mozlite.Extensions.Sites命名空间下。不同二级网站的配置都可以不一样，基类代码如下：

```csharp
    /// <summary>
    /// 网站信息实例基类。
    /// </summary>
    [Table("core_Sites")]
    public class SiteBase
    {
        /// <summary>
        /// 网站Id。
        /// </summary>
        [Identity]
        [JsonIgnore]
        public int SiteId { get; set; }

        /// <summary>
        /// 网站名称。
        /// </summary>
        [Size(64)]
        [JsonIgnore]
        public string SiteName { get; set; }

        /// <summary>
        /// 更新地址。
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset UpdatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 是否为空网站，非法站点。
        /// </summary>
        [JsonIgnore]
        [NotMapped]
        public bool IsNull { get; set; }
    }
```

一个网站可以绑定多个域名，这样二级域名访问后就会自动获得当前网站信息实例，在程序中就可以直接调用。

__2.模块功能配置__

这个配置无需继承任何对象，使用`Mozlite.Extensions.Settings.ISettingsManager`进行保存和检索操作即可，主要是应对各个扩展功能模块数据存储的方式。
