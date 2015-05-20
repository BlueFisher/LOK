using LOK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LOK.Controllers {
	public class ExpressCompanyController : Controller {

		public ExpressCompaniesManager ExpressCompaniesManager {
			get {
				return ExpressCompaniesManager.Create();
			}
		}

		[HttpPost]
		public async Task<ActionResult> Insert(string name) {
			FunctionResult result = await ExpressCompaniesManager.Insert(name);
			if(result.IsSucceed) {
				return Json(new JsonSucceedObj());
			}
			return Json(new JsonErrorObj(result.ErrorMessage));
		}
		[HttpPost]
		public async Task<ActionResult> Delete(int id) {
			FunctionResult result = await ExpressCompaniesManager.Delete(id);
			if(result.IsSucceed) {
				return Json(new JsonSucceedObj());
			}
			return Json(new JsonErrorObj(result.ErrorMessage));
		}
		[HttpPost]
		public async Task<ActionResult> Update(ExpressCompanyViewModel model) {
			FunctionResult result = await ExpressCompaniesManager.Update(model.Id, model.Discription);
			if(result.IsSucceed) {
				return Json(new JsonSucceedObj());
			}
			return Json(new JsonErrorObj(result.ErrorMessage));
		} 
	}
}