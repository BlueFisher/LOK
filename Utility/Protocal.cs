using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LOK.Models;

namespace LOK.Models {
	public class JsonErrorObj {
		public JsonErrorObj() { }
		public JsonErrorObj(string errorMessage) {
			ErrorMessage = errorMessage;
		}
		public JsonErrorObj(string errorMessage, string errorPosition) {
			ErrorMessage = errorMessage;
			ErrorPosition = errorPosition;
		}
		public JsonErrorObj(ModelStateDictionary model) {
			foreach(KeyValuePair<string, ModelState> item in model) {
				ModelErrorCollection errors = item.Value.Errors;
				if(errors.Count > 0) {
					ErrorMessage = errors[0].ErrorMessage;
					ErrorPosition = item.Key;
					break;
				}
			}
		}
		public bool IsSucceed = false;
		public string ErrorMessage;
		public string ErrorPosition;
	}
	public class JsonSucceedObj {
		public bool IsSucceed = true;
	}
	public class FunctionResult {
		public FunctionResult() {
			IsSucceed = true;
		}
		public FunctionResult(string errorMsg) {
			IsSucceed = false;
			ErrorMessage = errorMsg;
			ErrorHResult = 1000;
		}
		public FunctionResult(Exception e) {
			IsSucceed = false;
			ErrorMessage = e.Message;
			ErrorHResult = e.HResult;
		}
		public bool IsSucceed { get; set; }
		public string ErrorMessage { get; set; }
		public int ErrorHResult { get; set; }
	}

	public class OrderStatistics {
		public OrderStatistics() {
			OrdersAllCount = 0;
			OrdersActiveCount = 0;
			OrdersGettingCount = 0;
			OrdersGettingActiveCount = 0;
			OrdersSendingCount = 0;
			OrdersSendingActiveCount = 0;
		}
		public int OrdersAllCount { get; set; }
		public int OrdersActiveCount { get; set; }
		public int OrdersGettingCount { get; set; }
		public int OrdersGettingActiveCount { get; set; }
		public int OrdersSendingCount { get; set; }
		public int OrdersSendingActiveCount { get; set; }
	}

	public class UserStatistics {
		public UserStatistics() {
			UsersAllCount = 0;
			UsersSignedUpCount = 0;
			UsersGuestCount = 0;
		}
		public int UsersAllCount { get; set; }
		public int UsersSignedUpCount { get; set; }
		public int UsersGuestCount { get; set; }
	}
}