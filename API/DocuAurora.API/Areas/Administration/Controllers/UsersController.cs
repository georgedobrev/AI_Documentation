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


        public UsersController(UserManager<ApplicationUser> userManager, 
                                            IAdminService adminService  )
        {
            this._userManager = userManager;
            this._adminService = adminService;
        }


        // GET: api/values
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get()
        {
            var users = await this._adminService.GetAllUsersAsync();

            if(!users.Any())
            {
                return NotFound();
            }

            return Ok(users);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(string id)
        {
            //TO DO -> create viewModel
            var user = await this._userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put()
        {
            return Ok();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(string id, [FromBody] List<string> roles)
        {
            var user =await this._userManager.Users.Include(x => x.Roles).FirstOrDefaultAsync( x=>x.Id == id);

            
            if (user == null)
            {
                return NotFound();
            }

            // Remove existing roles
            var existingRoles = await this._userManager.GetRolesAsync(user);

            //TO DO to make check if role is assined 

            // Add new roles
            var result = await this._userManager.AddToRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(user);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
