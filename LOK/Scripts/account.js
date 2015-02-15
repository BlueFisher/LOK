$(document).ready(function() {
	$('#form-signin, #form-signup').submit(function(event) {
		var $form = $(this);
		event.preventDefault();
		$.post($form.attr('action'), $form.serialize(), function(data, textStatus, xhr) {
			// 提交的表单是否合法
			if (data.IsSucceed == true) {
				window.location = "/Business/Order";
			} else {
				if ($form.attr('id') == 'form-signin') {
					$('#signin-' + data.ErrorPosition.toLowerCase()).inputError(data.ErrorMessage);
				} else {
					$('#signup-' + data.ErrorPosition.toLowerCase()).inputError(data.ErrorMessage);
				}
			}
		});
	});
});