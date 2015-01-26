$(document).ready(function() {
	var $step1 = $('#step1').css({
		'-webkit-animation-duration': '0.3s',
		'animation-duration': '0.3s'
	});
	var $step2 = $('#step2').css({
		'-webkit-animation-duration': '0.3s',
		'animation-duration': '0.3s'
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

	$('#btn-choose-sending').click(function() {
		$sendingWrapper.addClass('fadeOut').one('webkitAnimationEnd mozAnimationEnd animationend', function() {
			$sendingWrapper.css('display', 'none').removeClass('fadeOut').addClass('fadeIn');
		});
		$gettingWrapper.css('display', 'block');
		$btnStep2.removeClass('active');
		$btnStep3.addClass('active');
	});
	$('#btn-choose-getting').click(function() {
		$gettingWrapper.addClass('fadeOut').one('webkitAnimationEnd mozAnimationEnd animationend', function() {
			$gettingWrapper.css('display', 'none').removeClass('fadeOut').addClass('fadeIn');
		});
		$sendingWrapper.css('display', 'block');
		$btnStep2.removeClass('active');
		$btnStep3.addClass('active');
	});

	$('#step2').css('display', 'none');
	$btnStep1.click(function() {
		if ($btnStep2.hasClass('active') || $btnStep3.hasClass('active')) {
			$step2.addClass('fadeOut').one('webkitAnimationEnd mozAnimationEnd animationend', function() {
				$step2.removeClass('fadeOut').css('display', 'none');
			});
		}
		$loader.css('display', 'block');
		setTimeout(function() {
			$step1.css('display', 'block');
			$loader.addClass('fadeOut').one('webkitAnimationEnd mozAnimationEnd animationend', function() {
				$loader.removeClass('fadeOut').css('display', 'none');
			});

		}, 500);
		$btnStep1.addClass('active');
		$btnStep2.removeClass('active');
		$btnStep3.removeClass('active');
	});

	$btnStep2.click(function() {
		if ($btnStep1.hasClass('active')) {
			$step1.addClass('fadeOut').one('webkitAnimationEnd mozAnimationEnd animationend', function() {
				$step1.removeClass('fadeOut').css('display', 'none');
			});
		}

		$loader.css('display', 'block');
		setTimeout(function() {
			$step2.css('display', 'block');
			$loader.addClass('fadeOut').one('webkitAnimationEnd mozAnimationEnd animationend', function() {
				$loader.removeClass('fadeOut').css('display', 'none');
			});
		}, 500);
		$btnStep1.removeClass('active');
		$btnStep2.addClass('active');
		$btnStep3.removeClass('active');

		$gettingWrapper.css('display', 'block');
		$sendingWrapper.css('display', 'block');
	});
	$.extend({
		test: function() {
			$btnStep2.click();
		}
	});
});