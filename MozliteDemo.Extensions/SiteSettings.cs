using Microsoft.AspNetCore.Html;
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
        public string LogoUrl { get; set; } = "/images/logo.png";

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
            get => _copyright ?? (_copyright = $@"Copyright &copy;$year www.mozlite.com ver $version");
            set => _copyright = value;
        }

        /// <summary>
        /// 版权信息。
        /// </summary>
        public IHtmlContent CopyrightHTML => new HtmlString(Copyright.Replace("$version", Cores.Version.ToString(3)).Replace("$year", DateTime.Now.Year.ToString()));
    }
}
