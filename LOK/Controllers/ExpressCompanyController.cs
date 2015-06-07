using LOK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LOK.Controllers {

	// Admin/ExpressCompany/{action}

	[Authorize(Roles = "Admin")]
	public class ExpressCompanyController : Controller {

		public ExpressCompaniesManager ExpressCompaniesManager {
			get {
				return ExpressCompaniesManager.Create();
			}
		}

		[HttpPost]
		public async Task<JsonResult> Insert(string name) {
			FunctionResult result = await ExpressCompaniesManager.Insert(name);
			if(result.IsSucceed) {
				return Json(new JsonSucceedObj());
			}
			return Json(new JsonErrorObj(result.ErrorMessage));
		}
		[HttpPost]
		public async Task<JsonResult> Delete(int id) {
			FunctionResult result = await ExpressCompaniesManager.Delete(id);
			if(result.IsSucceed) {
				return Json(new JsonSucceedObj());
			}
			return Json(new JsonErrorObj(result.ErrorMessage));
		}
		[HttpPost]
		public async Task<JsonResult> Update(ExpressCompanyViewModel model) {
			FunctionResult result = await ExpressCompaniesManager.Update(model.Id, model.Discription);
			if(result.IsSucceed) {
				return Json(new JsonSucceedObj());
			}
			return Json(new JsonErrorObj(result.ErrorMessage));
		}
		[HttpPost]
		public async Task<JsonResult> Find() {
			List<ExpressCompany> list = await ExpressCompaniesManager.GetExpressCompaniesAsync();
			return Json(list);
		}
	}
}