using AutoMapper;
using DocuAurora.Data.Models;
using DocuAurora.Services.Mapping;
using Microsoft.AspNetCore.Identity;
using System;

namespace DocuAurora.API.ViewModels.Administration.Users
{
    public abstract class BaseUserRoleViewModel : IMapFrom<IdentityUserRole<string>>, IHaveCustomMappings
    {
        public string Name { get; set; }

        public string UserId { get; set; }

        public string RoleId { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<ApplicationRole, BaseUserRoleViewModel>()
    .ForMember(x => x.Name, options =>
    {
        options.MapFrom(p => p.Name);


    });

        }
    }
}
