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
			// 网站第一次运行初始化
			using(ApplicationDbContext context = new ApplicationDbContext()) {
				if(context.Configs.FirstOrDefault()==null) {
					InitialInTheFirstTime();
				}
			}
			// 网站重启
			using(var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext()))) {
				List<ApplicationRole> roleList = roleManager.Roles.ToList();
				foreach(ApplicationRole role in roleList) {
					ConfigControls.RolesMap.Add(role.Id, role.Name);
				}
			}
			using(ApplicationDbContext context = new ApplicationDbContext()) {
				ConfigModels config = context.Configs.First();
				ConfigControls.IsOrderGettingOnService = config.IsOrderGettingOnService;
				ConfigControls.IsOrderSendingOnService = config.IsOrderSendingOnService;
			}
		}

		private void InitialInTheFirstTime() {
			using(var roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext()))) {
				roleManager.Create(new ApplicationRole(UserRolesEnum.SignedUp.ToString()));
				roleManager.Create(new ApplicationRole(UserRolesEnum.Guest.ToString()));
				roleManager.Create(new ApplicationRole(UserRolesEnum.Admin.ToString()));
				roleManager.Create(new ApplicationRole(UserRolesEnum.SuperAdmin.ToString()));
			}
			using(var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()))) {
				ApplicationUser superAdmin = new ApplicationUser() {
					Email = "blue_fisher@qq.com",
					NickName = "Fisher_SuperAdmin",
					UserName = "blue_fisher@qq.com"
				};
				userManager.Create(superAdmin, "BlueFisher_");
				userManager.AddToRoles(superAdmin.Id, new string[] { UserRolesEnum.Admin.ToString(), UserRolesEnum.SuperAdmin.ToString() });
			}
			using(ApplicationDbContext context = new ApplicationDbContext()) {
				context.Configs.Add(new ConfigModels() {
					IsInitialized = true,
					IsOrderGettingOnService = true,
					IsOrderSendingOnService = true
				});
				context.SaveChanges();
			}
		}
	}
}