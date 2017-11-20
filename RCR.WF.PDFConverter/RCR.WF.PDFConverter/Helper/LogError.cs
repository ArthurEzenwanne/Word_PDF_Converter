using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

using System.Net.Mail;

using RCR.WF.PDFConverter.Helper.SharePoint;

namespace RCR.WF.PDFConverter.Helper.LogError
{
    public class LogErrorHelper
    {
        #region Variables

        private const string APP_NAME = "LogErrorHelper Class";

        SharePointHelper spHelper = new SharePointHelper();
       
        #endregion

        #region Constructor

        /// <summary>
        ///      Initializes a new instance of class
        /// </summary>
        public LogErrorHelper()
        {

        }

        #endregion

        #region method


        /// <summary>
        ///     Function to email error messages 
        /// </summary>
        /// <param name="errSysMsg"></param>
        public bool logErrorEmail(string APP_NAME, Exception errSysMsg, string errTitle)
        {
            try
            {
                string EmailTo = spHelper.GetRCRSettingsItem("EmailTo").ToString(); 
                string strError = System.DateTime.Now + "<br>Application: " + APP_NAME + "<br>Error Message: " + errSysMsg.Message + "<br>";

                //Check the InnerException 
                while ((errSysMsg.InnerException != null))
                {
                    strError += errSysMsg.InnerException.ToString();
                    errSysMsg = errSysMsg.InnerException;
                }

                //Send error log via email
                SPUtility.SendEmail(SPContext.Current.Web, false, false, EmailTo, errTitle, strError);

                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(APP_NAME, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, strError, errSysMsg.StackTrace);
                return true;
            }
            catch
            {            
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(APP_NAME, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, errSysMsg.Message.ToString(), errSysMsg.StackTrace);
                return false;
            }

           
        }

        /// <summary>
        ///    Email and log error message using custom SMTP server
        /// </summary>
        /// <param name="APP_NAME"></param>
        /// <param name="errSysMsg"></param>
        /// <param name="errTitle"></param>
        /// <returns>Returns true if email was sucessfully sent</returns>
        public bool logErrorSMTPEmail(string APP_NAME, Exception errSysMsg, string errTitle, bool isHTMLFormat)
        {
            try
            {
                string EmailTo = spHelper.GetRCRSettingsItem("EmailTo").ToString();
                string EmailFrom = spHelper.GetRCRSettingsItem("EmailFrom").ToString();
                string SMPTServer = spHelper.GetRCRSettingsItem("SMTPServer").ToString();
                string useProxyEmail = spHelper.GetRCRSettingsItem("UseProxyEmail").ToString();

                string strError = System.DateTime.Now + "<br>Application: " + APP_NAME + "<br>Error Message: " + errSysMsg.Message + "<br>";

                //Check the InnerException 
                while ((errSysMsg.InnerException != null))
                {
                    strError += errSysMsg.InnerException.ToString();
                    errSysMsg = errSysMsg.InnerException;
                }

                //Send error log via email
                MailMessage message = new MailMessage();
                message.From = new MailAddress(EmailFrom);

                if (isHTMLFormat)
                    message.IsBodyHtml = true;

                message.ReplyTo = new MailAddress(EmailFrom);
                message.Sender = new MailAddress(EmailFrom);

                message.To.Add(new MailAddress(EmailTo));
                // message.CC.Add(new MailAddress("copy@domain.com"));  
                message.Subject = errTitle;
                message.Body = strError;

                SmtpClient client = new SmtpClient();
                client.Host = SMPTServer;

                if (useProxyEmail == "true")
                {
                    client.UseDefaultCredentials = true;
                    string Domain = spHelper.GetRCRSettingsItem("Domain").ToString();
                    string SysAcct = spHelper.GetRCRSettingsItem("ProxyUser").ToString();
                    string SysAcctPassword = spHelper.GetRCRSettingsItem("ProxyPassword").ToString();

                    client.Credentials = new System.Net.NetworkCredential(Domain + "\\" + SysAcct, SysAcctPassword);
                }

                client.Send(message);

                return true;
            }
            catch
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(APP_NAME, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, errSysMsg.Message.ToString(), errSysMsg.StackTrace);
                return false;
            }

        }

        /// <summary>
        ///     Logging function that enters updates to the Workflow History list
        /// </summary>
        /// <param name="logMessage"></param>
        public void LogWFHistoryComment(string logMessage, SPWorkflowActivationProperties workflowProperties, Guid WorkflowInstanceId)
        {
            SPWorkflow.CreateHistoryEvent(workflowProperties.Web, WorkflowInstanceId, 0, workflowProperties.Web.CurrentUser, new TimeSpan(), "Update", logMessage, string.Empty);
        }

        #endregion

    }
}
