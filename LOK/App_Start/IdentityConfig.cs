using LOK.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.IO;

namespace LOK {
	public class EmailService : IIdentityMessageService {
		public Task SendAsync(IdentityMessage message) {
			SendEmail(message.Destination, message.Subject, message.Body);
			return Task.FromResult(0);
		}

		public static void SendEmail(string mailTo, string mailSubject, string mailContent) {
			//MailMessage mailMsg = new MailMessage();
			//mailMsg.To.Add(new MailAddress(mailTo));
			//mailMsg.From = new MailAddress("noreply@lok.com", "LOK");
			//mailMsg.Subject = mailSubject;
			//string html = mailContent;

			//mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html));

			//SmtpClient smtpClient = new SmtpClient("localhost", 25);
			//smtpClient.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
			//smtpClient.Send(mailMsg);

			FileStream fs = new FileStream("d:/test.txt",FileMode.OpenOrCreate);
			StreamWriter sw = new StreamWriter(fs);
			sw.WriteLine(mailContent);
			sw.Dispose();
		}
	}
	public class ApplicationUserManager : UserManager<ApplicationUser> {
		public ApplicationUserManager(IUserStore<ApplicationUser> store)
			: base(store) { }
		public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) {
			var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
			manager.EmailService = new EmailService();
			var dataProtectionProvider = options.DataProtectionProvider;
			if(dataProtectionProvider != null) {
				manager.UserTokenProvider =
					new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
			}
			return manager;
		}
		public bool IsEmailDuplicated(string email) {
			using(ApplicationDbContext ctx = new ApplicationDbContext()) {
				return ctx.Users.FirstOrDefault(p => p.Email == email) != null;
			}
		}
		public bool IsPhoneNumberDuplicated(string phone) {
			using(ApplicationDbContext ctx = new ApplicationDbContext()) {
				return ctx.Users.FirstOrDefault(p => p.PhoneNumber == phone) != null;
			}
		}
		// 新建完整用户
		public async override Task<IdentityResult> CreateAsync(ApplicationUser user, string password) {
			IdentityResult result = await base.CreateAsync(user, password);
			if(result.Succeeded) {
				await ConfigManager.AddUserSignedUp();
			}
			return result;
		}
		// 将匿名用户转换为完整用户
		public async Task<IdentityResult> UpdateAsyncFromExistedUserAsync(ApplicationUser user) {
			IdentityResult result = await base.UpdateAsync(user);
			if(result.Succeeded) {
				await ConfigManager.GuestTurnToSignedUp();
			}
			return result;
		}
		// 新建匿名用户
		public async Task<ApplicationUser> CreateVoidUserAsync() {
			ApplicationUser voidUser = new ApplicationUser();
			voidUser.UserName = DateTime.Now.ToFileTime() + new Random().Next().ToString();
			var result = await CreateAsync(voidUser);
			if(result.Succeeded) {
				await ConfigManager.AddUserGuest();
				return voidUser;
			}
			return null;
		}
		public async Task<ApplicationUser> FindByPhoneNumberAsync(string phone) {
			using(ApplicationDbContext context = new ApplicationDbContext()) {
				return await context.Users.FirstOrDefaultAsync(p => p.PhoneNumber == phone);
			}
		}
		public async Task<List<ApplicationUser>> FindByRoleIdAsync(string roleId) {
			using(ApplicationDbContext context = new ApplicationDbContext()) {
				return await context.Users.Where(p => p.Roles.FirstOrDefault().RoleId == roleId).ToListAsync();
			}
		}
		public async Task<UserStatistics> GetUsersStatistics() {
			using(ApplicationDbContext context = new ApplicationDbContext()) {
				List<ApplicationUser> users = await context.Users.ToListAsync();
				UserStatistics us = new UserStatistics();
				foreach(ApplicationUser user in users) {
					us.UsersAllCount++;
					if(await IsInRoleAsync(user.Id, "SignedUp")) {
						us.UsersSignedUpCount++;
					}
					else if(await IsInRoleAsync(user.Id, "Guest")) {
						us.UsersGuestCount++;
					}
				}
				return us;
			}
		}
	}

	public class ApplicationSignInManager : SignInManager<ApplicationUser, string> {
		public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
			: base(userManager, authenticationManager) { }
		public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context) {
			return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
		}
	}

	public class ApplicationRoleManager : RoleManager<ApplicationRole> {
		public ApplicationRoleManager(IRoleStore<ApplicationRole, string> store)
			: base(store) { }
		public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context) {
			return new ApplicationRoleManager(new RoleStore<ApplicationRole>(context.Get<ApplicationDbContext>()));
		}
	}
}