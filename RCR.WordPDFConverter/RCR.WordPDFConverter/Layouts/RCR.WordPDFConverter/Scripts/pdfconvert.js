function RCROpenDialog(listid, listitemid) {
    var options = SP.UI.$create_DialogOptions();
    options.url = '/_layouts/RCR.WordPDFConverter/PDFConversion.aspx?listid=' + listid + "&listitemid=" + listitemid;
    options.width = 800;
    options.height = 200;
    options.dialogReturnValueCallback = Function.createDelegate(null, CloseCallback);
    SP.UI.ModalDialog.showModalDialog(options);
}

var messageId;


function CloseCallback(result, target) {
    if (result === SP.UI.DialogResult.OK) {
        if (target != 'bye') {
            //Get id
            messageId = SP.UI.Notify.addNotification('&lt;img src=&quot;_layouts/images/loading.gif&quot;&gt;Saying Please wait &lt;b&gt;' + target + '&lt;/b&gt;...', true, 'Dialog Response', null);
            //do someother work here.
        }
        else //target=='bye'
        {
            SP.UI.Notify.removeNotification(messageId); //simple way to remove it.
            return;
        }
    }
    if (result === SP.UI.DialogResult.cancel) {
        SP.UI.Notify.addNotification('Operation was cancelled...', false, '', null);
    }
}


function ConvertDocument(listid, selecteditemid) {
   
    var site = "http://" + window.location.hostname;
    var options = SP.UI.$create_DialogOptions();

    options.url = '/_layouts/RCR.WordPDFConverter/MultiPDFConversion.aspx?listid=' + listid + '&selecteditemid=' + selecteditemid + "&returnUrl=" + site;
    options.width = 1000;
    options.height = 1000;
    options.dialogReturnValueCallback = Function.createDelegate(null, CloseCallback);
    SP.UI.ModalDialog.showModalDialog(options);;
}