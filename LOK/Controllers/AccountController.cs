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
using LOK.Utility;

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
			if(model.Code.ToLower() != (string)Session["Code"]) {
				return Json(new JsonErrorObj("验证码错误", "Code"));
			}
			ApplicationUser user;
			if(Util.IsEmail(model.SignInString)) {
				user = await UserManager.FindByEmailAsync(model.SignInString);
			}
			else {
				user = await UserManager.FindByPhoneNumberAsync(model.SignInString);
			}
			if(user == null) {
				return Json(new JsonErrorObj("没有此用户", "SignInString"));
			}

			else if(!await UserManager.CheckPasswordAsync(user, model.Password)) {
				return Json(new JsonErrorObj("密码错误", "Password"));
			}
			// 如果登录之前有匿名用户提交过订单
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
			if(UserManager.IsPhoneNumberDuplicated(model.PhoneNumber)) {
				return Json(new JsonErrorObj("该手机号已被注册", "PhoneNumber"));
			}

			//如果在注册之前没用匿名用户提交过订单
			IdentityResult result;
			if(!Request.IsAuthenticated) {
				var newUser = new ApplicationUser() {
					UserName = model.Email,
					Email = model.Email,
					NickName = model.NickName,
					PhoneNumber = model.PhoneNumber
				};
				result = await UserManager.CreateAsync(newUser, model.Password);
				if(!result.Succeeded) {
					return Json(new JsonErrorObj(result.Errors.First()));
				}
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
			user.PhoneNumber = model.PhoneNumber;

			result = await UserManager.UpdateAsyncFromExistedUserAsync(user);
			if(!result.Succeeded) {
				return Json(new JsonErrorObj(result.Errors.First()));
			}
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
		public FileContentResult CodeImage() {
			string code = Util.CreateRandomCode();
			Session["Code"] = code.ToLower();
			return File(Util.CreateCheckCodeImage(code), "image/jpeg");
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
		[HttpPost]
		public async Task<JsonResult> ForgetPassword(string email) {
			ApplicationUser user = await UserManager.FindByEmailAsync(email);
			if(user == null) {
				return Json(new JsonErrorObj("不存在此邮箱"));
			}
			await sendConfirmEmail(user.Id);
			return Json(new JsonSucceedObj());
		}
		[HttpGet]
		public ActionResult ResetPassword(string userId, string code) {
			ViewBag.UserId = userId;
			ViewBag.Code = code;
			return View();
		}
		[HttpPost]
		public async Task<JsonResult> ResetPassword(ResetPasswordModel model) {
			if(!ModelState.IsValid) {
				return Json(new JsonErrorObj(ModelState));
			}
			IdentityResult result = await UserManager.ResetPasswordAsync(model.UserId, model.Code, model.NewPassword);
			if(result.Succeeded) {
				return Json(new JsonSucceedObj());
			}
			return Json(new JsonErrorObj(result.Errors.First()));
		}

		private async Task sendConfirmEmail(string userId) {
			string code = await UserManager.GeneratePasswordResetTokenAsync(userId);
			var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = userId, code = code }, protocol: Request.Url.Scheme);
			await UserManager.SendEmailAsync(userId, "密码重置", "<a href=\"" + callbackUrl + "\">重置密码</a>");
		}
	}
}