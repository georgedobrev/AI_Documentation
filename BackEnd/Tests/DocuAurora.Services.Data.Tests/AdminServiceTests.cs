using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.Services.Data.Tests
{
    public class AdminServiceTests : BaseServiceTests
    {
        private const string UserIdGUID = "98a1836c-0c6f-4976-8399-73cbccdfc719";

        private IAdminService AdminServiceMoq => this.ServiceProvider.GetRequiredService<IAdminService>();
    }
}
