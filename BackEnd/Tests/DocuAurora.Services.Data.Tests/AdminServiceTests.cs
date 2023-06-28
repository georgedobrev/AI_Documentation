using DocuAurora.Common;
using DocuAurora.Data;
using DocuAurora.Data.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DocuAurora.Services.Data.Tests
{
    public class AdminServiceTests : BaseServiceTests
    {
        private const string UserName = "Milcho";
        private const string emailTest = "Milcho.Kasmetov@blankfactor.com";
        private List<string> rolesExist = new List<string> { "Admin", "User", "TesT" };

        private IAdminService AdminServiceMoq => this.ServiceProvider.GetRequiredService<IAdminService>();

        private Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager => this.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>>();

        private Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> _roleManager => this.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole>>();

        [Fact]
        public async Task GetAllUsersAsyncSuccessfully()
        {
            var user = new ApplicationUser { UserName = UserName, Email = emailTest };
            var result = await _userManager.CreateAsync(user, GlobalConstants.UserPassword);

            await SeedRoleAsync(_roleManager, GlobalConstants.UserRoleName);

            await this._userManager.AddToRoleAsync(user, GlobalConstants.UserRoleName);

            var list = await this.AdminServiceMoq.GetAllUsersAsync();

            Assert.Single(list);
            Assert.Equal(UserName, list.FirstOrDefault(x => x.UserName == UserName).UserName);
        }

        [Fact]
        public async Task GetUserAsyncSuccessfully()
        {
            var user = new ApplicationUser { UserName = UserName, Email = emailTest };
            var result = await _userManager.CreateAsync(user, GlobalConstants.UserPassword);

            await SeedRoleAsync(_roleManager, GlobalConstants.UserRoleName);

            await this._userManager.AddToRoleAsync(user, GlobalConstants.UserRoleName);

            var userOutput = await this.AdminServiceMoq.GetUserAsync(user.Id);

            Assert.Equal(UserName, userOutput.UserName);
            Assert.Equal(emailTest, userOutput.Email);
        }

        [Fact]
        public async Task FilterRolesThatExistsAsyncSuccessfully()
        {
            var user = new ApplicationUser { UserName = UserName, Email = emailTest };
            var result = await _userManager.CreateAsync(user, GlobalConstants.UserPassword);

            await SeedRoleAsync(_roleManager, GlobalConstants.AdministratorRoleName);
            await SeedRoleAsync(_roleManager, GlobalConstants.UserRoleName);
            await SeedRoleAsync(_roleManager, GlobalConstants.TrainerRoleName);

            await this._userManager.AddToRoleAsync(user, GlobalConstants.UserRoleName);

            var output = await this.AdminServiceMoq.FilterRolesThatExistsAsync(rolesExist);

            Assert.Equal(2, output.Count());
            Assert.Equal(GlobalConstants.AdministratorRoleName, output.FirstOrDefault(x => x == GlobalConstants.AdministratorRoleName));
            Assert.Equal(GlobalConstants.UserRoleName, output.FirstOrDefault(x => x == GlobalConstants.UserRoleName));
        }

        private static async Task SeedRoleAsync(Microsoft.AspNetCore.Identity.RoleManager<ApplicationRole> roleManager, string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                var result = await roleManager.CreateAsync(new ApplicationRole(roleName));
            }
        }
    }
}
