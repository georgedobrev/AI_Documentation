namespace DocuAurora.API.Areas.Administration.Controllers
{
    using DocuAurora.API.Controllers;
    using DocuAurora.Common;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area("Administration")]
    public class AdministrationController : BaseController
    {
    }
}
