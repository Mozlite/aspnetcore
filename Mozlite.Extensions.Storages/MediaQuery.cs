using Mozlite.Data;
using System;

namespace Mozlite.Extensions.Storages
{
    /// <summary>
    /// 媒体查询实例。
    /// </summary>
    public class MediaQuery : QueryBase<MediaFile>
    {
        /// <summary>
        /// 目标Id。
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 扩展名称。
        /// </summary>
        public string Ext { get; set; }

        /// <summary>
        /// 名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 创建开始时间。
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// 创建结束时间。
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        /// 扩展名称。
        /// </summary>
        public string ExtensionName { get; set; }

        /// <summary>
        /// 初始化查询上下文。
        /// </summary>
        /// <param name="context">查询上下文。</param>
        protected override void Init(IQueryContext<MediaFile> context)
        {
            if (Id != null)
                context.Where(x => x.TargetId == Id);
            if (!string.IsNullOrEmpty(Ext))
                context.Where(x => x.Extension == Ext);
            if (!string.IsNullOrEmpty(Name))
                context.Where(x => x.Name.Contains(Name));
            if (Start != null)
                context.Where(x => x.CreatedDate >= Start);
            if (End != null)
                context.Where(x => x.CreatedDate <= End);
            if (ExtensionName != null)
                context.Where(x => x.ExtensionName == ExtensionName);
        }
    }
}