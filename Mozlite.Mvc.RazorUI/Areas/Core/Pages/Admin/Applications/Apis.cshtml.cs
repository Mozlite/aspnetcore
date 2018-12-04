using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Mozlite.Mvc.Apis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mozlite.Mvc.RazorUI.Areas.Core.Pages.Admin.Applications
{
    public class ApisModel : AdminModelBase
    {
        private readonly IApiManager _apiManager;
        private readonly IApiDescriptionGroupCollectionProvider _provider;

        public ApisModel(IApiManager apiManager, IApiDescriptionGroupCollectionProvider provider)
        {
            _apiManager = apiManager;
            _provider = provider;
        }

        public IEnumerable<ApiDescriptor> Apis { get; private set; }

        public IEnumerable<ApiDescription> ApiDescriptions { get; private set; }

        public Application Application { get; private set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Application = await _apiManager.FindAsync(id);
            if (Application == null)
                return ErrorPage("应用不存在！");
            Apis = await _apiManager.LoadApplicationApisAsync(id);
            ApiDescriptions = _provider.ApiDescriptionGroups.Items.SelectMany(x => x.Items)
                .Where(x => Apis.Any(api => api.Name.Equals(x.RelativePath, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            return Page();
        }
    }
}