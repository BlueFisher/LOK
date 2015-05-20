;
(function () {
	function _loadUsersTable(roleName, panelId) {
		var $panel = $('#' + panelId);
		$.post('/Admin/Ajax/UsersTable/', {
			HasFunction: true,
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

	_loadUsersTable('Guest', 'panel-guest');
	_loadUsersTable('Admin', 'panel-admin');
	_loadUsersTable('SignedUp', 'panel-signedup');

	var $modal = $('#modal-order');
	$('.panel').on('click', '.btn-find-order', function () {
		var $this = $(this);
		var userId = $this.parents('tr').attr('data-user-id');
		$modal.find('table').detach();
		$.post('/Admin/Ajax/OrdersTable', {
			hasFunction: false,
			type: 'Getting',
			userId: userId
		}, function (data, textStatus, xhr) {
			$modal.find('.modal-content').append(data);
		});
		$.post('/Admin/Ajax/OrdersTable', {
			hasFunction: false,
			type: 'Sending',
			userId: userId
		}, function (data, textStatus, xhr) {
			$data = $(data).addClass('no-margin-bottom');
			$modal.find('.modal-content').append($data);
			$modal.modal();
		});
	});
})();