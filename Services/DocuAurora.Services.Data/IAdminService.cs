using DocuAurora.API.ViewModels.Administration.Users;
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
    }
}
