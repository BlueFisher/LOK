$(document).ready(function() {
	var $wellOrderContainer = $('#well-order-container');
	var statusArr = ['All', 'Nearly', 'Active', 'Completed', 'Failed', 'Closed'];

	var $activeBtn = $('#btn-order-active').addClass('active');

	for (var i = 0; i < statusArr.length; addEvent(i++));
	function addEvent(i) {
		$('#btn-order-' + statusArr[i].toLowerCase()).click(function() {
			if ($(this).hasClass('active'))
				return;
			refreshOrderWells(statusArr[i]);
		});
	}
	var $loader = $($('#tpl-loader-static').html()).addClass('animated fadeIn').css({
		'-webkit-animation-duration': '0.3s',
		'animation-duration': '0.3s',
	});
	refreshOrderWells('Active');

	function refreshOrderWells(type) {
		var url;
		for(var i in statusArr){
			if(type == statusArr[i]){
				url = '/Business/OrderWells/' + type;
			}
		}
		$activeBtn.removeClass('active');
		$activeBtn = $('#btn-order-' + type.toLowerCase()).addClass('active');

		$wellOrderContainer.find('.col-order').detach();
		$wellOrderContainer.append($loader);
		$.ajax({
			url: url,
		}).done(function(data) {
			$wellOrderContainer.find('.col-order').detach();
			var $content = $(data);
			$content.addClass('animated fadeIn').css({
				'-webkit-animation-duration': '0.3s',
				'animation-duration': '0.3s',
			});
			var i = 0;
			var t = setInterval(function() {
				if (i < $content.length) {
					$wellOrderContainer.append($content.eq(i));
				} else {
					clearInterval(t);
				}
				i++;
			}, 10);
		});
	};

	$wellOrderContainer.on('click', '.btn-close-order', function() {
		var $this = $(this);
		$this.btnConfirm(function() {
			var id = $this.parent('[data-orderid]').attr('data-orderid');
			$.post('/Business/CloseOrder/' + id, function(data, textStatus, xhr) {
				if (data.IsSucceed) {
					refreshOrderWells('Closed');
				} else {
					toastr.error(data.ErrorMessage);
				}
			});
		}, '确认取消');
	});

	$('#form-change-password').submit(function() {
		var $form = $(this);
		event.preventDefault();
		$.post($form.attr('action'), $form.serialize(), function(data) {
			if (data.IsSucceed) {
				toastr.success("密码修改成功");
				$('#modal-change-password').modal('hide');
			} else {
				$('#' + data.ErrorPosition.toLowerCase()).inputError(data.ErrorMessage);
			}
		});
	});
});