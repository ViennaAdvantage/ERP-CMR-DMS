using System.Threading;
using System.Web.Mvc;
using System.Web.Optimization;
namespace VIS
{
    public class VISAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "VIS";
            }
        }


        public override void RegisterArea(AreaRegistrationContext context)
        {

            context.MapRoute(
                               "VIS_default",
                               "VIS/{controller}/{action}/{id}",
                               new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                               , new[] { "VIS.Controllers" }
                               );


            StyleBundle style = new StyleBundle("~/Areas/VIS/Content/VISstyle");
            //StyleBundle styleRTL = new StyleBundle("~/Areas/VIS/Content/VISstyleRTL");

            ScriptBundle modScript = new ScriptBundle("~/Areas/VIS/Script/VISjs");

            //modScript.Include(
            //    "~/Areas/VIS/Scripts/app/native-extension.js",
            //    "~/Areas/VIS/Scripts/app/utility.js",
            //    "~/Areas/VIS/Scripts/app/logger.js",
            //    "~/Areas/VIS/Scripts/app/controls.js",
            //    "~/Areas/VIS/Scripts/app/classes.js",
            //    "~/Areas/VIS/Scripts/app/desktopmgr.js",
            //    "~/Areas/VIS/Scripts/app/viewmanager.js",
            //    "~/Areas/VIS/Scripts/app/context.js",
            //    "~/Areas/VIS/Scripts/app/i18N.js",
            //    "~/Areas/VIS/Scripts/app/role.js",
            //    "~/Areas/VIS/Scripts/app/adialog.js",
            //    "~/Areas/VIS/Scripts/app/lookup.js",
            //    "~/Areas/VIS/Scripts/app/treepanel.js",
            //    "~/Areas/VIS/Scripts/app/windowframe.js",
            //    "~/Areas/VIS/Scripts/app/controller.js",
            //    "~/Areas/VIS/Scripts/app/formframe.js",
            //    "~/Areas/VIS/Scripts/app/processframe.js",
            //    "~/Areas/VIS/Scripts/processengine/processctl.js",
            //    "~/Areas/VIS/Scripts/processengine/processinfo.js",
            //    "~/Areas/VIS/Scripts/processengine/processparameter.js",
            //    "~/Areas/VIS/Scripts/forms/form.js",
            //    "~/Areas/VIS/Scripts/app/datacontext.js",
            //    "~/Areas/VIS/Scripts/app/calloutengine.js",
            //    "~/Areas/VIS/Scripts/model/callouts.js",
            //    "~/Areas/VIS/Scripts/home/home.js",
            //    "~/Areas/VIS/Scripts/home/changeuserimage.js",
            //    "~/Areas/VIS/Scripts/home/shortcut.js",
            //    "~/Areas/VIS/Scripts/app/historymgr.js",
            //    "~/Areas/VIS/Scripts/app/secure-engine.js",
            //    "~/Areas/VIS/Scripts/app/Framework/userpreference.js",
            //    "~/Areas/VIS/Scripts/app/Framework/vimageform.js",
            //    "~/Areas/VIS/Scripts/app/Framework/help.js",
            //    "~/Areas/VIS/Scripts/app/Framework/ini.js",
            //    "~/Areas/VIS/Scripts/app/Framework/locationform.js",
            //    "~/Areas/VIS/Scripts/app/Framework/locatorform.js",
            //    "~/Areas/VIS/Scripts/app/Framework/valuepreference.js",
            //    "~/Areas/VIS/Scripts/app/Framework/pattributesform.js",
            //    "~/Areas/VIS/Scripts/app/Framework/pattributeinstance.js",
            //    "~/Areas/VIS/Scripts/app/Framework/genralattributeform.js",
            //    "~/Areas/VIS/Scripts/app/Framework/accountform.js",
            //    "~/Areas/VIS/Scripts/app/Framework/archiveviewer.js",
            //    "~/Areas/VIS/Scripts/app/child-dialog.js",
            //    "~/Areas/VIS/Scripts/app/Framework/chat.js",
            //    "~/Areas/VIS/Scripts/app/Framework/infomenu.js",
            //    "~/Areas/VIS/Scripts/app/Framework/infowindow.js",
            //    "~/Areas/VIS/Scripts/app/Framework/infogeneral.js",
            //    "~/Areas/VIS/Scripts/app/Framework/infoproduct.js",
            //    "~/Areas/VIS/Scripts/app/Framework/infoscanform.js",
            //    "~/Areas/VIS/Scripts/app/Framework/attachmentform.js",
            //    "~/Areas/VIS/Scripts/app/Framework/help.js",
            //    "~/Areas/VIS/Scripts/app/Framework/wfactivity.js",
            //    "~/Areas/VIS/Scripts/app/Framework/vdocaction.js",
            //    "~/Areas/VIS/Scripts/app/Framework/appointments.js",
            //    "~/Areas/VIS/Scripts/app/Framework/vpayment.js",
            //    "~/Areas/VIS/Scripts/app/forms/form.js",
            //    "~/Areas/VIS/Scripts/app/forms/vcreatefrom.js",
            //    "~/Areas/VIS/Scripts/app/forms/vcreatefrominvoice.js",
            //    "~/Areas/VIS/Scripts/app/forms/vcreatefromshipment.js",
            //    "~/Areas/VIS/Scripts/app/forms/vcreatefromstatement.js",
            //    "~/Areas/VIS/Scripts/app/forms/generatexmodel.js",
            //    "~/Areas/VIS/Scripts/app/forms/acctviewer.js",
            //    "~/Areas/VIS/Scripts/app/forms/vinoutgen.js",
            //    "~/Areas/VIS/Scripts/app/forms/vinvoicegen.js",
            //     "~/Areas/VIS/Scripts/app/forms/vmatch.js",
            //     "~/Areas/VIS/Scripts/app/forms/vcharge.js",
            //     "~/Areas/VIS/Scripts/app/forms/vattributegrid.js",
            //     "~/Areas/VIS/Scripts/app/forms/vpayselect.js",
            //      "~/Areas/VIS/Scripts/app/forms/vpayprint.js",
            //      "~/Areas/VIS/Scripts/app/forms/vBOMdrop.js",
            //      "~/Areas/VIS/Scripts/app/forms/vtrxmaterial.js",
            //    "~/Areas/VIS/Scripts/app/Framework/find.js",
            //    "~/Areas/VIS/Scripts/app/azoomacross.js",
            //    "~/Areas/VIS/Scripts/app/Framework/sms.js",

            //     "~/Areas/VIS/Scripts/app/arequest.js",
            //     "~/Areas/VIS/Scripts/app/Framework/vsetup.js",
            //       "~/Areas/VIS/Scripts/home/favourite.js",
            //       "~/Areas/VIS/Scripts/app/Framework/bpartner.js",
            //        "~/Areas/VIS/Scripts/app/Framework/contactInfo.js",
            //        "~/Areas/VIS/Scripts/app/favouritehelper.js",
            //     "~/Areas/VIS/Scripts/app/Framework/email.js",
            //     "~/Areas/VIS/Scripts/app/Framework/newmailformat.js",
            //     "~/Areas/VIS/Scripts/app/Framework/openmailformats.js",
            //     "~/Areas/VIS/Scripts/app/areport.js",
            //        "~/Areas/VIS/Scripts/app/Framework/attachmentHistory.js",
            //        "~/Areas/VIS/Scripts/app/Framework/recordaccessdialog.js",
            //               "~/Areas/VIS/Scripts/app/forms/vallocation.js",

            //    "~/areas/vis/scripts/app/grouprights/group.js",
            //    "~/areas/vis/scripts/app/grouprights/createuser.js",
            //    "~/areas/vis/scripts/app/grouprights/orgaccess.js",
            //    "~/areas/vis/scripts/app/grouprights/groupinfo.js",
            //    "~/areas/vis/scripts/app/grouprights/createrole.js",
            //    "~/areas/vis/scripts/app/grouprights/gmailcontactsetting.js",
            //    "~/areas/vis/scripts/app/grouprights/importcontacts.js",
            //      "~/areas/vis/scripts/app/orgstructure/orgstructure.js",
            //        "~/Areas/VIS/Scripts/app/initialize.js" // bottom one
            //       );

            //style.Include("~/Areas/VIS/Content/Site.css",
            //    "~/Areas/VIS/Content/VISAD.css",
            //    "~/Areas/VIS/Content/PrefStyle.css",
            //    "~/Areas/VIS/Content/Home.css",
            //    "~/Areas/VIS/Content/PaymentRule.css",
            //    "~/Areas/VIS/Content/Style.css",
            //    "~/Areas/VIS/Content/ClientSetup.css",
            //    "~/Areas/VIS/Content/Find.css",
            //    "~/Areas/VIS/Content/Email.css",
            //    "~/Areas/VIS/Content/VPaySelect.css",
            //     "~/Areas/VIS/Content/reportstyle.css",
            //     "~/Areas/VIS/Content/RecordAccessDialog.css",
            //    "~/Areas/VIS/Content/GroupStyle.css"
            //    );

            //styleRTL.Include("~/Areas/VIS/Content/VIS.rtl.css");

            style.Include("~/Areas/VIS/Content/VIS.all.min.css");
            modScript.Include("~/Areas/VIS/Scripts/VIS.all.min.js");

            VAdvantage.ModuleBundles.RegisterScriptBundle(modScript, "VIS", -1);
            VAdvantage.ModuleBundles.RegisterStyleBundle(style, "VIS", -1);
            //VAdvantage.ModuleBundles.RegisterRTLStyleBundle(styleRTL, "VIS", -1);

        }
    }
}
