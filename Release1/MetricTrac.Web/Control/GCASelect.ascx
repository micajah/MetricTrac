<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GCASelect.ascx.cs" Inherits="MetricTrac.Control.GCASelect" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<script type="text/javascript">

    var ddlGCAText=null;
    var ddlGCAValue = null;

    function DdlGCAClientNodeClicked(tvGCAs, eventArgs) {
        var node = eventArgs.get_node();
        var combo = $find("<%= ddlGCA.ClientID %>");
        
        ddlGCAText = node.get_attributes().getAttribute("FullPath");
        ddlGCAValue = node.get_value();

        combo.set_text(ddlGCAText);
        combo.set_value(ddlGCAValue);
    }
    function DllGCAItemReqesting(ddlGCA, eventArgs) {
        ddlGCAText = null;
        var tvGCAs = $find('<%= ((Telerik.Web.UI.RadTreeView)ddlGCA.Items[0].FindControl("tvGCAs")).ClientID %>');
        var tvGCAsNodeCollection = tvGCAs.get_nodes();
        var SelectedNode = null;
        var FullSelectedNode = null;
    
    
        eventArgs.set_cancel(true);
        var InputText = eventArgs.get_text();
        var EndPos = -1;
        var BeginPos = 0;
        var GCAText;
        
        while (true) {
            EndPos = InputText.indexOf(">", BeginPos);
            if (EndPos < 0) {
                GCAText = InputText.substring(BeginPos);
            }
            else {
                GCAText = InputText.substring(BeginPos, EndPos);
                BeginPos = EndPos + 1;
            }
            
            while (GCAText.length > 0 && GCAText.charAt(0) == " ") GCAText = GCAText.substring(1);
            while (GCAText.length > 0 && GCAText.charAt(GCAText.length - 1) == " ") GCAText = GCAText.substring(0, GCAText.length - 1);
            if (EndPos < 0) break;
            
            while (GCAText.length > 0 && GCAText.charAt(0) == " ") GCAText = GCAText.substring(1);
            while (GCAText.length > 0 && GCAText.charAt(GCAText.length - 1) == " ") GCAText = GCAText.substring(0, GCAText.length - 1);
            for (var i = 0; i < tvGCAsNodeCollection.get_count(); i++) {
                if (tvGCAsNodeCollection.getNode(i).get_text().toLowerCase() == GCAText.toLowerCase()) {
                    SelectedNode = tvGCAsNodeCollection.getNode(i);
                    FullSelectedNode = SelectedNode;
                    tvGCAsNodeCollection = SelectedNode.get_nodes();
                    break;
                }
            }
        }

        if (GCAText.length > 0) {
            for (var i = 0; i < tvGCAsNodeCollection.get_count(); i++) {
                if (tvGCAsNodeCollection.getNode(i).get_text().length >= GCAText.length) {
                    if(tvGCAsNodeCollection.getNode(i).get_text().substring(0,GCAText.length).toLowerCase() == GCAText.toLowerCase()){
                        SelectedNode = tvGCAsNodeCollection.getNode(i);
                        if(tvGCAsNodeCollection.getNode(i).get_text().toLowerCase() == GCAText.toLowerCase()) FullSelectedNode = SelectedNode;
                        else FullSelectedNode = null;
                        break;
                    }
                }
            }
        }

        tvGCAsNodeCollection = tvGCAs.get_allNodes();
        for (var i = 0; i < tvGCAsNodeCollection.length; i++) {
            tvGCAsNodeCollection[i].collapse();
            tvGCAsNodeCollection[i].unselect();
        }

        if (SelectedNode != null) {
            SelectedNode.select();
            var Parent = SelectedNode;
            do{
                Parent.expand();
                Parent = Parent.get_parent();
            } while (Parent != null && Parent.get_text)
            SelectedNode.scrollIntoView();
        }

        if (FullSelectedNode == null) {
            ddlGCA.set_value(0);
        } else {
            ddlGCA.set_value(FullSelectedNode.get_value());
        }
    }
    
    var ddlGCAAlert;
    function GCAAlert()
    {
        alert('Can not find "' + ddlGCAAlert.get_text() + '" GCA. Selection is ignored. Correct GCA name and try again.');
    }
    
    function ddlGCASubmitChangeValue(ddlGCA)
    {
        var val = ddlGCA.get_value();
        var txt = ddlGCA.get_text();
        
        if ((val == null || val==0) && txt!=null && txt!="") {
            <%if(AutoPostBack){ %>
                ddlGCAAlert = ddlGCA;
                window.setTimeout("GCAAlert()",1);
            <%} %>
            return;
        }            
        
        <%if(mOnClientGCAChange!=null){ %>
            <%=mOnClientGCAChange%>(val);
        <%} %>
        <%if(AutoPostBack){ %>
            __doPostBack('<%=ddlGCA.ClientID%>','');
        <%} %>
    }

    function DdlGCADropDownClosed(ddlGCA, eventArgs) {
        if (ddlGCAText != null) {
            ddlGCA.set_text(ddlGCAText);
            ddlGCA.set_value(ddlGCAValue);
            ddlGCAText = null;
        }
        ddlGCASubmitChangeValue(ddlGCA);
    }
    
    function DdlGCAKeyPressing(ddlGCA, eventArgs)
    {
        if(eventArgs.get_domEvent().keyCode != 13) return;
        var dom = eventArgs.get_domEvent();
        dom.rawEvent.returnValue = false;
    }
        
</script>

<telerik:RadComboBox ID="ddlGCA" EnableLoadOnDemand="true" Height="300" Width="505" DropDownWidth="505" runat="server" 
    LoadingMessage="Loading GCA List..." ItemRequestTimeout="333" OnSelectedIndexChanged="ddlGCA_SelectedIndexChanged"
    OnClientItemsRequesting="DllGCAItemReqesting" OnClientDropDownClosed="DdlGCADropDownClosed" OnClientKeyPressing="DdlGCAKeyPressing" >
<Items>
    <telerik:RadComboBoxItem runat="server" Text="" />
</Items>
<ItemTemplate>
    <telerik:RadTreeView runat="server" ID="tvGCAs" OnClientNodeClicked="DdlGCAClientNodeClicked" CollapseAnimation-Type="None" ExpandAnimation-Type="None" Height="297">
    </telerik:RadTreeView>
</ItemTemplate>
</telerik:RadComboBox>