using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LOK.Models {
	public class UserTableViewModel {
		public bool HasFunction { get; set; }
		public string RoleName { get; set; }
		public string UserId { get; set; }
	}
	public class OrderTableViewModel {
		public bool HasFunction { get; set; }
		public string Type { get; set; }
		public string Status { get; set; }
		public string UserId { get; set; }
	}
	public class ChangeOrderStatusViewModel {
		public OrderStatusEnum Status { get; set; }
		public int OrderId { get; set; }
		public string AdminRemark { get; set; }
	}
	public class ChangeOrderAdminRemarkViewModel {
		public int OrderId { get; set; }
		public string AdminRemark { get; set; }
	}
}