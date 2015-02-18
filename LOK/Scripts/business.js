$(document).ready(function() {
	var $step1 = $('#step1').css({
		'-webkit-animation-duration': '0.3s',
		'animation-duration': '0.3s',
		'display': 'none'
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

	function InitializeMap() {
		$map = $('#map');
		$map.css('height', $(window).height() - 180 + 'px');
		$(window).resize(function() {
			$map.css('height', $(window).height() - 180 + 'px');
		});

		var sContent = $('#tpl-map-info').html();
		var map = new BMap.Map("map");
		map.enableScrollWheelZoom(true);
		var point = new BMap.Point(121.400532, 31.322212);
		var marker = new BMap.Marker(point, {
			icon: new BMap.Icon("/Content/image/map_icon.png", new BMap.Size(20, 25), {
				imageOffset: new BMap.Size(-46, -21)
			})
		});
		var infoWindow = new BMap.InfoWindow(sContent, {
			enableMessage: false,
		}); // 创建信息窗口对象
		map.centerAndZoom(point, 17);
		map.addOverlay(marker);
		marker.openInfoWindow(infoWindow);
		marker.addEventListener("click", function() {
			marker.openInfoWindow(infoWindow);
		});

		$('.map-panto').click(function() {
			if (!$(this).hasClass('disabled')) {
				map.panTo(new BMap.Point($(this).attr('data-cordinate-x'), $(this).attr('data-cordinate-y')));
				marker.openInfoWindow(infoWindow);
			}
		});
	}

	function step1Model() {
		$loader.css('display', 'block');

		$loader.addClass('fadeOut').afterAnimation(function(){
			$loader.removeClass('fadeOut').css('display', 'none');
		});

		$step2.css('display', 'none');
		$step1.css('display', 'block');

		$btnStep1.addClass('active');
		$btnStep2.removeClass('active');
		$btnStep3.removeClass('active');
		InitializeMap();
	}

	function step2Model() {
		var $labelGetting = $('#label-isOrderGettingOnService'),
			$labelSending = $('#label-isOrderSendingOnService');
		$loader.css('display', 'block');
		$.post('/Business/IsOrderOnService', function(data) {
			if (data.Getting) {
				$labelGetting.text('（接受订单）').parent().removeClass('disabled').parent().removeClass('disabled');
			} else {
				$labelGetting.text('（暂停业务）').parent().addClass('disabled').parent().addClass('disabled');
			}
			if (data.Sending) {
				$labelSending.text('（接受订单）').parent().removeClass('disabled').parent().removeClass('disabled');
			} else {
				$labelSending.text('（暂停业务）').parent().addClass('disabled').parent().addClass('disabled');
			}
		}).always(function() {
			$loader.addClass('fadeOut').afterAnimation(function() {
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
		$sendingWrapper.addClass('fadeOut').afterAnimation(function() {
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
		$gettingWrapper.addClass('fadeOut').afterAnimation(function() {
			$gettingWrapper.css('display', 'none').removeClass('fadeOut').addClass('fadeIn');
		});
		$sendingWrapper.css('display', 'block');
		$btnStep2.removeClass('active');
		$btnStep3.addClass('active');
	}

	if ($('#is-step1-model').val() == 1) {
		step1Model();
	} else {
		step2Model();
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
		$form.find('.input-region').val($('.map-panto.active').attr('data-region'));
		$.post($form.attr('data-valid-url'), $form.serialize(), function(data, textStatus, xhr) {
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
								}else{
									$modal.modal('hide');
									toastr.error(data.ErrorMessage);
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