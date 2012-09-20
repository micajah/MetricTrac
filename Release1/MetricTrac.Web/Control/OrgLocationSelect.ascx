<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrgLocationSelect.ascx.cs" Inherits="MetricTrac.Control.OrgLocationSelect" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<div runat="server">
<script type="text/javascript">

    var ddlOrgLocationText=null;
    var ddlOrgLocationValue = null;

    function DdlOrgLocationClientNodeClicked(tvOrgLocations, eventArgs) {
        var node = eventArgs.get_node();
        var combo = $find("<%= ddlOrgLocation.ClientID %>");
        
        ddlOrgLocationText = node.get_attributes().getAttribute("FullPath");
        ddlOrgLocationValue = node.get_value();

        combo.set_text(ddlOrgLocationText);
        combo.set_value(ddlOrgLocationValue);
    }
    function DllOrgLocationItemReqesting(ddlOrgLocation, eventArgs) {
        ddlOrgLocationText = null;
        var tvOrgLocations = $find('<%= ((Telerik.Web.UI.RadTreeView)ddlOrgLocation.Items[0].FindControl("tvOrgLocations")).ClientID %>');
        var tvOrgLocationsNodeCollection = tvOrgLocations.get_nodes();
        var SelectedNode = null;
        var FullSelectedNode = null;
    
    
        eventArgs.set_cancel(true);
        var InputText = eventArgs.get_text();
        var EndPos = -1;
        var BeginPos = 0;
        var OrgLocationText;
        
        while (true) {
            EndPos = InputText.indexOf(">", BeginPos);
            if (EndPos < 0) {
                OrgLocationText = InputText.substring(BeginPos);
            }
            else {
                OrgLocationText = InputText.substring(BeginPos, EndPos);
                BeginPos = EndPos + 1;
            }
            
            while (OrgLocationText.length > 0 && OrgLocationText.charAt(0) == " ") OrgLocationText = OrgLocationText.substring(1);
            while (OrgLocationText.length > 0 && OrgLocationText.charAt(OrgLocationText.length - 1) == " ") OrgLocationText = OrgLocationText.substring(0, OrgLocationText.length - 1);
            if (EndPos < 0) break;
            
            while (OrgLocationText.length > 0 && OrgLocationText.charAt(0) == " ") OrgLocationText = OrgLocationText.substring(1);
            while (OrgLocationText.length > 0 && OrgLocationText.charAt(OrgLocationText.length - 1) == " ") OrgLocationText = OrgLocationText.substring(0, OrgLocationText.length - 1);
            for (var i = 0; i < tvOrgLocationsNodeCollection.get_count(); i++) {
                if (tvOrgLocationsNodeCollection.getNode(i).get_text().toLowerCase() == OrgLocationText.toLowerCase()) {
                    SelectedNode = tvOrgLocationsNodeCollection.getNode(i);
                    FullSelectedNode = SelectedNode;
                    tvOrgLocationsNodeCollection = SelectedNode.get_nodes();
                    break;
                }
            }
        }

        if (OrgLocationText.length > 0) {
            for (var i = 0; i < tvOrgLocationsNodeCollection.get_count(); i++) {
                if (tvOrgLocationsNodeCollection.getNode(i).get_text().length >= OrgLocationText.length) {
                    if(tvOrgLocationsNodeCollection.getNode(i).get_text().substring(0,OrgLocationText.length).toLowerCase() == OrgLocationText.toLowerCase()){
                        SelectedNode = tvOrgLocationsNodeCollection.getNode(i);
                        if(tvOrgLocationsNodeCollection.getNode(i).get_text().toLowerCase() == OrgLocationText.toLowerCase()) FullSelectedNode = SelectedNode;
                        else FullSelectedNode = null;
                        break;
                    }
                }
            }
        }

        tvOrgLocationsNodeCollection = tvOrgLocations.get_allNodes();
        for (var i = 0; i < tvOrgLocationsNodeCollection.length; i++) {
            tvOrgLocationsNodeCollection[i].collapse();
            tvOrgLocationsNodeCollection[i].unselect();
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
            ddlOrgLocation.set_value(0);
        } else {
            ddlOrgLocation.set_value(FullSelectedNode.get_value());
        }
    }
    
    var ddlOrgLocationAlert;
    function OrgLocationAlert()
    {
        alert('Can not find "' + ddlOrgLocationAlert.get_text() + '" Org Location. Selection is ignored. Correct Org Location name and try again.');
    }
    
    function OrgLocationSubmitChangeValue(ddlOrgLocation)
    {
        var val = ddlOrgLocation.get_value();
        var txt = ddlOrgLocation.get_text();
        
        if ((val == null || val==0) && txt!=null && txt!="") {
            <%if(AutoPostBack){ %>
                ddlOrgLocationAlert = ddlOrgLocation;
                window.setTimeout("OrgLocationAlert()",1);
            <%} %>
            return;
        }            
        
        <%if(mOnClientOrgLocationChange!=null){ %>
            <%=mOnClientOrgLocationChange%>(val);
        <%} %>
        
        <%if(AutoPostBack){ %>
            __doPostBack('<%=ddlOrgLocation.ClientID%>','');
        <%} %>
    }

    function DdlOrgLocationDropDownClosed(ddlOrgLocation, eventArgs) {
        if (ddlOrgLocationText != null) {
            ddlOrgLocation.set_text(ddlOrgLocationText);
            ddlOrgLocation.set_value(ddlOrgLocationValue);
            ddlOrgLocationText = null;
        }
        OrgLocationSubmitChangeValue(ddlOrgLocation);
    }
    
    function DdlOrgLocationKeyPressing(ddlOrgLocation, eventArgs)
    {
        if(eventArgs.get_domEvent().keyCode != 13) return;
        var dom = eventArgs.get_domEvent();
        dom.rawEvent.returnValue = false;
    }
        
</script>
<telerik:RadComboBox ID="ddlOrgLocation" EnableLoadOnDemand="true" Height="300" Width="505" DropDownWidth="505" runat="server"
    LoadingMessage="Loading Org Locations List..." ItemRequestTimeout="333" OnSelectedIndexChanged="ddlOrgLocation_SelectedIndexChanged"
    OnClientItemsRequesting="DllOrgLocationItemReqesting" OnClientDropDownClosed="DdlOrgLocationDropDownClosed" OnClientKeyPressing="DdlOrgLocationKeyPressing" >    
<Items>
    <telerik:RadComboBoxItem runat="server" Text="" />
</Items>
<ItemTemplate>
    <telerik:RadTreeView runat="server" ID="tvOrgLocations" OnClientNodeClicked="DdlOrgLocationClientNodeClicked" CollapseAnimation-Type="None" ExpandAnimation-Type="None" Height="297">
    </telerik:RadTreeView>
</ItemTemplate>
</telerik:RadComboBox>
</div>