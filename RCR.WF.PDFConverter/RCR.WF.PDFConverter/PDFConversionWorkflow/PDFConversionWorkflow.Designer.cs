using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace RCR.WF.PDFConverter.PDFConversionWorkflow
{
    public sealed partial class PDFConversionWorkflow
    {
        #region Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
        private void InitializeComponent()
        {
            this.CanModifyActivities = true;
            System.Workflow.Runtime.CorrelationToken correlationtoken1 = new System.Workflow.Runtime.CorrelationToken();
            System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind2 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind3 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind4 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.Activities.CodeCondition codecondition1 = new System.Workflow.Activities.CodeCondition();
            System.Workflow.ComponentModel.ActivityBind activitybind6 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind5 = new System.Workflow.ComponentModel.ActivityBind();
            this.onWorkflowItemChanged1 = new Microsoft.SharePoint.WorkflowActions.OnWorkflowItemChanged();
            this.logToHistoryListActivity2 = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.sendUserEmail = new Microsoft.SharePoint.WorkflowActions.SendEmail();
            this.sendEmail = new System.Workflow.Activities.CodeActivity();
            this.codeActivity1 = new System.Workflow.Activities.CodeActivity();
            this.whileActivity1 = new System.Workflow.Activities.WhileActivity();
            this.logToHistoryListActivity1 = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.onWorkflowActivated1 = new Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated();
            // 
            // onWorkflowItemChanged1
            // 
            this.onWorkflowItemChanged1.AfterProperties = null;
            this.onWorkflowItemChanged1.BeforeProperties = null;
            correlationtoken1.Name = "workflowToken";
            correlationtoken1.OwnerActivityName = "PDFConversionWorkflow";
            this.onWorkflowItemChanged1.CorrelationToken = correlationtoken1;
            this.onWorkflowItemChanged1.Name = "onWorkflowItemChanged1";
            this.onWorkflowItemChanged1.Invoked += new System.EventHandler<System.Workflow.Activities.ExternalDataEventArgs>(this.onWorkflowItemChanged1_Invoked);
            // 
            // logToHistoryListActivity2
            // 
            this.logToHistoryListActivity2.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logToHistoryListActivity2.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logToHistoryListActivity2.HistoryDescription = "";
            this.logToHistoryListActivity2.HistoryOutcome = "Document Conversion Workflow Finished.";
            this.logToHistoryListActivity2.Name = "logToHistoryListActivity2";
            this.logToHistoryListActivity2.OtherData = "";
            this.logToHistoryListActivity2.UserId = -1;
            // 
            // sendUserEmail
            // 
            this.sendUserEmail.BCC = null;
            activitybind1.Name = "PDFConversionWorkflow";
            activitybind1.Path = "Email_Body1";
            this.sendUserEmail.CC = null;
            this.sendUserEmail.CorrelationToken = correlationtoken1;
            this.sendUserEmail.Enabled = false;
            activitybind2.Name = "PDFConversionWorkflow";
            activitybind2.Path = "sendEmail_From1";
            this.sendUserEmail.Headers = null;
            this.sendUserEmail.IncludeStatus = false;
            this.sendUserEmail.Name = "sendUserEmail";
            activitybind3.Name = "PDFConversionWorkflow";
            activitybind3.Path = "sendEmail_Subject1";
            activitybind4.Name = "PDFConversionWorkflow";
            activitybind4.Path = "sendEmail_To1";
            this.sendUserEmail.MethodInvoking += new System.EventHandler(this.sendUserEmail_MethodInvoking);
            this.sendUserEmail.SetBinding(Microsoft.SharePoint.WorkflowActions.SendEmail.ToProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind4)));
            this.sendUserEmail.SetBinding(Microsoft.SharePoint.WorkflowActions.SendEmail.SubjectProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind3)));
            this.sendUserEmail.SetBinding(Microsoft.SharePoint.WorkflowActions.SendEmail.BodyProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            this.sendUserEmail.SetBinding(Microsoft.SharePoint.WorkflowActions.SendEmail.FromProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind2)));
            // 
            // sendEmail
            // 
            this.sendEmail.Name = "sendEmail";
            this.sendEmail.ExecuteCode += new System.EventHandler(this.sendEmail_ExecuteCode);
            // 
            // codeActivity1
            // 
            this.codeActivity1.Name = "codeActivity1";
            this.codeActivity1.ExecuteCode += new System.EventHandler(this.codeActivity1_ExecuteCode);
            // 
            // whileActivity1
            // 
            this.whileActivity1.Activities.Add(this.onWorkflowItemChanged1);
            codecondition1.Condition += new System.EventHandler<System.Workflow.Activities.ConditionalEventArgs>(this.EvaluateCondition);
            this.whileActivity1.Condition = codecondition1;
            this.whileActivity1.Name = "whileActivity1";
            // 
            // logToHistoryListActivity1
            // 
            this.logToHistoryListActivity1.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.logToHistoryListActivity1.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.logToHistoryListActivity1.HistoryDescription = "";
            this.logToHistoryListActivity1.HistoryOutcome = "Document Conversion Workflow Started";
            this.logToHistoryListActivity1.Name = "logToHistoryListActivity1";
            this.logToHistoryListActivity1.OtherData = "";
            this.logToHistoryListActivity1.UserId = -1;
            activitybind6.Name = "PDFConversionWorkflow";
            activitybind6.Path = "workflowId";
            // 
            // onWorkflowActivated1
            // 
            this.onWorkflowActivated1.CorrelationToken = correlationtoken1;
            this.onWorkflowActivated1.EventName = "OnWorkflowActivated";
            this.onWorkflowActivated1.Name = "onWorkflowActivated1";
            activitybind5.Name = "PDFConversionWorkflow";
            activitybind5.Path = "workflowProperties";
            this.onWorkflowActivated1.Invoked += new System.EventHandler<System.Workflow.Activities.ExternalDataEventArgs>(this.onWorkflowActivated1_Invoked);
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowIdProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind6)));
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowPropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind5)));
            // 
            // PDFConversionWorkflow
            // 
            this.Activities.Add(this.onWorkflowActivated1);
            this.Activities.Add(this.logToHistoryListActivity1);
            this.Activities.Add(this.whileActivity1);
            this.Activities.Add(this.codeActivity1);
            this.Activities.Add(this.sendEmail);
            this.Activities.Add(this.sendUserEmail);
            this.Activities.Add(this.logToHistoryListActivity2);
            this.Name = "PDFConversionWorkflow";
            this.CanModifyActivities = false;

        }

        #endregion

        private CodeActivity sendEmail;

        private WhileActivity whileActivity1;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logToHistoryListActivity1;

        private Microsoft.SharePoint.WorkflowActions.OnWorkflowItemChanged onWorkflowItemChanged1;

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity logToHistoryListActivity2;

        private CodeActivity codeActivity1;

        private Microsoft.SharePoint.WorkflowActions.SendEmail sendUserEmail;

        private Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated onWorkflowActivated1;




















    }
}
