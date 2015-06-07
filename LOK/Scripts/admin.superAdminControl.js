;
(function () {
	function _loadUsersTable(roleName, panelId) {
		var $panel = $('#' + panelId);
		$.post('/Admin/Ajax/UsersTable/', {
			HasFunction: false,
			RoleName: roleName,
		}, function (data, textStatus, xhr) {
			$data = $(data).addClass('animated fadeIn').css({
				'-webkit-animation-duration': '0.3s',
				'animation-duration': '0.3s'
			});
			$panel.find('table').detach();
			$panel.append($data);
		});
	}

	_loadUsersTable('Admin', 'panel-admin');

	$('.panel').on('click', '.btn-find-order', function () {
		var $this = $(this);
		var userId = $this.parents('tr').attr('data-user-id');
		$modal.find('table').detach();
		$.post('/Admin/OrdersTable/Getting/NULL/' + userId, function (data, textStatus, xhr) {
			$modal.find('.modal-content').append(data);
		});
		$.post('/Admin/OrdersTable/Sending/NULL/' + userId, function (data, textStatus, xhr) {
			$data = $(data).addClass('no-margin-bottom');
			$modal.find('.modal-content').append($data);
			$modal.modal();
		});
	});

	$('#btn-add-admin').click(function () {
		$.post('/Admin/AddAdmin', {
			PhoneNumber: $('#input-phonenumber').val()
		}, function (data) {
			if (data.IsSucceed) {
				_loadUsersTable('Admin', 'panel-admin');
			} else {
				toastr.error(data.ErrorMessage);
			}
		});
	})
})();