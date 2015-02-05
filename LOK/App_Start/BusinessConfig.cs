using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using LOK.Models;

namespace LOK {
	public class ApplicationOrderManager {
		public ApplicationOrderManager() {
			context = new ApplicationDbContext();
		}
		public static ApplicationOrderManager Create() {
			return new ApplicationOrderManager();
		}
		private ApplicationDbContext context;
		public bool CreateOrderGetting(OrderGettingViewModels model, string userId) {
			//try {
			context.Orders.Add(new OrderModels() {
				OrderType = model.OrderType,
				ExpressCompany = model.ExpressCompany,
				ExpressNo = model.ExpressNo,
				Name = model.Name,
				Phone = model.Phone,
				Address = model.Address,
				Time = model.Time,
				Remark = model.Remark,
				UserId = userId
			});
			context.SaveChanges();
			return true;
			//}
			//catch(Exception e) {
			//	Debug.WriteLine(e);
			//	return false;
			//}
		}
		public bool CreateOrderSending(OrderSendingViewModels model, string userId) {
			//try {
			context.Orders.Add(new OrderModels() {
				OrderType = model.OrderType,
				ExpressCompany = model.ExpressCompany,
				Name = model.Name,
				Phone = model.Phone,
				Address = model.Address,
				ToAddress = model.ToAddress,
				Time = model.Time,
				Remark = model.Remark,
				UserId = userId
			});
			context.SaveChanges();
			return true;
			//}
			//catch(Exception e) {
			//	Debug.WriteLine(e);
			//	return false;
			//}
		}
		public List<OrderModels> GetOrders(string userId) {
			return context.Orders.Where(p => p.UserId == userId).ToList();
		}
		public List<OrderModels> GetOrdersNearly(string userId) {
			List<OrderModels> list = GetOrders(userId);
			return (from p in list
					where Convert.ToDateTime(p.Time) >= DateTime.Now.AddMonths(-1)
					select p).ToList();
		}
		public List<OrderModels> GetOrdersActive(string userId) {
			return (from p in context.Orders
					where p.UserId == userId && (p.OrderStatus == OrderStatusEnum.Confirming || p.OrderStatus == OrderStatusEnum.OnGoing)
					select p).ToList();
		}
	}
}