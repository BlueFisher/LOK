using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LOK.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Threading.Tasks;

namespace LOK.Controllers {
	public class BusinessController : Controller {
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
		public ApplicationOrderManager OrderManager {
			get {
				return ApplicationOrderManager.Create();
			}
		}
		#endregion

		public ActionResult Index() {
			return View();
		}
		public ActionResult Region() {
			return View();
		}
		public async Task<ActionResult> Order() {
			ViewBag.IsUserInGuest = false;
			ViewBag.User = null;
			if(Request.IsAuthenticated) {
				ViewBag.IsUserInGuest = await UserManager.IsInRoleAsync(User.Identity.GetUserId(), UserRolesEnum.Guest.ToString());
				ViewBag.User = await UserManager.FindByIdAsync(User.Identity.GetUserId());
				return View();
			}
			return View();
		}
		#region Ajax获取表单信息
		// 所有面板形式的详细订单
		public async Task<ActionResult> AjaxOrderWells(string id) {
			if(Request.IsAjaxRequest()) {
				string userId = User.Identity.GetUserId();
				switch(id) {
					case "All":
						return View(await OrderManager.GetOrdersAsync(p =>
							p.UserId == userId
						));
					case "Nearly":
						List<Order> list = await OrderManager.GetOrdersAsync(p =>
							p.UserId == userId
						);
						return View(list.Where(p =>
							Convert.ToDateTime(p.StartTime) >= DateTime.Now.AddMonths(-1)
						).ToList());
					case "Active":
						return View(await OrderManager.GetOrdersAsync(p =>
							p.UserId == userId && (p.OrderStatus == OrderStatusEnum.Confirming || p.OrderStatus == OrderStatusEnum.OnGoing)
						));
					case "Completed":
						return View(await OrderManager.GetOrdersAsync(p =>
							p.UserId == userId && p.OrderStatus == OrderStatusEnum.Completed
						));
					case "Failed":
						return View(await OrderManager.GetOrdersAsync(p =>
							p.UserId == userId && p.OrderStatus == OrderStatusEnum.Failed
						));
					case "Closed":
					default:
						return View(await OrderManager.GetOrdersAsync(p =>
							p.UserId == userId && p.OrderStatus == OrderStatusEnum.Closed
						));
				}
			}
			return RedirectToAction("Order");
		}

		// 所有菜单栏形式的简单性订单
		public async Task<ActionResult> AjaxOrderMenus() {
			if(Request.IsAjaxRequest()) {
				string userId = User.Identity.GetUserId();
				ApplicationUser user = UserManager.FindById(userId);
				if(user != null && !UserManager.IsInRole(userId, UserRolesEnum.Guest.ToString())) {
					ViewBag.NickName = user.NickName;
				}
				return View(await OrderManager.GetOrdersAsync(p =>
					p.UserId == userId && (p.OrderStatus == OrderStatusEnum.Confirming || p.OrderStatus == OrderStatusEnum.OnGoing)
				));
			}
			return RedirectToAction("Order");
		}
		#endregion

		#region 验证表单
		[HttpPost]
		public JsonResult IsOrderGettingValid(OrderGettingViewModel model) {
			if(!ModelState.IsValid) {
				return Json(new JsonErrorObj(ModelState));
			}
			return Json(new JsonSucceedObj());
		}
		[HttpPost]
		public JsonResult IsOrderSendingValid(OrderSendingViewModel model) {
			if(!ModelState.IsValid) {
				return Json(new JsonErrorObj(ModelState));
			}
			return Json(new JsonSucceedObj());
		}
		#endregion

