using DocuAurora.API.ViewModels.Administration.Users;
using DocuAurora.Data.Models;
using DocuAurora.Services.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.Services.Data
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
        }

        public async Task<IEnumerable<UserViewModel>> GetAllUsersAsync()
        {
            var users = await this._userManager.Users.Include(x => x.Roles).ToListAsync();

            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await this._userManager.GetRolesAsync(user);

                var userViewModel = new UserViewModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.Select(role => new UserRoleViewModel()
                    {
                        Name = role,
                        RoleId = user.Roles.Select(x => x.RoleId).FirstOrDefault(),
                        UserId = user.Roles.Select(r => r.UserId).FirstOrDefault(),
                    }).ToList(),
                };

                userViewModels.Add(userViewModel);
            }

            return userViewModels;
        }

    }
}
