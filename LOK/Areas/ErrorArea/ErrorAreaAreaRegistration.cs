using System.Web.Mvc;

namespace LOK.Areas.ErrorArea {
	public class ErrorAreaAreaRegistration : AreaRegistration {
		public override string AreaName {
			get {
				return "ErrorArea";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context) {
			context.MapRoute(
				"ErrorArea_default",
				"ErrorArea/{action}/{id}",
				new { controller = "Default", action = "Page404", id = UrlParameter.Optional }
			);
		}
	}
}