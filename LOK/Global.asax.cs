﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LOK {
	public class MvcApplication : System.Web.HttpApplication {
		protected void Application_Start() {
			
			RouteConfig.RegisterRoutes(RouteTable.Routes);
		}
	}
}
