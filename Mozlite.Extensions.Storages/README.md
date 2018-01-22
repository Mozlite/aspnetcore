# Mozlite.Extensions.Storages

文件存储模块，在Mozlite文件存储中使用MD5来进行哈希计算和唯一存储。主要包含存储文件夹操作，媒体文件（图片等）存储，文件缓存等

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

媒体文件主要使用MD5哈希进行唯一存储，只要是相同的文件在磁盘中将会唯一的存在，存储在文件夹`media`中。
