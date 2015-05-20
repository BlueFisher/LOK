using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LOK.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;

namespace LOK.Controllers {

	public class TestController : Controller {
		private ApplicationUserManager _userManager;
		private ApplicationRoleManager _roleManager;
		private ApplicationSignInManager _signInManager;
		public ApplicationUserManager UserManager {
			get {
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set {
				_userManager = value;
			}
		}
		public ApplicationRoleManager RoleManager {
			get {
				return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
			}
			private set {
				_roleManager = value;
			}
		}
		public ApplicationSignInManager SignInManager {
			get {
				return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set {
				_signInManager = value;
			}
		}
		// GET: Test
		public ActionResult Index() {
			return View();
		}
		
		public ActionResult Another() {
			return Content("!");
		}
	}
}