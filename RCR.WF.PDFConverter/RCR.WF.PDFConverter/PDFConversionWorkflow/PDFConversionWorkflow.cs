using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;

//using RCR.WF.PDFConverter.Helper.SharePoint;
//using RCR.WF.PDFConverter.Helper.LogError;
//using RCR.WF.PDFConverter.Helper.PDFConverter;
using RCR.SP.Framework.Helper.SharePoint;
using RCR.SP.Framework.Helper.LogError;
using RCR.SP.Framework.Helper.PDFConverter;

namespace RCR.WF.PDFConverter.PDFConversionWorkflow
{
    public sealed partial class PDFConversionWorkflow : SequentialWorkflowActivity
    {
        #region variables

        private const string APP_NAME = "PDFConversionWorkflow Class";
        private const string PDF_SETTING_LIST = "PDF Settings";
        private const string PDF_LOG_LIST = "PDF Notification Log";

        #endregion

        #region method

        public PDFConversionWorkflow()
        {
            InitializeComponent();
        }

        public Guid workflowId = default(System.Guid);
        public SPWorkflowActivationProperties workflowProperties = new SPWorkflowActivationProperties();

        public String sendEmail_From1 = default(System.String);
        public String sendEmail_Subject1 = default(System.String);
        public String sendEmail_To1 = default(System.String);
        public String Email_Body1 = default(System.String);
        private bool isDocWordFormat = false;

        /// <summary>
        ///     Function to log workflow activities into a SharePoint list
        /// </summary>
        /// <param name="logTitle"></param>
        /// <param name="logMsg"></param>
        /// <param name="spSiteUrl"></param>
        /// <param name="docFileName"></param>
        /// <param name="pdfFileName"></param>
        /// <param name="itemID"></param>
        /// <param name="PdfConvertStatus"></param>
        private void WriteWFLog(string logTitle, string logMsg, string spSiteUrl, string docFileName, string pdfFileName, string itemID, bool PdfConvertStatus)
        {
            SPWeb currentWeb = SPContext.Current.Web;
            SharePointHelper objSPHelper = new SharePointHelper(PDF_SETTING_LIST, "SPLogList", currentWeb);

            //Log start of workflow to SP list
            objSPHelper.AddLogListItem(spSiteUrl, logTitle, logMsg, docFileName, pdfFileName, itemID, PdfConvertStatus);
            objSPHelper = null;
            currentWeb = null;
        }

        private void EvaluateCondition(object sender, ConditionalEventArgs e)
        {

        }

        private void onWorkflowItemChanged1_Invoked(object sender, ExternalDataEventArgs e)
        {

        }

        /// <summary>
        ///     Workflow activity to convert selected word document to PDF file 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void codeActivity1_ExecuteCode(object sender, EventArgs e)
        {
            string srcFileName = workflowProperties.WebUrl + "/" + workflowProperties.ItemUrl;
            string trgFileName = Path.ChangeExtension(srcFileName, "pdf");
            string itemID = workflowProperties.Item.ID.ToString();
            string spSiteUrl = SPContext.Current.Site.Url;

            try
            {
                PDFConverterHelper convPDF = new PDFConverterHelper(PDF_LOG_LIST);
                isDocWordFormat = convPDF.isDocValidated(srcFileName);

                if (isDocWordFormat)
                {
                    if (convPDF.PDFConvertJob(srcFileName, trgFileName) == true)
                    {
                        WriteWFLog("PDF conversion Status", "PDF conversion workflow has started. Please wait while the document is in the queue for conversion...", spSiteUrl, srcFileName, trgFileName, itemID, true);
                    }
                    else
                    {
                        WriteWFLog("PDF conversion Status", "PDF conversion workflow has failed!", spSiteUrl, srcFileName, "", itemID, false);
                    }
                }
                else
                {
                    WriteWFLog("PDF conversion Status", "PDF conversion workflow will not start due to incorrect document type!", spSiteUrl, srcFileName, "", itemID, false);
                }
                convPDF = null;
            }
            catch (Exception err)
            {
                SPWeb currentWeb = SPContext.Current.Web;
                LogErrorHelper objErr = new LogErrorHelper(PDF_SETTING_LIST, currentWeb);
                objErr.logSysErrorEmail(APP_NAME, err, "Error at codeActivity1_ExecuteCode function");
                objErr = null;
                currentWeb = null;

                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(APP_NAME, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, err.Message.ToString(), err.StackTrace);
                WriteWFLog("ERROR converting docx to PDF!", err.Message.ToString(), spSiteUrl, srcFileName, "", itemID, false);
            }
        }

