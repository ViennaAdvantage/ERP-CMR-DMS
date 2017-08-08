/*** W2ui Add method **/
/*add recid attribute to record object if not exist */

; w2obj.grid.prototype.add = function (record) {
    if (!$.isArray(record)) record = [record];
    var added = 0;
    for (var o in record) {
        if (!record[o].recid) record[o].recid = added + 1;
        if (record[o].recid == null || typeof record[o].recid == 'undefined') {
            console.log('ERROR: Cannot add record without recid. (obj: ' + this.name + ')');
            continue;
        }
        this.records.push(record[o]);
        added++;
    }
    this.buffered = this.records.length;
    var url = (typeof this.url != 'object' ? this.url : this.url.get);
    if (!url) {
        this.localSort();
        this.localSearch();
    }
    this.refresh(); // ??  should it be reload?
    return added;
};

/* support jquery Object also */

$.fn.w2overlay = function (html, options) {
    var obj = this;
    var name = '';
    var defaults = {
        name: null,      // it not null, then allows multiple concurent overlays
        html: '',        // html text to display
        align: 'none',    // can be none, left, right, both
        left: 0,         // offset left
        top: 0,         // offset top
        tipLeft: 30,        // tip offset left
        width: 0,         // fixed width
        height: 0,         // fixed height
        maxWidth: null,      // max width if any
        maxHeight: null,      // max height if any
        style: '',        // additional style for main div
        'class': '',        // additional class name for main div
        onShow: null,      // event on show
        onHide: null,      // event on hide
        openAbove: false,     // show abover control
        tmp: {}
    };
    if (arguments.length == 1) {
        if (typeof html == 'object' && !html.length) {
            options = html;
        } else {
            options = { html: html };
        }
    }
    if (arguments.length == 2) options.html = html;
    if (!$.isPlainObject(options)) options = {};
    options = $.extend({}, defaults, options);
    if (options.name) name = '-' + options.name;
    // if empty then hide
    var tmp_hide;
    if (this.length === 0 || options.html === '' || options.html == null) {
        if ($('#w2ui-overlay' + name).length > 0) {
            tmp_hide = $('#w2ui-overlay' + name)[0].hide;
            if (typeof tmp_hide === 'function') tmp_hide();
        } else {
            $('#w2ui-overlay' + name).remove();
        }
        return $(this);
    }
    if ($('#w2ui-overlay' + name).length > 0) {
        tmp_hide = $('#w2ui-overlay' + name)[0].hide;
        $(document).off('click', tmp_hide);
        if (typeof tmp_hide === 'function') tmp_hide();
    }
    $('body').append('<div id="w2ui-overlay-main' + name + '" style="position:absolute;z-index:1001;height:100%;width:100%;display:block;top:0;left:0">' +
        '<div id="w2ui-overlay' + name + '" style="display: none"' +
        '        class="w2ui-reset w2ui-overlay ' + ($(this).parents('.w2ui-popup, .w2ui-overlay-popup').length > 0 ? 'w2ui-overlay-popup' : '') + '">' +
        '    <style></style>' +
        '    <div style="' + options.style + '" class="' + options['class'] + '"></div>' +
        '</div></div>'
    );
    // init
    var divMain =  $('#w2ui-overlay-main' + name);
    var div1 = $('#w2ui-overlay' + name);
    var div2 = div1.find(' > div');

    var isjObj = false;
    if (typeof (html) == "string")
        div2.html(html);
    else {
        div2.append(html);
        isjObj = true;
    }
    //div2.html(options.html);
    // pick bg color of first div
    var bc = div2.css('background-color');
    if (bc != null && bc !== 'rgba(0, 0, 0, 0)' && bc !== 'transparent') div1.css('background-color', bc);

    div1.data('element', obj.length > 0 ? obj[0] : null)
        .data('options', options)
        .data('position', $(obj).offset().left + 'x' + $(obj).offset().top)
        .fadeIn('fast').on('mousedown', function (event) {
            $('#w2ui-overlay' + name).data('keepOpen', true);
            if (['INPUT', 'TEXTAREA', 'SELECT'].indexOf(event.target.tagName) === -1) event.preventDefault();
        });
    div1[0].hide = hide;
    div1[0].resize = resize;
    

    // need time to display
    resize();
    setTimeout(function () {
        resize();
        //$(document).off('click', hide).on('click', hide);
        divMain.on('click', hide);
        if (isjObj) html.on('click', hide);
        if (typeof options.onShow === 'function') options.onShow();
    }, 10);

    monitor();
    return $(this);

    // monitor position
    function monitor() {
        var tmp = $('#w2ui-overlay' + name);
        if (tmp.data('element') !== obj[0]) return; // it if it different overlay
        if (tmp.length === 0) return;
        var pos = $(obj).offset().left + 'x' + $(obj).offset().top;
        if (tmp.data('position') !== pos) {
            hide();
        } else {
            setTimeout(monitor, 250);
        }
    }

    // click anywhere else hides the drop down
    function hide() {
        var div1 = $('#w2ui-overlay' + name);
        if (div1.data('keepOpen') === true) {
            div1.removeData('keepOpen');
            return;
        }
        var result;
        if (typeof options.onHide === 'function') result = options.onHide();
        if (result === false) return;
        div1.remove();
        //$(document).off('click', hide);
        divMain.off('click', hide);
        divMain.remove();
        if (isjObj) html.off('click', hide);
        clearInterval(div1.data('timer'));
    }

    function resize() {
        var div1 = $('#w2ui-overlay' + name);
        var div2 = div1.find(' > div');
        // if goes over the screen, limit height and width
        if (div1.length > 0) {
            div2.height('auto').width('auto');
            // width/height
            var overflowX = false;
            var overflowY = false;
            var h = div2.height();
            var w = div2.width();
            if (options.width && options.width < w) w = options.width;
            if (w < 30) w = 30;
            // if content of specific height
            if (options.tmp.contentHeight) {
                h = options.tmp.contentHeight;
                div2.height(h);
                setTimeout(function () {
                    if (div2.height() > div2.find('div.menu > table').height()) {
                        div2.find('div.menu').css('overflow-y', 'hidden');
                    }
                }, 1);
                setTimeout(function () { div2.find('div.menu').css('overflow-y', 'auto'); }, 10);
            }
            if (options.tmp.contentWidth) {
                w = options.tmp.contentWidth;
                div2.width(w);
                setTimeout(function () {
                    if (div2.width() > div2.find('div.menu > table').width()) {
                        div2.find('div.menu').css('overflow-x', 'hidden');
                    }
                }, 1);
                setTimeout(function () { div2.find('div.menu').css('overflow-y', 'auto'); }, 10);
            }
            // alignment
            switch (options.align) {
                case 'both':
                    options.left = 17;
                    if (options.width === 0) options.width = w2utils.getSize($(obj), 'width');
                    break;
                case 'left':
                    options.left = 17;
                    break;
                case 'right':
                    options.tipLeft = w - 45;
                    options.left = w2utils.getSize($(obj), 'width') - w + 10;
                    break;
            }
            // adjust position
            var tmp = (w - 17) / 2;
            var boxLeft = options.left;
            var boxWidth = options.width;
            var tipLeft = options.tipLeft;
            if (w === 30 && !boxWidth) boxWidth = 30; else boxWidth = (options.width ? options.width : 'auto');
            if (tmp < 25) {
                boxLeft = 25 - tmp;
                tipLeft = Math.floor(tmp);
            }
            // Y coord
            div1.css({
                top: (obj.offset().top + w2utils.getSize(obj, 'height') + options.top + 7) + 'px',
                left: ((obj.offset().left > 25 ? obj.offset().left : 25) + boxLeft) + 'px',
                'min-width': boxWidth,
                'min-height': (options.height ? options.height : 'auto')
            });
            // $(window).height() - has a problem in FF20
            var maxHeight = window.innerHeight + $(document).scrollTop() - div2.offset().top - 7;
            var maxWidth = window.innerWidth + $(document).scrollLeft() - div2.offset().left - 7;
            if ((maxHeight > -50 && maxHeight < 210) || options.openAbove === true) {
                // show on top
                maxHeight = div2.offset().top - $(document).scrollTop() - 7;
                if (options.maxHeight && maxHeight > options.maxHeight) maxHeight = options.maxHeight;
                if (h > maxHeight) {
                    overflowY = true;
                    div2.height(maxHeight).width(w).css({ 'overflow-y': 'auto' });
                    h = maxHeight;
                }
                div1.css('top', ($(obj).offset().top - h - 24 + options.top) + 'px');
                div1.find('>style').html(
                    '#w2ui-overlay' + name + ':before { display: none; margin-left: ' + parseInt(tipLeft) + 'px; }' +
                    '#w2ui-overlay' + name + ':after { display: block; margin-left: ' + parseInt(tipLeft) + 'px; }'
                );
            } else {
                // show under
                if (options.maxHeight && maxHeight > options.maxHeight) maxHeight = options.maxHeight;
                if (h > maxHeight) {
                    overflowY = true;
                    div2.height(maxHeight).width(w).css({ 'overflow-y': 'auto' });
                }
                div1.find('>style').html(
                    '#w2ui-overlay' + name + ':before { display: block; margin-left: ' + parseInt(tipLeft) + 'px; }' +
                    '#w2ui-overlay' + name + ':after { display: none; margin-left: ' + parseInt(tipLeft) + 'px; }'
                );
            }
            // check width
            w = div2.width();
            maxWidth = window.innerWidth + $(document).scrollLeft() - div2.offset().left - 7;
            if (options.maxWidth && maxWidth > options.maxWidth) maxWidth = options.maxWidth;
            if (w > maxWidth && options.align !== 'both') {
                options.align = 'right';
                setTimeout(function () { resize(); }, 1);
            }
            // check scroll bar
            if (overflowY && overflowX) div2.width(w + w2utils.scrollBarSize() + 2);
        }
    }
};

