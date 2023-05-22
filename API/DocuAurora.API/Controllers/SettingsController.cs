namespace DocuAurora.API.Controllers
{
    using System;
    using System.Threading.Tasks;
    using DocuAurora.Data.Common.Repositories;
    using DocuAurora.Data.Models;
    using DocuAurora.Services.Data;

    using Microsoft.AspNetCore.Mvc;

    public class SettingsController : BaseController
    {
        private readonly ISettingsService settingsService;

        private readonly IDeletableEntityRepository<Setting> repository;

        public SettingsController(ISettingsService settingsService, IDeletableEntityRepository<Setting> repository)
        {
            this.settingsService = settingsService;
            this.repository = repository;
        }

        public IActionResult Index()
        {
            
            return View();
        }

        public async Task<IActionResult> InsertSetting()
        {
            var random = new Random();
            var setting = new Setting { Name = $"Name_{random.Next()}", Value = $"Value_{random.Next()}" };

            await repository.AddAsync(setting);
            await repository.SaveChangesAsync();

            return RedirectToAction(nameof(this.Index));
        }
    }
}
