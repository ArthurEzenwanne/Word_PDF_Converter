<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MultiPDFConversion.aspx.cs" Inherits="RCR.WordPDFConverter.Layouts.RCR.WordPDFConverter.MultiPDFConversion" DynamicMasterPageFile="~masterurl/default.master" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="/_controltemplates/InputFormSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" src="/_controltemplates/ButtonSection.ascx" %> 
<%@ Register Tagprefix="wssawc" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> <%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
     <script language="javascript">
         function BtnCancel_Click() { 
             SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.cancel, 'Cancelled clicked'); 
         } 

         function BtnOk_Click() { 
             var form = document.forms.<%SPHttpUtility.NoEncode(Form.ClientID,Response.Output);%>; 
            var msg = form.<%SPHttpUtility.NoEncode(txtMessage.ClientID,Response.Output);%>.value; 
            SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.OK, msg); 
        } 
    </script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
     <style>
        .ms-descriptiontext {width: 60% !important;}
    </style>

    <table
     id="maintable"
     border="0"
     cellspacing="0"
     cellpadding="0"
     class="ms-propertysheet"
     width="100%"
    >
      <wssuc:InputFormSection Title="PDF Converter" runat="server">
        <Template_Description>
          <SharePoint:EncodedLiteral ID="EncodedLiteral1" runat="server"
                                       text="Converts DOCX to PDF" EncodeMethod='HtmlEncode'/>
        </Template_Description>
        <Template_InputFormControls>
          <wssuc:InputFormControl runat="server">
            <Template_Control>
              <table border="0" cellspacing="1">
                <tr>
                  <td class="ms-authoringcontrols" colspan="2" nowrap="nowrap">                  
                    <SharePoint:EncodedLiteral ID="lblInfo" runat="server" text="" EncodeMethod="NoEncode"  />
                    
                  </td>
                </tr>
                <tr>
                <td>
                <wssawc:InputFormTextBox Visible="false" title="Enter a message" class="ms-input" ID="txtMessage" Columns="35" Runat="server" maxlength="255" size="60" width="100%" />
            <asp:Panel ID="pnlStatus" runat="server" >
    <asp:Label id="lblStatus" runat="server" />
    </asp:Panel>  
                </td>
                </tr>
              </table>
            </Template_Control>
          </wssuc:InputFormControl>
        </Template_InputFormControls>
      </wssuc:InputFormSection>
      <wssuc:ButtonSection runat="server" ShowStandardCancelButton="False">
        <Template_Buttons>
          <asp:placeholder ID="Placeholder1" runat="server">
          <asp:Button CssClass="ms-ButtonHeightWidth"  ID="btnConvert" runat="server" Text="Convert" />
            <SeparatorHtml>
                <span id="idSpace" class="ms-SpaceBetButtons" />
            </SeparatorHtml>
          <asp:Button CssClass="ms-ButtonHeightWidth"  ID="btnCancel" runat="server" Text="Close" OnClientClick="BtnCancel_Click()" />     
          </asp:PlaceHolder>
        </Template_Buttons>
      </wssuc:ButtonSection>
    </table>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
   Document Converter
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
   Document Converter
</asp:Content>
