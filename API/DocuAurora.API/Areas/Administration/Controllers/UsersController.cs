using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DocuAurora.API.ViewModels.Administration.Users;
using DocuAurora.Data.Models;
using DocuAurora.Services.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DocuAurora.API.Areas.Administration.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAdminService _adminService;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UsersController(
                                            UserManager<ApplicationUser> userManager,
                                            IAdminService adminService,
                                            RoleManager<ApplicationRole> roleManager)
        {
            this._userManager = userManager;
            this._adminService = adminService;
            this._roleManager = roleManager;
        }

        // GET: api/users
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get()
        {
            var users = await this._adminService.GetAllUsersAsync();

            if (!users.Any())
            {
                return NotFound();
            }

            return Ok(users);
        }

        // GET api/users/bc719f0c-ad53-4d35-8bc4-b511dd94dc07
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(string id)
        {
            var user = await this._adminService.GetUserAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // Patch api/users/bc719f0c-ad53-4d35-8bc4-b511dd94dc07
        [HttpPatch("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Patch(string id, [FromBody] List<string> roles)
        { 
            var filteredRoleList = await this._adminService.FilterRolesThatExistsAsync(roles);

            if (!filteredRoleList.Any())
            {
                return BadRequest();
            }

            var user = await this._userManager.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var existingRoles = await this._userManager.GetRolesAsync(user);

            var checkUnique = await this._adminService.FilterRolesThatAreNotAlreadySetAsync(filteredRoleList, user);

            if (!checkUnique.Any())
            {
                return BadRequest();
            }

            // Add new roles
            var result = await this._userManager.AddToRolesAsync(user, checkUnique);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var userViewModel = await this._adminService.GetUserAsync(id);

            return Ok(userViewModel);
        }

        // DELETE api/users/bc719f0c-ad53-4d35-8bc4-b511dd94dc07
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string id, [FromBody] List<string> roles)
        {
            var filteredRoleList = await this._adminService.FilterRolesThatExistsAsync(roles);

            if (!filteredRoleList.Any())
            {
                return BadRequest();
            }

            var user = await this._userManager.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            var existingRoles = await this._userManager.GetRolesAsync(user);

            var rolesToRemove = filteredRoleList.Intersect(existingRoles);

            if (!rolesToRemove.Any())
            {
                return NotFound();
            }

            var result = await this._userManager.RemoveFromRolesAsync(user, rolesToRemove);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var userViewModel = await this._adminService.GetUserAsync(id);

            return Ok(userViewModel);

        }
    }
}
