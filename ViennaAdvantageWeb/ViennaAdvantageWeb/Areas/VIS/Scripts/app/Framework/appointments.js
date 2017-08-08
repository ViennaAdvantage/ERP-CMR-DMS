; (function (VIS, $) {
    function AppointmentsForm() {
        /* To create new appointments and task form widows
        * @ param {number} AD_Table_ID
        * @ param {number} Record_ID
        * @ param {number} s_Ad_User_ID :: selected user id
        * @ param {string} s_User_Name :: selected user name
        * @ param {Boolean} isTask or is Appointments
        */
        function initAppointments(AD_Table_ID, Record_ID, s_Ad_User_ID, s_User_Name, isTask) {
            if (window.WSP) {
                var divaptbusy = $("<div id='divAptBusy' class='wsp-busy-indicater'></div>");
                $("body").append(divaptbusy);
                if (isTask == false) {
                    divaptbusy.show();
                    WSP.AppointmentsForm.init(AD_Table_ID, Record_ID, s_Ad_User_ID, s_User_Name, divaptbusy);
                }
                else {
                    divaptbusy.show();
                    WSP.TaskForm.init(AD_Table_ID, Record_ID, s_Ad_User_ID, s_User_Name, divaptbusy);
                }

            }
            else {
                alert("please download WSP !!!");
            }
        };
        return {
            init: initAppointments
        };
    };
    VIS.AppointmentsForm = AppointmentsForm();
})(VIS, jQuery);
