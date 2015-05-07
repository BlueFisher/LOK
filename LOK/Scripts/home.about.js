;
(function() {
	var hash = location.hash.substr(1);
	if (hash != '') {
		$('#tab-' + hash).tab('show');
	} else {
		$('#tab-about-us').tab('show');
	}
})();