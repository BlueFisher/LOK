$(document).ready(function() {
	$('#btn-myorder').popover({
		html: true,
		template: '<div class="popover" role="tooltip"><div class="arrow"></div><h3 class="popover-title"></h3><div class="popover-content"></div></div>',
		content: $('#tpl-myorder').html(),
		placement: "bottom"
	});
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

}).ajaxError(function() {
	toastr.error("连接服务器失败");
});

$.fn.extend({
	inputError: function(content) {
		var $this = $(this);
		$this.focus();
		$this.on('keypress focusout', function() {
			$(this).popover('destroy').parents('.form-group').removeClass("has-error");
		});
		if (content == undefined || content == null) {
			content = "错误";
		}
		$(this).popover({
			trigger: "manual",
			container: "body",
			placement: "top",
			content: content
		}).popover("show").parents('.form-group').addClass("has-error");
		return $(this);
	}
});