using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mozlite.Extensions.Properties;
using Mozlite.Extensions.Tasks;

namespace Mozlite.Extensions.Searching
{
    /// <summary>
    /// 搜索实体生成服务。
    /// </summary>
    public abstract class SearchTaskService : TaskService
    {
        private readonly IEnumerable<ISearchIndexProvider> _providers;
        private readonly ILogger _logger;
        private readonly ISearchManager _searchManager;

        /// <summary>
        /// 初始化类<see cref="SearchTaskService"/>。
        /// </summary>
        /// <param name="providers">服务提供值接口列表。</param>
        /// <param name="logger">日志接口。</param>
        /// <param name="searchManager">搜索管理接口。</param>
        protected SearchTaskService(IEnumerable<ISearchIndexProvider> providers, ILogger<SearchTaskService> logger, ISearchManager searchManager)
        {
            _providers = providers;
            _logger = logger;
            _searchManager = searchManager;
        }

        /// <summary>
        /// 名称。
        /// </summary>
        public override string Name => Resources.Task_Searching_Name;

        /// <summary>
        /// 描述。
        /// </summary>
        public override string Description => Resources.Task_Searching_Description;

        /// <summary>
        /// 执行间隔时间。
        /// </summary>
        public override TaskInterval Interval => 30;

        /// <summary>
        /// 执行方法。
        /// </summary>
        /// <param name="argument">参数。</param>
        public override async Task ExecuteAsync(Argument argument)
        {
            foreach (var provider in _providers)
            {
                await Task.Delay(1000);
                try
                {
                    //获取实体内容
                    var entityType = provider.Model.GetEntityType();
                    var entry = await _searchManager.GetIndexAsync(entityType);
                    if (entry == null)
                        continue;
                    var search = new SearchDescriptor { ProviderName = provider.ProviderName, TargetId = entry.Id };
                    search.IndexedDate = DateTime.Now;
                    search.Summary = provider.Summarized(entry);
                    if (search.Summary == null)
                        continue;
                    var indexes = provider.Indexed(entry);
                    await _searchManager.SaveAsync(entityType, search, indexes);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(3, $"索引实体生成[{provider.GetType().FullName}]出现错误:{ex.Message}", ex);
                }
            }
        }
    }
}