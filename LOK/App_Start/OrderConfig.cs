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
	public class OrderManager {
		public OrderManager() {
			context = new ApplicationDbContext();
		}
		public static OrderManager Create() {
			return new OrderManager();
		}
		private ApplicationDbContext context;

		#region 新建订单
		// 新建取件单
		public async Task<OrderResult> CreateOrderGettingAsync(OrderGettingViewModel model, string userId) {
			try {
				context.Orders.Add(new Order() {
					OrderType = model.OrderType,
					ExpressCompanyId = model.ExpressCompany,
					ExpressNo = model.ExpressNo,
					Name = model.Name,
					Phone = model.Phone,
					Address = model.Address,
					Time = model.Time,
					Remark = model.Remark,
					UserId = userId
				});
				await context.SaveChangesAsync();
				await ConfigManager.AddOrderCount(true);
				return new OrderResult();
			}
			catch(Exception e) {
				return new OrderResult(e.Message);
			}
		}
		// 新建寄件单
		public async Task<OrderResult> CreateOrderSendingAsync(OrderSendingViewModel model, string userId) {
			try {
				context.Orders.Add(new Order() {
					OrderType = model.OrderType,
					ExpressCompanyId = model.ExpressCompany,
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
				await ConfigManager.AddOrderCount(false);
				return new OrderResult();
			}
			catch(Exception e) {
				return new OrderResult(e.Message);
			}
		}
		// 将旧用户下的所有订单转移到新用户上
		public async Task<OrderResult> TransferOrdersAsync(string oldUserId, string newUserId) {
			try {
				List<Order> orders = await GetOrdersAsync(p => p.UserId == oldUserId);
				foreach(Order o in orders) {
					o.ApplicationUser = null;
					o.UserId = newUserId;
					context.Entry<Order>(o).State = EntityState.Modified;
				}
				await context.SaveChangesAsync();
				return new OrderResult();
			}
			catch(Exception e) {
				return new OrderResult(e.Message);
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

		#region 修改订单
		public async Task<OrderResult> ChangeOrderAdminRemark(int orderId, string adminRemark) {
			Order model = await (from p in context.Orders
								 where p.OrderId == orderId
								 select p).FirstOrDefaultAsync();
			if(model == null) {
				return new OrderResult("未找到订单");
			}
			model.AdminRemark = adminRemark == string.Empty ? null : adminRemark;
			context.Entry(model).Property(p => p.AdminRemark).IsModified = true;
			await context.SaveChangesAsync();
			return new OrderResult();
		}

		public async Task<OrderResult> ChangeOrderStatus(int orderId, OrderStatusEnum status, string adminRemark) {
			Order order = await (from p in context.Orders
								 where p.OrderId == orderId
								 select p).FirstOrDefaultAsync();
			if(order == null) {
				return new OrderResult("订单不存在");
			}

			// 订单状态是否能被修改
			if(order.OrderStatus < status) {
				return new OrderResult("无法改变订单状态");
			}

			order.OrderStatus = status;
			context.Entry(order).Property(p => p.OrderStatus).IsModified = true;
			if(adminRemark != null) {
				order.AdminRemark = adminRemark;
				context.Entry(order).Property(p => p.AdminRemark).IsModified = true;
			}
			await context.SaveChangesAsync();

			// 增加统计数
			if(status == OrderStatusEnum.Confirming) {
				await ConfigManager.AddOrderActiveCount(order.OrderType == OrderTypeEnum.Get);
			}
			else if(status != OrderStatusEnum.OnGoing) {
				await ConfigManager.ReduceOrderActiveCount(order.OrderType == OrderTypeEnum.Get);
			}

			return new OrderResult();
		} 
		#endregion

		#region 订单变更记录
		public async Task<List<OrderRecord>> GetOrderRecordsAsync() {
			return await context.OrderRecords.OrderByDescending(p => p.Id).ToListAsync();
		}
		public async Task RecordEventAsync(int orderId, string userId, OrderStatusEnum toStatus) {
			OrderRecord record = new OrderRecord {
				OrderId = orderId,
				UserId = userId,
				ToStatus = toStatus
			};
			context.OrderRecords.Add(record);
			await context.SaveChangesAsync();
		}
		public async Task RecordEventAsync(int orderId, string userId, string other) {
			OrderRecord record = new OrderRecord {
				OrderId = orderId,
				UserId = userId,
				Other = other,
			};
			context.OrderRecords.Add(record);
			await context.SaveChangesAsync();
		}

		public async Task<OrderStatistics> GetOrdersStatistics() {
			OrderStatistics os;
			ConfigModels model = await context.Configs.FirstOrDefaultAsync();
			os = new OrderStatistics {
				OrdersGettingCount = model.OrdersGettingCount,
				OrdersSendingCount = model.OrdersSendingCount,
				OrdersGettingActiveCount = model.OrdersGettingActiveCount,
				OrdersSendingActiveCount = model.OrdersSendingActiveCount,
				OrdersActiveCount = model.OrdersGettingActiveCount + model.OrdersSendingActiveCount,
				OrdersAllCount = model.OrdersGettingCount + model.OrdersSendingCount
			};
			return os;
		} 
		#endregion

		public async Task<bool> IsOrderBelongToUser(int orderId, string userId) {
			if(userId == null) {
				return false;
			}
			Order model = await (from p in context.Orders
								 where p.UserId == userId && p.OrderId == orderId
								 select p).FirstOrDefaultAsync();
			return model == null ? false : true;
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