        /// <summary>
        ///    Email user using custom SMTP server
        /// </summary>
        /// <param name="APP_NAME"></param>
        /// <param name="errSysMsg"></param>
        /// <param name="errTitle"></param>
        /// <returns>Returns true if email was sucessfully sent</returns>
        private bool SendUserEmail(string emailBody, string Title, bool isHTMLFormat)
        {
            
            try
            {
               
                string EmailTo = string.Empty;

                using (SPWeb spWeb = SPContext.Current.Site.OpenWeb(SPContext.Current.Web.ServerRelativeUrl))
                {
                    SharePointHelper spHelper = new SharePointHelper(PDF_SETTING_LIST, "EmailTo", spWeb);
                    SPFieldUserValue currentUser = spHelper.GetCurrentUser(spWeb);
                    EmailTo = currentUser.User.Email;
                    currentUser = null;

                    string EmailFrom = spHelper.GetRCRSettingsItem("EmailFrom", PDF_SETTING_LIST).ToString();
                    string SMPTServer = spHelper.GetRCRSettingsItem("SMTPServer", PDF_SETTING_LIST).ToString();
                    string useProxyEmail = spHelper.GetRCRSettingsItem("UseProxyEmail", PDF_SETTING_LIST).ToString();


                    //Send error log via email
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(EmailFrom);

                    if (isHTMLFormat)
                        message.IsBodyHtml = true;

                    message.ReplyTo = new MailAddress(EmailFrom);
                    message.Sender = new MailAddress(EmailFrom);

                    message.To.Add(new MailAddress(EmailTo));
                    // message.CC.Add(new MailAddress("xxx@domain.com"));  
                    message.Subject = Title;
                    message.Body = emailBody;

                    SmtpClient client = new SmtpClient();
                    client.Host = SMPTServer;

                    if (useProxyEmail == "true")
                    {
                        client.UseDefaultCredentials = true;
                        string Domain = spHelper.GetRCRSettingsItem("Domain", PDF_SETTING_LIST).ToString();
                        string SysAcct = spHelper.GetRCRSettingsItem("ProxyUser", PDF_SETTING_LIST).ToString();
                        string SysAcctPassword = spHelper.GetRCRSettingsItem("ProxyPassword", PDF_SETTING_LIST).ToString();

                        client.Credentials = new System.Net.NetworkCredential(Domain + "\\" + SysAcct, SysAcctPassword);
                    }

                    client.Send(message);
                    spHelper = null;
                }
                return true;
            }
            catch (Exception err)
            {
                SPWeb spCurrentWeb = SPContext.Current.Site.OpenWeb(SPContext.Current.Web.ServerRelativeUrl);
                LogErrorHelper objErr = new LogErrorHelper(PDF_SETTING_LIST, spCurrentWeb);
                objErr.logSysErrorEmail(APP_NAME, err, "Error at SendUserEmail function");
                objErr = null;
                spCurrentWeb = null;

                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(APP_NAME, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, err.Message.ToString(), err.StackTrace);

                return false;
            }
        }

        /// <summary>
        ///     Workflow activity to send email to end users after completing pdf conversion workflow activity 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendUserEmail_MethodInvoking(object sender, EventArgs e)
        {
            string docFileName = workflowProperties.WebUrl + "/" + workflowProperties.ItemUrl;
            string pdfFileName = Path.ChangeExtension(docFileName, "pdf");
            string itemID = workflowProperties.Item.ID.ToString();
            string spSiteUrl = SPContext.Current.Site.Url;

            try
            {
                using (SPWeb spWeb = SPContext.Current.Site.OpenWeb(SPContext.Current.Web.ServerRelativeUrl))
                {
                    SharePointHelper objSPHelper = new SharePointHelper(PDF_SETTING_LIST, "EmailTo", spWeb);
                    SPFieldUserValue currentUser = objSPHelper.GetCurrentUser(spWeb);
                    StringBuilder emailBody = new StringBuilder();

                    emailBody.Append("Dear " + currentUser.User.Name + ",<br>");
                    emailBody.Append("<p>The document " + docFileName + " is in the process for PDF conversion. Please wait for the PDF file link below to be made availiable.</p><p><b>PDF file:</b> " + pdfFileName + "</p>");

                    sendEmail_From1 = objSPHelper.GetRCRSettingsItem("EmailFrom", PDF_SETTING_LIST);
                    sendEmail_Subject1 = "PDF Conversion Notification";
                    sendEmail_To1 = currentUser.User.Email;
                    Email_Body1 = emailBody.ToString();

                    WriteWFLog("PDF Notification Email", "Email has been successfully sent to " + currentUser.User.Email, spSiteUrl, docFileName, pdfFileName, itemID, true);
                    objSPHelper = null;
                    currentUser = null;
                }
            }
            catch (Exception err)
            {
                SPWeb spCurrentWeb = SPContext.Current.Site.OpenWeb(SPContext.Current.Web.ServerRelativeUrl);
                LogErrorHelper objErr = new LogErrorHelper(PDF_SETTING_LIST, spCurrentWeb);
                objErr.logSysErrorEmail(APP_NAME, err, "Error at sendUserEmail_MethodInvoking function");
                objErr = null;
                spCurrentWeb = null;

                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(APP_NAME, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, err.Message.ToString(), err.StackTrace);
                WriteWFLog("ERROR sending email to user!", err.Message.ToString(), spSiteUrl, docFileName, "", itemID, false);
            }
        }

