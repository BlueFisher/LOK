using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LOK.Models;
using System.Threading.Tasks;
using System.Data.Entity;

namespace LOK {
	public class ExpressCompaniesManager {
		private ApplicationDbContext context;
		public ExpressCompaniesManager() {
			context = new ApplicationDbContext();
		}
		public static ExpressCompaniesManager Create() {
			return new ExpressCompaniesManager();
		}
		public async Task<ExpressCompany> FindById(int id) {
			return await context.ExpressCompanies.Where(p => p.Id == id).FirstOrDefaultAsync();
		}
		public async Task<ExpressCompany> FindByName(string name) {
			return await context.ExpressCompanies.Where(p => p.Discription == name).FirstOrDefaultAsync();
		}
		public async Task<List<ExpressCompany>> GetExpressCompaniesAsync() {
			return await context.ExpressCompanies.ToListAsync();
		}
		public List<ExpressCompany> GetExpressCompanies() {
			return context.ExpressCompanies.ToList();
		}
		public async Task<FunctionResult> Insert(string name) {
			context.ExpressCompanies.Add(new ExpressCompany {
				Discription = name
			});
			try {
				await context.SaveChangesAsync();
				return new FunctionResult();
			}
			catch(Exception e) {
				return new FunctionResult(e);
			}
		}
		public async Task<FunctionResult> Delete(int id) {
			context.Entry(new ExpressCompany {
				Id = id
			}).State = EntityState.Deleted;
			try {
				await context.SaveChangesAsync();
				return new FunctionResult();
			}
			catch(Exception e) {
				return new FunctionResult(e);
			}
		}
		public async Task<FunctionResult> Update(int id, string discription) {
			context.Entry(new ExpressCompany {
				Id = id,
				Discription = discription
			}).State = EntityState.Modified;
			try {
				await context.SaveChangesAsync();
				return new FunctionResult();
			}
			catch(Exception e) {
				return new FunctionResult(e);
			}
		}
		~ExpressCompaniesManager() {
			context.Dispose();
		}
	}
}