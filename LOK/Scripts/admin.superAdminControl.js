;
(function() {
	$.loadUsersTable(false, 'Admin', null, 'panel-admin');

	$('.panel').on('click', '.btn-find-order', function() {
		var $this = $(this);
		var userId = $this.parents('tr').attr('data-user-id');
		$modal.find('table').detach();
		$.post('/Admin/OrdersTable/Getting/NULL/' + userId, function(data, textStatus, xhr) {
			$modal.find('.modal-content').append(data);
		});
		$.post('/Admin/OrdersTable/Sending/NULL/' + userId, function(data, textStatus, xhr) {
			$data = $(data).addClass('no-margin-bottom');
			$modal.find('.modal-content').append($data);
			$modal.modal();
		});
	});

	$('#form-add-admin').submit(function() {
		var $form = $(this);
		event.preventDefault();
		$.post($form.attr('action'), $form.serialize(), function(data, textStatus, xhr) {
			// 提交的表单是否合法
			if (data.IsSucceed == true) {
				$('#modal-add-admin').modal('hide');
				$.loadUsersTable(false, 'Admin', null, 'panel-admin');
			} else {
				$('#signup-' + data.ErrorPosition.toLowerCase()).inputError(data.ErrorMessage);
			}
		});
	})
})();