		#region 提交订单
		[HttpPost]
		public async Task<JsonResult> OrderGetting(OrderGettingViewModel model) {
			if(!ConfigControls.IsOrderGettingOnService) {
				return Json(new JsonErrorObj("取件服务暂时不开放"));
			}
			if(!ModelState.IsValid) {
				return Json(new JsonErrorObj(ModelState));
			}
			if(Request.IsAuthenticated) {
				bool result = await OrderManager.CreateOrderGettingAsync(model, User.Identity.GetUserId());
				if(result) {
					return Json(new JsonSucceedObj());
				}
				return Json(new JsonErrorObj());
			}
			ApplicationUser voidUser = await UserManager.CreateVoidUserAsync();
			if(voidUser != null) {
				UserManager.AddToRole(voidUser.Id, UserRolesEnum.Guest.ToString());
				SignInManager.SignIn(voidUser, true, true);
				bool result = await OrderManager.CreateOrderGettingAsync(model, voidUser.Id);
				if(result) {
					return Json(new JsonSucceedObj());
				}
				return Json(new JsonErrorObj());
			}
			else {
				return Json(new JsonErrorObj("匿名用户申请失败"));
			}
		}
		[HttpPost]
		public async Task<JsonResult> OrderSending(OrderSendingViewModel model) {
			if(!ConfigControls.IsOrderSendingOnService) {
				return Json(new JsonErrorObj("寄件服务暂时不开放"));
			}
			if(!ModelState.IsValid) {
				return Json(new JsonErrorObj(ModelState));
			}
			if(Request.IsAuthenticated) {
				bool result = await OrderManager.CreateOrderSendingAsync(model, User.Identity.GetUserId());
				if(result) {
					return Json(new JsonSucceedObj());
				}
				return Json(new JsonErrorObj());
			}
			ApplicationUser voidUser = await UserManager.CreateVoidUserAsync();
			if(voidUser != null) {
				UserManager.AddToRole(voidUser.Id, UserRolesEnum.Guest.ToString());
				SignInManager.SignIn(voidUser, true, true);
				bool result = await OrderManager.CreateOrderSendingAsync(model, voidUser.Id);
				if(result) {
					return Json(new JsonSucceedObj());
				}
				return Json(new JsonErrorObj());
			}
			else {
				return Json(new JsonErrorObj("匿名用户申请失败"));
			}
		}
		#endregion


		public JsonResult IsOrderOnService() {
			return Json(new {
				Getting = ConfigControls.IsOrderGettingOnService,
				Sending = ConfigControls.IsOrderSendingOnService,
			}, JsonRequestBehavior.AllowGet);
		}


		[HttpPost]
		public async Task<JsonResult> CloseOrder(int id) {
			string userId = User.Identity.GetUserId();
			if(await OrderManager.IsOrderBelongToUser(id, userId)) {
				Order order = (await OrderManager.GetOrdersAsync(p => p.OrderId == id)).FirstOrDefault();
				if(order.OrderStatus == OrderStatusEnum.Confirming || order.OrderStatus == OrderStatusEnum.Completed || order.OrderStatus == OrderStatusEnum.Failed) {
					if(await OrderManager.ChangeOrderStatus(id, OrderStatusEnum.Closed)) {
						return Json(new JsonSucceedObj());
					}
					return Json(new JsonErrorObj("关闭失败"));
				}
				return Json(new JsonErrorObj("无法关闭当前订单"));
			}
			return Json(new JsonErrorObj("非法请求，用户与订单不匹配！"));
		}

	}

	public class JsonErrorObj {
		public JsonErrorObj() { }
		public JsonErrorObj(string errorMessage) {
			ErrorMessage = errorMessage;
		}
		public JsonErrorObj(string errorMessage, string errorPosition) {
			ErrorMessage = errorMessage;
			ErrorPosition = errorPosition;
		}
		public JsonErrorObj(ModelStateDictionary model) {
			foreach(KeyValuePair<string, ModelState> item in model) {
				ModelErrorCollection errors = item.Value.Errors;
				if(errors.Count > 0) {
					ErrorMessage = errors[0].ErrorMessage;
					ErrorPosition = item.Key;
					break;
				}
			}
		}
		public bool IsSucceed = false;
		public string ErrorMessage;
		public string ErrorPosition;
	}
	public class JsonSucceedObj {
		public bool IsSucceed = true;
	}
}