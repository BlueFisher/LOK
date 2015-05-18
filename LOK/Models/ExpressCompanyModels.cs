using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LOK.Models {
	public class ExpressCompany {
		[Key]
		public int Id { get; set; }
		public string Discription { get; set; }
		public ICollection<Order> Orders { get; set; }
	}
}