using System;
namespace DocuAurora.API.ViewModels.Administration.Users
{
	public abstract class BaseUserRoleViewModel
	{
        public string Name { get; set; }	

        public string UserId { get; set; }

		public string RoleId { get; set; }
	}
}

