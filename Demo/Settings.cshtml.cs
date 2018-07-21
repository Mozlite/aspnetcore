using Mozlite.Mvc;

namespace Demo
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
