using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LOK.Models {
	public class ConfigModels {
		[Key]
		public int Id { get; set; }
		public bool IsOrderGettingOnService { get; set; }
		public bool IsOrderSendingOnService { get; set; }
		public bool IsInitialized { get; set; }

		public int OrdersGettingCount { get; set; }
		public int OrdersSendingCount { get; set; }
		public int OrdersGettingActiveCount { get; set; }
		public int OrdersSendingActiveCount { get; set; }

		public int UsersCount { get; set; }
		public int UserGuestCount { get; set; }
		public int UserSignedUpCount { get; set; }
	}
}