using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocuAurora.API.ViewModels.Administration.Users;
using DocuAurora.Data.Models;
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

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
        }


        // GET: api/values
        [HttpGet]
        public async Task<IEnumerable<UserViewModel>> Get()
        {

            return await this._userManager.Users
                                          .Select( x => new UserViewModel()
                                          {
                                              Id = x.Id,
                                              UserName = x.UserName,
                                              Email = x.Email,
                                              Roles = x.Roles.Select(r => new UserRoleViewModel()
                                              {
                                                  RoleId = r.RoleId,
                                                  UserId = r.UserId,
                                              }).ToList(),
                                          })
                                          .ToListAsync();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
