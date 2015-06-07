;
(function () {

	$('#refresh-code').click(function () {
		$(this).prev('input').val('');
		$(this).children('img').attr('src', '/Account/CodeImage?' + Math.random());
	});

	$('#form-signin, #form-signup').submit(function(event) {
		var $form = $(this);
		event.preventDefault();
		$.post($form.attr('action'), $form.serialize(), function(data, textStatus, xhr) {
			// 提交的表单是否合法
			if (data.IsSucceed == true) {
				window.location = '/Business/Order';
			} else {
				if ($form.attr('id') == 'form-signin') {
					$('#signin-' + data.ErrorPosition.toLowerCase()).inputError(data.ErrorMessage);
				} else {
					$('#signup-' + data.ErrorPosition.toLowerCase()).inputError(data.ErrorMessage);
				}

				if (data.ErrorPosition == 'Code') {
					$('#refresh-code').click();
				}
			}
		});
	});

	$('#form-forget-password').submit(function (event) {
		var $form = $(this);
		event.preventDefault();
		$.post($form.attr('action'), $form.serialize(), function (data, textStatus, xhr) {
			// 提交的表单是否合法
			if (data.IsSucceed == true) {
				$('#modal-forget-password').modal('hide');
				toastr.success("请查看您的邮件来重置密码");
			} else {
				toastr.error(data.ErrorMessage);
			}
		});
	});
})();