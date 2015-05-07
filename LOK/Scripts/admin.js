/* global $ */
;
(function () {
	$.post('/Admin/GetOrdersStatistics', function (data, textStatus, xhr) {
		$('#orders-all-statistics').highcharts({
			title: {
				text: false
			},
			legend: {
				enabled: false
			},
			tooltip: {
				formatter: function () {
					return this.y;
				}
			},
			series: [{
				type: 'pie',
				name: '订单总览',
				data: [
					['未完成', data.OrdersActiveCount],
					['已完成', data.OrdersAllCount - data.OrdersActiveCount],
				]
			}]
		});
		$('#orders-getting-statistics').highcharts({
			title: {
				text: false
			},
			legend: {
				enabled: false
			},
			tooltip: {
				formatter: function () {
					return this.y;
				}
			},
			series: [{
				type: 'pie',
				name: '取件单',
				data: [
					['未完成', data.OrdersGettingActiveCount],
					['已完成', data.OrdersGettingCount - data.OrdersGettingActiveCount],
				]
			}]
		});
		$('#orders-sending-statistics').highcharts({
			title: {
				text: false
			},
			legend: {
				enabled: false
			},
			tooltip: {
				formatter: function () {
					return this.y;
				}
			},
			series: [{
				type: 'pie',
				name: '寄件单',
				data: [
					['未完成', data.OrdersSendingActiveCount],
					['已完成', data.OrdersSendingCount - data.OrdersSendingActiveCount],
				]
			}]
		});
	});
	var $inputOrderType = $('input[name="OrderType"]');
	var $inputOrderStatus = $('input[name="OrderStatus"]');

	function _loadOrdersTable() {
		$.loadOrdersTable(true, $inputOrderType.val(), $inputOrderStatus.val(), null, 'panel-table')
	}

	_loadOrdersTable();

	$('.dropdown-select').on('click', 'ul li a', function () {
		_loadOrdersTable();
	});

	$('.btn-refresh').click(function () {
		_loadOrdersTable();
	});

	$('.panel').on('click', '.btn-accept-order', function () {
		var $this = $(this);
		$this.btnConfirm(function () {
			var orderId = $this.parents('tr').attr('data-order-id');
			$.post('/Admin/ChnageOrderStatus/', {
				OrderId: orderId,
				Status: 'OnGoing'
			}, function (data, textStatus, xhr) {
					if (data.IsSucceed) {
						_loadOrdersTable();
					} else {
						toastr.error(data.ErrorMessage);
					}
				});
		}, '确认接受');
	});

	$('.panel').on('click', '.btn-complete-order', function () {
		var $this = $(this);
		$this.btnConfirm(function () {
			var orderId = $this.parents('tr').attr('data-order-id');
			$.post('/Admin/ChnageOrderStatus/', {
				OrderId: orderId,
				Status: 'Completed'
			}, function (data, textStatus, xhr) {
					if (data.IsSucceed) {
						_loadOrdersTable();
					} else {
						toastr.error(data.ErrorMessage);
					}
				});

		}, '确认完成');
	});

	$('.panel').on('click', '.btn-fail-order', function () {
		var $this = $(this);
		$this.btnConfirm(function () {
			var orderId = $this.parents('tr').attr('data-order-id');
			var $modal = $('#modal-admin-remark')
			$modal.find('#form-admin-remark').one('submit', function () {
				event.preventDefault();
				$.post('/Admin/ChnageOrderStatus/', {
					OrderId: orderId,
					Status: 'Failed',
					AdminRemark: $modal.find('[name="AdminRemark"]').val()
				}, function (data, textStatus, xhr) {
						if (data.IsSucceed) {
							_loadOrdersTable();
							$modal.modal('hide');
						} else {
							toastr.error(data.ErrorMessage);
						}
					});
			})
			$modal.modal();
		}, '确认取消');
	});
	$('.panel').on('click', '.btn-admin-remark', function () {
		var $this = $(this);
		var orderId = $this.parents('tr').attr('data-order-id');
		var $modal = $('#modal-admin-remark')
		$modal.find('#form-admin-remark').one('submit', function () {
			event.preventDefault();
			$.post('/Admin/ChangeAdminRemark/', {
				OrderId: orderId,
				AdminRemark: $modal.find('[name="AdminRemark"]').val()
			}, function (data, textStatus, xhr) {
					if (data.IsSucceed) {
						_loadOrdersTable();
						$modal.modal('hide');
					} else {
						toastr.error(data.ErrorMessage);
					}
				});
		})
		$modal.modal();
	});


	var $modal = $('#modal-user');
	$('.panel').on('click', '.btn-find-user', function () {
		var $this = $(this);
		var userId = $this.parents('tr').attr('data-user-id');
		$.post('/Admin/UsersTable', {
			userId: userId
		}, function (data, textStatus, xhr) {
				$data = $(data).addClass('no-margin-bottom');
				$modal.find('table').detach();
				$modal.find('.modal-content').append($data);
				$modal.modal();
			});
	});
})();