using LOK.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LOK {
	public partial class Startup {
		public void ConfigureAuth(IAppBuilder app) {
			app.CreatePerOwinContext(ApplicationDbContext.Create);
			app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
			app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
			app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

			// 配置Middleware 組件
			app.UseCookieAuthentication(new CookieAuthenticationOptions {
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				LoginPath = new PathString("/Account"),
				CookieSecure = CookieSecureOption.Never,
				ExpireTimeSpan = new TimeSpan(7, 0, 0, 0, 0)
			});

			// 网站启动
			using(ApplicationDbContext context = new ApplicationDbContext()) {
				ConfigModels config = context.Configs.FirstOrDefault();
				// 网站第一次运行初始化
				if(config == null) {
					InitialInTheFirstTime();
				}
				config = context.Configs.FirstOrDefault();
				ConfigManager.IsOrderGettingOnService = config.IsOrderGettingOnService;
				ConfigManager.IsOrderSendingOnService = config.IsOrderSendingOnService;
			}
		}

		private void InitialInTheFirstTime() {
			using(ApplicationDbContext context = new ApplicationDbContext()) {
				context.Configs.Add(new ConfigModels() {
					IsInitialized = true,
					IsOrderGettingOnService = true,
					IsOrderSendingOnService = true
				});
				context.ExpressCompanies.Add(new ExpressCompany {
					Discription = "申通快递"
				});
				context.ExpressCompanies.Add(new ExpressCompany {
					Discription = "圆通快递"
				});
				context.ExpressCompanies.Add(new ExpressCompany {
					Discription = "韵达快递"
				});
				context.ExpressCompanies.Add(new ExpressCompany {
					Discription = "中通快递"
				});
				context.ExpressCompanies.Add(new ExpressCompany {
					Discription = "顺丰快递"
				});

				context.SaveChanges();
			}

			using(var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext()))) {
				roleManager.Create(new ApplicationRole("SignedUp"));
				roleManager.Create(new ApplicationRole("Guest"));
				roleManager.Create(new ApplicationRole("Admin"));
				roleManager.Create(new ApplicationRole("SuperAdmin"));
			}
			using(var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()))) {
				ApplicationUser superAdmin = new ApplicationUser() {
					Email = "blue_fisher@qq.com",
					NickName = "Fisher_SuperAdmin",
					UserName = "blue_fisher@qq.com",
					PhoneNumber = "13917601178"
				};
				userManager.Create(superAdmin, "BlueFisher_");
				userManager.AddToRoles(superAdmin.Id, new string[] { "Admin", "SuperAdmin" });
			}
		}
	}
}