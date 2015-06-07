using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LOK.Models {
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
		public ApplicationDbContext()
			: base("DefaultConnection") {

		}

		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderRecord> OrderRecords { get; set; }
		public DbSet<ConfigModels> Configs { get; set; }
		public DbSet<ExpressCompany> ExpressCompanies { get; set; }
		public static ApplicationDbContext Create() {
			return new ApplicationDbContext();
		}
	}

}