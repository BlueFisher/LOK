using LOK.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LOK {
	public static class ConfigManager {
		public static bool IsOrderGettingOnService = true;
		public static bool IsOrderSendingOnService = true;

		public static async Task ChangeOrderGettingService(bool id) {
			using(ApplicationDbContext context = new ApplicationDbContext()) {
				ConfigManager.IsOrderGettingOnService = id;
				ConfigModels config = await context.Configs.FirstOrDefaultAsync();
				config.IsOrderGettingOnService = id;
				context.Entry(config).Property(p => p.IsOrderGettingOnService).IsModified = true;
				await context.SaveChangesAsync();
			}
		}
		public static async Task ChangeOrderSendingService(bool id) {
			using(ApplicationDbContext context = new ApplicationDbContext()) {
				ConfigManager.IsOrderSendingOnService = id;
				ConfigModels config = await context.Configs.FirstOrDefaultAsync();
				config.IsOrderSendingOnService = id;
				context.Entry(config).Property(p => p.IsOrderSendingOnService).IsModified = true;
				await context.SaveChangesAsync();
			}
		}

		public static async Task AddOrderCount(bool isOrderGetting,int count = 1) {
			using(ApplicationDbContext context = new ApplicationDbContext()) {
				ConfigModels model = context.Configs.FirstOrDefault();
				if(isOrderGetting) {
					model.OrdersGettingCount+=count;
					model.OrdersGettingActiveCount++;
					context.Entry(model).Property(p => p.OrdersGettingCount).IsModified = true;
					context.Entry(model).Property(p => p.OrdersGettingActiveCount).IsModified = true;
				}
				else {
					model.OrdersSendingCount+=count;
					model.OrdersSendingActiveCount++;
					context.Entry(model).Property(p => p.OrdersSendingCount).IsModified = true;
					context.Entry(model).Property(p => p.OrdersSendingActiveCount).IsModified = true;
				}
				await context.SaveChangesAsync();
			}
		}
		public static async Task AddOrderActiveCount(bool isOrderGetting) {
			using(ApplicationDbContext context = new ApplicationDbContext()) {
				ConfigModels model = context.Configs.FirstOrDefault();
				if(isOrderGetting) {
					model.OrdersGettingActiveCount++;
					context.Entry(model).Property(p => p.OrdersGettingActiveCount).IsModified = true;
				}
				else {
					model.OrdersSendingActiveCount++;
					context.Entry(model).Property(p => p.OrdersSendingActiveCount).IsModified = true;
				}
				await context.SaveChangesAsync();
			}
		}
		public static async Task ReduceOrderActiveCount(bool isOrderGetting) {
			using(ApplicationDbContext context = new ApplicationDbContext()) {
				ConfigModels model = context.Configs.FirstOrDefault();
				if(isOrderGetting) {
					model.OrdersGettingActiveCount--;
					context.Entry(model).Property(p => p.OrdersGettingActiveCount).IsModified = true;
				}
				else {
					model.OrdersSendingActiveCount--;
					context.Entry(model).Property(p => p.OrdersSendingActiveCount).IsModified = true;
				}
				await context.SaveChangesAsync();
			}
		}

		public static async Task AddUserGuest() {
			using(ApplicationDbContext context = new ApplicationDbContext()) {
				ConfigModels model = context.Configs.FirstOrDefault();
				model.UserGuestCount++;
				context.Entry(model).Property(p => p.UserGuestCount).IsModified = true;
				await context.SaveChangesAsync();
			}
		}
		public static async Task AddUserSignedUp() {
			using(ApplicationDbContext context = new ApplicationDbContext()) {
				ConfigModels model = context.Configs.FirstOrDefault();
				model.UserSignedUpCount++;
				context.Entry(model).Property(p => p.UserSignedUpCount).IsModified = true;
				await context.SaveChangesAsync();
			}
		}
		public static async Task GuestTurnToSignedUp() {
			using(ApplicationDbContext context = new ApplicationDbContext()) {
				ConfigModels model = context.Configs.FirstOrDefault();
				model.UserGuestCount--;
				model.UserSignedUpCount++;
				context.Entry(model).Property(p => p.UserGuestCount).IsModified = true;
				context.Entry(model).Property(p => p.UserSignedUpCount).IsModified = true;
				await context.SaveChangesAsync();
			}
		}
	}
}