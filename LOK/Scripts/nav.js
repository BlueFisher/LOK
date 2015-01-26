$(document).ready(function() {
	$navContainer = $('#nav');
	$nav = $('#nav nav');
	$navContainer.css('display', 'none');
	$('#btn-nav').click(function() {
		$navContainer.css('display', 'block').addClass('animated fadeIn');
		$nav.addClass('animated fadeInLeft');
		$navContainer.one('click', function(event) {
			$navContainer.addClass('fadeOut');
			$nav.addClass('fadeOutLeft');
			$navContainer.one('webkitAnimationEnd mozAnimationEnd oanimationend animationend', function() {
				$navContainer.css('display', 'none');
				$navContainer.removeClass('animated fadeIn fadeOut');
				$nav.removeClass('animated fadeInLeft fadeOutLeft');
			});
		});
		
	});
	$nav.click(function(event) {
		event.stopPropagation();
	});
});