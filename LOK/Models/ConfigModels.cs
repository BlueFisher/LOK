using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOK.Models {
	public static class ConfigModels {
		public static bool IsOrderGettingOnService = true;
		public static bool IsOrderSendingOnService = true;
		public static List<string> RolesList = new List<string>{
			"Guest",
			"SignedUp",
			"Admin",
			"SuperAdmin"
		};
	}
}