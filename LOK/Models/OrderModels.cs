using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LOK.Models {
	public enum OrderTypeEnum {
		Get,
		Send
	}
	public enum ExpressCompanyEnum {
		快递公司1,
		快递公司2,
		快递公司3
	}
	public enum OrderStatusEnum {
		Completed,
		Closed,
		Failed,
		OnGoing,
		Confirming
	}
	public class Order {
		public Order() {
			StartTime = DateTime.Now.ToString();
			OrderStatus = OrderStatusEnum.Confirming;
		}
		[Key]
		public int OrderId { get; set; }
		public OrderTypeEnum OrderType { get; set; }
		public ExpressCompanyEnum ExpressCompany { get; set; }
		public string ExpressNo { get; set; }
		public string Name { get; set; }
		public long Phone { get; set; }
		public string Address { get; set; }
		public string ToAddress { get; set; }
		public string ToName { get; set; }
		public string Remark { get; set; }
		public string AdminRemark { get; set; }
		public int Time { get; set; }
		public string StartTime { get; set; }
		public OrderStatusEnum OrderStatus { get; set; }
		public string UserId { get; set; }
		[ForeignKey("UserId")]
		public virtual ApplicationUser ApplicationUser { get; set; }
	}
	public class OrderRecord {
		public OrderRecord() {
			StartTime = DateTime.Now.ToString();
		}
		[Key]
		public int Id { get; set; }
		public OrderStatusEnum ToStatus { get; set; }
		public string Other { get; set; }
		public string StartTime { get; set; }
		public int OrderId { get; set; }
		[ForeignKey("OrderId")]
		public virtual Order Order { get; set; }
		public string UserId { get; set; }
		[ForeignKey("UserId")]
		public virtual ApplicationUser ApplicationUser { get; set; }
	}
}