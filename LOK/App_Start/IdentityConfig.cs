using LOK.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Text;
using System.Data.Entity;

namespace LOK {
	//public class EmailService : IIdentityMessageService {
	//	public Task SendAsync(IdentityMessage message) {
	//		SendEmail(message.Destination, message.Subject, message.Body);
	//		return Task.FromResult(0);
	//	}

	//	public static void SendEmail(string mailTo, string mailSubject, string mailContent) {
	//		// 设置发送方的邮件信息,例如使用网易的smtp
	//		string smtpServer = "smtp.qq.com"; //SMTP服务器
	//		string mailFrom = "blue_fisher@qq.com"; //登陆用户名
	//		string userPassword = "BlueFisher_";//登陆密码

	//		// 邮件服务设置
	//		SmtpClient smtpClient = new SmtpClient();
	//		smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//指定电子邮件发送方式
	//		smtpClient.Host = smtpServer; //指定SMTP服务器
	//		smtpClient.Credentials = new System.Net.NetworkCredential(mailFrom, userPassword);//用户名和密码

	//		// 发送邮件设置        
	//		MailMessage mailMessage = new MailMessage(mailFrom, mailTo); // 发送人和收件人
	//		mailMessage.Subject = mailSubject;//主题
	//		mailMessage.Body = mailContent;//内容
	//		mailMessage.BodyEncoding = Encoding.UTF8;//正文编码
	//		mailMessage.IsBodyHtml = true;//设置为HTML格式
	//		mailMessage.Priority = MailPriority.Low;//优先级
	//		smtpClient.Send(mailMessage); // 发送邮件
	//	}
	//}
	public class ApplicationUserManager : UserManager<ApplicationUser> {
		public ApplicationUserManager(IUserStore<ApplicationUser> store)
			: base(store) { }
		public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) {
			var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
			//manager.EmailService = new EmailService();
			//var dataProtectionProvider = options.DataProtectionProvider;
			//if(dataProtectionProvider != null) {
			//	manager.UserTokenProvider =
			//		new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
			//}
			return manager;
		}
		public bool IsEmailDuplicated(string email) {
			foreach(ApplicationUser user in Users) {
				if(user.Email == email)
					return true;
			}
			return false;
		}
		// 新建完整用户
		public async override Task<IdentityResult> CreateAsync(ApplicationUser user, string password) {
			await ConfigManager.AddUserSignedUp();
			return await base.CreateAsync(user, password);
		}
		// 将匿名用户转换为完整用户
		public async Task<IdentityResult> AddFullIdentityToExistedUserAsync(string userId, string email, string password, string nickName) {
			ApplicationUser user = new ApplicationUser {
				Id = userId,
				Email = email,
				PasswordHash = PasswordHasher.HashPassword(password),
				NickName = nickName,
				UserName = email
			};
			await ConfigManager.GuestTurnToSignedUp();
			return await UpdateAsync(user);
		}
		// 新建匿名用户
		public async Task<ApplicationUser> CreateVoidUserAsync() {
			ApplicationUser voidUser = new ApplicationUser();
			voidUser.UserName = DateTime.Now.ToFileTime() + new Random().Next().ToString();
			var result = await CreateAsync(voidUser);
			if(result.Succeeded) {
				return voidUser;
			}
			await ConfigManager.AddUserGuest();
			return null;
		}
		public async Task<List<ApplicationUser>> FindByRoleId(string roleId) {
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