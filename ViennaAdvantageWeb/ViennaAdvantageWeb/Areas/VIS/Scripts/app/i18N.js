/**
 *	get cuture specific  Messages 
 *
 */

; VIS.Msg = {};

/**
 *	Get translated text for AD_Message
 *  @param	key - Message Key
 *  @return translated text
 */
VIS.I18N.getLabel = function (key, params, object, property) {
    var label, i;

    if (!VIS.I18N.labels[key]) {
        if (object && property) {
            VIS.I18N.getLabelFromServer(key, params, object, property);
        }
        return '[' + key + ']';
    }

    label = VIS.I18N.labels[key];

    if (params && params.length && params.length > 0) {
        for (var i = 0; i < params.length; i++) {
            label = label.replace("%" + i, params[i]);
        }
    }
    if (object && property) {
        if (Object.prototype.toString.call(object[property]) === '[object Function]') {
            object[property](label);
        } else {
            object[property] = label;
        }
    }
    return label;
};

VIS.I18N.getLabelFromServer = function (key, params, object, property) {

    // if (!z) {
    return 'UNDEFINED ' + key;
    //}

    //var clientContext = response.clientContext;
    //if (data.label) {
    //    VIS.I18N.labels[clientContext.key] = data.label;
    //    VIS.I18N.getLabel(clientContext.key, clientContext.params, clientContext.object, clientContext.property);
    //} else {
    //    if (isc.isA.Function(clientContext.object[clientContext.property])) {
    //        clientContext.object[clientContext.property]('LABEL NOT FOUND ' + clientContext.key);
    //    } else {
    //        clientContext.object[clientContext.property] = 'LABEL NOT FOUND ' + clientContext.key;
    //    }
    //}
};

/**
 *	Get translated text for AD_Message
 *  @param	key - Message Key
 *  @return translated text
 */
VIS.Msg.getMsg = function (key, msgPlusToolTip, onlyToolTip) {

    if (!VIS.I18N.labels[key]) {
        return '[' + key + ']';
    }
    label = VIS.Utility.Util.cleanMnemonic(VIS.I18N.labels[key]); //Edited by sarab

    var lbs = label.split('      ');
    if (!msgPlusToolTip) {
        label = lbs[0];
    }
    if (onlyToolTip && lbs.length > 1) {
        label = lbs[1];
    }
    return label;
};

/**
 *	Translate elements enclosed in "@" (at sign)
 *  @param ctx      Context
 *  @param text     Text
 *  @return translated text or original text if not found
 */
VIS.Msg.parseTranslation = function (ctx, text) {
    if (text == null || text.length == 0)
        return text;

    var inStr = text;
    var token;
    var outStr = "";

    var i = inStr.indexOf("@");
    while (i != -1)
    {
        outStr += inStr.substring(0, i);			// up to @
        inStr = inStr.substring(i+1, inStr.length);	// from first @

        var j = inStr.indexOf("@");						// next @
        if (j < 0)										// no second tag
        {
            inStr = "@" + inStr;
            break;
        }

        token = inStr.substring(0, j);
        outStr += this.translate(ctx, token);			// replace context

        inStr = inStr.substring(j+1, inStr.length);	// from second @
        i = inStr.indexOf("@");
    }

    outStr += inStr;           					//	add remainder
    return outStr;
};

VIS.Msg.translate = function (ctx, text) {

    if (text == null || text.length == 0) {
        return text;
    }

    //var s = VIS.Utility.Util.cleanMnemonic(VIS.I18N.labels[text]); //(String)ctx.Get(text);
    //if (s != null && s.length > 0) {
    //    return s;
    //}

    return VIS.Msg.translate3(VIS.Env.getAD_Language(ctx), ctx.isSOTrx(), text);
};

VIS.Msg.getElement = function (ctx, text) {
    return VIS.Msg.getElement3(VIS.Env.getAD_Language(ctx), text, true);
};

VIS.Msg.translate3 = function (ad_language, isSOTrx, text) {

    if (text == null || text.equals(""))
        return "";
    var AD_Language = ad_language;
    if (AD_Language == null || AD_Language.length == 0)
        AD_Language = VIS.Env.getBaseAD_Language();

    //	Check AD_Element
    var retStr = VIS.Msg.getElement3(AD_Language, text, isSOTrx);
    if (!retStr.equals(""))
        return retStr.trim();

    ////	Check AD_Message
    //if (DatabaseType.IsMSSql)
    //{
    //    if (text.Equals("Date"))
    //        text = "DATETIME";
    //}
    //retStr = Get().Lookup(AD_Language, text);
    //if (retStr != null)
    //    return retStr;
    return text;

};

VIS.Msg.getElement3 = function (ad_language, ColumnName, isSOTrx) {
    if (ColumnName == null || ColumnName.equals(""))
        return "";
    var AD_Language = ad_language;
    if (AD_Language == null || AD_Language.length == 0)
        AD_Language = VIS.Env.getBaseAD_Language();

    //	Check AD_Element
    var retStr = "";
    var sqlQry = "";
    var dr = null;
    try {
        if (AD_Language == null || AD_Language.length == 0 || VIS.Env.isBaseLanguage(AD_Language, "AD_Element")) {
            sqlQry = "SELECT Name, PO_Name FROM AD_Element WHERE UPPER(ColumnName)=@ColumnName";
            var param = [];
            param[0] = new VIS.DB.SqlParam("@ColumnName", ColumnName.toUpper());
            dr = VIS.DB.executeReader(sqlQry, param);
        }
        else {
            sqlQry = "SELECT t.Name, t.PO_Name FROM AD_Element_Trl t, AD_Element e "
                + "WHERE t.AD_Element_ID=e.AD_Element_ID AND UPPER(e.ColumnName)=@ColumnName "
                + "AND t.AD_Language=@AD_Language";
            var param = [];
            param[0] = new VIS.DB.SqlParam("@ColumnName", ColumnName.toUpper());
            param[1] = new VIS.DB.SqlParam("@AD_Language", AD_Language);
            dr = VIS.DB.executeReader(sqlQry, param);
        }

        if (dr.read()) {
            retStr = dr.getString(0);
            if (!isSOTrx) {
                var temp = dr.getString(1);
                if (temp != null && temp.length > 0)
                    retStr = temp;
            }
        }
        dr.close();
    }
    catch (e) {
        dr.close();
    }
    if (retStr != null)
        return retStr.trim();
    return retStr;
};


