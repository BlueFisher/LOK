using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOK.Models {
	public class ApplicationUser : IdentityUser {
		public string NickName { get; set; }
		public ICollection<Order> Orders { get; set; }
	}
	public class ApplicationRole : IdentityRole {
		public ApplicationRole()
			: base() {

		}
		public ApplicationRole(string Name)
			: base(Name) {
		}
	}
}