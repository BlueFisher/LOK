using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LOK.Models {
	public enum UserRolesEnum {
		SignedUp,
		Guest,
		Admin,
		SuperAdmin
	}
	public static class ConfigControls {
		public static bool IsOrderGettingOnService = true;
		public static bool IsOrderSendingOnService = true;
		public static Dictionary<string, string> RolesMap = new Dictionary<string, string>();
	}
	public class ConfigModels {
		[Key]
		public int Id { get; set; }
		public bool IsOrderGettingOnService { get; set; }
		public bool IsOrderSendingOnService { get; set; }
		public bool IsInitialized { get; set; }
	}
}