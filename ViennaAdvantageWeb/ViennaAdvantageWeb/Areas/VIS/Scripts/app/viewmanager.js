; (function (VIS) { //scope
    var dm = VIS.desktopMgr; // shorthand for desktopMge object
    var $mainConatiner = dm.getMainConatiner(); // main conatiner 

    function viewManager() {

        var ACTION_WORKBENCH = "B";
        /** WorkFlow = F */
        var ACTION_WORKFLOW = "F";
        /** Process = P */
        var ACTION_PROCESS = "P";
        /** Report = R */
        var ACTION_REPORT = "R";
        /** Task = T */
        var ACTION_TASK = "T";
        /** Window = W */
        var ACTION_WINDOW = "W";
        /** Form = X */
        var ACTION_FORM = "X";
        var SEPRATOR = "!"; // history action seprator
        var historyActions = []; // conatins history string array
        var windowObjects = {}; // store all window(javascrpt)  references
        var s_hiddenWindows = []; /** list of hidden Windows				*/


        var mainPage = {
            startAction: startAction,
            restoreActions: restoreActions,
            resize: refresh,
            sizeChanged: sizeChanged,
            startWindow: startWindow,
            startForm: startForm,
            startCFrame: startCFrame,
            startActionInNewTab: startActionInNewTab
        };

        return mainPage; // return mainpage object and public functions


        /*
         * start menu Item
         * @param action type of item
         * @param id id of item
         */
        function startAction(action, id) {

            if (ACTION_WINDOW === action) {
                startWindow(id);
            }
            else if (ACTION_FORM === action) {
                startForm(id);
                //
            }
            else if (ACTION_PROCESS === action || ACTION_REPORT === action) {
                startProcess(id);
            }
        };

        function addShortcut(id, imgName, name, hid) {
            var imgPath = VIS.Application.contextUrl + "Areas/VIS/Images/base/window-default.png";
            if (imgName) {
                imgPath = imgName;
            }

            dm.addTaskBarItem(id, imgPath, name);

            if (hid) {
                historyActions.push(hid);
                historyMgr.updateHistory(encode(historyActions.join(SEPRATOR)));
            }

        };

        function encode(action) {
            action = VIS.Utility.encode(action)
            return action;
        }


        /* Remove ShortCut icon form list
         * @param id parameter id of LI element to remove
         * @param $panel jquery window object to remove
         */

        function removeShortcut(id, $panel, hid, AD_Window_ID) {

            dm.unRegisterView(id); //remove from desktop
            if (hid) {
                var index = historyActions.indexOf(hid);
                if (index > -1) {
                    historyActions.splice(index, 1);
                }
                historyMgr.updateHistory(encode(historyActions.join(SEPRATOR)));
            }
            var window = windowObjects[id];
            windowObjects[id] = null;
            delete windowObjects[id];
            if (AD_Window_ID && hideWindow(window))
                return false;
            return true;
        };

        /* Start Window
         *@param id id of window
         *@param qry Query Object
         */
        function startWindow(id, qry) {

            var window = showWindow(id);
            if (window) {
                window.show($mainConatiner, addShortcut);
                registerView(window);
                return window;
            }

            window = new VIS.AWindow();
            if (window.initWindow(id, qry, addShortcut, ACTION_WINDOW)) {
                window.onClosed = removeShortcut;
                window.show($mainConatiner);
                registerView(window);
            }
            return window;
        };

        /* Start form
        *@param id id of form
        */
        function startForm(id) {
            var window = new VIS.AWindow();
            if (window.initForm(id, addShortcut, ACTION_FORM)) {
                // $mainConatiner.empty();
                window.onClosed = removeShortcut;
                window.show($mainConatiner);
                registerView(window);
            }
        };

        /* Start process
        *@param id id of process
        */
        function startProcess(id) {
            var window = new VIS.AWindow();
            if (window.initProcess(id, addShortcut, ACTION_PROCESS)) {
                window.onClosed = removeShortcut;
                window.show($mainConatiner);
                registerView(window);
            }
        };

        function startCFrame(window) {
            window.onClosed = removeShortcut;
            window.show($mainConatiner);
            addShortcut(window.getId(), null, window.getName(), null);
            registerView(window);
        };

        /*
          cache window object 
          and send window Ui root element to desktopmanager
          @param window object
        */
        function registerView(window) {
            dm.registerView(window.getId(), window.getRootLayout());
            windowObjects[window.getId()] = window;
        };

        /*
           restore history actions 
           @param actionstr url string
        */
        function restoreActions(actionStr) {
            try {
                historyMgr.updateHistory("#");
                actionStr = VIS.Utility.decode(actionStr); // decrypt url
                var actions = actionStr.split(SEPRATOR); // seprate actions
                for (var i = 0, len = actions.length; i < len; i++) {
                    if (actions[i].length > 0) {
                        var actionId = actions[i];
                        var action = actionId.substring(0, 1); // either W  or P or X etc...
                        var id = actionId.substring(2); // id of action
                        if (ACTION_FORM === action) {
                            if (!VIS.MRole.getFormAccess(id)) {
                                console.log("No Form Access");
                                continue;
                            }
                        }
                        else if (ACTION_PROCESS === action || ACTION_REPORT === action) {
                            if (!VIS.MRole.getProcessAccess(id)) {
                                console.log("No Process Access");
                                continue;
                            }
                        }
                        else if (ACTION_WINDOW === action) {
                            if (!VIS.MRole.getWindowAccess(id)) {
                                console.log("No Window Access");
                                continue;
                            }
                        }
                        startAction(actionId.substring(0, 1), actionId.substring(2)); //start menu action
                    }
                }
            }
            catch (e) {
                historyMgr.updateHistory("#"); //update url
                console.log(e);
            }
        };

        /* 
        - grid need to be resize when window get focus
        @param id unique name of window object
        */
        function refresh(id) {
            if (windowObjects[id])
                windowObjects[id].refresh();
        };

        /* resize all open windows when screen size is changed
        */
        function sizeChanged() {
            for (var id in windowObjects) {
                windowObjects[id].sizeChanged();
            }
            //alert("resize");
        };

        /*
          open action(W,P,X) in new tab of browser
          @param action type of menu item
          @param id of action
        */
        function startActionInNewTab(action, id) {
            if (action && id) {
                var qs = action + "=" + id;
                qs = encode(qs);
                window.open(VIS.Application.contextUrl + "#" + qs);
                return;
            }
            alert("improper data");
        };

        /*
          cache window object 
          if cache window option set to true in preference setting
          @param window object
        */
        function hideWindow(window) {
            if (!VIS.Ini.getIsCacheWindow())
                return false;
            for (var i = 0; i < s_hiddenWindows.length; i++) {
                var hidden = s_hiddenWindows[i];
                //_log.Info(i + ": " + hidden);
                if (hidden.getAD_Window_ID() == window.getAD_Window_ID())
                    return true;	//	already there
            }
            if (window.getAD_Window_ID() != 0)	
            {
                try {
                    s_hiddenWindows.push(window);
                    if (s_hiddenWindows.length > 10)
                        s_hiddenWindows.splice(0, 1);		//	sort of lru
                    return true;
                }
                catch (e) {
                    return false;
                }
            }
            return false;
        };

        /* show window from cache 
        @param AD_window_ID if of window
        */
        function showWindow(AD_Window_ID) {
            for (var i = 0; i < s_hiddenWindows.length; i++) {
                var hidden = s_hiddenWindows[i];
                if (hidden.getAD_Window_ID() == AD_Window_ID) {
                    s_hiddenWindows.splice(i, 1);// RemoveAt(i);
                    return hidden;
                }
            }
            return;
        };

    };




    function CFrame(windowNo) {
        this.frame = new VIS.AWindow();
        this.frame.setName("Form");
        this.onOpened = null;
        this.onClose = null;
        var content = null;
        this.contentDispose = null;
        this.error = false;;
        this.windowNo = windowNo || VIS.Env.getWindowNo();
        this.hideH = false;

        var self = this;
        this.setContent = function (root) {

            if (!root.getRoot || !root.getRoot()) {
                VIS.Dialog.error("Error", true, "class must define root element  and implement getRoot function");
                root.dispose();
                this.error = true;
            }

            if (!root.sizeChanged) root.sizeChanged = function () { };

            if (!root.refresh) root.refresh = function () { };

            if (root.dispose) {
                this.contentDispose = root.dispose;
                root.dispose = function ()
                { self.dispose(); }
            }
            content = root;
            this.frame.setCFrameContent(root, this.windowNo);
            if (this.hideH)
                this.frame.hideHeader(this.hideH);
        };

        this.dispose = function () {
            if (this.disposing)
                return;
            this.disposing = true;
            this.contentDispose.call(content);
            //if (this.frame && this.frame.onClosed) {
            //    this.frame.onClosed(this.frame.id, this.frame.$layout, this.frame.hid);
            //    this.frame.onClosed = null;
            //}

            if (this.frame)
                this.frame.dispose();
            this.frame = null;
            this.onOpened = null;
            this.onClose = null;
            content = null;
            this.contentDispose = null;
            self = null;
        };
    };

    CFrame.prototype.setTitle = function (title) {
        this.frame.setTitle(title);
    };

    CFrame.prototype.getTitle = function (title) {
        return this.frame.getTitle();
    };

    CFrame.prototype.setName = function (name) {
        this.frame.setName(name);
    };

    CFrame.prototype.show = function () {
        if (this.error) {
            this.dispose();
            return;
        }


        VIS.viewManager.startCFrame(this.frame);
        if (this.onOpened)
            this.onOpened();
    };

    CFrame.prototype.hideHeader = function (hide) {
        this.hideH = hide;
        if (this.frame.cPanel)
            this.frame.hideHeader(hide);
    };

    VIS.viewManager = viewManager();

    VIS.CFrame = CFrame;

})(VIS);

