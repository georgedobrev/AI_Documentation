using DocuAurora.API.ViewModels.Administration.Users;
using DocuAurora.Data.Models;
using DocuAurora.Services.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.Services.Data
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AdminService(
                            UserManager<ApplicationUser> userManager,
                            RoleManager<ApplicationRole> roleManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        public async Task<IEnumerable<string>> FilterRolesThatAreNotAlreadySetAsync(IEnumerable<string> roles, ApplicationUser user)
        {
            var existingRoles = await this._userManager.GetRolesAsync(user);

            var checkUnique = roles.Except(existingRoles);

            return checkUnique;
        }

        public async Task<IEnumerable<string>> FilterRolesThatExistsAsync(IEnumerable<string> roles)
        {
            var dbRoles = await this._roleManager.Roles
                                                 .Select(x => x.Name)
                                                 .ToListAsync();

            var filteredRoleList = dbRoles.Where(roles.Contains).ToList();

            return filteredRoleList;
        }

        public async Task<IEnumerable<T>> GetAllUsersAsync<T>()
        {

            IQueryable<ApplicationUser> query = this._userManager.Users.Include(x => x.Roles);
    


            return query.To<T>().ToList();





            //var users = await this._userManager.Users
            //                                   .Include(x => x.Roles)
            //                                   .ToListAsync();

            //var userViewModels = new List<UserViewModel>();

            //foreach (var user in users)
            //{
            //    var roles = await this._userManager.GetRolesAsync(user);

            //    var userViewModel = new UserViewModel()
            //    {
            //        Id = user.Id,
            //        UserName = user.UserName,
            //        Email = user.Email,
            //        Roles = roles.Select(role => new UserRoleViewModel()
            //        {
            //            Name = role,
            //            RoleId = user.Roles.Select(x => x.RoleId).FirstOrDefault(),
            //            UserId = user.Roles.Select(r => r.UserId).FirstOrDefault(),
            //        }).ToList(),
            //    };

            //    userViewModels.Add(userViewModel);
            //}

            //return userViewModels;
        }

        public async Task<UserViewModel> GetUserAsync(string id)
        {
            var user = await this._userManager.Users
                                              .Include(x => x.Roles)
                                              .FirstOrDefaultAsync(x => x.Id == id);

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

            return userViewModel;
        }
    }
}
