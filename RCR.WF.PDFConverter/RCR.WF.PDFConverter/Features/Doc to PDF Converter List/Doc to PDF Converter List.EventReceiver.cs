using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

using RCR.SP.Framework.Helper.SharePoint;
using RCR.SP.Framework.Helper.LogError;

namespace RCR.WF.PDFConverter.Features.Doc_to_PDF_Converter_List
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("704657cc-f1a5-4534-acd2-e4a5dff7ca46")]
    public class Doc_to_PDF_Converter_ListEventReceiver : SPFeatureReceiver
    {

        private const string APP_NAME = "Doc_to_PDF_Converter_ListEventReceiver class";
        private const string LIST_SETTING = "PDF Settings";

        // Uncomment the method below to handle the event raised after a feature has been activated.

        //public override void FeatureActivated(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            try
            {
                using (SPSite oSPSite = new SPSite(SPContext.Current.Site.Url))
                {
                    using (SPWeb oSPWeb = oSPSite.OpenWeb())
                    {
                        SharePointHelper spHelper = new SharePointHelper(LIST_SETTING, "EmailTo", oSPWeb);
                        spHelper.DeleteList(LIST_SETTING, oSPWeb);
                        spHelper = null;
                    }
                }
            }
            catch (Exception err)
            {
                using (SPSite oSPSite = new SPSite(SPContext.Current.Site.Url))
                {
                    using (SPWeb oSPWeb = oSPSite.OpenWeb())
                    {
                        emailUser(APP_NAME + "- FeatureDeactivating Error!", err.Message.ToString(), oSPWeb);
                    }
                }
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(APP_NAME, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, err.Message.ToString(), err.StackTrace);
            }
        }


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}

        /// <summary>
        ///     Send email to user
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        private void emailUser(string subject, string body, SPWeb currentWeb)
        {
            LogErrorHelper objEmail = new LogErrorHelper(LIST_SETTING, currentWeb);
            bool isEmailSent = objEmail.sendUserEmail(APP_NAME + " - EmailUser Function", subject, body, false);
            objEmail = null;
        }
    }
}
