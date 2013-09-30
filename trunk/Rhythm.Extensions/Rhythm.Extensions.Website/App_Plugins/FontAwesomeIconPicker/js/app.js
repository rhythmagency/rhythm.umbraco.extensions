'use strict';

require('select2/select2');

$(function () {
	function format (option) {
		if (!option.id) return option.text; // optgroup
		return '<i class="' + option.id.toLowerCase() + '"></i> ' + option.text;
	}

	$('select.umbEditorFontAwesomeIconPicker').select2({
		'width': 'resolve',
		'formatResult': format,
		'formatSelection': format,
		'escapeMarkup': function (m) { return m; }
	});
});