using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
    public partial class MultiPDFConversion : LayoutsPageBase
    {
        private const string ClassName = "MultiPDFConversion app";

        private string listId = null;
        //string listItemId = null;
        private string returnUrl = null;
        private string[] selectedItems;
        private List<string> srcFileName;
        private List<string> trgFileName;
        private SPList selectedDocumentLibrary = null;
        //SPListItem listItem = null;

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
                selectedItems = Request.QueryString["selecteditemid"].ToString().Split('|');

                if (string.IsNullOrEmpty(listId) || selectedItems.Length <= 0)
                {
                    Page.Response.Redirect(returnUrl, true);
                }

                // get all the selected documents
                using (SPWeb spWeb = SPContext.Current.Site.OpenWeb(SPContext.Current.Web.ServerRelativeUrl))
                {
                    srcFileName = new List<string>();
                    trgFileName = new List<string>();

                    selectedDocumentLibrary = spWeb.Lists[new Guid(listId)]; //Get the selected list
                    for (int index = 0; index <= selectedItems.Length-1; index++)
                    {
                        if (selectedItems[index].Length > 0)
                        {
                            SPListItem listItem = selectedDocumentLibrary.GetItemById(int.Parse(selectedItems[index])); //Get each selected document item
                            string url = SPContext.Current.Web.Url + "/" + listItem.Url;  //get the full URL
                            srcFileName.Add(url);
                            string tempTrgFileName = Path.ChangeExtension(url, "pdf");
                            trgFileName.Add(tempTrgFileName);
                        }

                    }
                    
                }

                DisplaySelectedDocument(); //Display all selected document onto this app page
               

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(ClassName, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, "Page load-" + ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        ///     Display the list of selected documents pending PDF conversion
        /// </summary>
        private void DisplaySelectedDocument()
        {
            //Display all selected document onto this app page
            StringBuilder sb = new StringBuilder();
            sb.Append("<b>Source:</b><br>");
            for (int i = 0; i <= srcFileName.Count - 1; i++)
            {
                int numDoc = i + 1;
                sb.AppendFormat(numDoc.ToString() + ". {0}<br>", srcFileName[i]);
            }

            sb.Append("<hr><b>Target:</b><br>");
            for (int i = 0; i <= trgFileName.Count - 1; i++)
            {
                int numPdf = i + 1;
                sb.AppendFormat(numPdf.ToString() + ". {0}<br>", trgFileName[i]);
            }

            lblInfo.Text = sb.ToString();
        }

        void DocConverter_PreRender(object sender, EventArgs e)
        {

        }

       
        /// <summary>
        ///     Convert selected multiple word documents into PDF files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnConvert_Click(object sender, EventArgs e)
        {
            try
            {
                PDFConverterHelper conv = new PDFConverterHelper("");
                StringBuilder sb = new StringBuilder("");
                int numConvertedPDF = 0;

               for (int index = 0; index <= selectedItems.Length-1; index++)
               {
                    if (selectedItems[index].Length > 0)
                    {
                       SPListItem listItem = selectedDocumentLibrary.GetItemById(int.Parse(selectedItems[index])); //Get each selected document item

                       if (conv.isDocIDValidated(listItem.ID.ToString(), selectedDocumentLibrary) == true)
                        {
                            conv.ConvertDocToPDF(listItem);
                            numConvertedPDF = numConvertedPDF + 1;
                        }
                        else
                        {
                            sb.Append("Invalid Word document file type (*.doc or *.docx) for: " + listItem.File.ToString() + "<br>");
                        }
                    }
               }

               if (numConvertedPDF > 0)
               {
                   if (numConvertedPDF > 1)
                   {
                       lblStatus.Text = sb.ToString() + "<br>PDF conversion has started. Please wait while " + numConvertedPDF + " documents are in the job queue for conversion...";
                   }
                   else
                   {
                       lblStatus.Text = sb.ToString() + "<br>PDF conversion has started. Please wait while " + numConvertedPDF + " document is in the job queue for conversion...";
                   }
               }
               else
               {
                   lblStatus.Text = sb.ToString();
               }
                conv = null;

            }
            catch (Exception ex)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(ClassName, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, "btnConvert_Click-" + ex.Message, ex.StackTrace);
            }
        }
    }
}
