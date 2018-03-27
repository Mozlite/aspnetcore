using Mozlite.Data;
using Mozlite.Extensions.Data;

namespace Mozlite.Extensions.Documents
{
    /// <summary>
    /// 文档管理实现类。
    /// </summary>
    public class DocumentManager : ObjectManager<Document>, IDocumentManager
    {
        /// <summary>
        /// 初始化类<see cref="DocumentManager"/>。
        /// </summary>
        /// <param name="context">数据库操作接口。</param>
        public DocumentManager(IDbContext<Document> context) : base(context)
        {
        }
    }
}