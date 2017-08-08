; VIS.Controls = {};
; (function ($, VIS) {

    /**
     *	System Display Types.
     *  <pre>
     *	SELECT AD_Reference_ID, Name FROM AD_Reference WHERE ValidationType = 'D'
     *  </pre>
     */
    VIS.DisplayType = {
        String: 10, Integer: 11, Amount: 12, ID: 13, Text: 14, Date: 15, DateTime: 16, List: 17, Table: 18, TableDir: 19,
        YesNo: 20, Location: 21, Number: 22, Binary: 23, Time: 24, Account: 25, RowID: 26, Color: 27, Button: 28, Quantity: 29,
        Search: 30, Locator: 31, Image: 32, Assignment: 33, Memo: 34, PAttribute: 35, TextLong: 36, CostPrice: 37, FilePath: 38,
        FileName: 39, URL: 40, PrinterName: 42, Label: 44, MultiKey: 45, GAttribute: 46,

        IsString: function (displayType) {
            return VIS.DisplayType.String == displayType;
        },
        IsText: function (displayType) {
            if (displayType == VIS.DisplayType.String || displayType == VIS.DisplayType.Text
               || displayType == VIS.DisplayType.TextLong || displayType == VIS.DisplayType.Memo
               || displayType == VIS.DisplayType.FilePath || displayType == VIS.DisplayType.FileName
               || displayType == VIS.DisplayType.URL || displayType == VIS.DisplayType.PrinterName)
                return true;
            return false;
        },
        IsLookup: function (displayType) {
            if (VIS.DisplayType.List == displayType || displayType == VIS.DisplayType.TableDir || displayType == VIS.DisplayType.Table
                   || displayType == VIS.DisplayType.Search || displayType == VIS.DisplayType.MultiKey) {
                return true;
            }
            return false;
        },
        IsInt: function (displayType) {
            if (displayType == VIS.DisplayType.Integer) {
                return true;
            }
            return false;
        },
        IsSearch: function (displayType) {
            if (displayType == this.Search) {
                return true;
            }
            return false;
        },
        IsID: function (displayType) {
            if (displayType == VIS.DisplayType.ID || displayType == VIS.DisplayType.Table || displayType == VIS.DisplayType.TableDir
                || displayType == VIS.DisplayType.Search || displayType == VIS.DisplayType.Location || displayType == VIS.DisplayType.Locator
                || displayType == VIS.DisplayType.Account || displayType == VIS.DisplayType.Assignment || displayType == VIS.DisplayType.PAttribute
                || displayType == VIS.DisplayType.Image || displayType == VIS.DisplayType.Color)
                return true;
            return false;
        },
        IsNumeric: function (displayType) {
            if (displayType == VIS.DisplayType.Amount || displayType == VIS.DisplayType.Number || displayType == VIS.DisplayType.CostPrice
                || displayType == VIS.DisplayType.Integer || displayType == VIS.DisplayType.Quantity)
                return true;
            return false;
        },	//	
        IsDate: function (displayType) {
            if (displayType == VIS.DisplayType.Date || displayType == VIS.DisplayType.DateTime || displayType == VIS.DisplayType.Time)
                return true;
            return false;
        },	//	isDate
        IsLOB: function (displayType) {
            if (displayType == VIS.DisplayType.Binary
                || displayType == VIS.DisplayType.TextLong)
                return true;
            return false;
        },
        MAX_DIGITS: 28, //  Oracle Standard Limitation 38 digits
        INTEGER_DIGITS: 10,
        MAX_FRACTION: 12,
        AMOUNT_FRACTION: 2,
        GetNumberFormat: function (displayType) {

            var format = null;
            if (displayType == this.Integer) {
                format = new VIS.Format(this.INTEGER_DIGITS, 0, 0);
            }
            else if (displayType == this.Quantity) {
                format = new VIS.Format(this.MAX_DIGITS, this.MAX_FRACTION, 0);
            }
            else if (displayType == this.Amount) {
                format = new VIS.Format(this.MAX_DIGITS, this.AMOUNT_FRACTION, this.AMOUNT_FRACTION);
            }
            else if (displayType == this.CostPrice) {
                format = new VIS.Format(this.MAX_DIGITS, this.MAX_FRACTION, this.AMOUNT_FRACTION);
            }
            else //	if (displayType == Number)
            {
                format = new VIS.Format(this.MAX_DIGITS, this.MAX_FRACTION, 1);
            }
            return format;
        }
    };

    /*********** END DisplayType********************/

    /**
     *  Factory for Control and its Label for single Row display 
     *
     */
    VIS.VControlFactory = {
        /**
         *  Create control for MField.
         *  The Name is set to the column name for dynamic display management
         *  @param mTab MTab
         *  @param mField MField
         *  @param tableEditor true if table editor
         *  @param disableValidation show all values
         *  @return icontrol
         */
        getControl: function (mTab, mField, tableEditor, disableValidation, other) {
            if (!mField)
                return null;
            var columnName = mField.getColumnName();
            var isMandatory = mField.getIsMandatory(false);      //  no context check
            //  Not a Field
            if (mField.getIsHeading())
                return null;
            var isDisplayed = mField.getIsDisplayed();

            var ctrl = null;
            var displayType = mField.getDisplayType();

            var isReadOnly = mField.getIsReadOnly();
            var isUpdateable = mField.getIsEditable(false);
            var windowNo = mField.getWindowNo();

            if (displayType == VIS.DisplayType.Button) {

                var btn = new VButton(columnName, isMandatory, isReadOnly, isUpdateable, mField.getHeader(), mField.getDescription(), mField.getHelp(), mField.getAD_Process_ID(), mField.getIsLink(), mField.getIsRightPaneLink())
                btn.setField(mField);
                ctrl = btn;
            }

            else if (displayType == VIS.DisplayType.String) {

                //if (mField.getIsEncryptedField()) {
                //    //VPassword vs = new VPassword (columnName, mandatory, readOnly, updateable,
                //    //    mField.getDisplayLength(), mField.getFieldLength(), mField.getVFormat());
                //    //vs.setName (columnName);
                //    //vs.setField (mField);
                //    //editor = vs;
                //}
                //else {
                var txt = new VTextBox(columnName, isMandatory, isReadOnly, isUpdateable, mField.getDisplayLength(), mField.getFieldLength(),
                                        mField.getVFormat(), mField.getObscureType(), mField.getIsEncryptedField());
                txt.setField(mField);
                ctrl = txt;
                //}
            }
            else if (displayType == VIS.DisplayType.YesNo) {
                //columnName, mandatory, isReadOnly, isUpdateable, text, description, tableEditor
                var chk = new VCheckBox(columnName, isMandatory, isReadOnly, isUpdateable, mField.getHeader(), mField.getDescription());
                chk.setField(mField);
                ctrl = chk;
            }
            else if (VIS.DisplayType.IsDate(displayType)) {

                if (displayType == VIS.DisplayType.DateTime)
                    readOnly = true;
                var vd = new VDate(columnName, isMandatory, isReadOnly, isUpdateable,
                     displayType, mField.getHeader());
                vd.setName(columnName);
                vd.setField(mField);
                ctrl = vd;
            }
            else if (VIS.DisplayType.IsLookup(displayType) || VIS.DisplayType.ID == displayType) {
                var lookup = mField.getLookup();
                if (disableValidation && lookup != null)
                    lookup.disableValidation();

                if (!disableValidation) {

                    if (displayType != VIS.DisplayType.Search && displayType != VIS.DisplayType.MultiKey) {

                        var cmb = new VComboBox(columnName, isMandatory, isReadOnly, isUpdateable, lookup, mField.getDisplayLength(), displayType, mField.getZoomWindow_ID());
                        cmb.setField(mField);
                        ctrl = cmb;
                        //ctrl = new VComboBox();
                    }
                    else {

                        var txtb = new VTextBoxButton(columnName, isMandatory, isReadOnly, isUpdateable, displayType, lookup, mField.getZoomWindow_ID());
                        txtb.setField(mField);
                        ctrl = txtb;
                    }
                }

                else {
                    if (lookup == null || (displayType != VIS.DisplayType.Search && lookup.getDisplayType() != VIS.DisplayType.Search)) {
                        var cmb = new VComboBox(columnName, isMandatory, isReadOnly, isUpdateable, lookup, mField.getDisplayLength(), displayType, mField.getZoomWindow_ID());
                        cmb.setDisplayType(displayType);
                        cmb.setField(mField);
                        ctrl = cmb;
                        // ctrl = new VComboBox();
                    }
                    else {
                        displayType = VIS.DisplayType.Search;
                        var txtb = new VTextBoxButton(columnName, isMandatory, isReadOnly, isUpdateable, displayType, lookup, mField.getZoomWindow_ID());
                        txtb.setField(mField);
                        ctrl = txtb;
                    }
                }
            }
            else if (displayType == VIS.DisplayType.Location) {

                var txtLoc = new VLocation(columnName, isMandatory, isReadOnly, isUpdateable, displayType, mField.getLookup());
                txtLoc.setField(mField);
                ctrl = txtLoc;
            }
            else if (displayType == VIS.DisplayType.Locator) {
                var txtLocator = new VLocator(columnName, isMandatory, isReadOnly, isUpdateable, displayType, mField.getLookup());
                txtLocator.setField(mField);
                ctrl = txtLocator;
            }
            else if (VIS.DisplayType.Text == displayType || VIS.DisplayType.TextLong == displayType) {
                var tl = new VTextArea(columnName, isMandatory, isReadOnly, isUpdateable, mField.getDisplayLength(), mField.getFieldLength(), displayType);
                tl.setField(mField);
                ctrl = tl;
            }
            else if (VIS.DisplayType.IsNumeric(displayType)) {

                if (VIS.DisplayType.Amount == displayType || VIS.DisplayType.Quantity == displayType || VIS.DisplayType.CostPrice == displayType) {
                    var amt = new VAmountTextBox(columnName, isMandatory, isReadOnly, isUpdateable, mField.getDisplayLength(), mField.getFieldLength(), displayType, "FixedHeader");// mField.getHeader());
                    amt.setField(mField);
                    ctrl = amt;
                }
                else if (VIS.DisplayType.Integer == displayType) {
                    var int = new VNumTextBox(columnName, isMandatory, isReadOnly, isUpdateable, mField.getDisplayLength(), mField.getFieldLength(), "FixedHeader");// mField.getHeader());
                    int.setField(mField);
                    ctrl = int;
                }
                else if (VIS.DisplayType.Number == displayType) {
                    var num = new VAmountTextBox(columnName, isMandatory, isReadOnly, isUpdateable, mField.getDisplayLength(), mField.getFieldLength(), displayType, "FixedHeader");// mField.getHeader());
                    num.setField(mField);
                    ctrl = num;
                }
            }
            else if (displayType == VIS.DisplayType.PAttribute) {

                var txtP = new VPAttribute(columnName, isMandatory, isReadOnly, isUpdateable, displayType, mField.getLookup(), windowNo, false, false, false, true);
                txtP.setField(mField);
                ctrl = txtP;
            }
            else if (displayType == VIS.DisplayType.GAttribute) {
                var txtP = new VPAttribute(columnName, isMandatory, isReadOnly, isUpdateable, displayType, mField.getLookup(), windowNo, false, false, false, false);
                txtP.setField(mField);
                ctrl = txtP;
            }
            else if (displayType == VIS.DisplayType.Account) {
                var txtA = new VAccount(columnName, isMandatory, isReadOnly, isUpdateable, displayType, mField.getLookup(), windowNo, mField.getHeader());
                txtA.setField(mField);
                ctrl = txtA;
            }
            else if (displayType == VIS.DisplayType.Binary) {
                var bin = new VBinary(columnName, isMandatory, isReadOnly, isUpdateable, windowNo);
                bin.setField(mField);
                ctrl = bin;
            }
            else if (displayType == VIS.DisplayType.Image) {
                var image = new VImage(columnName, isMandatory, isReadOnly, windowNo);
                image.setField(mField);
                ctrl = image;
            }
            else if (displayType == VIS.DisplayType.URL) {
                var vs = new VURL(columnName, isMandatory, isReadOnly, isUpdateable, mField.getDisplayLength(), mField.getFieldLength());
                vs.setField(mField);
                ctrl = vs;
            }
            else if (displayType == VIS.DisplayType.FileName || displayType == VIS.DisplayType.FilePath) {
                var vs = new VFile(columnName, isMandatory, isReadOnly, isUpdateable, windowNo, displayType);
                vs.setField(mField);
                ctrl = vs;
            }

            else if (displayType == VIS.DisplayType.Label) {
                ///******14/2/2012 *******///
                ///implement Action pane 
                ///

                var txt = new VLabel(mField.getHelp(), columnName, false, true);
                //VAdvantage.Controls.VButton button = new VAdvantage.Controls.VButton();
                //button.SetAttribute(mField, columnName, mandatory, isReadOnly, isUpdateable,
                //                    mField.GetHeader(), mField.GetDescription(), mField.GetHelp(), mField.GetAD_Process_ID());
                txt.setField(mField);
                ctrl = txt;
            }
            return ctrl;
        },

        /**
         *  Create Label for MField. (null for YesNo/Button)
         *  The Name is set to the column name for dynamic display management
         *
         *  @param mField MField
         *  @return Label
         */
        getLabel: function (mField) {
            if (mField == null)
                return null;

            var displayType = mField.getDisplayType();

            //	No Label for FieldOnly, CheckBox, Button
            if (mField.getIsFieldOnly()
                    || displayType == VIS.DisplayType.YesNo
                    || displayType == VIS.DisplayType.Button
                    || displayType == VIS.DisplayType.Label)
                return null;
            return new VIS.Controls.VLabel(mField.getHeader(), mField.getColumnName(), mField.getIsMandatory());
        }
    };



    /***************************************************************************
     *	base class  for single Row controls 
     *  <p>
     *  Controls fire VetoableChange to inform about new entered values
     *  and listen to propertyChange (MField.PROPERTY) to receive new values
     *  and performed action to inform action listner (etc click event);
     *  or  changes of Background or Editability
     *
     ***************************************************************************/

    function IControl(ctrl, displayType, isReadOnly, colName, isMandatory) {
        if (this instanceof IControl) {
            this.ctrl = ctrl;
            this.displayType = displayType;
            this.isReadOnly = isReadOnly;
            this.colName = colName;
            this.isMandatory = isMandatory;
            this.oldValue = "oldValue";
            this.visible;
            this.tag;
            this.setVisible(true);
        }
        else {
            return new VIS.controls.IControl(ctrl, displayType, isReadOnly, colName, isMandatory);
        }
    };

    /**
     *	Get control
     * 	@return control
     */
    IControl.prototype.getControl = function () { return this.ctrl };
    /**
     *	Get model field
     * 	@return field
     */
    IControl.prototype.getField = function () { return this.mField };
    /**
     *	Get Column Name
     * 	@return column name
     */
    IControl.prototype.getName = function () { return this.colName };
    /**
     *	Get Column Name
     * 	@return column name
     */
    IControl.prototype.getColumnName = function () { return this.colName };
    /**
     *	Get is Mandatory
     * 	@return true control can not be empty
     */
    IControl.prototype.getIsMandatory = function () { return this.isMandatory; };
    /**
     *	Get is readonly
     * 	@return read only
     */
    IControl.prototype.getIsReadonly = function () { return this.readOnly };
    /**
     *	Get type of control
     * 	@return display type
     */
    IControl.prototype.getDisplayType = function () { return this.displayType };
    /**
     *	Get visibility
     * 	@return visible or not
     */
    IControl.prototype.getIsVisible = function () {
        return this.visible;
    };
    /**
     *	Get value of control
     * 	@return value
     */
    IControl.prototype.getValue = function () {
        return this.ctrl.val();
    };	//

    IControl.prototype.getDisplay = function () {
        return this.ctrl.val();
    };

    /**
     * get additinal action button count with this control
     -  zoom 0r Info
    */
    IControl.prototype.getBtnCount = function () {
        return 0;
    };	//

    /**
     *	set name of control
     *
     * @param name
     */
    IControl.prototype.setName = function (name, prefix) { if (!prefix) { this.colName = name; } else { this.name = prefix + name; } };
    /**
     *	set model field
     *
     * @param model field
     */
    IControl.prototype.setField = function (mField) {
        this.mField = mField;
    };
    /**
     *	set value 
     *
     * @param value to set 
     */
    IControl.prototype.setValue = function (val) {
    };
    /**
     *	show hide control
     *
     * @param visible
     */
    IControl.prototype.setVisible = function (visible) {
        this.visible = visible;
        if (visible) {
            this.ctrl.show();
        } else {
            this.ctrl.hide();
        }
    };
    /**
     *	set readonly
     *
     * @param readOnly
     */
    IControl.prototype.setReadOnly = function (readOnly) {
        this.isReadOnly = readOnly;
        this.ctrl.prop('disabled', readOnly ? true : false);
        this.setBackground(false);
    };
    /**
     *	set mandatory
     *
     * @param ismandotry
     */
    IControl.prototype.setMandatory = function (isMandatory) {
        this.isMandatory = isMandatory;
        this.setBackground(false);
    };
    /**
     *	set backgoud color of control
     *
     * @param iserror
     */
    IControl.prototype.setBackground = function (e) {

        if (this.colName.startsWith("lbl") || this.displayType == VIS.DisplayType.Label ||
            this.displayType == VIS.DisplayType.YesNo || this.displayType == VIS.DisplayType.Button)
            return;


        //console.log(typeof (e));
        if (typeof (e) == "boolean") {
            // console.log("1");
            if (e)
                this.setBackground("vis-gc-vpanel-table-error");// CompierePLAF.getFieldBackground_Error());
            else if (this.isReadOnly)
                this.setBackground("vis-gc-vpanel-table-readOnly");// CompierePLAF.getFieldBackground_Inactive());
            else if (this.isMandatory) {
                var val = this.getValue();
                if (val && val.toString().length > 0) {
                    this.setBackground("");
                    return;
                }
                this.setBackground("vis-gc-vpanel-table-mandatory");
            }
            else
                this.setBackground("");//CompierePLAF.getFieldBackground_Normal());
        }
        else {
            // console.log("2");
            //var c = this.ctrl.css('background-color');
            //if (c == color)
            //    return;
            if (this.activeClass == e)
                return;

            this.ctrl.removeClass();
            if (e.length > 0)
                this.ctrl.addClass(e);
            this.activeClass = e;
            //this.ctrl.css('background-color', color);
            //console.log(this.ctrl.css('background-color'));
        }
    };

    IControl.prototype.setDisplayType = function (displayType) {
        this.displayType = displayType;
    };
    /**
     *	value Change Listener 
     *  @param listener
     */
    IControl.prototype.addVetoableChangeListener = function (listner) {
        this.vetoablechangeListner = listner;
    };
    /**
     *	Notify value changed
     *  @param event
     */
    IControl.prototype.fireValueChanged = function (evt) {
        if (this.vetoablechangeListner) {
            window.setTimeout(function (self) {
                self.vetoablechangeListner.vetoablechange(evt);
                self = null;
                evt = null;
            }, 10, this);
        }
    };
    /**
     *	Refresh UI
     *  @param event
     */
    IControl.prototype.fireRefreshUI = function (evt) {
        if (this.vetoablechangeListner) {
            window.setTimeout(function (self) {
                self.vetoablechangeListner.refreshUI(evt);
                self = null;
                evt = null;
            }, 10, this);
        }
    };


    /**
    *	action listner
    *   @param event
    */
    IControl.prototype.addActionListner = function (listner) {
        this.actionListner = listner;
    };
    /**
     *	Notify action (eg click )
     *  @param event
     */
    IControl.prototype.invokeActionPerformed = function (evt) {
        if (this.actionListner) {
            this.actionListner.actionPerformed(evt);
        }
    };

    /**
     *	clean up
     */
    IControl.prototype.dispose = function () {
        console.log("dispose ");
        this.ctrl.remove();
        this.ctrl = null;
        this.mField = null;
        this.displayType = null;
        this.isReadOnly = null;
        this.colName = null;
        this.isMandatory = null;
        this.oldValue = null;
        this.vetoablechangeListner = null;
        this.actionListner = null;
        this.disposeComponent();
    };
    //END IControls 


    /**************************************************************************
     *	Data Binding:
     *		Icontrols call fireVetoableChange(m_columnName, null, getText());
     *		GridController (for Single-Row) 
     *      listen to Vetoable Change Listener (vetoableChange)
     *		then set the value for that column in the current row in the table
     *
     **************************************************************************/



    /**************************************************************************
     *	visual control for system displaytype text or string
     *	Detail Constructor
     *  @param columnName column name
     *  @param mandatory mandatory
     *  @param isReadOnly read only
     *  @param isUpdateable updateable
     *  @param displayLength display length
     *  @param fieldLength field length
     *  @param VFormat format
     *  @param ObscureType obscure type
     ***************************************************************************/

    function VTextBox(columnName, isMandatory, isReadOnly, isUpdateable, displayLength, fieldLength, vFormat, obscureType, isPwdField) {

        var displayType = VIS.DisplayType.String;

        //Init Control
        var $ctrl = $('<input>', { type: (isPwdField) ? 'password' : 'text', name: columnName, maxlength: fieldLength });
        //Call base class
        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory);

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
            //this.Enabled = false;
        }
        else {
            this.setReadOnly(false);
        }

        var self = this; //self pointer

        /* Event */
        $ctrl.on("change", function (e) {
            e.stopPropagation();
            var newVal = $ctrl.val();
            this.value = newVal;
            if (newVal !== self.oldValue) {
                var evt = { newValue: newVal, propertyName: self.getName() };
                self.fireValueChanged(evt);
                evt = null;
            }
        });

        this.disposeComponent = function () {
            $ctrl.off("change"); //u bind event
            $ctrl = null;
            self = null;
        }
    };

    VIS.Utility.inheritPrototype(VTextBox, IControl);//Inherit from IControl

    /** 
     *  set value 
     *  @param new value to set
     */
    VTextBox.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;
            //console.log(newValue);
            this.ctrl.val(newValue);
            //this.setBackground("white");
        }
    };

    /** 
     *  get display text of control
     *  @return text of control
     */
    VTextBox.prototype.getDisplay = function () {
        return this.ctrl.val();
    };
    //END VTextbox


    //2.  VLabel

    /**
     *  Label with Mnemonics interpretation
     *  Label against model field control like (textbox, combobox etc)
     *  @param value  The text to be displayed by the label.
     *  @param name  name of control to bind label with
     */
    function VLabel(value, name, isMandatory, isADControl) {
        value = value != null ? value.replace("[&]", "") : "";

        var strFor = ' for="' + name + '"';
        if (isADControl)
            strFor = '';

        var $ctrl = $('<label ' + strFor + '>');

        IControl.call(this, $ctrl, VIS.DisplayType.Label, true, isADControl ? name : "lbl" + name);
        if (isMandatory) {
            $ctrl.text(value).append("<sup>*</sup>");
        }
        else {
            $ctrl.text(value);
        }

        this.disposeComponent = function () {
            $ctrl = null;
            self = null;
        }
    };

    VIS.Utility.inheritPrototype(VLabel, IControl); //Inherit



    // END VLabel 



    //3. VButton 
    /***************************************************************************
     *  General Button.
     *  <pre>
     *  Special Buttons:
     *      Payment,
     *      Processing,
     *      CreateFrom,
     *      Record_ID       - Zoom
     *  </pre>
     *  Maintains all values for display in m_values.
     *  implement action listner to notify click event
     *
     *  @param columnName column
     *  @param mandatory mandatory
     *  @param isReadOnly read only
     *  @param isUpdateable updateable
     *  @param text text
     *  @param description description
     *  @param help help
     *  @param AD_Process_ID process to start
    
     ***************************************************************************/
    function VButton(columnName, mandatory, isReadOnly, isUpdateable, text, description, help, AD_Process_ID, isLink, isRightLink) {

        this.actionListner;
        this.AD_Process_ID = AD_Process_ID;
        this.description = description;
        this.help = help;
        this.text = text;
        this.isLink = isLink;
        this.isRightLink = isRightLink;
        this.actionLink = null;

        this.values = null;

        var $img = $("<img style='min-width:15px'>");
        var $txt = $("<span>").text(text);
        var rootPath = VIS.Application.contextUrl + "Areas/VIS/Images/base/";

        var $ctrl = null;
        //Init Control
        if (!isLink) {
            $ctrl = $('<button class="vis-gc-vpanel-table-btn-blue">', { type: 'button', name: columnName });
            $img.css("margin-right", "8px");
        }
        else
            $ctrl = $('<li>');




        //	Special Buttons
        if (columnName.equals("PaymentRule")) {
            this.readReference(195);
            $txt.css("color", "blue"); //
            setIcon("Payment20.png");    //  29*14
        }
        else if (columnName.equals("DocAction")) {
            this.readReference(135);
            $txt.css("color", "blue"); //
            setIcon("Process20.png");    //  16*16
        }
        else if (columnName.equals("CreateFrom")) {
            setIcon("Copy16.png");       //  16*16
        }
        else if (columnName.equals("Record_ID")) {
            setIcon("Zoom20.png");       //  16*16
            $txt.text(VIS.Msg.getMsg("ZoomDocument"));
        }
        else if (columnName.equals("Posted")) {
            this.readReference(234);
            $txt.css("color", "magenta"); //
            setIcon("InfoAccount20.png");    //  16*16
        }


        function setIcon(img) {
            $img.attr('src', rootPath + img);
        };

        $ctrl.append($img).append($txt);
        IControl.call(this, $ctrl, VIS.DisplayType.Button, isReadOnly, columnName, mandatory);

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
            //this.Enabled = false;
        }
        else {
            this.setReadOnly(false);
        }

        var self = this; //self pointer

        $ctrl.on(VIS.Events.onClick, function (evt) { //click handler
            evt.stopPropagation();
            if (!self.isReadOnly)
                self.invokeActionPerformed({ source: self });
        });

        this.setText = function (text) {
            if (text == null) {
                $txt.text("");
                return;
            }
            var pos = text.indexOf('&');
            if (pos != -1)					//	We have a nemonic - creates ALT-_
            {
                var mnemonic = text.toUpperCase().charAt(pos + 1);
                if (mnemonic != ' ') {
                    //setMnemonic(mnemonic);
                    text = text.substring(0, pos) + text.substring(pos + 1);
                }
            }
            $txt.text(text);

        };

        this.disposeComponent = function () {
            $ctrl.off(VIS.Events.onClick);
            $ctrl = null;
            self = null;
            //this.actionListner = null;
            this.AD_Process_ID = null;
            this.description = null;
            this.help = null;
            this.setText = null;
        };
    };
    VIS.Utility.inheritPrototype(VButton, IControl);//Inherit

    VButton.prototype.setValue = function (value) {
        this.value = value;
        var text = this.text;

        //	Nothing to show or Record_ID
        if (value == null || this.colName.equals("Record_ID"))
            ;
        else if (this.values != null)
            text = this.values[value];
        else if (this.lookup != null) {
            var pp = this.lookup.get(value);
            if (pp != null)
                text = pp.getName();
        }
        //	Display it
        this.setText(text != null ? text : "");
    };

    VButton.prototype.setReadOnly = function (readOnly) {
        this.isReadOnly = readOnly;
        this.ctrl.css('opacity', readOnly ? .6 : 1);
        if (this.isLink) {
        }
        else {
            this.ctrl.prop('disabled', readOnly ? true : false);
        }
        this.setBackground(false);
    };

    /**
     *	Return Value
     *  @return value
     */

    VButton.prototype.getValue = function () {
        if (this.value != null) {
            return this.value.toString();
        }
        else {
            return null;
        }
    };	//	getValue

    /**
     *  Return Display Value
     *  @return String value
     */
    VButton.prototype.getDisplay = function () {
        return this.value;
    };  //  getDispl

    /**
     *	Fill m_Values with Ref_List values
     *  @param AD_Reference_ID reference
     */
    VButton.prototype.readReference = function (AD_Reference_ID) {
        this.values = {};
        var SQL;
        if (VIS.Env.isBaseLanguage(VIS.Env.getCtx(), "AD_Ref_List"))
            SQL = "SELECT Value, Name FROM AD_Ref_List WHERE AD_Reference_ID=" + AD_Reference_ID
                + " AND IsActive='Y'";
        else
            SQL = "SELECT l.Value, t.Name FROM AD_Ref_List l, AD_Ref_List_Trl t "
                + "WHERE l.AD_Ref_List_ID=t.AD_Ref_List_ID"
                + " AND t.AD_Language='" + VIS.Env.getAD_Language(VIS.Env.getCtx()) + "'"
                + " AND l.AD_Reference_ID=" + AD_Reference_ID
                + " AND l.IsActive='Y'";
        try {
            var dr = VIS.DB.executeDataReader(SQL);
            while (dr.read()) {

                this.values[dr.getString(0)] = dr.getString(1);
            }
            dr.close();
        }
        catch (e) {
            VIS.Logging.VLogger.getVLogger().get().log(VIS.Logging.Level.SEVERE, SQL, e);
        }
    };	//	readReferenc
    /**
     *  Return process id
     *  @return ad_process_id
     */
    VButton.prototype.getProcess_ID = function () {
        return this.AD_Process_ID;
    };
    /**
     *  Return description
     *  @return String value
     */
    VButton.prototype.getDescription = function () {
        return this.description;
    };
    /**
     *  Return help[ text
     *  @return String value
     */
    VButton.prototype.getHelp = function () {
        return this.help;
    };
    //End Button 





    //4. VCheckBox 
    /*********************************************************************
    *  Checkbox Control
    *
    *  @param columnName
    *  @param mandatory
    *  @param isReadOnly
    *  @param isUpdateable
    *  @param title
    *  @param description
    **********************************************************************/

    function VCheckBox(columnName, mandatory, isReadOnly, isUpdateable, text, description) {
        var $ctrl = $('<input>', { type: 'checkbox', name: columnName, value: text });
        var $lbl = $('<label class="vis-gc-vpanel-table-label-checkbox" />').html(text).prepend($ctrl);
        IControl.call(this, $lbl, VIS.DisplayType.YesNo, isReadOnly, columnName, mandatory);

        this.cBox = $ctrl;
        var self = this;

        this.setReadOnly = function (isReadOnly) {
            this.isReadOnly = isReadOnly;
            $ctrl.prop('disabled', isReadOnly);
            $lbl.css('opacity', isReadOnly ? .7 : 1);
        }

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
            //this.Enabled = false;
        }
        else {
            this.setReadOnly(false);
        }

        $ctrl.on("change", function (e) {
            e.stopPropagation();
            var newVal = $ctrl.prop('checked');
            if (newVal !== self.oldValue) {
                var evt = { newValue: newVal, propertyName: self.getName() };
                self.fireValueChanged(evt);
                evt = null;
            }
        });
        this.disposeComponent = function () {
            $ctrl.off("change");
            $ctrl = null;
            self = null;
            this.cBox = null;
        }
    };

    VIS.Utility.inheritPrototype(VCheckBox, IControl);//Inherit
    /** 
     *  Set Value 
     *  @param new Value
     */
    VCheckBox.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;
            //this.ctrl.val(newValue);
            this.cBox.prop('checked', newValue);
        }
    };

    /** @override  
     *  get Value 
     *  @return value
     */
    VCheckBox.prototype.getValue = function () {
        return this.cBox.prop('checked');
    };

    /** 
     *  get display
     *  @return value
     */
    VCheckBox.prototype.getDisplay = function () {
        return this.cBox.prop('checked').toString();
    };

    //END 


    //5.VCombobox

    /******************************************************************
    *  select control for Lookup Visual Field.
    *  Special handling of BPartner and Product
    *
    *  @param columnName column
    *  @param mandatory mandatory
    *  @param isReadOnly read only
    *  @param isUpdateable updateable
    *  @param lookup lookup
    *******************************************************************/
    function VComboBox(columnName, mandatory, isReadOnly, isUpdateable, lookup, displayLength, displayType, zoomWindow_ID) {
        if (!displayType)
            displayType = VIS.DisplayType.Table;

        var $ctrl = $('<select>', { name: columnName });
        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, mandatory);
        this.lookup = lookup;
        this.lastDisplay = "";
        this.settingFocus = false;
        this.inserting = false;
        this.settingValue = false;
        this.loading = false;

        if (lookup != null && lookup.getIsValidated()) {
            // if (lookup.getIsLoading())
            this.loading = true;
            //else {
            //  lookup.fillCombobox(mandatory, false, false, false);
            //  this.refreshOptions(lookup.data);
            // }
        }
        $ctrl[0].selectedIndex = -1;

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
            //this.Enabled = false;
        }
        else {
            this.setReadOnly(false);
        }

        //Set Buttons and [pop up]
        var isZoom = false;
        var $btnPop = null;
        var btnCount = 0;
        var $ulPopup = null;
        var options = {};
        var disabled = false;
        if (lookup != null) {

            if ((lookup.getDisplayType() == VIS.DisplayType.List && VIS.context.getAD_Role_ID() == 0)
                || lookup.getDisplayType() != VIS.DisplayType.List)     //  only system admins can change lists, so no need to zoom for others
            {
                isZoom = true;

                if (lookup instanceof VIS.MLookup) {
                    if (lookup.getZoomWindow() == 0 && (zoomWindow_ID < 1)) {
                        disabled = true;
                    }
                }

                //$btnZoom = VIS.AEnv.getZoomButton(disabled);
                options[VIS.Actions.zoom] = disabled;
                // btnCount += 1;
            }

            if ((this.lookup &&
                (this.lookup.info.keyColumn.toLowerCase() == "ad_user.ad_user_id"
                 || this.lookup.info.keyColumn.toLowerCase() == "ad_user_id"))
                 || columnName === "AD_User_ID" || columnName === "SalesRep_ID") {
                options[VIS.Actions.contact] = true;
            }

            $btnPop = $('<button tabindex="-1" class="vis-controls-txtbtn-table-td2"><img tabindex="-1" src="' + VIS.Application.contextUrl + "Areas/VIS/Images/base/Info20.png" + '" /></button>');
            options[VIS.Actions.refresh] = true;
            if (VIS.MRole.getIsShowPreference())
                options[VIS.Actions.preference] = true;
            $ulPopup = VIS.AEnv.getContextPopup(options);
            btnCount += 1;
            options = null;
        }


        /* provilized function */

        /* @overridde
        */
        this.getBtnCount = function () {
            return btnCount;
        };

        /** 
            get contols button by index 
            -  zoom or info button 
            - index 0 for info button 
            - index 1 for zoom button
            - control must tell or implemnt getBtnCount default is zero [no button]
        *
        */
        this.getBtn = function (index) {

            return $btnPop;
        };

        this.setVisible = function (visible) {
            this.visible = visible;
            if (visible) {
                if ($btnPop)
                    $btnPop.show();
                $ctrl.show();

            } else {
                $ctrl.hide();
                if ($btnPop)
                    $btnPop.hide();
            }
        };

        var self = this; //self pointer


        //$ctrl.on("mousedown", function (e) {
        //    e.preventDefault();
        //});

        //$ctrl.on("touchend", function (e) {
        //    e.preventDefault();
        //});

        $ctrl.on("focus", function (e) {


            var outp = [];
            if (self.lookup == null)
                return;

            var selVal = $ctrl.val();
            if (self.lookup.getIsValidated() && !self.lookup.getHasInactive()) {

                if (self.loading) {
                    self.lookup.fillCombobox(mandatory, false, false, false);
                    if (self.lookup.loading) return;
                    self.refreshOptions(self.lookup.data, selVal);

                    if (selVal != null && $ctrl[0].selectedIndex < 0) {
                        //fire change event
                        self.oldValue = "old";
                        // fire  on change to clear datasource value

                        $ctrl.trigger("change")
                    }
                    self.loading = false;
                }
                return;
            }
            if (self.getIsReadonly())
                return;

            self.settingFocus = true;

            //var obj = lookup.getSelectedItem();
            var selVal = $ctrl.val();

            self.lookup.fillCombobox(self.getIsMandatory(), true, true, true);     //  only validated & active & temporary
            //self.lookup.setSelectedItem(selVal);
            //obj = self.lookup.getSelectedItem();

            self.refreshOptions(self.lookup.data, selVal);

            if (selVal && $ctrl[0].selectedIndex < 0) {
                self.oldValue = "old";
                // fire  on change to clear datasource value

                $ctrl.trigger("change")
            }
            self.settingFocus = false;

        });

        $ctrl.on("change", function (e) {

            var newVal = $ctrl.val();
            if (newVal !== self.oldValue) {
                if (newVal == -1 || newVal == "") {
                    newVal = null;
                }
                var evt = { newValue: newVal, propertyName: self.getName() };
                self.fireValueChanged(evt);
                evt = null;
            }
            e.stopPropagation();
        });

        function zoomAction() {
            if (!self.lookup || disabled)
                return;
            //
            var zoomQuery = self.lookup.getZoomQuery();
            var value = self.getValue();
            if (value == null) {
                //   value = selectedItem;
            }

            if (value == "")
                value = null;

            if (value != null && !isNaN(value))
                value = parseInt(value);


            //	If not already exist or exact value
            if ((zoomQuery == null) || (value != null)) {
                zoomQuery = new VIS.Query();	//	ColumnName might be changed in MTab.validateQuery

                var keyColumnName = null;
                //	Check if it is a Table Reference
                if ((self.lookup != null) && (self.lookup instanceof VIS.MLookup)) {
                    var AD_Reference_ID = self.lookup.getAD_Reference_Value_ID();
                    if (AD_Reference_ID != 0) {
                        var query = "SELECT kc.ColumnName"
                            + " FROM AD_Ref_Table rt"
                            + " INNER JOIN AD_Column kc ON (rt.Column_Key_ID=kc.AD_Column_ID)"
                            + "WHERE rt.AD_Reference_ID=" + AD_Reference_ID;

                        try {
                            var dr = VIS.DB.executeDataReader(query);
                            if (dr.read()) {
                                keyColumnName = dr.getString(0);
                            }
                            dr.dispose();
                        }
                        catch (e) {
                            this.log.log(VIS.Logging.Level.SEVERE, query, e);
                        }
                    }	//	Table Reference
                }	//	MLookup

                if ((keyColumnName != null) && (keyColumnName.length != 0))
                    zoomQuery.addRestriction(keyColumnName, VIS.Query.prototype.EQUAL, value);
                else
                    zoomQuery.addRestriction(self.getColumnName(), VIS.Query.prototype.EQUAL, value);
                zoomQuery.setRecordCount(1);	//	guess
            }

            var AD_Window_ID = 0;
            if (self.mField.getZoomWindow_ID() > 0) {
                AD_Window_ID = self.mField.getZoomWindow_ID();
            }
            else {
                AD_Window_ID = self.lookup.getZoomWindow(zoomQuery);
            }



            //
            //this.log.info(this.getColumnName() + " - AD_Window_ID=" + AD_Window_ID
            //    + " - Query=" + zoomQuery + " - Value=" + value);
            //
            //setCursor(Cursor.getPredefinedCursor(Cursor.WAIT_CURSOR));
            //
            VIS.viewManager.startWindow(AD_Window_ID, zoomQuery);

            //setCursor(Cursor.getDefaultCursor());
        };

        var option

        if ($btnPop) {
            $btnPop.on(VIS.Events.onClick, function (e) {
                $btnPop.w2overlay($ulPopup.clone(true));
                e.stopPropagation();
            });
        }

        if ($ulPopup) {
            $ulPopup.on(VIS.Events.onTouchStartOrClick, "LI", function (e) {

                var action = $(e.target).data("action");
                if (action == VIS.Actions.preference) {
                    var obj = new VIS.ValuePreference(self.mField, self.getValue(), self.getDisplay());
                    if (obj != null) {
                        obj.showDialog();
                    }
                    obj = null;
                }
                else if (action == VIS.Actions.refresh) {
                    if (lookup == null)
                        return;
                    //
                    //setCursor(Cursor.getPredefinedCursor(Cursor.WAIT_CURSOR));
                    //
                    self.settingFocus = true;

                    var selVal = $ctrl.val();

                    self.lookup.refresh();
                    if (self.lookup.getIsValidated())
                        self.lookup.fillCombobox(self.getIsMandatory(), false, false, false);
                    else
                        self.lookup.fillCombobox(self.getIsMandatory(), true, false, false);
                    //m_combo.setSelectedItem(obj);

                    //	m_combo.revalidate();
                    //
                    self.refreshOptions(self.lookup.data, selVal);
                    self.settingFocus = false;
                    //alert(selVal);
                    //log.info(m_columnName + " #" + m_lookup.getSize() + ", Selected=" + m_combo.getSelectedItem());
                }
                else if (action == VIS.Actions.contact) {
                    var val = self.getValue();
                    if (val && val.toString().length > 0) {
                        //var contactInfo = new VIS.ContactInfo(val, this.mField.getWindowNo());
                        var contactInfo = new VIS.ContactInfo(val, self.mField.getWindowNo());
                        contactInfo.show();

                    }
                }
                else if (action == VIS.Actions.zoom) {
                    if (!disabled)
                        zoomAction();
                }
            });
        }

        this.disposeComponent = function () {
            $ctrl.off("focus");
            $ctrl.off("change");
            if ($btnPop) {
                $btnPop.off(VIS.Events.onClick);
                $btnPop.remove();
            }

            $btnPop = null;

            if ($ulPopup)
                $ulPopup.remove();
            $ulPopup = null;

            this.lookup = null;
            this.lastDisplay = null;
            this.settingFocus = null;
            this.inserting = null;
            this.settingValue = null;
            $ctrl = null;
            self = null;
        }
    };
    VIS.Utility.inheritPrototype(VComboBox, IControl);//inherit IControl

    /**
     *  Set control to value
     *  @param newValue new Value
     */
    VComboBox.prototype.setValue = function (newValue, inserting) {




        if (this.oldValue != newValue) {
            this.settingValue = true;
            this.oldValue = newValue;

            //	Set comboValue
            this.ctrl.val(newValue);

            if (newValue == null) {
                this.lastDisplay = "";
                this.settingValue = false;
                return;
            }
            if (this.lookup == null) {
                this.lastDisplay = newValue.toString();
                this.settingValue = false;
                return;
            }

            this.lastDisplay = this.lookup.getDisplay(newValue, true);
            if (this.lastDisplay.equals("<-1>")) {
                this.lastDisplay = "";
                this.oldValue = null;
            }

            this.inserting = inserting;	//	MField.setValue

            var notFound = this.lastDisplay.startsWith("<") && this.lastDisplay.endsWith(">");

            var selValue = this.ctrl.val();

            if ((selValue == null || (this.inserting && (this.lookup.getDisplayType() != VIS.DisplayType.Search)))) {
                //  lookup found nothing too

                if (notFound && this.lookup.loading) { //wait for fill lookup operation completation
                    window.setTimeout(function (that) {
                        that.setValue(newValue, inserting);
                        that = null;
                        return;
                    }, 500, this);
                    this.oldValue = "oldValue";
                    return;
                }

                if (notFound) {
                    //  we may have a new value
                    // this.lookup.refresh();
                    // this.lookup.fillCombobox(
                    // this.refreshOptions(this.lookup.data, newValue);

                    this.lastDisplay = this.lookup.getDisplay(newValue, false);
                    notFound = this.lastDisplay.startsWith("<") && this.lastDisplay.endsWith(">");
                }
                if (notFound)	//	<key>
                {
                    this.oldValue = "old";
                    this.ctrl.val(null);
                    // fire  on change to clear datasource value
                    if (this.inserting)
                        this.ctrl.trigger("change");
                }
                    //  we have lookup
                else if (this.ctrl.val() == null) {

                    var pp = null;
                    if (!this.lookup.loading) {
                        pp = this.lookup.getFromList(newValue);
                    }
                    if (pp == null) {
                        pp = this.lookup.get(newValue);
                    }
                    if (pp != null) {
                        this.ctrl.append('<option value="' + pp.Key + '">' + pp.Name + '</option>');
                        this.ctrl.val(newValue);
                    }
                }
                //  Not in Lookup - set to Null
                if ((this.ctrl.val() == null) && (newValue != null)) {
                    this.oldValue = null;
                }
            }
            this.settingValue = false;
            //this.setBackground("white");
        }
        this.inserting = false;
    };

    /**
     *  recrete options of control
     *  @param data  object array
     *  @param selVal value to select
     */
    VComboBox.prototype.refreshOptions = function (data, selVal) {
        var output = [];
        var selIndex = -1;
        for (var i = 0; i < data.length; i++) {
            if (selVal && selVal == data[i].Key) {
                selIndex = i;
            }
            output[i] = '<option value="' + data[i].Key + '">' + data[i].Name + '</option>';
        }
        this.ctrl.empty();
        this.ctrl.html(output.join(''));

        //if (selVal) {
        this.ctrl[0].selectedIndex = selIndex;
        // }
    };

    /**
     *  Return control display
     *  @return display value
     */
    VComboBox.prototype.getDisplay = function () {
        var retValue = "";
        if (this.lookup == null)
            retValue = this.ctrl.val();
        else
            retValue = this.lookup.getDisplay(this.ctrl.val());
        return retValue;
    };


    //VComboBox.prototype.dispose = function () {
    //    this.ctrl.off("focus");
    //    this.ctrl.off("change");
    //    this.ctrl.remove();
    //    this.ctrl = null;
    //    this.mField = null;
    //}
    //End VCombobox


    //6. VDate

    /**
     *	Create Date field
     *  @param columnName column name
     *  @param mandatory mandatory
     *  @param isReadOnly read only
     *  @param isUpdateable updateable
     *  @param displayType display type
     *  @param title title
     */
    function VDate(columnName, isMandatory, isReadOnly, isUpdateable, displayType, title) {

        var type = 'date';

        if (displayType == VIS.DisplayType.Time) {
            type = 'time'
        }
        if (displayType == VIS.DisplayType.DateTime) {
            type = 'datetime-local'
        }

        var $ctrl = $('<input>', {
            'type': type, name: columnName
        });

        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory);

        //	ReadWrite
        if (isReadOnly || !isUpdateable)
            this.setReadOnly(true);
        else
            this.setReadOnly(false);


        var self = this;

        $ctrl.on("change", function (e) {
            e.stopPropagation();
            var newVal = self.getValue();
            self.oldValue = newVal;
            var evt = { newValue: newVal, propertyName: self.getName() };
            self.fireValueChanged(evt);
            evt = null;


        });


        /* privilized function */
        this.disposeComponent = function () {
            $ctrl.off("change");
            self = null;
            $ctrl = null;
        };
    }

    VIS.Utility.inheritPrototype(VDate, IControl);//inherit IControl

    VDate.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;
            if (newValue == null || newValue == "") {
                this.ctrl.val("");
                return;
            }

            var date = new Date(newValue);
            date.setMinutes(-date.getTimezoneOffset() + date.getMinutes());

            newValue = date.toISOString();
            var val = newValue.substring(0, newValue.length - 1);
            var indexTime = newValue.indexOf("T");
            if (this.displayType == VIS.DisplayType.DateTime) {

                this.ctrl.val(val);
            }
            else if (this.displayType == VIS.DisplayType.Date) {
                this.ctrl.val(val.substring(0, indexTime));
            }
            else {
                //var d = new Date(newValue);
                //var h = d.getHours();
                //var m = d.getMinutes();
                var h = date.getUTCHours();
                var m = date.getUTCMinutes();

                this.ctrl.val(((h < 10) ? ("0" + h) : h) + ":" + ((m < 10) ? ("0" + m) : m));//.substring(indexTime + 1, val.length));
            }

            //this.setBackground("white");
        }
    };

    VDate.prototype.getValue = function () {
        var val = this.ctrl.val();
        if (val == null || val == "")
            return null;
        var d = null;

        if (this.displayType == VIS.DisplayType.Time) {
            d = new Date(0);
            var parts = val.match(/(\d+)\:(\d+)/);
            var hours = parseInt(parts[1], 10),
            minutes = parseInt(parts[2], 10);
            d.setHours(hours);
            d.setMinutes(minutes);
        }
        else {
            d = new Date(val);
        }

        try {
            if (this.displayType == VIS.DisplayType.DateTime)
                d.setMinutes(d.getTimezoneOffset() + d.getMinutes());
            return d.toISOString();
        }
        catch (e) {
            console.log(val);
            return new Date();
        }
    };

    VDate.prototype.getDisplay = function () {
        return this.getValue();
    };

    //EndDate

    //6. VSearch

    /**
     *	Create lookup search field
     *  @param columnName column name
     *  @param mandatory mandatory
     *  @param isReadOnly read only
     *  @param isUpdateable updateable
     *  @param displayType display type
     *  @param title title
     */

    function VTextBoxButton(columnName, isMandatory, isReadOnly, isUpdateable, displayType, lookup, zoomWindow_ID) {
        if (!displayType)
            displayType = VIS.DisplayType.Search;

        this.lookup = lookup;
        this.isMultiKeyTextBox = displayType === VIS.DisplayType.MultiKey;
        this.value = null;
        var _TableName = null;
        var _KeyColumnName = null;
        this.infoMultipleIds = null;
        var _value = null;
        this.log = VIS.Logging.VLogger.getVLogger("VTextBoxButton");
        // WindowNo for PrintCustomize */
        var WINDOW_INFO = 1113;

        // Tab for Info                */
        var TAB_INFO = 1113;

        var src = VIS.Application.contextUrl + "Areas/VIS/Images/base/";

        if (displayType == VIS.DisplayType.Location || displayType == VIS.DisplayType.MultiKey) {
            if (!this.isMultiKeyTextBox) {
                src += "Location10.png";
            }
            else {
                src += "PickOpen20.png";//delete10.png";
            }
            //txtText.IsReadOnly = true;
        }
        else if (displayType == VIS.DisplayType.Locator) {
            src += "Locator10.png";
        }
        else if (displayType == VIS.DisplayType.Search) {
            if (columnName.equals("C_BPartner_ID")
            || (columnName.equals("C_BPartner_To_ID") && lookup.getColumnName().equals("C_BPartner.C_BPartner_ID"))) {
                src += "BPartner20.png";
            }
            else if (columnName.equals("M_Product_ID")
            || (columnName.equals("M_Product_To_ID") && lookup.getColumnName().equals("M_Product.M_Product_ID"))) {
                src += "Product20.png";
            }
            else {
                src += "PickOpen20.png";
            }
        }

        var btnCount = 0;
        //create ui
        var $ctrl = $('<input >', { type: 'text', name: columnName });

        var $btnSearch = $('<button  tabindex="-1" class="vis-controls-txtbtn-table-td2"><img  tabindex="-1" src="' + src + '" /></button>');
        btnCount += 1;

        //Set Buttons and [pop up]

        var $btnPop = null;
        var $ulPopup = null;
        var $btnDelete = null;
        var options = {};
        var disabled = false;


        if (lookup != null && !this.isMultiKeyTextBox) {

            if (lookup instanceof VIS.MLookup) {
                if (lookup.getZoomWindow() == 0 && (zoomWindow_ID < 1)) {
                    disabled = true;
                }
            }

            //$btnZoom = VIS.AEnv.getZoomButton(disabled);
            // btnCount += 1;
            options[VIS.Actions.zoom] = disabled;

            $btnPop = $('<button  tabindex="-1" class="vis-controls-txtbtn-table-td2"><img tabindex="-1" src="' + VIS.Application.contextUrl + "Areas/VIS/Images/base/Info20.png" + '" /></button>');
            //	VBPartner quick entry link
            var isBP = false;
            if (columnName === "C_BPartner_ID") {
                options[VIS.Actions.add] = true;
                options[VIS.Actions.update] = true;
            }

            if ((this.lookup &&
                (this.lookup.info.keyColumn.toLowerCase() == "ad_user.ad_user_id"
                 || this.lookup.info.keyColumn.toLowerCase() == "ad_user_id"))
                 || columnName === "AD_User_ID" || columnName === "SalesRep_ID") {
                options[VIS.Actions.contact] = true;
            }

            if (VIS.MRole.getIsShowPreference())
                options[VIS.Actions.preference] = true;
            options[VIS.Actions.refresh] = true;
            options[VIS.Actions.remove] = true;

            $ulPopup = VIS.AEnv.getContextPopup(options);
            btnCount += 1;
            options = null;
        }

        if (this.isMultiKeyTextBox) {
            $btnPop = $('<button  tabindex="-1" class="vis-controls-txtbtn-table-td2"><img  tabindex="-1" src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/clear16.png' + '" /></button>');
            btnCount += 1;
        }

        var self = this;

        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory); //call base function


        this.setReadOnly = function (readOnly) {

            this.isReadOnly = readOnly;
            $ctrl.prop('disabled', readOnly ? true : false);
            // this.$super.setReadOnly(readonly);
            if (readOnly) {
                $btnSearch.css("opacity", .7);
            } else {
                $btnSearch.css("opacity", 1);
            }
        };


        $ctrl.on("keydown", function (event) {
            if (event.keyCode == 13 || event.keyCode == 9) {//will work on press of Tab key OR Enter Key
                if (self.actionText()) {
                    event.stopPropagation();
                    event.preventDefault();
                }
            }
        });


        /// <summary>
        ///Check, if data returns unique entry, otherwise involve Info via Button
        /// </summary>
        this.actionText = function () {
            var text = $ctrl.val().trim();
            ;
            VIS.context.setContext(self.lookup.windowNo, "AttrCode", text);
            //	Nothing entered
            if (text == null || text.length == 0 || text.equals("%")) {
                self.openSearchForm();
                return true;
            }
            text = text.toUpper();

            var id = -3;
            var keyId = null;
            var dr = null;


            var finalSql = VIS.Msg.parseTranslation(VIS.context, self.getDirectAccessSQL(text));
            try {
                dr = VIS.DB.executeReader(finalSql);
                if (dr.read()) {
                    try {
                        keyId = parseInt(dr.get(0));	//	first
                    }
                    catch (ex) {
                        keyId = dr.get(0);
                    }
                    id = 1;
                    if (dr.read()) {
                        id = -1;			//	only if unique
                        keyId = null;
                    }
                }
                dr.close();
                dr = null;
            }
            catch (ee) {
                if (dr != null)
                    dr.close();
                dr = null;
                this.log.log(VIS.Logging.Level.SEVERE, e);

                id = -2;
            }

            //	Try like
            if (id == -3 && !text.endsWith("%")) {
                text += "%";
                finalSql = VIS.Msg.parseTranslation(VIS.context, self.getDirectAccessSQL(text));
                try {
                    dr = VIS.DB.executeReader(finalSql);
                    if (dr.read()) {
                        //id = rs.getInt(1);		//	first
                        try {
                            keyId = parseInt(dr.get(0));	//	first
                        }
                        catch (es) {
                            keyId = dr.get(0);
                        }
                        id = 1;
                        if (dr.read()) {
                            keyId = null;
                            id = -1;			//	only if unique
                        }
                    }
                    dr.close();
                }
                catch (ewe) {
                    if (dr != null)
                        dr.close();
                    dr = null;
                    this.log.log(VIS.Logging.Level.SEVERE, e);

                    id = -2;
                }
            }


            //	No (unique) result
            if (id < 0 && keyId == null) {
                //if (id == -3)
                // this.log.log(VIS.Logging.Level.INFO, _columnName + " - Not Found - " + finalSql);
                //else
                //  this.log.log(VIS.Logging.Level.INFO, _columnName + " - Not Unique - " + finalSql);
                _value = {};	// force re-display
                self.openSearchForm();
                return true;
            }

            //if (this.oldValue == keyId) {
            //    return false;
            //}

            text = "";

            self.setValue(keyId, true, true);; //bind value and text
            return false;


        };


        /// <summary>
        /// Generate Access SQL for Search.
        /// The SQL returns the ID of the value entered
        /// Also sets _tableName and _keyColumnName
        /// </summary>
        /// <param name="text">uppercase text for LIKE comparison</param>
        /// <returns>sql or ""</returns>
        this.getDirectAccessSQL = function (text) {
            var result = self.getDirectAccessSQL1(VIS.context, columnName, lookup, text);
            _TableName = result[1];
            _KeyColumnName = result[2];
            return result[0];
        };


        /// <summary>
        /// Generate Access SQL for Search.
        /// The SQL returns the ID of the value entered
        /// Also sets _tableName and _keyColumnName
        /// </summary>
        /// <param name="ctx">Current Context</param>
        /// <param name="_columnName">Column Name</param>
        /// <param name="_lookup">Lookup Object</param>
        /// <param name="text">uppercase text for LIKE comparison</param>
        /// <returns>An array of 3 Strings; 0=SQL, 1=_tableName, 2=_keyColumnName</returns>
        this.getDirectAccessSQL1 = function (ctx, _columnName, _lookup, text) {
            var sql = "";
            var retVal = [];
            var _tableName = _columnName.substring(0, _columnName.length - 3);
            var _keyColumnName = _columnName;
            //
            if (_columnName.equals("M_Product_ID")) {
                //	Reset
                ctx.setContext(WINDOW_INFO, TAB_INFO, "M_Product_ID", "0");
                ctx.setContext(WINDOW_INFO, TAB_INFO, "M_AttributeSetInstance_ID", "0");
                ctx.setContext(WINDOW_INFO, TAB_INFO, "M_Locator_ID", "0");
                //
                sql += "SELECT DISTINCT p.M_Product_ID FROM M_Product p LEFT OUTER JOIN M_Manufacturer mr ON (p.M_Product_ID=mr.M_Product_ID)" +
                    " LEFT OUTER JOIN M_ProductAttributes patr ON (p.M_Product_ID=patr.M_Product_ID) WHERE (UPPER(p.Value) LIKE " + VIS.DB.to_string(text) +
                    " OR UPPER(p.Name) LIKE " + VIS.DB.to_string(text) + " OR mr.UPC LIKE " + VIS.DB.to_string(text) +
                    " OR patr.UPC LIKE " + VIS.DB.to_string(text) + " OR p.UPC LIKE " + VIS.DB.to_string(text) + ")";
            }
            else if (_columnName.equals("C_BPartner_ID")) {
                sql += "SELECT C_BPartner_ID FROM C_BPartner WHERE (UPPER(Value) LIKE ";
                sql += VIS.DB.to_string(text) + " OR UPPER(Name) LIKE " + VIS.DB.to_string(text) + ")";
            }
            else if (_columnName.equals("C_Order_ID")) {
                sql += "SELECT C_Order_ID FROM C_Order WHERE UPPER(DocumentNo) LIKE ";
                sql += VIS.DB.to_string(text);
            }
            else if (_columnName.equals("C_Invoice_ID")) {
                sql += "SELECT C_Invoice_ID FROM C_Invoice WHERE UPPER(DocumentNo) LIKE ";
                sql += VIS.DB.to_string(text);
            }
            else if (_columnName.equals("M_InOut_ID")) {
                sql += "SELECT M_InOut_ID FROM M_InOut WHERE UPPER(DocumentNo) LIKE ";
                sql += VIS.DB.to_string(text);
            }
            else if (_columnName.equals("C_Payment_ID")) {
                sql += "SELECT C_Payment_ID FROM C_Payment WHERE UPPER(DocumentNo) LIKE ";
                sql += VIS.DB.to_string(text);
            }
            else if (_columnName.equals("GL_JournalBatch_ID")) {
                sql += "SELECT GL_JournalBatch_ID FROM GL_JournalBatch WHERE UPPER(DocumentNo) LIKE ";
                sql += VIS.DB.to_string(text);
            }
            else if (_columnName.equals("SalesRep_ID")) {
                sql += "SELECT AD_User_ID FROM AD_User WHERE UPPER(Name) LIKE ";
                sql += VIS.DB.to_string(text);
                _tableName = "AD_User";
                _keyColumnName = "AD_User_ID";
            }
            //	Predefined
            if (sql.length > 0) {
                var wc = self.getWhereClause(ctx, _columnName, _lookup);
                if (_columnName.equals("M_Product_ID")) {
                    if (wc != null && wc.length > 0)
                        sql += " AND " + wc.replace(/M_Product\./g, "p.") + " AND p.IsActive='Y'";
                }
                else {
                    if (wc != null && wc.length > 0)
                        sql += " AND " + wc + " AND IsActive='Y'";
                }
                //	***
                // //log.finest(_columnName + " (predefined) " + sql.toString());

                retVal.push(VIS.MRole.getDefault().addAccessSQL(sql, _tableName, VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO));
                retVal.push(_tableName);
                retVal.push(_keyColumnName);
                return retVal;

            }

            //	Check if it is a Table Reference
            var dr = null;
            if (_lookup != null && _lookup instanceof VIS.MLookup) {
                var AD_Reference_ID = _lookup.getAD_Reference_Value_ID();
                if (AD_Reference_ID != 0) {
                    var query = "SELECT kc.ColumnName, dc.ColumnName, t.TableName "
                        + "FROM AD_Ref_Table rt"
                        + " INNER JOIN AD_Column kc ON (rt.Column_Key_ID=kc.AD_Column_ID)"
                        + " INNER JOIN AD_Column dc ON (rt.Column_Display_ID=dc.AD_Column_ID)"
                        + " INNER JOIN AD_Table t ON (rt.AD_Table_ID=t.AD_Table_ID) "
                        + "WHERE rt.AD_Reference_ID=@refid";
                    var displayColumnName = null;

                    try {
                        var param = [];
                        param[0] = new VIS.DB.SqlParam("@refid", AD_Reference_ID);
                        dr = VIS.DB.executeReader(query, param);
                        while (dr.read()) {
                            _keyColumnName = dr.get(0);
                            displayColumnName = dr.get(1);
                            _tableName = dr.get(2);
                        }
                        dr.close();
                        dr = null;

                    }
                    catch (e) {
                        if (dr != null) {
                            dr.close();
                            dr = null;
                        }
                        this.log.log(VIS.Logging.Level.SEVERE, query, e);
                    }

                    if (displayColumnName != null) {
                        sql = "";
                        sql += "SELECT " + _keyColumnName + " FROM " + _tableName + " WHERE UPPER(" + displayColumnName + ") LIKE ";
                        sql += VIS.DB.to_string(text) + " AND IsActive='Y'";
                        var wc = self.getWhereClause(ctx, _columnName, _lookup);
                        if (wc != null && wc.length > 0)
                            sql += " AND " + wc;
                        //	***
                        //log.finest(_columnName + " (Table) " + sql.toString());

                        retVal.push(VIS.MRole.getDefault().addAccessSQL(sql, _tableName, VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RO));
                        retVal.push(_tableName);
                        retVal.push(_keyColumnName);
                        return retVal;

                    }
                }	//	Table Reference
            }	//	MLookup

            /** Check Well Known Columns of Table - assumes TableDir	**/
            var squery = "SELECT t.TableName, c.ColumnName "
                + "FROM AD_Column c "
                + " INNER JOIN AD_Table t ON (c.AD_Table_ID=t.AD_Table_ID AND t.IsView='N') "
                + "WHERE (c.ColumnName IN ('DocumentNo', 'Value', 'Name') OR c.IsIdentifier='Y')"
                + " AND c.AD_Reference_ID IN (10,14)"
                + " AND EXISTS (SELECT * FROM AD_Column cc WHERE cc.AD_Table_ID=t.AD_Table_ID"
                    + " AND cc.IsKey='Y' AND cc.ColumnName=@colname)";
            _keyColumnName = _columnName;
            sql = "(";

            //IDataReader dr = null;
            try {
                var param = [];
                param[0] = new VIS.DB.SqlParam("@colname", _keyColumnName);
                dr = VIS.DB.executeReader(squery, param);

                while (dr.read()) {
                    if (sql.length > 1)
                        sql += " OR ";
                    _tableName = dr.get(0);
                    sql += "UPPER(" + dr.get(1) + ") LIKE " + VIS.DB.to_string(text);
                }
                sql += ")";
                dr.close();
                dr = null;

            }
            catch (e) {
                if (dr != null) {
                    dr.close();
                    dr = null;
                }
                this.log.log(VIS.Logging.Level.SEVERE, squery, e);
                //Logging.VLogger.Get().Log(Logging.Level.SEVERE, sql.ToString(), ex);
            }


            if (sql.length == 0) {
                this.log.log(VIS.Logging.Level.SEVERE, _columnName + " (TableDir) - no standard/identifier columns");
                //Logging.VLogger.Get().Log(Logging.Level.SEVERE, _columnName + " (TableDir) - no standard/identifier columns");
                retVal.push("");
                retVal.push(_tableName);
                retVal.push(_keyColumnName);
                return retVal;
                //return new String[] { "", _tableName, _keyColumnName };
            }
            //
            var retValue = "SELECT " + _columnName + " FROM " + _tableName + " WHERE " + sql + " AND IsActive='Y'";
            var _wc = self.getWhereClause(ctx, _columnName, _lookup);
            if (_wc != null && _wc.length > 0)
                retValue += " AND " + _wc;
            //	***
            ////log.finest(_columnName + " (TableDir) " + sql.toString());
            retVal.push(VIS.MRole.getDefault().addAccessSQL(retValue, _tableName, VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RO));
            retVal.push(_tableName);
            retVal.push(_keyColumnName);
            return retVal;

        };	//	getDirectAccessSQL





        /// <summary>
        /// Get Where Clause
        /// </summary>
        /// <param name="ctx">Current Context</param>
        /// <param name="_ColumnName">Column Name</param>
        /// <param name="_Lookup">Lookup reference</param>
        /// <returns>where clause or ""</returns>
        this.getWhereClause = function (ctx, _ColumnName, _Lookup) {
            //_Lookup = (MLookup)_Lookup;
            var WhereClause = "";
            try {

                if ((_Lookup) == null)
                    return "";
                if (_Lookup.getZoomQuery() != null)
                    WhereClause = _Lookup.getZoomQuery().getWhereClause();
                var validation = _Lookup.getValidation();
                if (validation == null)
                    validation = "";
                if (WhereClause.length == 0)
                    WhereClause = validation;
                else if (validation.length > 0)
                    WhereClause += " AND " + validation;
                //	//log.finest("ZoomQuery=" + (_lookup.getZoomQuery()==null ? "" : _lookup.getZoomQuery().getWhereClause())
                //		+ ", Validation=" + _lookup.getValidation());
                if (WhereClause.indexOf('@') != -1) {
                    var validated = VIS.Env.parseContext(ctx, _Lookup.getWindowNo(), WhereClause, false);
                    if (validated.length == 0) {
                        ////log.severe(_columnName + " - Cannot Parse=" + whereClause);
                    }
                    else {
                        ////log.fine(_columnName + " - Parsed: " + validated);
                        return validated;
                    }
                }
            }
            catch (eee) {
            }
            return WhereClause;
        };

        //	ReadWrite
        if (isReadOnly || !isUpdateable)
            this.setReadOnly(true);
        else
            this.setReadOnly(false);
        /* provilized function */

        /* @overridde
        */
        this.getBtnCount = function () {
            return btnCount;
        };

        /** 
            get contols button by index 
            -  zoom or info button 
            - index 0 for info button 
            - index 1 for zoom button
            - control must tell or implemnt getBtnCount default is zero [no button]
        *
        */
        this.getBtn = function (index) {
            if (index == 0) {
                return $btnSearch;
            }

            if (index == 1) { //zoom
                if ($btnPop)
                    return $btnPop;
            }
        };


        this.setVisible = function (visible) {
            this.visible = visible;
            if (visible) {
                //$ctrl.show();
                $ctrl.css("visibility", "visible");
                if ($btnPop)
                    $btnPop.show();
                $btnSearch.show();

            } else {
                //$ctrl.hide();
                $ctrl.css("visibility", "hidden");
                if ($btnPop)
                    $btnPop.hide();
                $btnSearch.hide();
            }
        };

        this.openSearchForm = function () {
            debugger;
            // e.stopPropagation();
            //alert("[pending] Info window for [" + self.value + "] => " + self.getName());
            var text = $ctrl.val().trim();
            if (typeof (text) == "object") {
                text = "";
            }
            if (self.isReadOnly)
                return;
            if (self.lookup == null)
                return;
            var infoWinID = 0;
            var wc = self.getWhereClause(VIS.Env.getCtx(), columnName, self.lookup);
            if (self.getField() != null) {
                infoWinID = self.getField().getAD_InfoWindow_ID();
            }
            var InfoWindow = null;

            if (infoWinID != 0) {
                InfoWindow = new VIS.InfoWindow(infoWinID, text, self.lookup.windowNo, wc, self.isMultiKeyTextBox);

            }
            else {
                var tableName = tableName;
                var _keyColumnName = columnName;
                var query = null;
                var dr = null;

                // Added by Bharat 
                var M_Warehouse_ID = 0, M_PriceList_ID = 0, window_ID = 0;

                //
                var AD_Reference_ID = self.lookup.getAD_Reference_Value_ID();
                if (AD_Reference_ID > 0) {
                    query = "SELECT kc.ColumnName, dc.ColumnName, t.TableName "
                        + "FROM AD_Ref_Table rt"
                        + " INNER JOIN AD_Column kc ON (rt.Column_Key_ID=kc.AD_Column_ID)"
                        + " INNER JOIN AD_Column dc ON (rt.Column_Display_ID=dc.AD_Column_ID)"
                        + " INNER JOIN AD_Table t ON (rt.AD_Table_ID=t.AD_Table_ID) "
                        + "WHERE rt.AD_Reference_ID=" + AD_Reference_ID;
                    var displayColumnName = null;

                    try {

                        dr = VIS.DB.executeDataReader(query);
                        while (dr.read()) {
                            _keyColumnName = dr.getString(0);//.Table.rows[0].cells['ColumnName']; [0].ToString();
                            displayColumnName = dr.getString(1); //dr[1].ToString();
                            tableName = dr.getString(2); //dr[2].ToString();
                        }
                        dr.close();
                        dr = null;

                    }
                    catch (e) {
                        if (dr != null) {
                            dr.close();
                            dr = null;
                        }
                        //Logging.VLogger.Get().Log(Logging.Level.SEVERE, query, e);
                    }
                }
                else {
                    tableName = String(columnName).substr(0, columnName.length - 3);
                    _keyColumnName = columnName;
                }

                // Added by Bharat    For Product Info
                if (_keyColumnName.equals("M_Product_ID")) {
                    query = "SELECT AD_Window_ID FROM AD_Window WHERE Name = '" + VIS.context.getContext(self.lookup.windowNo, "WindowName") + "'";
                    window_ID = VIS.DB.executeScalar(query);

                    if (window.DTD001 && window_ID == 170) {
                        M_Warehouse_ID = VIS.context.getContextAsInt(self.lookup.windowNo, "DTD001_MWarehouseSource_ID");
                    }
                    else {
                        M_Warehouse_ID = VIS.context.getContextAsInt(self.lookup.windowNo, "M_Warehouse_ID");
                    }
                    M_PriceList_ID = VIS.context.getContextAsInt(self.lookup.windowNo, "M_PriceList_ID");
                    var multipleSelection = false;
                    if (self.lookup.windowNo > 0) {
                        multipleSelection = (VIS.context.getWindowTabContext(self.lookup.windowNo, 1, "KeyColumnName") == "C_OrderLine_ID") ||
                                  (VIS.context.getWindowTabContext(self.lookup.windowNo, 1, "KeyColumnName") == "C_InvoiceLine_ID") ||
                                  (VIS.context.getWindowTabContext(self.lookup.windowNo, 1, "KeyColumnName") == "M_InOutLine_ID") ||
                                  (VIS.context.getWindowTabContext(self.lookup.windowNo, 1, "KeyColumnName") == "M_PackageLine_ID") ||
                                  (VIS.context.getWindowTabContext(self.lookup.windowNo, 1, "KeyColumnName") == "M_MovementLine_ID") ||
                                  (VIS.context.getWindowTabContext(self.lookup.windowNo, 1, "KeyColumnName") == "M_InventoryLine_ID") ||
                                  (VIS.context.getWindowTabContext(self.lookup.windowNo, 1, "KeyColumnName") == "M_ProductPrice_ID") //;
                        ;
                    }
                    InfoWindow = new VIS.infoProduct(true, self.lookup.windowNo, M_Warehouse_ID, M_PriceList_ID,
                        text, tableName, _keyColumnName, multipleSelection, wc);
                }
                else {
                    //try get dynamic window
                    query = "SELECT AD_InfoWindow_ID FROM AD_InfoWindow WHERE AD_Table_ID = (SELECT AD_Table_ID FROM AD_Table WHERE TableName='" + tableName + "') AND IsActive='Y'";
                    dr = VIS.DB.executeDataReader(query);
                    while (dr.read()) {
                        infoWinID = dr.getString(0);
                        break;
                    }
                    dr.close();
                    dr = null;
                    if (infoWinID > 0) {
                        InfoWindow = new VIS.InfoWindow(infoWinID, text, self.lookup.windowNo, wc, self.isMultiKeyTextBox);
                    }
                    else {
                        InfoWindow = new VIS.infoGeneral(true, self.lookup.windowNo, text,
                            tableName, _keyColumnName, self.isMultiKeyTextBox, wc);
                    }
                }
            }


            InfoWindow.onClose = function () {
                //self.setValue(InfoWindow.getSelectedValues(), false, true);
                debugger;
                var objResult = InfoWindow.getSelectedValues();

                if (self.isMultiKeyTextBox) {

                    var sb = "";
                    var i = 0;
                    var j = 0;
                    for (i = 0, j = objResult.length; i < j; i++) {
                        if (sb.length == 0) {
                            sb += objResult[i];
                            continue;
                        }
                        sb += "," + objResult[i];
                    }

                    self.setValue(sb, false, true);
                }
                else {

                    var newVal = null;
                    if (InfoWindow.getRefreshStatus && InfoWindow.getRefreshStatus()) {
                        self.fireRefreshUI();
                    }

                    else {

                        if (objResult != null && objResult.length == 1) {
                            newVal = objResult[0];
                        }

                        else {
                            //if ((_value.ToString() != objResult.ToString()))
                            InfoMultipleIds = null;

                            if (objResult.length > 1) {
                                newVal = objResult[0];
                            }
                        }
                        if (newVal != null) {
                            self.setValue(newVal, false, true);
                        }
                    }
                }
                InfoWindow = null;
            };
            InfoWindow.show();
        };

        $btnSearch.on(VIS.Events.onClick, self.openSearchForm);

        function zoomAction() {

            if (!self.lookup || disabled)
                return;
            //
            var zoomQuery = self.lookup.getZoomQuery();
            var value = self.getValue();
            if (value == null) {
                //   value = selectedItem;
            }

            if (value == "")
                value = null;

            //	If not already exist or exact value
            if ((zoomQuery == null || $.isEmptyObject(zoomQuery)) || (value != null)) {
                zoomQuery = new VIS.Query();	//	ColumnName might be changed in MTab.validateQuery

                var keyColumnName = null;
                //	Check if it is a Table Reference
                if ((self.lookup != null) && (self.lookup instanceof VIS.MLookup)) {
                    var AD_Reference_ID = self.lookup.getAD_Reference_Value_ID();
                    if (AD_Reference_ID != 0) {
                        var query = "SELECT kc.ColumnName"
                            + " FROM AD_Ref_Table rt"
                            + " INNER JOIN AD_Column kc ON (rt.Column_Key_ID=kc.AD_Column_ID)"
                            + "WHERE rt.AD_Reference_ID=" + AD_Reference_ID;

                        try {
                            var dr = VIS.DB.executeDataReader(query);
                            if (dr.read()) {
                                keyColumnName = dr.getString(0);
                            }
                            dr.dispose();
                        }
                        catch (e) {
                            this.log.log(VIS.Logging.Level.SEVERE, query, e);
                        }
                    }	//	Table Reference
                }	//	MLookup

                if ((keyColumnName != null) && (keyColumnName.length != 0))
                    zoomQuery.addRestriction(keyColumnName, VIS.Query.prototype.EQUAL, value);
                else
                    zoomQuery.addRestriction(self.getColumnName(), VIS.Query.prototype.EQUAL, value);
                zoomQuery.setRecordCount(1);	//	guess
            }

            var AD_Window_ID = 0;
            if (self.mField != null && self.mField.getZoomWindow_ID() > 0) {
                AD_Window_ID = self.mField.getZoomWindow_ID();
            }
            else {
                AD_Window_ID = self.lookup.getZoomWindow(zoomQuery);
            }



            //
            //this.log.info(this.getColumnName() + " - AD_Window_ID=" + AD_Window_ID
            //    + " - Query=" + zoomQuery + " - Value=" + value);
            //
            //setCursor(Cursor.getPredefinedCursor(Cursor.WAIT_CURSOR));
            //
            VIS.viewManager.startWindow(AD_Window_ID, zoomQuery);

            //setCursor(Cursor.getDefaultCursor());

        };


        if ($btnPop) {
            $btnPop.on(VIS.Events.onClick, function (e) {
                if (!self.isMultiKeyTextBox)
                    $btnPop.w2overlay($ulPopup.clone(true));
                else {
                    var val = self.getValue();
                    if (val && val.toString().length > 0) {
                        self.setValue(null, false, true);
                    }
                }
                e.stopPropagation();
            });
        }

        if ($ulPopup) {
            $ulPopup.on(VIS.Events.onTouchStartOrClick, "LI", function (e) {

                var action = $(e.target).data("action");
                if (action == VIS.Actions.zoom) {
                    if (disabled)
                        return;
                    zoomAction();
                }
                else if (action == VIS.Actions.preference) {
                    var obj = new VIS.ValuePreference(self.mField, self.getValue(), self.getDisplay());
                    if (obj != null) {
                        obj.showDialog();
                    }
                    obj = null;
                }
                else if (action == VIS.Actions.refresh) {

                    if (!self.lookup || self.isReadOnly)
                        return;
                    self.lookup.refresh();
                }
                else if (action == VIS.Actions.add) {
                    var val = self.getValue();
                    // if (val && val.toString().length > 0) {
                    VIS.AddUpdateBPartner(self.mField.getWindowNo(), 0, VIS.Msg.getMsg("Customer"), null, 0, 0);
                    //  var contactInfo = new VIS.ContactInfo(101, 1);
                    // contactInfo.show();
                    //}
                }
                else if (action == VIS.Actions.update) {
                    var val = self.getValue();
                    if (val && val.toString().length > 0) {
                        VIS.AddUpdateBPartner(self.mField.getWindowNo(), val, VIS.Msg.getMsg("Customer"), null, 0, 0);
                    }
                }
                else if (action == VIS.Actions.contact) {
                    var val = self.getValue();
                    if (val && val.toString().length > 0) {
                        //var contactInfo = new VIS.ContactInfo(val, this.mField.getWindowNo());
                        var contactInfo = new VIS.ContactInfo(val, self.mField.getWindowNo());
                        contactInfo.show();

                    }
                }
                else if (action == VIS.Actions.remove) {
                    if (self.isReadOnly)
                        return;
                    self.setValue(null, false, true);
                }
            });
        }

        /** 
        *  dispose 
        */
        this.disposeComponent = function () {
            $btnSearch.off(VIS.Events.onClick);

            if ($btnPop)
                $btnPop.off(VIS.Events.onClick);
            if ($ulPopup)
                $ulPopup.off(VIS.Events.onTouchStartOrClick);

            $ulPopup = null;
            self = null;
            $ctrl = null;
            $btnSearch = null;
            $btnPop = null;
            this.getBtn = null;
            this.setVisible = null;
        };
    };

    VIS.Utility.inheritPrototype(VTextBoxButton, IControl);//inherit IControl

    VTextBoxButton.prototype.setValue = function (newValue, refrsh, fire) {
        if (this.oldValue != newValue || refrsh) {
            this.settingValue = true;
            this.oldValue = newValue;
            this.value = newValue
            //	Set comboValue
            if (newValue == null) {
                this.lastDisplay = "";
                this.ctrl.val("");
                this.settingValue = false;
                if (fire) {
                    var evt = { newValue: newValue, propertyName: this.getName() };
                    this.fireValueChanged(evt);
                    evt = null;
                }
                return;
            }
            if (this.lookup == null) {
                this.ctrl.val(newValue.toString());
                this.lastDisplay = newValue.toString();
                this.settingValue = false;
                return;
            }

            if (this.displayType !== VIS.DisplayType.MultiKey)
                this.lastDisplay = this.lookup.getDisplay(newValue);
            else {
                var arr = newValue.toString().split(',');
                var sb = "";

                for (var i = 0, j = arr.length; i < j; i++) {
                    var val = arr[i];
                    if (!isNaN(val)) {
                        val = Number(val);
                    }
                    if (sb.length == 0) {
                        sb += this.lookup.getDisplay(val);
                        continue;
                    }
                    sb += "," + this.lookup.getDisplay(val);
                }
                this.lastDisplay = sb;
            }
            if (this.lastDisplay.equals("<-1>")) {
                this.lastDisplay = "";
                this.oldValue = null;
                this.value = null;
            }

            this.value = newValue;
            this.ctrl.val(this.lastDisplay);

            this.settingValue = false;
            //this.setBackground("white");
            if (fire) {
                if (newValue == -1 || newValue == "") {
                    newValue = null;
                }
                var evt = { newValue: newValue, propertyName: this.getName() };
                this.fireValueChanged(evt);
                evt = null;
            }
        }
    };

    VTextBoxButton.prototype.getValue = function () {
        return this.value;
    };

    VTextBoxButton.prototype.getMultipleIds = function () {
        return this.infoMultipleIds;
    };

    VTextBoxButton.prototype.getDisplay = function () {
        var retValue = "";
        if (this.lookup == null)
            retValue = this.value;
        else
            retValue = this.lookup.getDisplay(this.value);
        return retValue;
    };

    //7. 
    function VTextArea(columnName, isMandatory, isReadOnly, isUpdateable, displayLength, fieldLength, displayType) {

        var rows = 7;
        if (displayType != VIS.DisplayType.Memo) {
            if (displayType == VIS.DisplayType.TextLong) {
                rows = 7;
                fieldLength = 20000;
            }
            else {
                rows = fieldLength < 300 ? 3 : (fieldLength < 2000) ? 5 : 7;
            }
        }
        else {
            try {
                rows = fieldLength < 300 ? 3 : (fieldLength < 2000) ? 5 : (fieldLength / 500);
            }
            catch (e) {
                rows = fieldLength < 300 ? 3 : (fieldLength < 2000) ? 5 : 7;
            }
        }


        //Init Control
        var $ctrl = $('<textarea>', { name: columnName, maxlength: fieldLength, rows: rows });
        //Call base class
        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory);

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
            //this.Enabled = false;
        }
        else {
            this.setReadOnly(false);
        }

        var self = this; //self pointer

        /* Event */
        $ctrl.on("change", function (e) {
            e.stopPropagation();
            var newVal = $ctrl.val();
            if (newVal !== self.oldValue) {
                var evt = { newValue: newVal, propertyName: self.getName() };
                self.fireValueChanged(evt);
                evt = null;
            }
        });

        this.disposeComponent = function () {
            $ctrl.off("change"); //u bind event
            $ctrl = null;
            self = null;
        }
    };

    VIS.Utility.inheritPrototype(VTextArea, IControl);//inherit IControl
    /** 
    *  set value 
    *  @param new value to set
    */
    VTextArea.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;
            //console.log(newValue);
            this.ctrl.val(newValue);
            //this.setBackground("white");
        }
    };
    /** 
     *  get display text of control
     *  @return text of control
     */
    VTextArea.prototype.getDisplay = function () {
        return this.ctrl.val();
    };

    /*************HTML 5 Amount,Integer,Number Txtbox Block Code By raghu 06-05-2014****************************************/

    //8.
    /**
    * Create VAmountTextBox text box Control
    *  @param columnName column name
    *  @param mandatory mandatory
    *  @param isReadOnly read only
    *  @param isUpdateable updateable
    *  @param displayLength textbox lenght
    *  @param fieldLength column lenght
    *  @param title title
    */
    function VAmountTextBox(columnName, isMandatory, isReadOnly, isUpdateable, displayLength, fieldLength, controlDisplayType, title) {

        var displayType = controlDisplayType;
        var length = displayLength;
        //Init Control
        var $ctrl = $('<input>', { type: 'number', step: 'any', name: columnName, maxlength: length });
        //Call base class
        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory);
        //Set Fration,min,max value for control according to there dispay type
        this.format = VIS.DisplayType.GetNumberFormat(displayType);

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
        }
        else {
            this.setReadOnly(false);
        }

        var self = this; //self pointer

        //On key down event
        $ctrl.on("keydown", function (event) {
            if (event.keyCode == 189 || event.keyCode == 109 || event.keyCode == 173) { // dash (-)
                if (event.keyCode == 189 && this.value.length == 0) {
                    return true;
                }
                this.value = Number(this.value * -1);
                setTimeout(function () {
                    $ctrl.trigger("change");
                }, 100);
                return false;
            }

            if (event.shiftKey) {
                return false;
            }

            if ((event.keyCode >= 37 && event.keyCode <= 40) || // Left, Up, Right and Down        
        event.keyCode == 8 || // backspaceASKII
        event.keyCode == 9 || // tabASKII
        event.keyCode == 16 || // shift
        event.keyCode == 17 || // control
        event.keyCode == 35 || // End
        event.keyCode == 36 || // Home
        event.keyCode == 46) // deleteASKII
            {
                return true;
            }
            // 0-9 numbers (the numeric keys at the right of the keyboard)
            if ((event.keyCode >= 48 && event.keyCode <= 57 && event.shiftKey == false) || (event.keyCode >= 96 && event.keyCode <= 105 && event.shiftKey == false)) {
                if (this.value.length >= length) {
                    return false;
                }
                return true;
            }
            else if (event.keyCode == 189 || event.keyCode == 109 || event.keyCode == 173) { // dash (-)
                this.value = this.value * -1;
                return false;
            }
            if (event.keyCode == 190 || event.keyCode == 110) {// decimal (.)
                if (this.value.indexOf('.') > -1) {
                    this.value = this.value.replace('.', '');
                }
                if (this.value.length >= length) {
                    return false;
                }
                return true;
            }
            /* Check Only for . and , */
            if (event.keyCode == 188) {
                return false;
            }
            else {
                return false;
            }
        });

        //text change Event
        $ctrl.on("change", function (e) {
            e.stopPropagation();
            // var newVal = $ctrl.val();
            //alert(self.getValue());
            var newVal = self.getValue();
            this.value = newVal;
            if (newVal !== self.oldValue) {
                var evt = { newValue: newVal, propertyName: self.getName() };
                self.fireValueChanged(evt);
                evt = null;
            }
        });

        this.disposeComponent = function () {
            $ctrl.off("keydown"); //u bind event
            $ctrl.off("change"); //u bind event
            $ctrl = null;
            self = null;
            this.format.dispose();
            this.format = null;
            length = null;
        }
    };

    VIS.Utility.inheritPrototype(VAmountTextBox, IControl);//Inherit from IControl

    // set value   @param new value to set
    VAmountTextBox.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;
            newValue = this.format.GetFormatedValue(newValue);
            //console.log(newValue);
            this.ctrl.val(newValue);
            //this.setBackground("white");
        }
    };

    VAmountTextBox.prototype.getValue = function () {
        var val = this.$super.getValue.call(this);
        if (isNaN(val) || val === null) {
            return null;
        }
        return Number(val);
    };

    //get display text of control  @return text of control
    VAmountTextBox.prototype.getDisplay = function () {
        return this.ctrl.val();
    };

    /***END VAmountTextBox***/

    //9. 
    /**
    * Create VNumTextBox Allow only Integer values (-,+) decimal not allowed
    *  @param columnName column name
    *  @param mandatory mandatory
    *  @param isReadOnly read only
    *  @param isUpdateable updateable
    *  @param displayLength textbox lenght
    *  @param fieldLength column lenght
    *  @param title title
    */
    function VNumTextBox(columnName, isMandatory, isReadOnly, isUpdateable, displayLength, fieldLength, title) {

        var displayType = VIS.DisplayType.Integer;
        var length = displayLength;
        //Init Control
        var $ctrl = $('<input>', { type: 'text', name: columnName, maxlength: length });
        //Call base class
        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory);
        //Set Fration,min,max value for control according to there dispay type
        this.format = VIS.DisplayType.GetNumberFormat(displayType);

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
            //this.Enabled = false;
        }
        else {
            this.setReadOnly(false);
        }
        var self = this; //self pointer


        //On key down event
        $ctrl.on("keydown", function (event) {
            if (event.keyCode == 189 || event.keyCode == 109 || event.keyCode == 173) { // dash (-)
                if (event.keyCode == 189 && this.value.length == 0) {
                    return true;
                }
                this.value = Number(this.value * -1);
                setTimeout(function () {
                    $ctrl.trigger("change");
                }, 100);
                return false;
            }

            if (event.shiftKey) {
                return false;
            }

            if ((event.keyCode >= 37 && event.keyCode <= 40) || // Left, Up, Right and Down        
        event.keyCode == 8 || // backspaceASKII
        event.keyCode == 9 || // tabASKII
        event.keyCode == 16 || // shift
        event.keyCode == 17 || // control
        event.keyCode == 35 || // End
        event.keyCode == 36 || // Home
        event.keyCode == 46) // deleteASKII
            {
                return true;
            }
            if (this.value < 0) {
                if (this.value.length > VIS.DisplayType.INTEGER_DIGITS && event.keyCode != 8 && event.keyCode != 9 && event.keyCode != 46) {
                    return false;
                }
            }
            else {
                if (this.value.length >= VIS.DisplayType.INTEGER_DIGITS && event.keyCode != 8 && event.keyCode != 9 && event.keyCode != 46) {
                    return false;
                }
            }
            // 0-9 numbers (the numeric keys at the right of the keyboard)
            if ((event.keyCode >= 48 && event.keyCode <= 57 && event.shiftKey == false) || (event.keyCode >= 96 && event.keyCode <= 105 && event.shiftKey == false)) {
                if (this.value.length >= length) {
                    return false;
                }
                return true;
            }
            else {
                return false;
            }

        });

        // text change Event 
        $ctrl.on("change", function (e) {
            e.stopPropagation();
            //var newVal = $ctrl.val();
            var newVal = self.getValue();
            newVal = Globalize.parseInt(newVal.toString());
            //alert(newVal);
            var newFormatedVal = Number(self.format.GetFormatedValue(newVal));
            if (newVal != newFormatedVal) {
                self.mField.setValue(null);
                newVal = newFormatedVal;
            }

            if (newVal !== self.oldValue) {
                var evt = { newValue: newVal, propertyName: self.getName() };
                self.fireValueChanged(evt);
                evt = null;
            }
        });

        this.disposeComponent = function () {
            $ctrl.off("keydown"); //u bind event
            $ctrl.off("change"); //u bind event
            $ctrl = null;
            self = null;
            this.format.dispose();
            this.format = null;
            length = null;
        }
    };

    //Inherit from IControl
    VIS.Utility.inheritPrototype(VNumTextBox, IControl);

    // set value   @param new value to set
    VNumTextBox.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;
            //console.log(newValue);
            newValue = Number(this.format.GetFormatedValue(newValue));
            // newValue = Globalize.format(newValue, "n0");
            this.ctrl.val(newValue);
            //this.setBackground("white");
        }
    };

    VNumTextBox.prototype.getValue = function () {
        var val = this.$super.getValue.call(this);
        if (val === null) {
            return null;
        }
        //return Number(val);
        return val;
    };

    //get display text of control @return text of control
    VNumTextBox.prototype.getDisplay = function () {
        return this.ctrl.val();
    };

    /***END VNumTextBox***/



    //10. Location

    /**
     *	Create lookup for location field
     *  @param columnName column name
     *  @param mandatory mandatory
     *  @param isReadOnly read only
     *  @param isUpdateable updateable
     *  @param displayType display type
     *  @param title title
     */

    function VLocation(columnName, isMandatory, isReadOnly, isUpdateable, displayType, lookup) {
        if (!displayType) {
            displayType = VIS.DisplayType.Location;
        }

        this.lookup = lookup;
        this.lastDisplay = "";
        this.settingFocus = false;
        this.inserting = false;
        this.settingValue = false;

        this.value = null;

        //create ui
        var $ctrl = $('<input readonly>', { type: 'text', name: columnName });
        var $btnMap = $('<button class="vis-controls-txtbtn-table-td2"><img src="' + VIS.Application.contextUrl + "Areas/VIS/Images/base/ToLink20.png" + '" /></button>');
        var $btnLocation = $('<button class="vis-controls-txtbtn-table-td2"><img src="' + VIS.Application.contextUrl + "Areas/VIS/Images/base/Location20.png" + '" /></button>');
        var btnCount = 2;
        //$ctrl.append($btnMap).append($btnLocation);
        var self = this;
        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory); //call base function

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
        }
        else {

            this.setReadOnly(false);
        }

        this.getBtn = function (index) {
            if (index == 0) {
                return $btnMap;
            }
            if (index == 1) {
                return $btnLocation;
            }
        };

        this.getBtnCount = function () {
            return btnCount;
        };

        this.setVisible = function (visible) {
            this.visible = visible;
            if (visible) {
                $ctrl.show();
                $btnMap.show();
                $btnLocation.show();

            } else {
                $ctrl.hide();
                $btnMap.hide();
                $btnLocation.hide();
            }
        };

        $btnMap.on(VIS.Events.onClick, function (e) {
            var url = "http://local.google.com/maps?q=" + self.getDisplay();
            window.open(url);
            e.stopPropagation();
        });

        $btnLocation.on(VIS.Events.onClick, function (e) {
            e.stopPropagation();
            var obj = new VIS.LocationForm(self.value);
            obj.load();
            obj.showDialog();
            obj.onClose = function (location, change) {
                //if (self.oldValue != location)
                {
                    if (change) {
                        self.oldValue = 0;
                        self.lookup.refreshLocation(location);

                        self.setValue(location);
                        var evt = { newValue: location, propertyName: self.getColumnName() };
                        self.fireValueChanged(evt);
                        evt = null;
                    }
                }
            };
            obj = null;
            //alert("Map button [" + self.value + "] => " + self.getName());
        });

        //dispose 
        this.disposeComponent = function () {
            $btnMap.off(VIS.Events.onClick);
            $btnLocation.off(VIS.Events.onClick);
            self = null;
            $ctrl = null;
            $btnMap = null;
            $btnLocation = null;
            this.setVisible = null;
            //this.lookup = null;
            //this.lastDisplay = "";
            //this.settingFocus = false;
            //this.inserting = false;
            //this.settingValue = false;
        };
    };

    VIS.Utility.inheritPrototype(VLocation, IControl);//inherit IControl

    VLocation.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.settingValue = true;
            this.oldValue = newValue;
            this.value = newValue

            //	Set comboValue
            if (newValue == null) {
                this.lastDisplay = "";
                this.ctrl.val("");
                this.settingValue = false;
                return;
            }
            if (this.lookup == null) {
                this.ctrl.val(newValue.toString());
                this.lastDisplay = newValue.toString();
                this.settingValue = false;
                return;
            }

            this.lastDisplay = this.lookup.getDisplay(newValue);
            if (this.lastDisplay.equals("<-1>")) {
                this.lastDisplay = "";
                this.oldValue = null;
                this.value = null;
            }
            this.value = newValue;
            //this.ctrl.val(this.lastDisplay);
            this.ctrl.val(VIS.Utility.decodeText(this.lastDisplay));
            this.settingValue = true;

        }
    };

    VLocation.prototype.getValue = function () {
        return this.value;
    };

    VLocation.prototype.getDisplay = function () {
        var retValue = "";
        if (this.lookup == null)
            retValue = this.value;
        else
            retValue = this.lookup.getDisplay(this.value);
        return retValue;
    };

    //END

    //11. Locator

    /**
     *	Create lookup for locator field
     *  @param columnName column name
     *  @param mandatory mandatory
     *  @param isReadOnly read only
     *  @param isUpdateable updateable
     *  @param displayType display type
     *  @param title title
     */
    function VLocator(columnName, isMandatory, isReadOnly, isUpdateable, displayType, lookup) {

        this.mandatory = isMandatory;
        this.onlyWarehouseId = 0;
        this.onlyProductId = 0;
        this.onlyOutgoing = null;
        this.windowNum = lookup.getWindowNo();
        this.columnName = columnName;
        this.lookup = lookup;

        var src = VIS.Application.contextUrl + "Areas/VIS/Images/base/Locator20.png";
        this.value = null;

        if (!displayType)
            displayType = VIS.DisplayType.Locator;


        //create ui
        var $ctrl = $('<input readonly>', { type: 'text', name: columnName });
        var $btn = $('<button class="vis-controls-txtbtn-table-td2"><img src="' + src + '" /></button>');
        var $btnZoom = $('<button class="vis-controls-txtbtn-table-td2"><img src="' + VIS.Application.contextUrl + "Areas/VIS/Images/base/Zoom20.png" + '" /></button>');
        var btnCount = 2;

        var self = this;
        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory);
        // @overridde
        this.getBtnCount = function () {
            return btnCount;
        };

        // get contols button by index 
        this.getBtn = function (index) {
            if (index == 0) {
                return $btn;
            }
            if (index == 1) { //zoom
                return $btnZoom;
            }
        };
        //show visivility
        this.setVisible = function (visible) {
            this.visible = visible;
            if (visible) {
                $ctrl.show();
                $btn.show();
                $btnZoom.show();

            } else {

                $btn.hide();
                $btnZoom.hide();
                $ctrl.hide();
            }
        };

        $btn.on(VIS.Events.onClick, function (e) {
            e.stopPropagation();
            //	Warehouse/Product
            var warehouseId = self.getOnlyWarehouseID();
            var productId = self.getOnlyProductID();

            self.showLocatorForm(warehouseId, productId);

        });

        $btnZoom.on(VIS.Events.onClick, function (e) {
            e.stopPropagation();
            if (!self.lookup)
                return;
            //
            var zoomQuery = self.lookup.getZoomQuery();
            var value = self.getValue();
            if (value == null) {
                //   value = selectedItem;
            }
            if (value == "")
                value = null;
            //	If not already exist or exact value
            if ((zoomQuery == null) || (value != null)) {
                zoomQuery = new VIS.Query();	//	ColumnName might be changed in MTab.validateQuery

                var keyColumnName = null;
                //	Check if it is a Table Reference
                if ((self.lookup != null) && (self.lookup instanceof VIS.MLookup)) {
                    var AD_Reference_ID = self.lookup.getAD_Reference_Value_ID();
                    if (AD_Reference_ID != 0) {
                        var query = "SELECT kc.ColumnName"
                            + " FROM AD_Ref_Table rt"
                            + " INNER JOIN AD_Column kc ON (rt.Column_Key_ID=kc.AD_Column_ID)"
                            + "WHERE rt.AD_Reference_ID=" + AD_Reference_ID;

                        try {
                            var dr = VIS.DB.executeDataReader(query);
                            if (dr.read()) {
                                keyColumnName = dr.getString(0);
                            }
                            dr.dispose();
                        }
                        catch (e) {
                            this.log.log(VIS.Logging.Level.SEVERE, query, e);
                        }
                    }	//	Table Reference
                }	//	MLookup

                if ((keyColumnName != null) && (keyColumnName.length != 0))
                    zoomQuery.addRestriction(keyColumnName, VIS.Query.prototype.EQUAL, value);
                else
                    zoomQuery.addRestriction(self.getColumnName(), VIS.Query.prototype.EQUAL, value);
                zoomQuery.setRecordCount(1);	//	guess
            }

            var AD_Window_ID = 0;
            if (self.mField.getZoomWindow_ID() > 0) {
                AD_Window_ID = self.mField.getZoomWindow_ID();
            }
            else {
                AD_Window_ID = self.lookup.getZoomWindow(zoomQuery);
            }
            //
            //this.log.info(this.getColumnName() + " - AD_Window_ID=" + AD_Window_ID
            //    + " - Query=" + zoomQuery + " - Value=" + value);
            //
            //setCursor(Cursor.getPredefinedCursor(Cursor.WAIT_CURSOR));
            //
            VIS.viewManager.startWindow(AD_Window_ID, zoomQuery);
            //setCursor(Cursor.getDefaultCursor());

        });

        //dispose
        this.disposeComponent = function () {
            $btn.off(VIS.Events.onClick);
            $btnZoom.off(VIS.Events.onClick);
            self = null;
            $ctrl = null;
            $btn = null;
            this.getBtn = null;
            this.setVisible = null;
        };
    };

    VIS.Utility.inheritPrototype(VLocator, IControl);

    VLocator.prototype.getOnlyWarehouseID = function () {
        var ctx = VIS.Env.getCtx();
        // gwu: do not restrict locators by warehouse when in Import Inventory Transactions window 
        var AD_Table_ID = ctx.getContext(this.windowNum, "0|AD_Table_ID", true);
        // Import Inventory Transactions
        if ("572" == AD_Table_ID) {
            return 0;
        }
        var only_Warehouse = ctx.getContext(this.windowNum, "M_Warehouse_ID", true);
        var only_Warehouse_ID = 0;
        try {
            if (only_Warehouse != null && only_Warehouse.length > 0) {
                only_Warehouse_ID = Number(only_Warehouse);
            }
        }
        catch (ex) {
            // log.Log(Logging.Level.SEVERE, ex.Message);
        }
        return only_Warehouse_ID;
    };

    VLocator.prototype.getOnlyProductID = function () {

        var ctx = VIS.Env.getCtx();
        // gwu: do not restrict locators by product when in Import Inventory Transactions window 
        var AD_Table_ID = ctx.getContext(this.windowNum, "0|AD_Table_ID", true);
        // Import Inventory Transactions
        if ("572" == AD_Table_ID) {
            return 0;
        }

        var only_Product = ctx.getContext(this.windowNum, "M_Product_ID", true);
        var only_Product_ID = 0;
        try {
            if (only_Product != null && only_Product.length > 0) {
                only_Product_ID = Number(only_Product);
            }
        }
        catch (ex) {
            //log.Log(Logging.Level.SEVERE, ex.Message);
        }
        return only_Product_ID;
    };

    //Function which show form
    VLocator.prototype.showLocatorForm = function (warehouseId, productId) {
        var M_Locator_ID = 0;
        if (this.value != null) {
            M_Locator_ID = Number(this.value);
        }

        this.lookup.setOnlyWarehouseID(warehouseId);
        this.lookup.setOnlyProductID(productId);

        var isReturnTrx = "Y".equals(VIS.context.getWindowContext(this.windowNum, "IsReturnTrx"))
        var isSOTrx = VIS.Env.getCtx().isSOTrx(this.windowNum);

        var isOnlyOutgoing = ((isSOTrx && !isReturnTrx) || (!isSOTrx && isReturnTrx)) && this.columnName == "M_Locator_ID";
        this.lookup.setOnlyOutgoing(isOnlyOutgoing);
        this.lookup.refresh();

        //Open locator form
        self = this;
        var obj = new VIS.LocatorForm(this.columnName, this.lookup, M_Locator_ID, this.mandatory, warehouseId, this.windowNum);
        obj.load();
        obj.showDialog();
        obj.onClose = function (locator) {
            if (self.oldValue != locator) {
                self.setValue(locator);
                var evt = { newValue: locator, propertyName: self.columnName };
                self.fireValueChanged(evt);
                evt = null;
            }
        };
    };

    VLocator.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.settingValue = true;
            this.oldValue = newValue;
            this.value = newValue
            //	Set comboValue
            if (newValue == null) {
                this.lastDisplay = "";
                this.ctrl.val("");
                this.settingValue = false;
                return;
            }
            if (this.lookup == null) {
                this.ctrl.val(newValue.toString());
                this.lastDisplay = newValue.toString();
                this.settingValue = false;
                return;
            }

            this.lastDisplay = this.lookup.getDisplay(newValue);
            if (this.lastDisplay.equals("<-1>")) {
                this.lastDisplay = "";
                this.oldValue = null;
                this.value = null;
            }
            //console.log(newValue);

            this.value = newValue;
            this.ctrl.val(this.lastDisplay);

            this.settingValue = true;
            //this.setBackground("white");
        }
    };

    VLocator.prototype.getValue = function () {
        return this.value;
    };

    VLocator.prototype.getDisplay = function () {
        var retValue = "";
        if (this.lookup == null)
            retValue = this.value;
        else
            retValue = this.lookup.getDisplay(this.value);
        return retValue;
    };
    //END

    //pAttribute control
    function VPAttribute(columnName, isMandatory, isReadOnly, isUpdateable, displayType, lookup, windowNop, isActivityForm, search, fromDMS, pAttribute) {

        //Variable region
        /**	No Instance Key					*/
        this.NO_INSTANCE = 0;
        /**	Calling Window Info	*/
        this.AD_Column_ID = 0;
        var colName = "M_AttributeSetInstance_ID";
        var focus = false;

        //For genral attribute variable settings
        this.C_GenAttributeSet_ID = 0;
        this.C_GenAttributeSetInstance_ID = 0;
        this.M_Locator_ID = 0;
        colName = columnName;
        this.value = null;

        this.windowNo = windowNop;
        //set lookup into current object from pttribute/gattribute lookup
        this.lookup = lookup;

        this.C_BPartner_ID = VIS.Env.getCtx().getContextAsInt(this.windowNo, "C_BPartner_ID");


        /**	Logger			*/
        this.log = VIS.Logging.VLogger.getVLogger("VPAttribute");

        var src = VIS.Application.contextUrl + "Areas/VIS/Images/base/PAttribute20.png";
        if (!displayType) {
            displayType = VIS.DisplayType.PAttribute;
        }

        //Genral Attribute parameter

        this.isFromActivityForm = isActivityForm;
        this.isSearch = search;
        this.isFromDMS = fromDMS;
        this.isPAttribute = pAttribute;
        this.canSaveRecord = true;


        //create ui
        var $ctrl = $('<input readonly>', { type: 'text', name: columnName });
        var $btn = $('<button class="vis-controls-txtbtn-table-td2"><img src="' + src + '" /></button>');
        var btnCount = 1;

        var self = this;

        this.setInstanceIDs = null;

        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory);

        //read only control
        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
        }
        else {

            this.setReadOnly(false);
        }


        // @overridde
        this.getBtnCount = function () {
            return btnCount;
        };

        // get contols button by index 
        this.getBtn = function (index) {
            if (index == 0) {
                return $btn;
            }

        };
        //show visivility
        this.setVisible = function (visible) {
            this.visible = visible;
            if (visible) {
                $ctrl.show();
                $btn.show();

            } else {

                $btn.hide();
                $ctrl.hide();
            }
        };

        //Open Genral attribute Dialog form
        function OpenGeneralAttributeDialog(VADMS_AttributeSet_ID, oldValue) {
            var valueChange = false;
            var C_GenAttributeSetInstance_IDWin = VIS.Env.getCtx().getContextAsInt(self.windowNo, "C_GenAttributeSetInstance_ID");
            if (self.isFromActivityForm) {
                C_GenAttributeSetInstance_IDWin = self.C_GenAttributeSetInstance_ID;
            }

            if (C_GenAttributeSetInstance_IDWin == 0) {
                //txtAttribute.Text = string.Empty;
            }

            var obj = new VIS.GenralAttributeForm(C_GenAttributeSetInstance_IDWin, VADMS_AttributeSet_ID, self.windowNo, self.isSearch, self.getCanSaveRecord(), self.isFromDMS);
            obj.showDialog();
            obj.onClose = function (mGenAttributeSetInstanceId, name, instanceIDs) {
                VIS.Env.getCtx().setContext(windowNop, "C_GenAttributeSetInstance_ID", mGenAttributeSetInstanceId);
                setValueInControl(mGenAttributeSetInstanceId, name);
                if (instanceIDs != null)
                    self.setInstanceIDs = instanceIDs;
                self.getControl().val(name);
            };
        }

        function setValueInControl(mAttributeSetInstanceId, name) {
            self.log.finest("Changed M_AttributeSetInstance_ID=" + mAttributeSetInstanceId);
            if (self.oldValue != mAttributeSetInstanceId) {
                if (mAttributeSetInstanceId == 0) {
                    self.setValue(null);
                }
                self.setValue(mAttributeSetInstanceId);
                var evt = { newValue: mAttributeSetInstanceId, propertyName: self.colName };
                self.fireValueChanged(evt);
                evt = null;
            }
        }

        //Open PAttribute form
        function OpenPAttributeDialog(oldValue) {

            var M_AttributeSetInstance_ID = (oldValue == null) ? 0 : oldValue;
            var M_Product_ID = VIS.Env.getCtx().getContextAsInt(windowNop, "M_Product_ID");
            var M_ProductBOM_ID = VIS.Env.getCtx().getContextAsInt(windowNop, "M_ProductBOM_ID");
            var M_Locator_ID = VIS.Env.getCtx().getContextAsInt(windowNop, "M_Locator_ID");

            self.log.config("M_Product_ID=" + M_Product_ID + "/" + M_ProductBOM_ID + ",M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID + ", AD_Column_ID=" + self.AD_Column_ID);
            var productWindow = self.AD_Column_ID == 8418;		//	HARDCODED

            //	Exclude ability to enter ASI
            var exclude = true;
            var changed = false;

            if (M_Product_ID != 0) {
                //call controller of pAttributeForm to get is value should include or exclude
                $.ajax({
                    url: VIS.Application.contextUrl + "PAttributes/ExcludeEntry",
                    dataType: "json",
                    async: false,
                    data: {
                        productId: M_Product_ID,
                        adColumn: self.AD_Column_ID,
                        windowNo: windowNop
                    },
                    success: function (data) {
                        exclude = data.result;
                    }
                });
            }

            if (M_ProductBOM_ID != 0)	//	Use BOM Component
            {
                M_Product_ID = M_ProductBOM_ID;
            }

            if (!productWindow && (M_Product_ID == 0 || exclude)) {
                changed = true;
                M_AttributeSetInstance_ID = 0;
                setValueInControl(M_AttributeSetInstance_ID);
            }
            else {
                var obj = new VIS.PAttributesForm(M_AttributeSetInstance_ID, M_Product_ID, M_Locator_ID, self.C_BPartner_ID, productWindow, self.AD_Column_ID, windowNop);
                if (obj.hasAttribute) {
                    obj.showDialog();
                }
                obj.onClose = function (mAttributeSetInstanceId, name, mLocatorId) {
                    this.M_Locator_ID = mLocatorId;
                    setValueInControl(mAttributeSetInstanceId, name);
                };
            }
        }

        $btn.on(VIS.Events.onClick, function (e) {
            e.stopPropagation();
            var oldValue = self.getValue();
            //Genral Attribute Logic
            if (!self.isPAttribute) {
                var genAttributeSetId = 0;
                genAttributeSetId = VIS.Env.getCtx().getContextAsInt(self.windowNo, "C_GenAttributeSet_ID");

                if (genAttributeSetId == 0) {
                    genAttributeSetId = self.C_GenAttributeSet_ID;
                    if (genAttributeSetId == 0) {
                        VIS.ADialog.info("NoAttributeSet", null, null, null);
                        return;
                    }
                }
                OpenGeneralAttributeDialog(genAttributeSetId, oldValue);
                return;
            }
            //PAttribute logic for open form
            OpenPAttributeDialog(oldValue);
        });

        this.SetC_GenAttributeSet_ID = function (instanceID) {
            this.C_GenAttributeSet_ID = instanceID;
        }

        //dispose
        this.disposeComponent = function () {
            $btn.off(VIS.Events.onClick);
            self = null;
            $ctrl = null;
            $btn = null;
            this.getBtn = null;
            this.setVisible = null;
            this.value = null;
            this.lookup = null;
        }
    };

    VIS.Utility.inheritPrototype(VPAttribute, IControl);

    /**************************************************************************
      * Set/lookup Value
      * 
      * @param value
      *            value
      */
    VPAttribute.prototype.setValue = function (value) {
        if (value == null || 0 === value) {
            this.ctrl.text("");
            this.value = value;
        }
        // The same
        //if (value === this.value)
        //    return;
        this.value = value;
        this.ctrl.val(this.lookup.getDisplay(value)); // loads value
    }; // setValue

    VPAttribute.prototype.setField = function (mField) {
        if (mField != null) {
            this.windowNo = mField.getWindowNo();
            this.AD_Column_ID = mField.getAD_Column_ID();
        }
        this.mField = mField;
    }

    /**
     * Get Value
     * 
     * @return value
     */
    VPAttribute.prototype.getValue = function () {
        return this.value;
    }; // getValue

    VPAttribute.prototype.genSetInstanceIDs = function () {
        return this.setInstanceIDs;
    };


    /**
     * Get Display Value
     * 
     * @return info
     */
    VPAttribute.prototype.getDisplay = function () {
        return this.ctrl.val();
    }; // getDisplay

    VPAttribute.prototype.getCanSaveRecord = function () {
        return this.canSaveRecord;
    }; // getCanSaveRecord

    VPAttribute.prototype.setCanSaveRecord = function (value) {
        return this.canSaveRecord = value;
    };


    //End


    //Account
    function VAccount(columnName, isMandatory, isReadOnly, isUpdateable, displayType, lookup, windowNo, title) {

        this.value = null;
        this.windowNo = windowNo;
        this.lookup = lookup;
        this.title = title;
        var colName = columnName;
        var src = VIS.Application.contextUrl + "Areas/VIS/Images/base/Account20.png";
        if (!displayType)
            displayType = VIS.DisplayType.PAttribute;


        //create ui
        var $ctrl = $('<input readonly>', { type: 'text', name: columnName });
        var $btn = $('<button class="vis-controls-txtbtn-table-td2"><img src="' + src + '" /></button>');
        var btnCount = 1;

        var self = this;
        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, isMandatory);

        // @overridde
        this.getBtnCount = function () {
            return btnCount;
        };

        // get contols button by index 
        this.getBtn = function (index) {
            if (index == 0) {
                return $btn;
            }
        };
        //show visivility
        this.setVisible = function (visible) {
            this.visible = visible;
            if (visible) {
                $ctrl.show();
                $btn.show();

            } else {

                $ctrl.hide();
                $btn.hide();
            }
        };

        $btn.on(VIS.Events.onClick, function (e) {
            e.stopPropagation();
            var C_AcctSchema_ID = VIS.Env.getCtx().getContextAsInt(self.windowNo, "C_AcctSchema_ID");
            var obj = new VIS.AccountForm(self.title, self.lookup, C_AcctSchema_ID);
            obj.load();
            obj.showDialog();
            obj.onClose = function (location) {
                if (self.oldValue != location) {
                    self.setValue(location);
                    var evt = { newValue: location, propertyName: colName };
                    self.fireValueChanged(evt);
                    evt = null;
                }
            };
            obj = null;
        });


        //dispose
        this.disposeComponent = function () {
            $btn.off(VIS.Events.onClick);
            self = null;
            $ctrl = null;
            $btn = null;
            this.getBtn = null;
            this.setVisible = null;
            this.value = null;
            this.lookup = null;
            this.value = null;
            this.windowNo = null;
            this.title = null;

        }
    };
    VIS.Utility.inheritPrototype(VAccount, IControl);
    /*
      * Set/lookup Value
      * 
      * @param value
      *            value
      */
    VAccount.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;
            this.value = newValue
            this.ctrl.val(this.lookup.getDisplay(newValue));	//	loads value
        }
    }; // setValue
    /**
     * Get Value
     * 
     * @return value
     */
    VAccount.prototype.getValue = function () {
        return this.value;
    }; // getValue
    /**
     * Get Display Value
     * 
     * @return info
     */
    VAccount.prototype.getDisplay = function () {
        return this.ctrl.val();
    }; // getDisplay
    //End




    //VImage
    function VImage(colName, mandatoryField, isReadOnly, winNo) {
        this.values = null;
        this.log = VIS.Logging.VLogger.getVLogger("VImage");
        var windowNo = winNo;
        var columnName = colName;// "AD_Image_ID";
        var $img = $("<img style='width: 20px;height: 20px;'>");
        var $txt = $("<span>").text("-");
        var $ctrl = null;

        $ctrl = $('<button>', { type: 'button', name: columnName });
        $txt.css("color", "blue");



        $ctrl.append($img).append($txt);

        IControl.call(this, $ctrl, VIS.DisplayType.Button, isReadOnly, columnName, mandatoryField);

        if (isReadOnly) {
            this.setReadOnly(true);
            //this.Enabled = false;
        }
        else {
            this.setReadOnly(false);
        }

        var self = this; //self pointer

        $ctrl.on(VIS.Events.onClick, function (e) { //click handler
            e.stopPropagation();
            if (!self.isReadOnly) {
                //self.invokeActionPerformed({ source: self });
                var obj = new VIS.VImageForm(self.getValue(), $txt.text().trim().length);
                obj.showDialog();
                obj.onClose = function (ad_image_Id, change) {
                    //call set only when change call 
                    if (change) {
                        self.oldValue = 0;
                        if (self.oldValue != ad_image_Id) {
                            self.setValue(ad_image_Id);
                            var evt = { newValue: ad_image_Id, propertyName: columnName };
                            self.fireValueChanged(evt);
                            evt = null;
                        }
                    }
                };
                obj = null;
            }
        });

        this.setText = function (text) {
            if (text == null) {
                $txt.text("");
                $img.image.src = '';
                return;
            }
            var pos = text.indexOf('&');
            if (pos != -1)					//	We have a nemonic - creates ALT-_
            {
                var mnemonic = text.toUpperCase().charAt(pos + 1);
                if (mnemonic != ' ') {
                    //setMnemonic(mnemonic);
                    text = text.substring(0, pos) + text.substring(pos + 1);
                }
            }
            $txt.text(text);
        };

        this.setIcon = function (resImg) {
            //$img.attr('src', rootPath + img);
            if (resImg != null) {
                $img.attr('src', "data:image/jpg;base64," + resImg);
                $img.show();
                $txt.text("");
            }
            else {
                $img.attr('src', "data:image/jpg;base64," + resImg);
                $img.hide();
                $txt.text("-");
            }
        };

        this.disposeComponent = function () {
            $ctrl.off(VIS.Events.onClick);
            $ctrl = null;
            this.log = null;
            windowNo = null;
            $img = null;
            $txt = null;
            this.setText = null;
            this.setIcon = null;
            self = null;
        };
    };

    VIS.Utility.inheritPrototype(VImage, IControl);//Inherit

    VImage.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;
            this.value = newValue
            //	Set comboValue
            if (newValue == null) {
                this.setIcon(null);
                this.value = 0;
                return;
            }

            //  Get/Create Image byte array
            //var sql = "select * from AD_Image where AD_Image_ID=" + newValue;
            //var dr = VIS.DB.executeDataReader(sql.toString());
            //if (dr.read()) {
            //    var data = dr.getString("BINARYDATA");
            //    this.setIcon(data);
            //}

            //By Ajex request
            var localObj = this;
            $.ajax({
                url: VIS.Application.contextUrl + "VImageForm/GetImageAsByte",
                dataType: "json",
                data: {
                    ad_image_id: newValue
                },
                success: function (data) {
                    returnValue = data.result;
                    localObj.setIcon(returnValue);
                    localObj.ctrl.val(newValue);
                    localObj = null;
                }
            });
        }
    };

    VImage.prototype.setReadOnly = function (readOnly) {
        this.isReadOnly = readOnly;
        if (this.isLink) {
            this.ctrl.css('opacity', readOnly ? .6 : 1);
        }
        else {
            this.ctrl.prop('disabled', readOnly ? true : false);
        }
        this.setBackground(false);
    };

    VImage.prototype.getValue = function () {
        return this.value;
    };

    VImage.prototype.getDisplay = function () {
        return this.value;
    };

    //End VImage 

    //VBinary
    function VBinary(colName, mandatoryField, isReadOnly, isUpdateable, winNo) {

        this.values = null;
        var $txt = $("<span>").text("");

        var windowNo = winNo;
        var columnName = colName;// "AD_Image_ID";

        var $ctrl = null;
        var $ulPopup = null;
        var SaveTolocalFile = 'SaveTolocalFile';
        var LoadIntoDatabase = 'LoadIntoDatabase';

        this.data = null;
        var inputCtrl = $("<input type='file' class='file' name='file'/>");

        $ctrl = $('<button>', { type: 'button', name: columnName });
        $ctrl.append($txt);

        IControl.call(this, $ctrl, VIS.DisplayType.Button, isReadOnly, columnName, mandatoryField);

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
        }
        else {
            this.setReadOnly(false);
        }

        var self = this; //self pointer

        function getPopupList() {
            var ullst = $("<ul class='vis-apanel-rb-ul'>");
            ullst.append($("<a id='link' style='border-bottom: 1px solid #CCDADE;color: #535353;font-size: 12px;display: block;margin-bottom: 8px;'  href='javascript:void(0)'>" + VIS.Msg.getMsg("SaveTolocalFile") + "</a>"));
            // ullst.append($("<li data-action='" + SaveTolocalFile + "'>").text(VIS.Msg.getMsg("SaveTolocalFile")));
            ullst.append($("<li data-action='" + LoadIntoDatabase + "'>").text(VIS.Msg.getMsg("Open/LoadIntoDatabase")));
            return ullst;
        };

        $ulPopup = getPopupList();

        if ($ulPopup) {
            $ulPopup.on(VIS.Events.onTouchStartOrClick, "LI", function (e) {

                var action = $(e.target).data("action");
                if (action == SaveTolocalFile) {
                    // onDownload(self.getValue());
                    //var d = new Date().toISOString().slice(0, 19).replace(/-/g, "");
                    //$("#link").attr("href", self.getValue()).attr("download", "file-" + d + ".log");
                }
                else if (action == LoadIntoDatabase) {
                    inputCtrl.trigger('click');
                }
            });

            $ulPopup.on(VIS.Events.onTouchStartOrClick, "a", function (e) {

                if (self.getValue() != null) {
                    var d = new Date().toISOString().slice(0, 19).replace(/-/g, "");
                    var fileData = "data:;base64," + self.getValue();
                    $(this).attr("href", fileData).attr("download", "file-" + d + ".log");
                }
            });
        }

        //Upload File on client side and get byte array from file on client side
        inputCtrl.change(function () {
            var file = inputCtrl[0].files[0];
            var reader = new FileReader();
            var ary = reader.readAsDataURL(file);

            reader.onloadend = function () {
                var base64data = (reader.result).split(',')[1];
                if (self.oldValue != base64data) {
                    self.setValue(base64data);
                    var evt = { newValue: base64data, propertyName: columnName };
                    self.fireValueChanged(evt);
                    evt = null;
                }
            }
            return;

            //get byte array from server
            //var xhr = new XMLHttpRequest();
            //var fd = new FormData();
            //fd.append("file", file);
            //xhr.open("POST", VIS.Application.contextUrl + "VImageForm/GetFileByteArray", true);
            //xhr.send(fd);
            //xhr.addEventListener("load", function (event) {
            //    alert("response");
            //    var dd = event.target.response;
            //    dd = JSON.parse(dd);
            //    var newByt = dd.result;

            //    if (self.oldValue != newByt) {
            //        self.setValue(newByt);
            //        var evt = { newValue: newByt, propertyName: columnName };
            //        self.fireValueChanged(evt);
            //        evt = null;
            //    }
            //}, false);
        });

        $ctrl.on(VIS.Events.onClick, function (evt) { //click handler
            if (!self.isReadOnly) {
                $ctrl.w2overlay($ulPopup.clone(true));
            }
            evt.stopPropagation();
        });

        this.setText = function (text) {
            if (text == null) {
                $txt.text("");
                return;
            }
            var pos = text.indexOf('&');
            if (pos != -1)					//	We have a nemonic - creates ALT-_
            {
                var mnemonic = text.toUpperCase().charAt(pos + 1);
                if (mnemonic != ' ') {
                    //setMnemonic(mnemonic);
                    text = text.substring(0, pos) + text.substring(pos + 1);
                }
            }
            $txt.text(text);

        };

        this.disposeComponent = function () {
            $ctrl.off(VIS.Events.onClick);
            $ctrl = null;
            this.setText = null;
            this.values = null;
            $txt = null;
            windowNo = null;
            columnName = null;
            $ulPopup = null;
            SaveTolocalFile = null;
            LoadIntoDatabase = null;
            this.data = null;
            inputCtrl = null;
            self = null;
        };
    };

    VIS.Utility.inheritPrototype(VBinary, IControl);//Inherit

    VBinary.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;

            if (newValue == null) {
                this.setText("-");
                this.value = newValue;
                this.ctrl.val(newValue);
            }
            else {
                var text = "?";
                if (newValue.length > 0) {
                    var bb = newValue;
                    text = "#" + bb.length;
                }
                else {
                    text = newValue.GetType().FullName;
                }
                //	Display it
                this.setText(text != null ? text : "");
                this.value = newValue;
                this.ctrl.val(newValue);
            }
        }
    };

    VBinary.prototype.setReadOnly = function (readOnly) {
        this.isReadOnly = readOnly;
        this.ctrl.prop('disabled', readOnly ? true : false);
        this.setBackground(false);
    };

    VBinary.prototype.getValue = function () {
        return this.value;
    };

    VBinary.prototype.getDisplay = function () {
        return this.value;
    };

    //End VBinary 

    //VURL
    function VURL(columnName, isMandatory, isReadOnly, isUpdateable, displayLength, fieldLength) {
        this.value = null;
        var src = VIS.Application.contextUrl + "Areas/VIS/Images/base/Url20.png";
        var btnCount = 0;
        //create ui
        var $ctrl = $('<input>', { type: 'text', name: columnName, maxlength: fieldLength });
        var $btnSearch = $('<button class="vis-controls-txtbtn-table-td2"><img src="' + src + '" /></button>');
        btnCount += 1;

        //Set Buttons and [pop up]
        var self = this;
        IControl.call(this, $ctrl, VIS.DisplayType.URL, isReadOnly, columnName, isMandatory);

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
        }
        else {
            this.setReadOnly(false);
        }

        //provilized function
        this.getBtnCount = function () {
            return btnCount;
        };

        //Get url Button
        this.getBtn = function (index) {
            if (index == 0) {
                return $btnSearch;
            }
        };

        this.setVisible = function (visible) {
            this.visible = visible;
            if (visible) {
                $ctrl.show();
                $btnSearch.show();

            }
            else {
                $ctrl.hide();
                $btnSearch.hide();
            }
        };

        $btnSearch.on(VIS.Events.onClick, function (e) {
            e.stopPropagation();
            if (self.value == null) {
                return;
            }
            var urlString = self.value.trim();
            if (urlString.length > 0) {
                if (urlString.contains("http://") || urlString.contains("https://")) {
                    ;
                }
                else {
                    urlString = "http://" + urlString;
                }

                window.open(urlString);
                return;
            }
            VIS.ADialog.warn("URLnotValid", true, null);
        });

        /* Event */
        $ctrl.on("change", function (e) {
            e.stopPropagation();
            var newVal = $ctrl.val();
            if (newVal !== self.oldValue) {
                var evt = { newValue: newVal, propertyName: self.getName() };
                self.fireValueChanged(evt);
                evt = null;
            }
        });



        //  dispose 
        this.disposeComponent = function () {
            $btnSearch.off(VIS.Events.onClick);
            $ctrl.off("change");
            self = null;
            $ctrl = null;
            $btnSearch = null;
            this.getBtn = null;
            this.setVisible = null;
            this.value = null;
            src = null;
            btnCount = null;
            this.getBtnCount = null;
            this.getBtn = null;
        };
    };

    VIS.Utility.inheritPrototype(VURL, IControl);//inherit IControl

    VURL.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;
            this.value = newValue
            //	Set comboValue
            if (newValue == null) {
                this.lastDisplay = "";
                this.ctrl.val("");
                return;
            }

            this.value = newValue;
            this.ctrl.val(newValue);
            this.ctrl.val(newValue);

        }
    };

    VURL.prototype.getValue = function () {
        return this.value;
    };

    VURL.prototype.getDisplay = function () {
        return this.ctrl.val();
    };


    //File

    function VFile(colName, mandatoryField, isReadOnly, isUpdateable, winNo, displayType) {

        var files = false;
        if (displayType == displayType.FileName) {
            files = true;
        }

        DialogType = {
            OpenFile: 0,
            SaveFile: 1,
            Custom: 2
        };

        SelectionType = {
            FilesOnly: 0,
            DirectoryOnly: 1
        };
        //Selection Mode					
        var selectionMode = SelectionType.DirectoryOnly;
        //Save/Open						
        var dialogType = DialogType.Custom;
        //Logger
        this.log = VIS.Logging.VLogger.getVLogger("VFile");

        var displayType = VIS.DisplayType.FileName;

        var src = VIS.Application.contextUrl + "Areas/VIS/Images/base/Folder20.png";
        if (files) {
            selectionMode = SelectionType.FilesOnly;
            src = VIS.Application.contextUrl + "Areas/VIS/Images/base/File20.png";
        }
        var col = colName.toUpperCase();

        if (col.indexOf("open") != -1 || col.indexOf("load") != -1) {
            dialogType = DialogType.OpenFile;
        }
        else if (col.indexOf("save") != -1) {
            dialogType = DialogType.SaveFile;
        }



        var windowNo = winNo;
        var columnName = colName;
        this.value = null;
        var btnCount = 0;

        var $ctrl = $('<input>', { type: 'text', name: columnName });
        var $btnSearch = $('<button class="vis-controls-txtbtn-table-td2"><img src="' + src + '" /></button>');
        btnCount += 1;

        var inputCtrl = $("<input type='file' class='file' name='file'/>");
        $ctrl.append($btnSearch);

        IControl.call(this, $ctrl, displayType, isReadOnly, columnName, mandatoryField);

        if (isReadOnly || !isUpdateable) {
            this.setReadOnly(true);
        }
        else {
            this.setReadOnly(false);
        }

        //provilized function
        this.getBtnCount = function () {
            return btnCount;
        };

        //Get url Button
        this.getBtn = function (index) {
            if (index == 0) {
                return $btnSearch;
            }
        };

        this.setVisible = function (visible) {
            this.visible = visible;
            if (visible) {
                $ctrl.show();
                $btnSearch.show();

            }
            else {
                $ctrl.hide();
                $btnSearch.hide();
            }
        };

        var self = this; //self pointer

        $btnSearch.on(VIS.Events.onClick, function (evt) {
            evt.stopPropagation();
            if (selectionMode == SelectionType.DirectoryOnly) {
                inputCtrl.trigger('click');
            }
            else {
                if (dialogType == DialogType.SaveFile) {
                    if (self.getValue() != null) {
                        var d = new Date().toISOString().slice(0, 19).replace(/-/g, "");
                        var fileData = "data:;base64," + self.getValue();
                        $(this).attr("href", fileData).attr("download", "file-" + d + ".log");
                    }
                }
                else {
                    inputCtrl.trigger('click');
                }
            }
        });

        //Upload File on client side and get byte array from file on client side
        inputCtrl.change(function (e) {
            e.stopPropagation();
            var file = $(inputCtrl).val().split('\\').pop();
            if (self.oldValue != file) {
                self.setValue(file);
                var evt = { newValue: file, propertyName: columnName };
                self.fireValueChanged(evt);
                evt = null;
            }
        });

        this.disposeComponent = function () {
            $ctrl.off(VIS.Events.onClick);
            $ctrl = null;
            windowNo = null;
            columnName = null;
            inputCtrl = null;
            self = null;
        };
    };

    VIS.Utility.inheritPrototype(VFile, IControl);//Inherit

    VFile.prototype.setValue = function (newValue) {
        if (this.oldValue != newValue) {
            this.oldValue = newValue;
            this.value = newValue;
            this.ctrl.val(newValue);
        }
    };

    VFile.prototype.setReadOnly = function (readOnly) {
        this.isReadOnly = readOnly;
        this.ctrl.prop('disabled', readOnly ? true : false);
        this.setBackground(false);
    };

    VFile.prototype.getValue = function () {
        return this.value;
    };

    VFile.prototype.getDisplay = function () {
        return this.value;
    };

    //End File

    //VLabel




    //To implement culture change
    //1.Control type number to textbox:number text not comma in un english culture
    //2.implement formate in setValue Globalize.format(newValue, "n0");
    //3.Change event part formated value Globalize.parseInt(newVal.toString());

    /* NameSpace */
    VIS.Controls.IControl = IControl;
    VIS.Controls.VTextBox = VTextBox;
    VIS.Controls.VLabel = VLabel;
    VIS.Controls.VButton = VButton;
    VIS.Controls.VCheckBox = VCheckBox;
    VIS.Controls.VComboBox = VComboBox;
    VIS.Controls.VTextBoxButton = VTextBoxButton;
    VIS.Controls.VTextArea = VTextArea;
    VIS.Controls.VAmountTextBox = VAmountTextBox;
    VIS.Controls.VNumTextBox = VNumTextBox;
    VIS.Controls.VLocation = VLocation;
    VIS.Controls.VDate = VDate;
    VIS.Controls.VLocator = VLocator;
    VIS.Controls.VAccount = VAccount;
    VIS.Controls.VPAttribute = VPAttribute;
    VIS.Controls.VImage = VImage;
    VIS.Controls.VBinary = VBinary;
    VIS.Controls.VFile = VFile;
    /* END */
}(jQuery, VIS));


