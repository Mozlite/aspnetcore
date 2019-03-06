using Mozlite;
using System;

namespace MozliteDemo.Extensions
{
    /// <summary>
    /// 网站配置。
    /// </summary>
    public class SiteSettings
    {
        /// <summary>
        /// 网站名称。
        /// </summary>
        public string SiteName { get; set; } = "Mozlite Demo";

        /// <summary>
        /// Logo地址。
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// 描述。
        /// </summary>
        public string Description { get; set; }

        private string _copyright;
        /// <summary>
        /// 版权信息。
        /// </summary>
        public string Copyright
        {
            get => _copyright ?? (_copyright = $@"Copyright &copy;{DateTime.Now.Year} www.mozlite.com ver {Cores.Version.ToString(3)}");
            set => _copyright = value;
        }
    }
}