        /// <summary>
        ///     Task to start the workflow activity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onWorkflowActivated1_Invoked(object sender, ExternalDataEventArgs e)
        {
            string docFileName = workflowProperties.WebUrl + "/" + workflowProperties.ItemUrl;
            string itemID = workflowProperties.Item.ID.ToString();
            string listID = workflowProperties.List.ID.ToString();
            string spSiteUrl = SPContext.Current.Site.Url;

            try
            {
                //Log start of workflow to SP list   
                WriteWFLog("Starting PDF Conversion Workflow", "New workflow instance has started<br>List ID: " + listID, spSiteUrl, docFileName, "", itemID, true);
            }
            catch (Exception err)
            {
                SPWeb spCurrentWeb = SPContext.Current.Site.OpenWeb(SPContext.Current.Web.ServerRelativeUrl);
                LogErrorHelper objErr = new LogErrorHelper(PDF_SETTING_LIST, spCurrentWeb);
                objErr.logSysErrorEmail(APP_NAME, err, "Error at onWorkflowActivated1_Invoked function");
                objErr = null;
                spCurrentWeb = null;

                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(APP_NAME, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, err.Message.ToString(), err.StackTrace);
                WriteWFLog("ERROR starting PDF Conversion Workflow!", err.Message.ToString(), spSiteUrl, docFileName, "", itemID, false);
            }
        }

        #endregion



        /// <summary>
        ///     Workflow activity to send email
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendEmail_ExecuteCode(object sender, EventArgs e)
        {
            string docFileName = workflowProperties.WebUrl + "/" + workflowProperties.ItemUrl;
            string pdfFileName = "";
            string itemID = workflowProperties.Item.ID.ToString();
            string spSiteUrl = SPContext.Current.Site.Url;

            try
            {
                using (SPWeb spWeb = SPContext.Current.Site.OpenWeb(SPContext.Current.Web.ServerRelativeUrl))
                {
                    SharePointHelper objSPHelper = new SharePointHelper(PDF_SETTING_LIST, "EmailFrom", spWeb);
                    SPFieldUserValue currentUser = objSPHelper.GetCurrentUser(spWeb);
                    StringBuilder emailBody = new StringBuilder();
                    string Title = "PDF Conversion Notification";
                    string EmailTo = currentUser.User.Email;

                    emailBody.Append("Dear " + currentUser.User.Name + ",<br>");
                    if (isDocWordFormat)
                    {
                        pdfFileName = Path.ChangeExtension(docFileName, "pdf");
                        emailBody.Append("<p>The document " + docFileName + " is in the process for PDF conversion. Please wait for the PDF file link below to be made availiable.</p><p><b>PDF file:</b> " + pdfFileName + "</p>");          
                    }
                    else
                    {
                        emailBody.Append("<p>The document " + docFileName + " is NOT a valid word document file type. Please select the correct word document (*.docx) format for PDF conversion.");
                    }

                    if (EmailTo.Length > 0)
                    {
                        //Send email to user
                        SPUtility.SendEmail(SPContext.Current.Web, true, false, EmailTo, Title, emailBody.ToString());

                        WriteWFLog("PDF Notification Email", "Email has been successfully sent to " + EmailTo, spSiteUrl, docFileName, pdfFileName, itemID, true);
                    }
                    else
                    {
                        WriteWFLog("PDF Notification Email", "Email address is blank and has not been successfully sent", spSiteUrl, docFileName, pdfFileName, itemID, true);
                    }
                    objSPHelper = null;
                    currentUser = null;
                }

            }
            catch (Exception err)
            {
                SPWeb spCurrentWeb = SPContext.Current.Site.OpenWeb(SPContext.Current.Web.ServerRelativeUrl);
                LogErrorHelper objErr = new LogErrorHelper(PDF_SETTING_LIST, spCurrentWeb);
                objErr.logSysErrorEmail(APP_NAME, err, "Error at sendEmail_ExecuteCode function");
                objErr = null;
                spCurrentWeb = null;

                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(APP_NAME, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, err.Message.ToString(), err.StackTrace);
                WriteWFLog("ERROR sending email to user!", err.Message.ToString(), spSiteUrl, docFileName, "", itemID, false);
            }



        }


    }

}
