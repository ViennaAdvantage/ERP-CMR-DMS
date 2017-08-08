; (function (VIS, $) {

    /**
     *  Info Dialog Management
     *  
     */

    VIS.ADialog = {

        /**
         *  Show PAIN message with info 
         *  @method info
         *  @param keyName Keyword Name
         *  @param isTextMsg if true returns Message Text, if false returns Message Tip
         *     and if null then returns both message text and tip.
         *  @param extraMsg extra message to be displayed
         */
        info: function (keyName, isMsgText, extraMsg, header) {

            var content = "";
            // if user has given a key
            if (keyName != null && !keyName.equals("")) {
                // get key's value
                content += VIS.Msg.getMsg(keyName);
            }
            // if user has given any extra content
            if (extraMsg != null && extraMsg.length > 0) {
                // add the content
                content += "\n" + extraMsg;
            }
            alert(content);
            content = null;
        },

        /**
       *	Ask Question with question icon and (OK) (Cancel) buttons
       *    @method ask
       *	@param	keyName	Message to be translated
       *	@param	msg			Additional clear text message
       *	@return true, if OK
       */

        ask: function (keyName, isMsgText, extraMsg, header) {

            var content = "";
            // if user has given a key
            if (keyName != null && !keyName.equals("")) {
                // get key's value
                content += VIS.Msg.getMsg(keyName);
            }
            // if user has given any extra content
            if (extraMsg != null && extraMsg.length > 0) {
                // add the content
                content += "\n" + extraMsg;
            }
            var retValue = false;
            // opens message window
            //Message d = new Message(header, content.ToString(), Message.MessageType.QUESTION);
            if (confirm(content))// d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // if user clicks on OK button change the value
                retValue = true;
            }
            return retValue;
        },


        /**
	     *	Display error with error icon
         *  @method error
	     *	@param	keyName	Message to be translated
         *  @param  isMsgText 
	     *	@param	extraMsg			Additional message
	     */
        error: function (keyName, isMsgText, extraMsg) {
            var content = "";
            // if user has given a key
            if (keyName != null && !keyName.equals("")) {
                // get key's value
                content += VIS.Msg.getMsg(keyName);
            }
            // if user has given any extra content
            if (extraMsg != null && extraMsg.length > 0) {
                // add the content
                content += "\n" + extraMsg;
            }
            // if user has given statusbar label, then show the messsage on status bar also
            alert(content);

            content = null;
        },

        /**
	     *	Display warning with warning icon
         *  @method warn
	     *	@param	keyName	Message to be translated
         *  @param  isMsgText 
	     *	@param	extraMsg			Additional message
	     */
        warn: function (keyName, isMsgText, extraMsg) {
            var content = "";
            // if user has given a key
            if (keyName != null && !keyName.equals("")) {
                // get key's value
                content += VIS.Msg.getMsg(keyName);
            }
            // if user has given any extra content
            if (extraMsg != null && extraMsg.length > 0) {
                // add the content
                content += "\n" + extraMsg;
            }

            alert(content);
            content = null;
        }
    };
}(VIS, jQuery));