;
(function() {
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
})();