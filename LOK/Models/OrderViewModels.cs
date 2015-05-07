using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LOK.Models {
	public class OrderGettingViewModel {
		public OrderTypeEnum OrderType = OrderTypeEnum.Get;
		[Required(ErrorMessage = "必须填写取件人姓名")]
		public string Name { get; set; }
		[Required(ErrorMessage = "必须填写联系电话")]
		public long Phone { get; set; }
		[Required]
		public ExpressCompanyEnum ExpressCompany { get; set; }
		[Required(ErrorMessage = "必须填写快递号")]
		public string ExpressNo { get; set; }
		[Required(ErrorMessage = "必须填写上门地址")]
		public string Address { get; set; }
		[Required]
		public int Time { get; set; }
		public string Remark { get; set; }
	}
	public class OrderSendingViewModel {
		public OrderTypeEnum OrderType = OrderTypeEnum.Send;
		[Required]
		public ExpressCompanyEnum ExpressCompany { get; set; }
		[Required(ErrorMessage="必须填写寄件人姓名")]
		public string Name { get; set; }
		[Required(ErrorMessage = "必须填写联系电话")]
		public long Phone { get; set; }
		[Required(ErrorMessage = "必须填写上门地址")]
		public string Address { get; set; }
		[Required(ErrorMessage = "必须填写目的地址")]
		public string ToAddress { get; set; }
		[Required(ErrorMessage = "必须填写取件人姓名")]
		public string ToName { get; set; }
		[Required]
		public int Time { get; set; }
		public string Remark { get; set; }
	}
}