using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LOK.Models {
	public class SignInModel {
		[Required(ErrorMessage = "邮箱不能为空")]
		public string Email { get; set; }
		[Required(ErrorMessage = "密码不能为空")]
		public string Password { get; set; }
	}
	public class SignUpModel {
		[Required(ErrorMessage = "邮箱不能为空")]
		[RegularExpression(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "邮箱格式错误")]
		public string Email { get; set; }

		[Required(ErrorMessage = "昵称不能为空")]
		[StringLength(10, ErrorMessage = "昵称最多只能输入10个字符")]
		public string NickName { get; set; }

		[Required(ErrorMessage = "密码不能为空")]
		[StringLength(16, MinimumLength = 6, ErrorMessage = "密码必须在6-16个字符之间")]
		public string Password { get; set; }
		[Required(ErrorMessage = "必须再次输入密码")]
		[Compare("Password", ErrorMessage = "两次密码输入不一致")]
		public string PasswordRepeat { get; set; }
	}
	public class ChangePasswordModel {
		[Required(ErrorMessage = "原密码不能为空")]
		public string OriPassword { get; set; }
		[Required(ErrorMessage = "新密码不能为空")]
		[StringLength(16, MinimumLength = 6, ErrorMessage = "密码必须在6-16个字符之间")]
		public string NewPassword { get; set; }
		[Required(ErrorMessage = "必须再次输入密码")]
		[Compare("NewPassword", ErrorMessage = "两次密码输入不一致")]
		public string NewPasswordRepeat { get; set; }
	}
}