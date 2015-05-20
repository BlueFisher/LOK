using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using LOK.Models;
using System.Threading.Tasks;

namespace LOK.Controllers {
	public class AccountController : Controller {
		#region ManagerDefinition
		public ApplicationUserManager UserManager {
			get {
				return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
		}
		public ApplicationSignInManager SignInManager {
			get {
				return HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
		}
		public OrderManager OrderManager {
			get {
				return OrderManager.Create();
			}
		}
		#endregion

		public ActionResult Index() {
			return View();
		}
		[HttpPost]
		public async Task<JsonResult> SignIn(SignInModel model) {
			if(!ModelState.IsValid) {
				return Json(new JsonErrorObj(ModelState));
			}
			var user = await UserManager.FindByEmailAsync(model.Email);
			if(user == null) {
				return Json(new JsonErrorObj("没有此用户", "Email"));
			}
			else if(UserManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, model.Password) == PasswordVerificationResult.Failed) {
				return Json(new JsonErrorObj("密码错误", "Password"));
			}
			if(User.Identity.IsAuthenticated) {
				ApplicationUser oldUser = await UserManager.FindByIdAsync(User.Identity.GetUserId());
				
				if(await UserManager.IsInRoleAsync(oldUser.Id, "Guest")) {
					await OrderManager.TransferOrdersAsync(oldUser.Id, user.Id);
					await UserManager.DeleteAsync(oldUser);
				}
			}
			SignInManager.SignIn(user, true, false);
			return Json(new JsonSucceedObj());
		}
		[HttpPost]
		public async Task<JsonResult> SignUp(SignUpModel model) {
			if(!ModelState.IsValid) {
				return Json(new JsonErrorObj(ModelState));
			}
			if(UserManager.IsEmailDuplicated(model.Email)) {
				return Json(new JsonErrorObj("该邮箱已被注册", "Email"));
			}

			//如果在注册之前没用匿名用户提交过订单
			IdentityResult result;
			if(!Request.IsAuthenticated) {
				var newUser = new ApplicationUser() {
					UserName = model.Email,
					Email = model.Email,
					NickName = model.NickName
				};
				result = await UserManager.CreateAsync(newUser, model.Password);
				if(!result.Succeeded) {
					return Json(new JsonErrorObj(result.Errors.First()));
				}
				//await sendConfirmEmail(newUser.Id);
				await UserManager.AddToRoleAsync(newUser.Id, "SignedUp");
				await SignInManager.SignInAsync(newUser, true, false);
				return Json(new JsonSucceedObj());
			}
			//如果在注册之前已经用匿名用户提交过订单
			ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
			user.UserName = model.Email;
			user.Email = model.Email;
			user.NickName = model.NickName;
			user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.Password);

			result = await UserManager.UpdateAsync(user);
			if(!result.Succeeded) {
				return Json(new JsonErrorObj(result.Errors.First()));
			}
			//await sendConfirmEmail(user.Id);
			await UserManager.RemoveFromRoleAsync(user.Id, "Guest");
			await UserManager.AddToRoleAsync(user.Id, "SignedUp");
			SignInManager.AuthenticationManager.SignOut();
			await SignInManager.SignInAsync(user, true, false);
			return Json(new JsonSucceedObj());
		}
		public ActionResult SignOut() {
			SignInManager.AuthenticationManager.SignOut();
			return RedirectToAction("Index", "Home");
		}
		[HttpPost]
		public async Task<JsonResult> ChangePassword(ChangePasswordModel model) {
			if(!ModelState.IsValid) {
				return Json(new JsonErrorObj(ModelState));
			}
			var user = UserManager.FindById(User.Identity.GetUserId());
			if(UserManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, model.OriPassword) == PasswordVerificationResult.Failed) {
				return Json(new JsonErrorObj("原密码不匹配", "OriPassword"));
			}
			var result = await UserManager.ChangePasswordAsync(user.Id, model.OriPassword, model.NewPassword);
			if(!result.Succeeded) {
				return Json(new JsonErrorObj(result.Errors.First()));
			}
			return Json(new JsonSucceedObj());
		}
		//public async Task<ActionResult> VerifyEmail(string userId, string code) {
		//	if(userId == null || code == null) {
		//		ViewBag.ErrorMessage = "邮箱验证失败";
		//		return RedirectToAction("Index", "Business");
		//	}
		//	var result = await UserManager.ConfirmEmailAsync(userId, code);
		//	if(result.Succeeded) {
		//		return RedirectToAction("Index", "Business");
		//	}
		//	else {
		//		ViewBag.ErrorMessage = "邮箱验证失败";
		//		return RedirectToAction("Index", "Business");
		//	}
		//}
		//public async Task<ActionResult> SendConfirmEmail() {
		//	string userId = User.Identity.GetUserId();
		//	await sendConfirmEmail(userId);
		//	return RedirectToAction("Index", "Business");
		//}

		//private async Task sendConfirmEmail(string userId) {
		//	string code = await UserManager.GenerateEmailConfirmationTokenAsync(userId);
		//	var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = userId, code = code }, protocol: Request.Url.Scheme);
		//	await UserManager.SendEmailAsync(userId, "确认你的帐户", "请通过单击 <a href=\"" + callbackUrl + "\">这里</a> 来确认你的帐户");
		//}
	}
}