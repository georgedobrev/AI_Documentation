using System;

using Microsoft.AspNetCore.Identity;

using System.Collections.Generic;

namespace DocuAurora.API.ViewModels.Administration.Users
{
	public abstract class BaseUserViewModel
	{
		//protected BaseUserViewModel()
       // {
       //     this.Roles = new HashSet<UserRoleViewModel>();
       // }

		public string Id { get; set; }

		public string UserName { get; set; }

		public string Email { get; set; }

		public virtual ICollection<UserRoleViewModel> Roles { get; set; }

    }
}
