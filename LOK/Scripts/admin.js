$(document).ready(function() {
	$inputOrderType = $('input[name="OrderType"]');
	$inputOrderStatus = $('input[name="OrderStatus"]');
	function loadOrdersTable() {
		var $panel = $('#panel-table');
		$.post('/Admin/OrdersTable/' + $inputOrderType.val() + '/' + $inputOrderStatus.val() + '/NULL', function(data, textStatus, xhr) {
			$data = $(data).addClass('animated fadeIn').css({
				'-webkit-animation-duration': '0.3s',
				'animation-duration': '0.3s'
			});
			$panel.find('table').detach();
			$panel.append($data);
		});
	}
	
	loadOrdersTable();

	$('.dropdown-select').on('click', 'ul li a', function() {
		var $this = $(this);
		var text = $this.text(),
			value = $this.attr('data-value');
		var $dropdownSelect = $this.parents('.dropdown-select')
		$dropdownSelect.find('input[type="hidden"]').val(value);
		$dropdownSelect.find('.label-text').text(text);
		loadOrdersTable();
	});

	$('.panel').on('click', '.btn-accept-order', function() {
		var $this = $(this);
		var orderId = $this.parents('tr').attr('data-order-id');
		$.post('/Admin/AcceptOrder/' + orderId, function(data, textStatus, xhr) {
			if (data.IsSucceed) {
				loadOrdersTable();
			} else {
				toastr.error(data.ErrorMessage);
			}
		});
	});

	$('.panel').on('click', '.btn-complete-order', function() {
		var $this = $(this);
		var orderId = $this.parents('tr').attr('data-order-id');
		var oriText = $this.text();
		$this.text('确定完成');
		var isFail = true;
		$this.one({
			'click': function() {
				if (isFail) {
					$.post('/Admin/CompleteOrder/' + orderId, function(data, textStatus, xhr) {
						if (data.IsSucceed) {
							loadOrdersTable();
						} else {
							toastr.error(data.ErrorMessage);
						}
					}).always(function() {
						$this.text(oriText);
					});
				}
			},
			'mouseout': function() {
				isFail = false;
				$this.text(oriText);
			}
		});
	});

	$('.panel').on('click', '.btn-fail-order', function() {
		var $this = $(this);
		var orderId = $this.parents('tr').attr('data-order-id');
		var oriText = $this.text();
		$this.text('确定取消');
		var isFail = true;
		$this.one({
			'click': function() {
				if (isFail) {
					$.post('/Admin/FailOrder/' + orderId, function(data, textStatus, xhr) {
						if (data.IsSucceed) {
							loadOrdersTable();
						} else {
							toastr.error(data.ErrorMessage);
						}
					}).always(function() {
						$this.text(oriText);
					});
				}
			},
			'mouseout': function() {
				isFail = false;
				$this.text(oriText);
			}
		});
	});
	var $modal = $('#modal-user');
	$('.panel').on('click', '.btn-find-user', function() {
		var $this = $(this);
		var userId = $this.parents('tr').attr('data-user-id');
		$.post('/Admin/UsersTable/' + userId, function(data, textStatus, xhr) {
			$data = $(data).addClass('no-margin-bottom');
			$modal.find('table').detach();
			$modal.find('.modal-content').append($data);
			$modal.modal();
		});
	});
});