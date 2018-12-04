using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Mozlite.Mvc.Apis;

namespace Mozlite.Mvc.RazorUI.Areas.Core.Pages.Admin.Apis
{
    public class IndexModel : AdminModelBase
    {
        private readonly IApiManager _apiManager;
        private readonly IApiDescriptionGroupCollectionProvider _provider;

        public IndexModel(IApiManager apiManager, IApiDescriptionGroupCollectionProvider provider)
        {
            _apiManager = apiManager;
            _provider = provider;
        }

        public IEnumerable<ApiDescriptor> Apis { get; private set; }
        
        public IEnumerable<ApiDescription> ApiDescriptions { get; private set; }

        public async Task OnGetAsync()
        {
            Apis = await _apiManager.LoadApisAsync();
            ApiDescriptions = _provider.ApiDescriptionGroups.Items.SelectMany(x => x.Items)
                .Where(x => Apis.Any(api => api.Name.Equals(x.RelativePath, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }
    }
}