//$.fn.w2overlay = function (html, options) {
//    if (!$.isPlainObject(options)) options = {};
//    if (!$.isPlainObject(options.css)) options.css = {};
//    if (this.length == 0 || typeof html == 'undefined' || html == '') { hide(); return $(this); }
//    if ($('#w2ui-overlay').length > 0) {
//        // $(document).click();
//        hide();
//    }
//    $('body').append('<div id="w2ui-overlay-main" style="position:absolute;height:100%;width:100%;display:block;top:0;left:0"><div id="w2ui-overlay" class="w2ui-reset w2ui-overlay ' +
//                        ($(this).parents('.w2ui-popup').length > 0 ? 'w2ui-overlay-popup' : '') + '">' +
//                    '	<div></div>' +
//                    '</div></div>');
//    var evt = "click";
//    if ('ontouchstart' in document) {
//        evt = 'touchstart';
//    }

//    // init
//    var obj = this;
//    var div = $('#w2ui-overlay div');
//    var divMain = $('#w2ui-overlay-main');
//    //div.css(options.css).html(html); //original line;
//    /* added lines */
//    var isjObj = false;
//    if (typeof (html) == "string")
//        div.html(html);
//    else {
//        div.append(html);
//        isjObj = true;
//    }

//    if (typeof options['class'] != 'undefined') div.addClass(options['class']);
//    if (typeof options.top == 'undefined') options.top = 0;
//    if (typeof options.left == 'undefined') options.left = 0;
//    if (typeof options.width == 'undefined') options.width = 100;
//    if (typeof options.height == 'undefined') options.height = 0;
//    // pickup bg color of first div
//    var bc = div.css('background-color');
//    var div = $('#w2ui-overlay');
//    if (typeof bc != 'undefined' && bc != 'rgba(0, 0, 0, 0)' && bc != 'transparent') div.css('background-color', bc);


