# Mozlite.Extensions.Storages

文件存储模块，在Mozlite文件存储中使用MD5来进行哈希计算和唯一存储。主要包含存储文件夹操作，媒体文件（图片等）存储，文件缓存等。本项目依赖于Mozlite.Core以及Mozlite.Data项目，媒体文件存储抽象虚拟文件使用数据库进行保存。

## 存储文件夹操作

在Mozlite中，可以配置存储文件夹，为了安全性考虑尽量把文件存储放置在同一个目录中，默认为当前文件夹的父级目录下的文件夹：`../storages`。如果直接存储在其他盘下，可以直接配置物理路径。当前文件夹操作使用`IStorageDirectory`接口。

```csharp
    /// <summary>
    /// 存储文件夹提供者接口。
    /// </summary>
    public interface IStorageDirectory : ISingletonService
    {
        /// <summary>
        /// 获取当前相对路径的物理存储路径。
        /// </summary>
        /// <param name="path">相对路径，以子文件夹或文件开始。</param>
        /// <returns>返回当前相对路径的物理路径。</returns>
        string GetPhysicalPath(string path = null);

        /// <summary>
        /// 获取临时目录得物理路径。
        /// </summary>
        /// <param name="fileName">文件名称。</param>
        /// <returns>返回当前临时文件物理路径。</returns>
        string GetTempPath(string fileName);

        /// <summary>
        /// 获取当前路径文件的操作提供者接口实例。
        /// </summary>
        /// <param name="path">文件相对路径。</param>
        /// <returns>文件的操作提供者接口实例。</returns>
        IStorageFile GetFile(string path);

        /// <summary>
        /// 将表单文件实例保存到特定的文件夹中。
        /// </summary>
        /// <param name="file">表单文件实例。</param>
        /// <param name="directoryName">文件夹名称。</param>
        /// <param name="fileName">文件名称，如果为空，则直接使用表单实例的文件名。</param>
        /// <returns>返回文件提供者操作接口实例。</returns>
        Task<IStorageFile> SaveAsync(IFormFile file, string directoryName, string fileName = null);

        /// <summary>
        /// 清理空文件夹。
        /// </summary>
        void ClearEmptyDirectories();
    }
```

## 媒体文件（图片等）存储

媒体文件主要使用MD5哈希进行唯一存储，只要是相同的文件在磁盘中将会唯一的存在，存储在文件夹`media`中。而可见的文件夹和文件保存在数据库中，如果需要进行文件夹分类需要对数据库进行归类和关联。

```csharp
    /// <summary>
    /// 媒体文件。
    /// </summary>
    [Table("core_Storages_Medias")]
    public class MediaFile
    {
        /// <summary>
        /// 文件唯一Id，会暴露给URL地址。
        /// </summary>
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 实体文件Id。
        /// </summary>
        [Size(32)]
        public string FileId { get; set; }

        /// <summary>
        /// 名称。
        /// </summary>
        [Size(256)]
        public string Name { get; set; }

        /// <summary>
        /// 后缀名。
        /// </summary>
        [Size(32)]
        public string Extension { get; set; }

        /// <summary>
        /// 文件名称。
        /// </summary>
        public string FileName => Id.ToString("N") + Extension;

        /// <summary>
        /// 扩展名称。
        /// </summary>
        [Size(32)]
        public string ExtensionName { get; set; }

        /// <summary>
        /// 关联Id。
        /// </summary>
        public int? TargetId { get; set; }

        /// <summary>
        /// 添加日期。
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// 访问地址。
        /// </summary>
        public string Url => $"/s-medias/{FileName}".ToLower();

        /// <summary>
        /// 访问地址。
        /// </summary>
        public string DownloadUrl => $"/s-download/{FileName}".ToLower();
    }
```

其中，文件抽象后的访问路径就是`Url`以及`DownloadUrl`两个地址，通过这两个地址就可以读取或者下载当前的媒体文件，具体实现在控制器：`StorageController`中。

## 文件缓存

这个功能是2.0版本加入的，主要应用于生成静态标签缓存。在自定义标签中可以生成相应的HTML代码，然后将代码存储在缓存文件中，并且将当前文件设置为缓存依赖项，如果当前文件修改，将在下一次访问时候重新生成HTML代码并且缓存。

> todo:需要后台线程清理缓存文件，否则长期没有访问就可能照成磁盘浪费。


