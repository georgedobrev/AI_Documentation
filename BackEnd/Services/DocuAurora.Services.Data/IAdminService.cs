using DocuAurora.API.ViewModels.Administration.Users;
using DocuAurora.Data.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.Services.Data
{
    public interface IAdminService
    {
        Task<IEnumerable<UserViewModel>> GetAllUsersAsync();

        Task<UserViewModel> GetUserAsync(string id);

        Task<IEnumerable<string>> FilterRolesThatExistsAsync(IEnumerable<string> roles);

        Task<IEnumerable<string>> FilterRolesThatAreNotAlreadySetAsync(IEnumerable<string> roles, ApplicationUser user);
    }
}
