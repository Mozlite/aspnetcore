using Mozlite.Mvc;

namespace Mozlite
{
    public class SettingsModel : ModelBase
    {
        public string Message { get; set; }

        public void OnGet()
        {
            Message = "Your contact page.";
        }
    }
}