//    var left = ($(obj).offset().left + options.left);

//    var screenSize = $(document).width();
//    if (screenSize <= (parseInt(left) + parseInt(options.width))) {
//        left = left - parseInt(options.width);
//    }

//    var top = ($(obj).offset().top + w2utils.getSize($(obj), 'height') + 3 + options.top);
//    screenSize = $(document).height();
//    if (screenSize <= (parseInt(top) + parseInt(options.height))) {
//        top = top - parseInt(options.height);
//    }


//    div.css({
//        display: 'none',
//        left: left + 'px',
//        top: top + 'px',
//        'min-width': (options.width ? options.width : 'auto'),
//        'min-height': (options.height ? options.height : 'auto')
//    })
//        .fadeIn('fast')
//        .data('position', ($(obj).offset().left) + 'x' + ($(obj).offset().top + obj.offsetHeight))
//        .on('touchstart', function (event) {
//            if (event.stopPropagation) event.stopPropagation(); else event.cancelBubble = true;
//        });

//    // click anywhere else hides the drop down
//    function hide() {
//        var result;
//        if (typeof options.onHide == 'function') result = options.onHide();
//        if (result === false) return;
//        $('#w2ui-overlay-main').off(evt, hide);
//        $('#w2ui-overlay-main').remove();
//        $('#w2ui-overlay').remove();
//        // $(document).off('click', hide);
//        if (isjObj) html.off(evt, hide);
//    }

//    // need time to display
//    setTimeout(fixSize, 0);
//    return $(this);

//    function fixSize() {
//        // $(document).on('click', hide);
//        $('#w2ui-overlay-main').on(evt, hide);
//        if (isjObj) html.on(evt, hide);
//        // if goes over the screen, limit height and width
//        if ($('#w2ui-overlay > div').length > 0) {
//            var h = $('#w2ui-overlay > div').height();
//            var w = $('#w2ui-overlay> div').width();
//            // $(window).height() - has a problem in FF20
//            var max = window.innerHeight - $('#w2ui-overlay > div').offset().top - 7;
//            if (h > max) $('#w2ui-overlay> div').height(max).width(w + w2utils.scrollBarSize()).css({ 'overflow-y': 'auto' });
//            // check width
//            w = $('#w2ui-overlay> div').width();
//            max = window.innerWidth - $('#w2ui-overlay > div').offset().left - 7;
//            if (w > max) $('#w2ui-overlay> div').width(max).css({ 'overflow-x': 'auto' });
//            // onShow event
//            if (typeof options.onShow == 'function') options.onShow();
//        }
//    }
//}