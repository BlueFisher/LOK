$(document).ready(function() {
	$.loadUsersTable(true, 'Guest', null, 'panel-guest');
	$.loadUsersTable(true, 'Admin', null, 'panel-admin');
	$.loadUsersTable(true, 'SignedUp', null, 'panel-signedup');

	var $modal = $('#modal-order');
	$('.panel').on('click', '.btn-find-order', function() {
		var $this = $(this);
		var userId = $this.parents('tr').attr('data-user-id');
		$modal.find('table').detach();
		$.post('/Admin/OrdersTable', {
			hasFunction: false,
			type: 'Getting',
			userId: userId
		}, function(data, textStatus, xhr) {
			$modal.find('.modal-content').append(data);
		});
		$.post('/Admin/OrdersTable', {
			hasFunction: false,
			type: 'Sending',
			userId: userId
		}, function(data, textStatus, xhr) {
			$data = $(data).addClass('no-margin-bottom');
			$modal.find('.modal-content').append($data);
			$modal.modal();
		});
	});
});