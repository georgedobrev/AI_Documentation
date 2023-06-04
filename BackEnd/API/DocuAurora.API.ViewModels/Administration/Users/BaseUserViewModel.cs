using System;


using System.Collections.Generic;
using DocuAurora.Services.Mapping;
using DocuAurora.Data.Models;
using AutoMapper;


namespace DocuAurora.API.ViewModels.Administration.Users
{
	public abstract class BaseUserViewModel : IMapFrom<ApplicationUser> 
	{
		protected BaseUserViewModel()
		{
			this.Roles = new HashSet<UserRoleViewModel>();
		}

		public string Id { get; set; }

		public string UserName { get; set; }

		public string Email { get; set; }

		public virtual ICollection<UserRoleViewModel> Roles { get; set; }


    }
}
