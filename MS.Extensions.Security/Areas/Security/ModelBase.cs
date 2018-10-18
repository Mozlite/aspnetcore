using Mozlite.Extensions.Settings;
using MS.Extensions.Security;

namespace MS.Areas.Security
{
    /// <summary>
    /// 页面模型基类。
    /// </summary>
    public abstract class ModelBase : MS.Extensions.ModelBase
    {
        private SecuritySettings _settings;
        /// <summary>
        /// 安全配置。
        /// </summary>
        public SecuritySettings Settings => _settings ??
                                            (_settings = GetRequiredService<ISettingsManager>()
                                                .GetSettings<SecuritySettings>());
    }
}