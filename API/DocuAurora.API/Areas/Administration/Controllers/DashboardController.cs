namespace DocuAurora.API.Areas.Administration.Controllers
{
    using DocuAurora.Services.Data;

    using Microsoft.AspNetCore.Mvc;

    public class DashboardController : AdministrationController
    {
        private readonly ISettingsService settingsService;

        public DashboardController(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
