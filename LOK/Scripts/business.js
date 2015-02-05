$(document).ready(function() {
	var ANIMATION_FINISH = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
	var $step1 = $('#step1').css({
		'-webkit-animation-duration': '0.3s',
		'animation-duration': '0.3s'
	});
	var $step2 = $('#step2').css({
		'-webkit-animation-duration': '0.3s',
		'animation-duration': '0.3s',
		'display': 'none'
	});
	var $btnStep1 = $('.steps ul li:eq(0) a'),
		$btnStep2 = $('.steps ul li:eq(1) a'),
		$btnStep3 = $('.steps ul li:eq(2) a');

	var $sendingWrapper = $('#btn-choose-sending').css({
		'-webkit-animation-duration': '0.3s',
		'animation-duration': '0.2s'
	}).addClass('animated');
	var $gettingWrapper = $('#btn-choose-getting').css({
		'-webkit-animation-duration': '0.3s',
		'animation-duration': '0.2s'
	}).addClass('animated');

	var $loader = $('.load-container').css({
		'display': 'none',
		'-webkit-animation-duration': '0.3s',
		'animation-duration': '0.3s'
	}).addClass('animated fadeIn');

	function step1Model() {
		$loader.css('display', 'block');

		$loader.addClass('fadeOut').one(ANIMATION_FINISH, function() {
			$loader.removeClass('fadeOut').css('display', 'none');
		});

		$step2.css('display', 'none');
		$step1.css('display', 'block');

		$btnStep1.addClass('active');
		$btnStep2.removeClass('active');
		$btnStep3.removeClass('active');

	}

	function step2Model() {
		var $labelGetting = $('#label-isOrderGettingOnService'),
			$labelSending = $('#label-isOrderSendingOnService');
		$loader.css('display', 'block');
		$.ajax({
			url: '/Business/IsOrderOnService'
		}).done(function(data) {
			if (data.Getting) {
				$labelGetting.text('（接受订单）').parent().removeClass('disabled').parent().removeClass('disabled')
			} else {
				$labelGetting.text('（暂停业务）').parent().addClass('disabled').parent().addClass('disabled')
			}
			if (data.Sending) {
				$labelSending.text('（接受订单）').parent().removeClass('disabled').parent().removeClass('disabled')
			} else {
				$labelSending.text('（暂停业务）').parent().addClass('disabled').parent().addClass('disabled')
			}
		}).always(function() {
			$loader.addClass('fadeOut').one(ANIMATION_FINISH, function() {
				$loader.removeClass('fadeOut').css('display', 'none');
			});

			$step1.css('display', 'none');
			$step2.css('display', 'block');

			$btnStep1.removeClass('active');
			$btnStep2.addClass('active');
			$btnStep3.removeClass('active');

			$gettingWrapper.css('display', 'block');
			$sendingWrapper.css('display', 'block');
		});
	}

	function sendingModel() {
		if ($sendingWrapper.hasClass('disabled')) {
			return;
		}
		$sendingWrapper.addClass('fadeOut').one(ANIMATION_FINISH, function() {
			$sendingWrapper.css('display', 'none').removeClass('fadeOut').addClass('fadeIn');
		});
		$gettingWrapper.css('display', 'block');
		$btnStep2.removeClass('active');
		$btnStep3.addClass('active');
	}

	function gettingModel() {
		if ($gettingWrapper.hasClass('disabled')) {
			return;
		}
		$gettingWrapper.addClass('fadeOut').one(ANIMATION_FINISH, function() {
			$gettingWrapper.css('display', 'none').removeClass('fadeOut').addClass('fadeIn');
		});
		$sendingWrapper.css('display', 'block');
		$btnStep2.removeClass('active');
		$btnStep3.addClass('active');
	}

	$('#btn-choose-sending').click(function() {
		sendingModel();
	});
	$('#btn-choose-getting').click(function() {
		gettingModel();
	});

	$btnStep1.click(function() {
		step1Model();
	});

	$btnStep2.click(function() {
		step2Model();
	});

	$('.goto-step1').click(function() {
		step1Model();
	});
	$.extend({
		gotoStep1: function() {
			step1Model();
		},
		gotoStep2: function() {
			step2Model();
		}
	});

	$('#form-getting, #form-sending').submit(function(event) {
		var $form = $(this);
		var $modal = $('#modal-submit');
		event.preventDefault();
		$.ajax({
			url: $form.attr('data-valid-url'),
			type: 'POST',
			data: $form.serialize(),
		}).done(function(data) {
			// 提交的表单是否合法
			if (data.IsSucceed == true) {
				// 提交模态框弹出
				$modal.modal({
					backdrop: 'static'
				}).one('shown.bs.modal', function() {
					var progressValue = 0;
					var $progressSubmit = $('#progress-bar-submit');
					var interval = setInterval(function() {
						$progressSubmit.css('width', progressValue + '%').text(progressValue + '%');
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
								}
							}).always(function() {
								$progressSubmit.css('width', '0%').text('0%');
							});
						}
						progressValue += 1;
					}, 20);
					$('#btn-cancel-submit').one('click', function() {
						clearInterval(interval);
						$modal.modal('hide');
						$progressSubmit.css('width', '0%').text('0%');
					});
				});
			} else {
				if ($form.attr('id') == 'form-getting') {
					console.log($('#getting-' + data.ErrorPosition.toLowerCase())[0])
					$('#getting-' + data.ErrorPosition.toLowerCase()).inputError(data.ErrorMessage);
				} else {
					$('#sending-' + data.ErrorPosition.toLowerCase()).inputError(data.ErrorMessage);
				}
			}
		});
	});
	$('.dropdown-select').on('click', 'ul li a', function() {
		var $this = $(this);
		var text = $this.text(),
			value = $this.attr('data-value');
		var $dropdownSelect = $this.parents('.dropdown-select')
		$dropdownSelect.find('input[type="hidden"]').val(value);
		$dropdownSelect.find('.label-text').text(text);
	});
});