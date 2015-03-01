using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using LOK.Models;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LOK {


	public class ApplicationOrderManager {
		public ApplicationOrderManager() {
			context = new ApplicationDbContext();
		}
		public static ApplicationOrderManager Create() {
			return new ApplicationOrderManager();
		}
		private ApplicationDbContext context;

		#region 新建订单
		// 新建取件单
		public async Task<bool> CreateOrderGettingAsync(OrderGettingViewModel model, string userId) {
			try {
				context.Orders.Add(new Order() {
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
				await context.SaveChangesAsync();
				return true;
			}
			catch {
				return false;
			}
		}
		// 新建寄件单
		public async Task<bool> CreateOrderSendingAsync(OrderSendingViewModel model, string userId) {
			try {
				context.Orders.Add(new Order() {
					OrderType = model.OrderType,
					ExpressCompany = model.ExpressCompany,
					Name = model.Name,
					Phone = model.Phone,
					Address = model.Address,
					ToAddress = model.ToAddress,
					ToName = model.ToName,
					Time = model.Time,
					Remark = model.Remark,
					UserId = userId
				});
				await context.SaveChangesAsync();
				return true;
			}
			catch {
				return false;
			}
		}
		#endregion

		#region 订单查询
		public async Task<List<Order>> GetOrdersAsync() {
			return await (from p in context.Orders
						  orderby p.OrderId descending
						  select p).ToListAsync();
		}
		public async Task<List<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate) {
			return await context.Orders.Where(predicate).OrderByDescending(p => p.OrderId).ToListAsync();
		}
		#endregion

		public async Task<bool> CloseOrderAsync(int orderId, string userId) {
			try {
				Order model = await (from p in context.Orders
									 where p.UserId == userId && p.OrderId == orderId
									 select p).FirstOrDefaultAsync();
				if(model == null || model.OrderStatus == OrderStatusEnum.Failed) {
					return false;
				}
				if(model.OrderStatus == OrderStatusEnum.Confirming) {
					model.OrderStatus = OrderStatusEnum.Failed;
				}
				else if(model.OrderStatus == OrderStatusEnum.Completed) {
					model.OrderStatus = OrderStatusEnum.Closed;
				}
				context.Entry(model).Property(a => a.OrderStatus).IsModified = true;
				await context.SaveChangesAsync();
				return true;
			}
			catch {
				return false;
			}
		}
		public async Task<bool> ChangeOrderAdminRemark(int orderId, string adminRemark) {
			Order model = await (from p in context.Orders
								 where p.OrderId == orderId
								 select p).FirstOrDefaultAsync();
			if(model == null) {
				return false;
			}
			model.AdminRemark = adminRemark == string.Empty ? null : adminRemark;
			context.Entry(model).Property(p => p.AdminRemark).IsModified = true;
			await context.SaveChangesAsync();
			return true;
		}
		public async Task<bool> ChangeOrderStatus(int orderId, OrderStatusEnum status) {
			Order model = await (from p in context.Orders
								 where p.OrderId == orderId
								 select p).FirstOrDefaultAsync();
			if(model == null || model.OrderStatus == status) {
				return false;
			}
			model.OrderStatus = status;
			context.Entry(model).Property(p => p.OrderStatus).IsModified = true;
			await context.SaveChangesAsync();
			return true;
		}
		public async Task<bool> ChangeOrderStatus(int orderId, OrderStatusEnum status, string adminRemark) {
			Order model = await (from p in context.Orders
								 where p.OrderId == orderId
								 select p).FirstOrDefaultAsync();
			if(model == null || model.OrderStatus == status) {
				return false;
			}
			model.OrderStatus = status;
			model.AdminRemark = adminRemark;
			context.Entry(model).Property(p => p.OrderStatus).IsModified = true;
			context.Entry(model).Property(p => p.AdminRemark).IsModified = true;
			await context.SaveChangesAsync();
			return true;
		}
		public async Task<bool> IsOrderBelongToUser(int orderId, string userId) {
			if(userId == null) {
				return false;
			}
			Order model = await (from p in context.Orders
								 where p.UserId == userId && p.OrderId == orderId
								 select p).FirstOrDefaultAsync();
			return model == null ? false : true;
		}
		public async Task<OrderStatistics> GetOrdersStatistics() {
			List<Order> orders = await GetOrdersAsync();
			OrderStatistics os = new OrderStatistics();
			foreach(Order item in orders) {
				os.OrdersAllCount++;
				if(item.OrderType == OrderTypeEnum.Get) {
					os.OrdersGettingCount++;
					if(item.OrderStatus == OrderStatusEnum.Confirming || item.OrderStatus == OrderStatusEnum.OnGoing) {
						os.OrdersActiveCount++;
						os.OrdersGettingActiveCount++;
					}
				}
				else {
					os.OrdersSendingCount++;
					if(item.OrderStatus == OrderStatusEnum.Confirming || item.OrderStatus == OrderStatusEnum.OnGoing) {
						os.OrdersActiveCount++;
						os.OrdersSendingActiveCount++;
					}
				}
			}
			return os;
		}

		public async Task ChangeOrderGettingService(bool id) {
			ConfigControls.IsOrderGettingOnService = id;
			ConfigModels config = await context.Configs.FirstOrDefaultAsync();
			config.IsOrderGettingOnService = id;
			context.Entry(config).Property(p => p.IsOrderGettingOnService).IsModified = true;
			await context.SaveChangesAsync();
		}
		public async Task ChangeOrderSendingService(bool id) {
			ConfigControls.IsOrderSendingOnService = id;
			ConfigModels config = await context.Configs.FirstOrDefaultAsync();
			config.IsOrderSendingOnService = id;
			context.Entry(config).Property(p => p.IsOrderSendingOnService).IsModified = true;
			await context.SaveChangesAsync();
		}
		public async Task<List<OrderRecord>> GetOrderRecordsAsync() {
			return await context.OrderRecords.ToListAsync();
		}
		public async Task RecordEventAsync(int orderId, string userId, OrderStatusEnum toStatus) {
			OrderRecord record = new OrderRecord() {
				OrderId = orderId,
				UserId = userId,
				ToStatus = toStatus
			};
			context.OrderRecords.Add(record);
			await context.SaveChangesAsync();
		}
	}

	public class OrderStatistics {
		public OrderStatistics() {
			OrdersAllCount = 0;
			OrdersActiveCount = 0;
			OrdersGettingCount = 0;
			OrdersGettingActiveCount = 0;
			OrdersSendingCount = 0;
			OrdersSendingActiveCount = 0;
		}
		public int OrdersAllCount { get; set; }
		public int OrdersActiveCount { get; set; }
		public int OrdersGettingCount { get; set; }
		public int OrdersGettingActiveCount { get; set; }
		public int OrdersSendingCount { get; set; }
		public int OrdersSendingActiveCount { get; set; }
	}
}