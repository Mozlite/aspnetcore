using System;
using Mozlite.Extensions.Data;

namespace Mozlite.Extensions.Documents
{
    /// <summary>
    /// 文档管理接口。
    /// </summary>
    public interface IDocumentManager : IObjectManager<Document>, ISingletonService
    {
    }
}
