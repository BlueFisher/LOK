using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LOK.Models {
	public class ApplicationUser : IdentityUser {
		public string NickName { get; set; }
	}
	public class ApplicationRole : IdentityRole {
		public ApplicationRole()
			: base() {

		}
		public ApplicationRole(string Name)
			: base(Name) {
		}
	}

	public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
		public ApplicationDbContext()
			: base("DefaultConnection") {
		}
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderRecord> OrderRecords { get; set; }
		public DbSet<ConfigModels> Configs { get; set; }
		public static ApplicationDbContext Create() {
			return new ApplicationDbContext();
		}
	}
}