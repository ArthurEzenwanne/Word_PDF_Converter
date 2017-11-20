using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;

using RCR.WF.PDFConverter.Helper.LogError;

namespace RCR.WF.PDFConverter.Helper.SharePoint
{
    public class SharePointHelper
    {
        #region constructor
        
        public SharePointHelper()
        {

        }

        #endregion

        #region variables
        
        private const string APP_NAME = "SharePointHelper Class";
        
        #endregion

        #region method

        /// <summary>
        ///     Method to insert a list item into a specific SharePoint list
        /// </summary>
        /// <param name="spSiteUrl"></param>
        /// <param name="listName"></param>
        /// <param name="colName"></param>
        /// <param name="itemValue"></param>
        public void AddListItem(string spSiteUrl, string listName, string colName, string itemValue)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(spSiteUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;

                            SPList list = web.Lists[listName];
                            SPListItem listItem = list.Items.Add();
                            listItem[colName] = itemValue;
                            listItem.Update();
                            Thread.Sleep(1000); //Give SharePoint some time to update the changes

                            web.Update();
                            Thread.Sleep(1000); //Give SharePoint some time to update the changes
                            web.AllowUnsafeUpdates = false;

                        }
                    }
                });
            }
            catch (Exception err)
            {
                LogErrorHelper objErr = new LogErrorHelper();
                objErr.logErrorEmail(APP_NAME, err, "Error at AddListItem function");
                objErr = null;

                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(APP_NAME, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, err.Message.ToString(), err.StackTrace);

            }
        }


        /// <summary>
        ///     Add a new choice column to a specific SharePoint list
        /// </summary>
        /// <param name="spSiteUrl"></param>
        /// <param name="listName"></param>
        /// <param name="displayColName"></param>
        /// <param name="arrChoices"></param>
        public void CreateNewColumnListChoiceType(string spSiteUrl, string listName, string displayColName, string[] arrChoices, string Description)
        {

            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(spSiteUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SPList list = web.Lists[listName];

                            //check if column exist
                            if (list.Fields.ContainsField(displayColName) == false)
                            {   //Only add new column if one does not yet exist
                                web.AllowUnsafeUpdates = true;
                                list.Fields.Add(displayColName, SPFieldType.Choice, false);

                                var fldChoice = list.Fields[displayColName] as SPFieldChoice;

                                if (fldChoice != null)
                                {
                                    string defaultChoiceValue = "";
                                    for (int i = 0; i <= arrChoices.Length - 1; i++)
                                    {
                                        if (i == 0)
                                            defaultChoiceValue = arrChoices[i];

                                        fldChoice.Choices.Add(arrChoices[i].ToString());
                                    }
                                    fldChoice.DefaultValue = defaultChoiceValue;
                                    fldChoice.Description = Description;
                                    fldChoice.Update();
                                }
                                list.Update();
                                web.Update();
                                web.AllowUnsafeUpdates = false;
                            }
                        }
                    }
                });
            }
            catch (Exception err)
            {
                LogErrorHelper objErr = new LogErrorHelper();
                objErr.logErrorEmail(APP_NAME, err, "Error at CreateNewColumnListChoiceType function");
                objErr = null;

                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(APP_NAME, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, err.Message.ToString(), err.StackTrace);

            }
        }


        /// <summary>
        ///     Add a new Note column into a specific list
        /// </summary>
        /// <param name="spSiteUrl"></param>
        /// <param name="listName"></param>
        /// <param name="displayColName"></param>
        /// <param name="isMandatory"></param>
        /// <param name="isMultiLine"></param>
        public void CreateNewColumnListNoteType(string spSiteUrl, string listName, string displayColName, bool isMandatory, bool isMultiLine)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(spSiteUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SPList list = web.Lists[listName];

                            //check if column exist
                            if (list.Fields.ContainsField(displayColName) == false)
                            {//Only add new column if one does not yet exist
                                web.AllowUnsafeUpdates = true;

                                list.Fields.Add(displayColName, SPFieldType.Note, isMandatory);

                                if (isMultiLine)
                                {
                                    var fldMultiLine = list.Fields[displayColName] as SPFieldMultiLineText;

                                    if (fldMultiLine != null)
                                    {
                                        fldMultiLine.NumberOfLines = 6;
                                        fldMultiLine.Update();
                                    }
                                }
                                list.Update();
                                web.Update();
                                web.AllowUnsafeUpdates = false;
                            }
                        }
                    }
                });
            }
            catch (Exception err)
            {
                LogErrorHelper objErr = new LogErrorHelper();
                objErr.logErrorEmail(APP_NAME, err, "Error at CreateNewColumnListNoteType function");
                objErr = null;

                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(APP_NAME, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, err.Message.ToString(), err.StackTrace);

            }
        }

        /// <summary>
        ///     Get value content of a specific list
        /// </summary>
        /// <param name="spListName"></param>
        /// <param name="spQueryStr"></param>
        /// <param name="spFieldName"></param>
        /// <returns></returns>
        public string GetSharePointListItem(string spListName, string spQueryStr, string spFieldName, bool displayTitle)
        {
            try
            {
                string spListContentItem = "";

                using (SPWeb web = SPContext.Current.Web)
                {

                    SPList spList = web.Lists[spListName];
                    SPQuery spQuery = new SPQuery();
                    spQuery.Query = spQueryStr;
                    SPListItemCollection spListItems = spList.GetItems(spQuery);

                    if (spListItems.Count >= 1)
                    {
                        foreach (SPListItem item in spListItems)
                        {

                            if (displayTitle)
                            {
                                string strDate = "";
                                string title = "";

                                if (spListName.ToString().ToLower() == "news")
                                {
                                    DateTime dat = Convert.ToDateTime(item["PublishDate"].ToString());
                                    strDate = "<div class='NewsDate'>" + dat.Day.ToString() + " " + getMonthName(dat.Month) + " " + dat.Year.ToString() + "</div><br>";
                                }

                                title = strDate + "<div class='NewsItemTitle'>" + item["Title"].ToString() + "</div><br>";

                                spListContentItem = title + item[spFieldName].ToString();
                            }
                            else
                            {
                                spListContentItem = item[spFieldName].ToString();
                            }

                        }
                    }
                }

                return spListContentItem;
            }
            catch (Exception err)
            {
                LogErrorHelper objErr = new LogErrorHelper();
                objErr.logErrorEmail(APP_NAME, err, "Error at GetSharePointListItem function");
                objErr = null;

                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(APP_NAME, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, err.Message.ToString(), err.StackTrace);

                return string.Empty;
            }
        }

        /// <summary>
        ///     Function to insert a log item into a SharePoint log list
        /// </summary>
        /// <param name="spSiteUrl"></param>
        /// <param name="listName"></param>
        /// <param name="errTitle"></param>
        /// <param name="errMsg"></param>
        public void AddLogListItem(string spSiteUrl, string errTitle, string errMsg, string docFileName, string pdfFileName, string itemID, bool convertStatus)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(spSiteUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            string listName = GetRCRSettingsItem("SPLogList");
                            SPFieldUserValue loginUser = GetCurrentUser(web);

                            web.AllowUnsafeUpdates = true;
                            SPList list = web.Lists[listName];
                            SPListItem listItem = list.Items.Add();
                            listItem["Title"] = errTitle;
                            listItem["DescriptionLog"] = errMsg;
                            listItem["DocFileName"] = docFileName;
                            listItem["PdfFileName"] = pdfFileName;
                            listItem["DocID"] = itemID;
                            listItem["ConvertDate"] = DateTime.Now;

                            if (convertStatus == true)
                            {
                                listItem["ConvertStatus"] = "Success";
                            }
                            else
                            {
                                listItem["ConvertStatus"] = "Failed";
                            }

                            if (loginUser != null)
                                listItem["Modified By"] = loginUser;

                            listItem.Update();
                            Thread.Sleep(1000); //Give SharePoint some time to update the changes

                            web.Update();
                            Thread.Sleep(1000); //Give SharePoint some time to update the changes
                            web.AllowUnsafeUpdates = false;

                        }
                    }
                });
            }
            catch (Exception err)
            {
                LogErrorHelper objErr = new LogErrorHelper();
                objErr.logErrorEmail(APP_NAME, err, "Error at AddLogListItem function");
                objErr = null;

                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(APP_NAME, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, err.Message.ToString(), err.StackTrace);
            }
        }

        /// <summary>
        ///     Function to get current login SP user
        /// </summary>
        /// <param name="oWeb"></param>
        /// <returns></returns>
        public SPFieldUserValue GetCurrentUser(SPWeb oWeb)
        {
            try
            {
                //Variable to store the user
                SPUser oUser = oWeb.CurrentUser;
                SPFieldUserValue loginUser = new SPFieldUserValue(oWeb, oUser.ID, oUser.LoginName);
                oUser = null;

                return loginUser;
            }
            catch (Exception err)
            {
                LogErrorHelper objErr = new LogErrorHelper();
                objErr.logErrorEmail(APP_NAME, err, "Error at GetCurrentUser function");
                objErr = null;

                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(APP_NAME, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, err.Message.ToString(), err.StackTrace);
                SPFieldUserValue ret = null;

                return ret;
            }

        }

        /// <summary>
        ///     Get configuration settings from the list
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public string GetRCRSettingsItem(string category)
        {
            string description = string.Empty;
            try
            {
                if (category == string.Empty)
                {
                    return description;
                }

               
                using (SPWeb web = SPContext.Current.Web)
                {
                    SPList spList = web.Lists["PDF Settings"];
                    SPQuery spQuery = new SPQuery();
                    spQuery.Query = @"<Where><Eq><FieldRef Name='SettingsCategory'/><Value Type='CHOICE'>" + category + "</Value></Eq></Where>";
                    spQuery.ViewFields = String.Concat("<FieldRef Name='SettingsValue'/>");
                    spQuery.ViewFieldsOnly = true;
                    spQuery.RowLimit = 1;
                    SPListItemCollection spListItems = spList.GetItems(spQuery);
                    if (spListItems != null)
                    {
                        foreach (SPListItem spListItem in spListItems)
                        {
                            if (spListItem["SettingsValue"] != null)
                            {
                                description = spListItem["SettingsValue"].ToString();
                            }
                        }
                    }
                }
                return description;
            }
            catch (Exception err)
            {
                LogErrorHelper objErr = new LogErrorHelper();
                objErr.logErrorEmail(APP_NAME, err, "Error at GetRCRSettingsItem function");
                objErr = null;

                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory(APP_NAME, TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, err.Message.ToString(), err.StackTrace);
                return string.Empty;
            }
        }

        /// <summary>
        ///     Function to convert month integer to the month long name 
        /// </summary>
        /// <param name="intMth"></param>
        /// <returns>
        ///      Month long name
        /// </returns>
        private string getMonthName(int intMth)
        {
            string strMth = "";

            switch (intMth)
            {
                case 2:
                    strMth = "February";
                    break;

                case 3:
                    strMth = "March";
                    break;

                case 4:
                    strMth = "April";
                    break;

                case 5:
                    strMth = "May";
                    break;

                case 6:
                    strMth = "June";
                    break;

                case 7:
                    strMth = "July";
                    break;

                case 8:
                    strMth = "August";
                    break;

                case 9:
                    strMth = "September";
                    break;

                case 10:
                    strMth = "October";
                    break;

                case 11:
                    strMth = "November";
                    break;

                case 12:
                    strMth = "December";
                    break;

                default:
                    strMth = "January";
                    break;
            }

            return strMth;
        }

        #endregion

    


    }
}
