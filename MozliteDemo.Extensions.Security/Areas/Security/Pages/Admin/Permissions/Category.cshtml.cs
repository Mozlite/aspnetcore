using System.Collections.Generic;
using Mozlite.Extensions.Security.Permissions;

namespace MozliteDemo.Extensions.Security.Areas.Security.Pages.Admin.Permissions
{
    public class CategoryModel : ModelBase
    {
        private readonly ICategoryManager _categoryManager;

        public CategoryModel(ICategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        public IEnumerable<Category> Categories { get; private set; }

        public void OnGet()
        {
            Categories = _categoryManager.Fetch();
        }
    }
}