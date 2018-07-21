using System.Collections.Generic;
using System.Threading.Tasks;
using Mozlite.Mvc;
using Mozlite.Mvc.Themes;

namespace Demo
{
    public class MenuModel : ModelBase
    {
        private readonly IThemeApplicationManager _applicationManager;
        public MenuModel(IThemeApplicationManager applicationManager)
        {
            _applicationManager = applicationManager;
        }

        public IEnumerable<IThemeApplication> Applications { get; private set; }

        public async Task OnGetAsync()
        {
            Applications = await _applicationManager.LoadApplicationsAsync();
        }
    }
}
