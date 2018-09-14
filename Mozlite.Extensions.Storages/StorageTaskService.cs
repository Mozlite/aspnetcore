using System;
using System.Threading.Tasks;
using Mozlite.Extensions.Storages.Properties;
using Mozlite.Extensions.Tasks;

namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 存储文件夹清理服务。
    /// </summary>
    public class StorageTaskService : TaskService
    {
        private readonly IStorageDirectory _storageDirectory;
        /// <summary>
        /// 初始化类<see cref="StorageTaskService"/>。
        /// </summary>
        /// <param name="storageDirectory">存储文件夹接口。</param>
        public StorageTaskService(IStorageDirectory storageDirectory)
        {
            _storageDirectory = storageDirectory;
        }

        /// <summary>
        /// 名称。
        /// </summary>
        public override string Name => Resources.StorageClearTaskExecutor_Name;

        /// <summary>
        /// 描述。
        /// </summary>
        public override string Description => Resources.StorageClearTaskExecutor_Description;

        /// <summary>
        /// 执行间隔时间。
        /// </summary>
        public override TaskInterval Interval => new TimeSpan(3, 0, 0);

        /// <summary>
        /// 执行方法。
        /// </summary>
        /// <param name="argument">参数。</param>
        public override async Task ExecuteAsync(Argument argument)
        {
            _storageDirectory.ClearEmptyDirectories();
            await Task.Delay(100);
        }
    }
}