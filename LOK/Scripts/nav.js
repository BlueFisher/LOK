$(document).ready(function() {
	var ANIMATION_FINISH = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
	var $menuContainer = $('.menu-container').css({
		'-webkit-animation-duration': '0.3s',
		'animation-duration': '0.3s',
		'display': 'none'
	}).addClass('animated fadeIn');
	$menu = $('.menu-container .menu').css({
		'-webkit-animation-duration': '0.3s',
		'animation-duration': '0.3s',
		'display': 'none'
	}).addClass('animated fadeInLeft');
	$('.menu-toggle').click(function() {
		$menuContainer.css('display', 'block');
		$menu.css('display', 'block');
		$menuContainer.one('click', function() {
			$menuContainer.addClass('fadeOut');
			$menu.addClass('fadeOutLeft').one(ANIMATION_FINISH, function() {
				$menuContainer.css('display', 'none').removeClass('fadeOut');
				$menu.css('display', 'none').removeClass('fadeOutLeft');
			});
		});
	});
	$menu.click(function(event) {
		event.stopPropagation()
	})

	var isPopoverOpen = false;
	$('#btn-myorder').click(function() {
		var $this = $(this);
		if (isPopoverOpen) {
			$this.popover('destroy');
		} else {
			var oriText = $this.text();
			$this.text('正在获取');
			$.get('/Business/OrderMenus', function(data) {
				$this.popover({
					html: true,
					placement: 'bottom',
					trigger: 'manual',
					content: data
				}).popover('show');
			}).always(function() {
				$this.text(oriText);
			});
		}
		isPopoverOpen = !isPopoverOpen;
	});

	$('.btn:not(.disabled)').css('position', 'relative').ripples();
	$('a.list-group-item:not(.disabled)').css('position', 'relative').ripples();
	$('.btn-ripples:not(.disabled)').css('position', 'relative').ripples();
	$('.ripples:not(.disabled)').ripples();

	toastr.options = {
		onclick: function() {
			// alert();
		},
		progressBar: true,
		showDuration: "300",
		hideDuration: "1000",
		timeOut: "5000",
		extendedTimeOut: "1000",
	};
}).ajaxError(function(event, jqxhr, settings, exception) {
	toastr.error("连接服务器失败 " + settings.url);
});

$.ajaxSetup({
	cache: false
});

$.fn.extend({
	inputError: function(content) {
		var $this = $(this);
		$this.focus();
		$this.on('keypress focusout', function() {
			$this.popover('destroy').parents('.form-group').removeClass("has-error");
		});
		if (content == undefined || content == null) {
			content = "错误";
		}
		$this.popover({
			trigger: "manual",
			container: "body",
			placement: "top",
			content: content
		}).popover("show").parents('.form-group').addClass("has-error");
		return $this;
	},
	afterAnimation: function(fun) {
		var $this = $(this);
		$this.one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function() {
			fun();
		});
		setTimeout(function() {
			fun();
		}, 300);
	},
	btnConfirm: function(callBack, changedText) {
		var $this = $(this);
		var oriText = $this.text();
		var id = $this.parent('[data-orderid]').attr('data-orderid');
		$this.text(changedText);
		var isFail = true;
		$this.one({
			'click': function() {
				if (isFail) {
					callBack.call($this);
					$this.text(oriText);
				}
			},
			'mouseout': function() {
				isFail = false;
				$this.text(oriText);
			}
		});
	}
});
$.extend({
	loadUsersTable: function(hasFunction, roleName, userId, panelId) {
		var $panel = $('#' + panelId);
		$.post('/Admin/UsersTable/', {
			HasFunction: hasFunction,
			RoleName: roleName,
			UserId: userId
		}, function(data, textStatus, xhr) {
			$data = $(data).addClass('animated fadeIn').css({
				'-webkit-animation-duration': '0.3s',
				'animation-duration': '0.3s'
			});
			$panel.find('table').detach();
			$panel.append($data);
		});
	},
	loadOrdersTable: function(hasFunction, type, status, userId, panelId) {
		var $panel = $('#' + panelId);
		$.post('/Admin/OrdersTable/', {
			HasFunction: hasFunction,
			Type: type,
			Status: status,
			UserId: userId
		}, function(data, textStatus, xhr) {
			$data = $(data).addClass('animated fadeIn').css({
				'-webkit-animation-duration': '0.3s',
				'animation-duration': '0.3s'
			});
			$panel.find('table').detach();
			$panel.append($data);
		});
	}
})