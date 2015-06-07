using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LOK.Areas.ErrorArea.Controllers {
	public class DefaultController : Controller {
		// GET: ErrorArea/Default
		public ActionResult Page404() {
			return View();
		}
	}
}