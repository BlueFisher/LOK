$(document).ready(function() {
	var $wellOrderContainer = $('#well-order-container');
	var $btnOrderAll = $('#btn-order-all'),
		$btnOrderNearly = $('#btn-order-nearly'),
		$btnOrderActive = $('#btn-order-active'),
		$btnOrderFail = $('#btn-order-fail');
	var $activeBtn = $btnOrderActive.addClass('active');
	$btnOrderAll.click(function() {
		if ($(this).hasClass('active'))
			return;
		refreshOrderWells('All');
	});
	$btnOrderNearly.click(function() {
		if ($(this).hasClass('active'))
			return;
		refreshOrderWells('Nearly');
	});
	$btnOrderActive.click(function() {
		if ($(this).hasClass('active'))
			return;
		refreshOrderWells('Active');
	});
	$btnOrderFail.click(function() {
		if ($(this).hasClass('active'))
			return;
		refreshOrderWells('Fail');
	});

	var $loader = $($('#tpl-loader-static').html()).addClass('animated fadeIn').css({
		'-webkit-animation-duration': '0.3s',
		'animation-duration': '0.3s',
	});
	refreshOrderWells('Active');

	function refreshOrderWells(type) {
		var url, $btnReady;
		if (type == 'All') {
			url = '/Business/OrderWells/All';
			$btnReady = $btnOrderAll;
		} else if (type == 'Nearly') {
			url = '/Business/OrderWells/Nearly';
			$btnReady = $btnOrderNearly;
		} else if (type == 'Active') {
			url = '/Business/OrderWells/Nearly';
			$btnReady = $btnOrderActive;
		} else {
			url = '/Business/OrderWells/Fail';
			$btnReady = $btnOrderFail;
		}
		$activeBtn.removeClass('active');
		$activeBtn = $btnReady.addClass('active');

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
			}, 50);
		});
	};

	$wellOrderContainer.on('click', '.btn-close-order', function() {
		var id = $(this).parent('[data-orderid]').attr('data-orderid');
		$.post('/Business/CloseOrder/' + id, function(data) {
			if (data.IsSucceed) {
				refreshOrderWells('Active');
			} else {
				toastr.error(data.ErrorMessage);
			}
		});
	});

	$('#form-change-password').submit(function(){
		var $form = $(this);
		event.preventDefault();
		$.post($form.attr('action'), $form.serialize(), function(data) {
			if(data.IsSucceed){
				toastr.success("密码修改成功");
				$('#modal-change-password').modal('hide');
			}else{
				$('#' + data.ErrorPosition.toLowerCase()).inputError(data.ErrorMessage);
			}
		});
	});
});