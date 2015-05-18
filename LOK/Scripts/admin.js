;
(function () {

	var $inputOrderType = $('input[name="OrderType"]');
	var $inputOrderStatus = $('input[name="OrderStatus"]');

	function _loadOrdersTable() {
		var $panel = $('#panel-table');
		$.post('/Admin/OrdersTable/', {
			HasFunction: true,
			Type: $inputOrderType.val(),
			Status: $inputOrderStatus.val(),
			UserId: null
		}, function (data, textStatus, xhr) {
			$data = $(data).addClass('animated fadeIn').css({
				'-webkit-animation-duration': '0.3s',
				'animation-duration': '0.3s'
			});
			$panel.find('table').detach();
			$panel.append($data);
		});
	}


	function _loadOrdersStatistics() {
		$.post('/Admin/GetOrdersStatistics', function (data, textStatus, xhr) {
			$('#orders-statistics').highcharts({
				chart: {
					animation: false,
					type: 'bar'
				},
				credits: { enabled: false },
				title: { text: false },
				plotOptions: {
					bar: {
						dataLabels: {
							enabled: true
						}
					}
				},
				legend: {
					layout: 'vertical',
					align: 'right',
					verticalAlign: 'top',
					shadow: true
				},
				xAxis: {
					categories: ['取件单', '寄件单'],
					title: {
						text: null
					}
				},
				yAxis: {
					title: {
						text: null,
					},
					minTickInterval: 1
				},
				series: [{
					name: '已完成',
					data: [data.OrdersGettingCount - data.OrdersGettingActiveCount,
						data.OrdersSendingCount - data.OrdersSendingActiveCount]
				}, {
					name: '未完成',
					data: [data.OrdersGettingActiveCount,
						data.OrdersSendingActiveCount]
				}]
			});
		});
	}

	_loadOrdersStatistics();
	_loadOrdersTable();


	$('.dropdown-select').on('click', 'ul li a', function () {
		_loadOrdersTable();
	});

	$('.btn-refresh').click(function () {
		_loadOrdersTable();
		_loadOrdersStatistics();
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
					_loadOrdersStatistics();
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
						_loadOrdersStatistics();
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