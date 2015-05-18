using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using LOK.Models;
using System.Linq.Expressions;
using LOK.ExpressionExtensions;

namespace LOK.Controllers {
	[Authorize(Roles = "Admin")]
	public class AdminController : Controller {
		#region ManagerDefinition
		public ApplicationUserManager UserManager {
			get {
				return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
		}
		public ApplicationRoleManager RoleManager {
			get {
				return HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
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
		public ActionResult Config() {
			return View();
		}
		public ActionResult UsersControl() {
			return View();
		}

		[Authorize(Roles = "SuperAdmin")]
		public async Task<ActionResult> SuperAdminControl() {
			return View(await OrderManager.GetOrderRecordsAsync());
		}

		[HttpPost]
		[Authorize(Roles = "SuperAdmin")]
		public async Task<JsonResult> AddAdmin(SignUpModel model) {
			if(!ModelState.IsValid) {
				return Json(new JsonErrorObj(ModelState));
			}
			if(UserManager.IsEmailDuplicated(model.Email)) {
				return Json(new JsonErrorObj("该邮箱已被注册", "Email"));
			}
			var newUser = new ApplicationUser() {
				UserName = model.Email,
				Email = model.Email,
				NickName = model.NickName
			};
			IdentityResult result = await UserManager.CreateAsync(newUser, model.Password);
			if(!result.Succeeded) {
				return Json(new JsonErrorObj(result.Errors.First()));
			}
			await UserManager.AddToRoleAsync(newUser.Id, "Admin");
			return Json(new JsonSucceedObj());

		}

		[HttpPost]
		public async Task<JsonResult> GetOrdersStatistics() {
			OrderStatistics os = await OrderManager.GetOrdersStatistics();
			return Json(os);
		}

		// Ajax
		public async Task<ActionResult> UsersTable(UserTableViewModel model) {
			if(!Request.IsAjaxRequest()) {
				return RedirectToAction("Index");
			}
			ViewBag.hasFunctionBtn = model.HasFunction;
			if(model.UserId != null) {
				var user = UserManager.FindById(model.UserId);
				ViewBag.RoleName = (await RoleManager.FindByIdAsync(user.Roles.First().RoleId)).Name;
				return View(new List<ApplicationUser> { user });
			}
			ViewBag.RoleName = model.RoleName;
			string roleId = (await RoleManager.FindByNameAsync(model.RoleName)).Id;
			List<ApplicationUser> userList = await UserManager.FindByRoleId(roleId);
			return View(userList);
		}

		// Ajax
		public async Task<ActionResult> OrdersTable(OrderTableViewModel model) {
			if(!Request.IsAjaxRequest()) {
				return RedirectToAction("Index");
			}
			ViewBag.hasFunctionBtn = model.HasFunction;

			Expression<Func<Order, bool>> exp;

			OrderTypeEnum orderType = model.Type == "Getting" ? OrderTypeEnum.Get : OrderTypeEnum.Send;
			ViewBag.OrderTableType = orderType;
			exp = (p => p.OrderType == orderType);

			switch(model.Status) {
				case "Full":
					break;
				case "OnGoing":
					exp = exp.And(p => p.OrderStatus == OrderStatusEnum.OnGoing);
					break;
				case "Closed":
					exp = exp.And(p => p.OrderStatus == OrderStatusEnum.Closed);
					break;
				case "Completed":
					exp = exp.And(p => p.OrderStatus == OrderStatusEnum.Completed);
					break;
				case "Confirming":
					exp = exp.And(p => p.OrderStatus == OrderStatusEnum.Confirming);
					break;
				case "Failed":
					exp = exp.And(p => p.OrderStatus == OrderStatusEnum.Failed);
					break;
				case "Active":
				default:
					exp = exp.And(p => (p.OrderStatus == OrderStatusEnum.Confirming || p.OrderStatus == OrderStatusEnum.OnGoing));
					break;
			}

			if(model.UserId != null) {
				exp = exp.And(p => p.UserId == model.UserId);
			}

			return View(await OrderManager.GetOrdersAsync(exp));
		}


		public async Task<JsonResult> ChangeOrderGettingService(bool id) {
			await ConfigManager.ChangeOrderGettingService(id);
			return Json(new JsonSucceedObj(), JsonRequestBehavior.AllowGet);
		}
		public async Task<JsonResult> ChangeOrderSendingService(bool id) {
			await ConfigManager.ChangeOrderSendingService(id);
			return Json(new JsonSucceedObj(), JsonRequestBehavior.AllowGet);
		}

		public async Task<JsonResult> ChnageOrderStatus(ChangeOrderStatusViewModel model) {
			OrderResult result = await OrderManager.ChangeOrderStatus(model.OrderId, model.Status, model.AdminRemark);
			if(result.IsSucceed) {
				await OrderManager.RecordEventAsync(model.OrderId, User.Identity.GetUserId(), model.Status);
				return Json(new JsonSucceedObj());
			}
			return Json(new JsonErrorObj(result.ErrorMessage));
		}
		public async Task<JsonResult> ChangeAdminRemark(ChangeOrderAdminRemarkViewModel model) {
			OrderResult result = await OrderManager.ChangeOrderAdminRemark(model.OrderId, model.AdminRemark);
			if(result.IsSucceed) {
				await OrderManager.RecordEventAsync(model.OrderId, User.Identity.GetUserId(), "更改备注：" + model.AdminRemark);
				return Json(new JsonSucceedObj());
			}
			return Json(new JsonErrorObj(result.ErrorMessage));
		}
	}

}