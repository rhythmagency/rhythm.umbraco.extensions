'use strict';

var $ = require('./components/jquery/jquery');
require('./components/selectik/src/jquery.mousewheel');
require('./components/selectik/src/jquery.selectik');

$(function () {
	$('select.umbEditorFontAwesomeIconPicker').selectik(
		{ width: 200, smartPosition: false, maxItems: 20 },{
		_generateHtml: function(){
			this.$collection = this.$cselect.children();
			var html = '';
			for (var i = 0; i < this.$collection.length; i++){
				var $this = $(this.$collection[i]);
				html += '<li class="'+ ($this.attr('disabled') === 'disabled' ? 'disabled' : '') +'" data-value="'+$this[0].value+'"><i class="' + $this[0].text + '"></i>&nbsp;&nbsp;'+($this.data('selectik') ? $this.data('selectik') : $this[0].text)+'</li>';
				};
			return html;
		},
		_changeSelectedHtml: function(dataValue, textValue, index){
			if (index > this.count || index == 0) { return false;}
			this.change = true;
			var $selected = $('.selected', this.$list);
			$('option:eq('+$selected.index()+')', this.$cselect).prop('selected', false); //
			$('option:eq('+(index-1)+')', this.$cselect).prop('selected', true);

			this.$cselect.prop('value', dataValue).change();
			$selected.removeClass('selected');
			$('li:nth-child('+ index +')', this.$list).addClass('selected');
			//this.$text.text(textValue);
				
			this.$text.html('<i class="' + textValue + '"></i>&nbsp;&nbsp;' + textValue);
		},
		_getList: function(e){
			this.count = this.cselect.length;
			if (e.refreshSelect){ $('.select-list', this.$container).remove(); }

			// loop html
			var html = this._generateHtml();

			// html for control
			var scrollHtml = (this.config.maxItems > 0 && this.config.customScroll == 1) ? '<div class="select-scroll"><span class="scroll-drag"><!-- --></span></div>' : '';
			var scrollClass = (this.config.customScroll == 1) ? 'custom-scroll' : 'default-scroll';

			// selected
			this.$selected = $('option:selected', this.$cselect);

			// check if first time or refresh
			if (e.refreshSelect){
				html = '<div class="select-list '+scrollClass+'">'+scrollHtml+'<ul>'+html+'</ul></div>';
				$(html).prependTo(this.$container);
			}else{
				//html = '<span class="custom-text">'+this.$selected[0].text+'</span><div class="select-list '+scrollClass+'">'+scrollHtml+'<ul>'+html+'</ul></div>';
				html = '<span class="custom-text"><i class="' + this.$selected[0].text + '"></i>&nbsp;&nbsp;'+this.$selected[0].text+'</span><div class="select-list '+scrollClass+'">'+scrollHtml+'<ul>'+html+'</ul></div>';
				$(html).prependTo(this.$container);
			}

			this.$list = $('ul', this.$container);
			this.$text = $('.custom-text', this.$container);
			this.$listContainer = $('.select-list', this.$container);
			this._clickHandler();

			// give class to the selected element
			$('li:eq('+(this.$selected.index())+')', this.$list).addClass('selected');

			// give width to elements
			this.$container.removeClass('done');
			this.setWidthCS(this.config.width);

			// standard top distance
			this.standardTop = parseInt(this.$listContainer.css('top'));

			// fire function for max length
			this._getLength({refreshSelect: e.refreshSelect });
		},
	});
});