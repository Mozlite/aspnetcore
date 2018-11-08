using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mozlite.Mvc.Apis
{
    /// <summary>
    /// 应用程序激活的API服务表。
    /// </summary>
    [Table("apis_Applications_Services")]
    public class ApplicationService
    {
        /// <summary>
        /// 应用程序Id。
        /// </summary>
        [Key]
        public int AppicationId { get; set; }

        /// <summary>
        /// 服务Id。
        /// </summary>
        [Key]
        public int ServiceId { get; set; }
    }
}