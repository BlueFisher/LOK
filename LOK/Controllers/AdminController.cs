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

		public async Task<ActionResult> Index() {
			ViewBag.UsersStatistics = await UserManager.GetUsersStatistics();
			ViewBag.OrdersStatistics = await OrderManager.GetOrdersStatistics();
			return View();
		}
		public ActionResult Config() {
			return View();
		}
		public ActionResult UsersControl() {
			return View();
		}
		[Authorize(Roles = "SuperAdmin")]
		public ActionResult SupreAdminControl() {
			return View();
		}

		public ActionResult UsersTable(string id) {
			if(!Request.IsAjaxRequest()) {
				return RedirectToAction("Index");
			}
			ViewBag.hasFunctionBtn = true;
			if(!ConfigControls.RolesMap.Values.Contains(id)) {
				ViewBag.hasFunctionBtn = false;
				var user = UserManager.FindById(id);
				ViewBag.RoleName = ConfigControls.RolesMap[user.Roles.First().RoleId];
				List<ApplicationUser> list = new List<ApplicationUser>();
				list.Add(user);
				return View(list);
			}
			ViewBag.RoleName = id;
			List<ApplicationUser> userList = UserManager.Users.ToList();
			userList.RemoveAll(p => ConfigControls.RolesMap[p.Roles.First().RoleId] != id);
			return View(userList);
		}

		// Ajax
		public async Task<ActionResult> OrdersTable(string type, string status, string userId) {
			if(!Request.IsAjaxRequest()) {
				return RedirectToAction("Index");
			}
			ViewBag.hasFunctionBtn = true;
			OrderTypeEnum orderType = type == "Getting" ? OrderTypeEnum.Get : OrderTypeEnum.Send;
			ViewBag.OrderTableType = orderType;

			if(status == "Active") {
				return View(await OrderManager.GetOrdersAsync(p =>
					p.OrderType == orderType && (p.OrderStatus == OrderStatusEnum.Confirming || p.OrderStatus == OrderStatusEnum.OnGoing)
				));
			}
			else if(status == "Full") {
				return View(await OrderManager.GetOrdersAsync(p =>
					p.OrderType == orderType
				));
			}

			OrderStatusEnum orderStatus;
			if(status == "Closed") {
				orderStatus = OrderStatusEnum.Closed;
			}
			else if(status == "Completed") {
				orderStatus = OrderStatusEnum.Completed;
			}
			else if(status == "Confirming") {
				orderStatus = OrderStatusEnum.Confirming;
			}
			else if(status == "Failed") {
				orderStatus = OrderStatusEnum.Failed;
			}
			else {
				orderStatus = OrderStatusEnum.OnGoing;
			}
			if(userId != "NULL") {
				ViewBag.hasFunctionBtn = false;
				return View(await OrderManager.GetOrdersAsync(p => p.UserId == userId && p.OrderType == orderType));
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

		public async Task<JsonResult> AcceptOrder(int id) {
			if(await OrderManager.ChangeOrderStatus(id, OrderStatusEnum.OnGoing)) {
				return Json(new JsonSucceedObj());
			}
			return Json(new JsonErrorObj("确认失败"));
		}
		public async Task<JsonResult> FailOrder(int id) {
			if(await OrderManager.ChangeOrderStatus(id, OrderStatusEnum.Failed)) {
				return Json(new JsonSucceedObj());
			}
			return Json(new JsonErrorObj("取消失败"));
		}
		public async Task<JsonResult> CompleteOrder(int id) {
			if(await OrderManager.ChangeOrderStatus(id, OrderStatusEnum.Completed)) {
				return Json(new JsonSucceedObj());
			}
			return Json(new JsonErrorObj("完成失败"));
		}
	}
}