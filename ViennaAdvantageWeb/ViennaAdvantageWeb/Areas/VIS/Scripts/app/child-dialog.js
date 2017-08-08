
; (function (VIS, $) {

    function ChildDialog() {
        var $a = $("<div></div>");
        var title = "Dialog";
        var height = "auto";
        var width = "auto";
        var modal = true;
        var resize = true;
        //var position = { at: "center center", of: window };

        this.onOkClick = null;
        this.onCancelClick = null;
        this.onClose = null;


        this.setTitle = function (t) {
            title = t;
        }

        this.setModal = function (m) {
            modal = m;
        }

        this.setHeight = function (h) { height = h; };
        this.setWidth = function (w) { width = w; };
        //this.setPostion = function (p) { position = p; };

        this.setEnableResize = function (isResize) { resize = isResize; };

        var self = this;

        this.setContent = function (content) {
            $a.empty();

            content.css("margin-left", "-6px");
            content.css("margin-right", "-7px");
            //content.css("margin-top", "3px");
            $a.append(content);
        };
        this.getRoot = function () {
            return $a;
        };


        function onClosing() {
            if (self.onClose)
                self.onClose();
            self.dispose();
            self = null;
        }

        this.hidebuttons = function () {
            $a.dialog('widget').find('.ui-dialog-buttonpane .ui-button').hide();
            $a.dialog('widget').find('.ui-widget-content').css('border', '0px');
            //$a.dialog('widget').removeClass('.ui-dialog');
            //$a.dialog('widget').removeClass('.ui-dialog-buttonpane');
            $a.dialog('widget').find('.ui-dialog-buttonpane').css('padding', '0px');
            $a.dialog('widget').find('.ui-dialog-buttonpane').css('margin-top', '0px');
            $a.dialog('widget').find('.ui-dialog .ui-dialog-buttonpane').hide();
        };

        this.hideButtons = function () {
            $a.dialog('widget').find('.ui-dialog-buttonpane .ui-button').hide();
            $a.dialog('widget').find('.ui-widget-content').css('border', '0px');
            //$a.dialog('widget').removeClass('.ui-dialog');
            //$a.dialog('widget').removeClass('.ui-dialog-buttonpane');
            $a.dialog('widget').find('.ui-dialog-buttonpane').css('padding', '0px');
            $a.dialog('widget').find('.ui-dialog-buttonpane').css('margin-top', '0px');
            $a.dialog('widget').find('.ui-dialog .ui-dialog-buttonpane').hide();
        };



        this.close = function () {
            $a.dialog("close");
            this.dispose();
        }


        this.show = function () {
            var styleCancel = "margin-right:-2px;margin-top:-10px;margin-bottom:3px;margin-right:-4px";
            var styleOK = "margin-top:-10px;margin-bottom:3px;margin-right:12px";
            if (VIS.Application.isRTL) {
                styleCancel = "margin-right:7px;margin-top:-5px;margin-bottom:3px";
                styleOK = "margin-top:-5px;margin-bottom:3px;margin-right:-3px";
            }
            $a.dialog({
                height: height,
                width: width,
                title: title,
                modal: modal,
                resizable: resize,
                // position: position,
                buttons: [
                                {
                                    text: VIS.Msg.getMsg("Ok"),
                                    click: function (evt) {
                                        var buttonDomElement = evt.target;
                                        // Disable the button 
                                        //$(buttonDomElement).css('background-color', 'red');
                                        //$(buttonDomElement).css('color', 'blue');
                                        $(buttonDomElement).attr('disabled', true);

                                        var res = true;
                                        if (self.onOkClick) {
                                            res = self.onOkClick(evt);
                                        }
                                        if (res == true || res == undefined) {
                                            if ($a != null)
                                                $a.dialog("close");
                                        }
                                        else {
                                            $(buttonDomElement).attr('disabled', false);
                                        }
                                    },
                                    style: styleOK
                                },
                {
                    text: VIS.Msg.getMsg("Cancel"),
                    click: function () { if (self.onCancelClick) self.onCancelClick(); if ($a) $a.dialog("close"); },
                    style: styleCancel
                }
                ]
                ,
                close: onClosing
            });
            //$('.ui-dialog .ui-dialog-buttonpane').css("margin-top", "-10px");
            $('.ui-dialog .ui-dialog-buttonpane').css("border-width", "0 0 0");
            return $a;
        };

        //$('.ui-dialog .ui-dialog-buttonpane').css("margin", "0px");
        //$('.ui-dialog .ui-dialog-buttonpane').css("padding", "0px");
        this.dispose = function () {
            if ($a != null) {
                $a.dialog('destroy');
                $a.remove();
            }
            $a = null;
            this.Show = null;
            this.setContent = null;
            this.setTitle = null;
        }
    };

    VIS.ChildDialog = ChildDialog;

})(VIS, jQuery);