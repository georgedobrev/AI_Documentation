﻿using DocuAurora.Common;
using DocuAurora.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuAurora.Data.Seeding
{
    public class UserSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = new ApplicationUser
            {
                UserName = GlobalConstants.UserUserName,
                Email = GlobalConstants.UserEmail,
            };

            var result = await userManager.CreateAsync(user, GlobalConstants.UserPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, GlobalConstants.UserRoleName);
            }
        }
    }
}
