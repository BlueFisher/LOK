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
		public ApplicationOrderManager OrderManager {
			get {
				return ApplicationOrderManager.Create();
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
		[HttpPost]
		public async Task<JsonResult> GetOrdersStatistics() {
			return Json(await OrderManager.GetOrdersStatistics());
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
			await UserManager.AddToRoleAsync(newUser.Id, UserRolesEnum.Admin.ToString());
			return Json(new JsonSucceedObj());

		}

		// Ajax
		public ActionResult UsersTable(UserTableViewModel model) {
			if(!Request.IsAjaxRequest()) {
				return RedirectToAction("Index");
			}
			ViewBag.hasFunctionBtn = model.HasFunction;
			if(model.UserId != null) {
				var user = UserManager.FindById(model.UserId);
				ViewBag.RoleName = ConfigControls.RolesMap[user.Roles.First().RoleId];
				return View(new List<ApplicationUser> { user });
			}
			ViewBag.RoleName = model.RoleName;
			List<ApplicationUser> userList = UserManager.Users.ToList();
			userList.RemoveAll(p => ConfigControls.RolesMap[p.Roles.First().RoleId] != model.RoleName);
			return View(userList);
		}

		// Ajax
		public async Task<ActionResult> OrdersTable(OrderTableViewModel model) {
			if(!Request.IsAjaxRequest()) {
				return RedirectToAction("Index");
			}
			ViewBag.hasFunctionBtn = model.HasFunction;
			OrderTypeEnum orderType = model.Type == "Getting" ? OrderTypeEnum.Get : OrderTypeEnum.Send;
			ViewBag.OrderTableType = orderType;

			if(model.Status == "Active") {
				return View(await OrderManager.GetOrdersAsync(p =>
					p.OrderType == orderType && (p.OrderStatus == OrderStatusEnum.Confirming || p.OrderStatus == OrderStatusEnum.OnGoing)
				));
			}
			else if(model.Status == "Full") {
				return View(await OrderManager.GetOrdersAsync(p =>
					p.OrderType == orderType
				));
			}

			OrderStatusEnum orderStatus;
			if(model.Status == "Closed") {
				orderStatus = OrderStatusEnum.Closed;
			}
			else if(model.Status == "Completed") {
				orderStatus = OrderStatusEnum.Completed;
			}
			else if(model.Status == "Confirming") {
				orderStatus = OrderStatusEnum.Confirming;
			}
			else if(model.Status == "Failed") {
				orderStatus = OrderStatusEnum.Failed;
			}
			else {
				orderStatus = OrderStatusEnum.OnGoing;
			}
			if(model.UserId != null) {
				return View(await OrderManager.GetOrdersAsync(p => p.UserId == model.UserId && p.OrderType == orderType));
			}
			else {
				return View(await OrderManager.GetOrdersAsync(p => p.OrderType == orderType && p.OrderStatus == orderStatus));
			}
		}


		public async Task<JsonResult> ChangeOrderGettingService(bool id) {
			await OrderManager.ChangeOrderGettingService(id);
			return Json(new JsonSucceedObj(), JsonRequestBehavior.AllowGet);
		}
		public async Task<JsonResult> ChangeOrderSendingService(bool id) {
			await OrderManager.ChangeOrderSendingService(id);
			return Json(new JsonSucceedObj(), JsonRequestBehavior.AllowGet);
		}

		public async Task<JsonResult> ChnageOrderStatus(ChangeOrderStatusViewModel model) {
			if(model.AdminRemark == null) {
				if(await OrderManager.ChangeOrderStatus(model.OrderId, model.Status)) {
					await OrderManager.RecordEventAsync(model.OrderId, User.Identity.GetUserId(), model.Status);
					return Json(new JsonSucceedObj());
				}
				return Json(new JsonErrorObj("修改失败"));
			}
			if(await OrderManager.ChangeOrderStatus(model.OrderId, model.Status, model.AdminRemark)) {
				await OrderManager.RecordEventAsync(model.OrderId, User.Identity.GetUserId(), model.Status);
				return Json(new JsonSucceedObj());
			}
			return Json(new JsonErrorObj("修改失败"));
		}
		public async Task<JsonResult> ChangeAdminRemark(ChangeOrderAdminRemarkViewModel model) {
			if(await OrderManager.ChangeOrderAdminRemark(model.OrderId, model.AdminRemark)) {
				await OrderManager.RecordEventAsync(model.OrderId, User.Identity.GetUserId(), "更改备注：" + model.AdminRemark);
				return Json(new JsonSucceedObj());
			}
			return Json(new JsonErrorObj("修改失败"));
		}
	}
}