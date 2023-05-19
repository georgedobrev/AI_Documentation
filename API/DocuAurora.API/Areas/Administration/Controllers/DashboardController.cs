namespace DocuAurora.API.Areas.Administration.Controllers
{
    using DocuAurora.Services.Data;
    using DocuAurora.Web.ViewModels.Administration.Dashboard;

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
            var viewModel = new IndexViewModel { SettingsCount = settingsService.GetCount(), };
            return View(viewModel);
        }
    }
}
