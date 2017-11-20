using System;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;

using Microsoft.Office.Word.Server.Conversions;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

//using RCR.WordPDFConverter.Helper;
using RCR.SP.Framework.Helper.PDFConverter;

namespace RCR.WordPDFConverter.Layouts.RCR.WordPDFConverter
{
    public partial class PDFConversion : LayoutsPageBase
    {
        private const string ClassName = "PDFConversion app";

        private string listId = null;
        private string listItemId = null;
        private string returnUrl = null;
        private string srcFileName = string.Empty;
        private string trgFileName = string.Empty;
        private SPList list = null;
        private SPListItem item = null ;

        /// <summary>
        ///     Page load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.PreRender += new EventHandler(DocConverter_PreRender);

                btnConvert.Click += new EventHandler(btnConvert_Click);

                returnUrl = Page.Request["returnUrl"];
                listId = Page.Request["listid"];
                listItemId = Page.Request["listitemid"];

                if (string.IsNullOrEmpty(listId) || string.IsNullOrEmpty(listItemId))
                {
                    Page.Response.Redirect(returnUrl, true);
                }

                //get details about the file
                list = SPContext.Current.Web.Lists[new Guid(listId)];
                item = list.GetItemById(int.Parse(listItemId));

                //get the full URL
                string url = SPContext.Current.Web.Url + "/" + item.Url;
                srcFileName = url;

                //work out the target filename

                //remove the last file extension.
                int iPos = srcFileName.LastIndexOf(".");
                if ((iPos > -1) && (srcFileName.Length - iPos <= 5))
                {
                    trgFileName = srcFileName.Substring(0, iPos) + ".PDF";
                }
                else
                {
                    trgFileName = srcFileName + ".PDF";
                }


                lblInfo.Text = string.Format("Source:- <b>{2}</b><BR>Size:- {1} bytes<HR>Target:- <b>{3}</b>", item.Title, item["File_x0020_Size"], srcFileName, trgFileName);
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(ClassName, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, "Page load-" + ex.Message, ex.StackTrace);
            }
        }

        

        void DocConverter_PreRender(object sender, EventArgs e)
        {

        }

        /// <summary>
        ///     Convert word document into PDF file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnConvert_Click(object sender, EventArgs e)
        {
            try
            {
                //PDFConverter conv = new PDFConverter();
                PDFConverterHelper conv = new PDFConverterHelper("");

                if (conv.isDocIDValidated(listItemId, list) == true)
                {
                    if (conv.PDFConvertJob(srcFileName, trgFileName) == true)
                    {
                        lblStatus.Text = "PDF conversion has started. Please wait while the document is in the queue for conversion...";// + conv.PDFConvertJob(srcFileName, trgFileName).ToString();
                    }
                    else
                    {
                        lblStatus.Text = "PDF conversion has failed.";
                    }
                }
                else
                {
                    lblStatus.Text = "Please select a valid word document (*.doc or *.docx) file type!";
                }
                conv = null;
            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(ClassName, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, "Convert_Click-" + ex.Message, ex.StackTrace);
            }
        }

       
    }
}
