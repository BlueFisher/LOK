$(document).ready(function() {
	$.post('/Business/IsOrderOnService', function(data) {
		if (data.Getting) {
			$('#btn-choose-getting').css('display', 'none');
		}
		if (data.Sending) {
			$('#btn-choose-sending').css('display', 'none');
		}
	});

	$('#form-getting, #form-sending').submit(function(event) {
		var $form = $(this);
		var $modal = $('#modal-submit');
		event.preventDefault();
		$form.find('.input-region').val($('.map-panto.active').attr('data-region'));
		$.post($form.attr('data-valid-url'), $form.serialize(), function(data, textStatus, xhr) {
			// 提交的表单是否合法
			if (data.IsSucceed == true) {
				// 提交模态框弹出
				$modal.modal({
					backdrop: 'static'
				});
				var progressValue = 0;
				var time = 5; //等待时间（秒）
				var $progressSubmit = $('#progress-bar-submit');
				var interval = setInterval(function() {
					$progressSubmit.css('width', progressValue + '%').text(parseInt(progressValue) + '%');
					if (progressValue >= 100) {
						clearInterval(interval);
						// 提交表单
						$.ajax({
							url: $form.attr('action'),
							type: 'POST',
							data: $form.serialize(),
						}).done(function(data) {
							if (data.IsSucceed == true) {
								window.location = "/Business/Order";
							} else {
								$modal.modal('hide');
								toastr.error(data.ErrorMessage);
							}
						}).always(function() {
							$progressSubmit.css('width', '0%').text('0%');
						});
					}
					progressValue += 100 / (time * 1000 / 10);
				}, 10);
				$('#btn-cancel-submit').one('click', function() {
					clearInterval(interval);
					$modal.modal('hide');
					$progressSubmit.css('width', '0%').text('0%');
				});
			} else {
				if ($form.attr('id') == 'form-getting') {
					$('#getting-' + data.ErrorPosition.toLowerCase()).inputError(data.ErrorMessage);
				} else {
					$('#sending-' + data.ErrorPosition.toLowerCase()).inputError(data.ErrorMessage);
				}
			}
		});
	});
});