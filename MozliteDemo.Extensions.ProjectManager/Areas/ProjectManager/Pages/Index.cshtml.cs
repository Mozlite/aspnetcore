using Microsoft.AspNetCore.Mvc;
using MozliteDemo.Extensions.ProjectManager.Issues;

namespace MozliteDemo.Extensions.ProjectManager.Areas.ProjectManager.Pages
{
    public class IndexModel : ModelBase
    {
        private readonly IIssueManager _issueManager;

        public IndexModel(IIssueManager issueManager)
        {
            _issueManager = issueManager;
        }

        [BindProperty(SupportsGet = true)]
        public IssueQuery Query { get; set; }

        public void OnGet()
        {
            Query = _issueManager.Load(Query);
        }
    }
}