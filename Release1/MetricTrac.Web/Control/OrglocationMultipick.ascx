<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrglocationMultipick.ascx.cs" Inherits="MetricTrac.Control.OrglocationMultipick" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<div id="Div1" runat="server">
<style type="text/css">
.WSAClass
{
    background-color:#f3f3f3; 
}
.RadComboBoxDropDown .rcbItem, .RadComboBoxDropDown .rcbHovered, .RadComboBoxDropDown .rcbDisabled, .RadComboBoxDropDown .rcbLoading 
{
    padding-left:0px !important;
    padding-right:0px !important;
}
</style>
<script type="text/javascript">
    // Global variables
    var tvNodeClicked = false;
    
    // Combobox client events
    function comboClientDropDownClosing(ddlOrgLocation, eventArgs)
    {   
        eventArgs.set_cancel(tvNodeClicked);
        tvNodeClicked = false;
    }
    
    function comboClientDropDownClosed(ddlOrgLocation, eventArgs) {        
        <%if(mOnClientOrgLocationChange!=null){ %>
            <%=mOnClientOrgLocationChange%>();
        <%} %>
        
        <%if(AutoPostBack){ %>
            ObjectsCountInitialized = 0;
            __doPostBack('<%=ddlOrgLocation.ClientID%>','');
        <%} %>
    }
    
    function comboClientLoad(ddlOrgLocation)
    {   
        loadCustomText();
    }
    
    // Treeview client events
    function tvClientLoad(tvOrgLocations, eventArgs)
    {
        loadCustomText();
        var selectedNodes = tvOrgLocations.get_selectedNodes();
        for (var i = 0; i < selectedNodes.length; i++)
        {
            var parent = selectedNodes[i].get_parent();
            if (parent.expand)
                parent.expand();
            else
               selectedNodes[i].expand();
            selectedNodes[i].scrollIntoView();
        }
    }
     
    var ObjectsCountInitialized = 0;
    function loadCustomText()
    {
        ObjectsCountInitialized++;
        if (ObjectsCountInitialized >= 2)
        {
            var ddlOrgLocation = $find("<%= ddlOrgLocation.ClientID %>");
            var tvOrgLocations = ddlOrgLocation.get_items().getItem(0).findControl("tvOrgLocations");
            updateComboText(ddlOrgLocation, tvOrgLocations);
            ObjectsCountInitialized = 0;
        }
    }
    
    function tvClientNodeClicked(tvOrgLocations, eventArgs) {        
        tvNodeClicked = true;
        var combo = $find("<%= ddlOrgLocation.ClientID %>");
        updateComboText(combo, tvOrgLocations)
    }
    
    function updateComboText(combo, tvOrgLocations)
    {
        var selectedNodes = tvOrgLocations.get_selectedNodes();
        var selectedNodesCount = selectedNodes.length;
        combo.trackChanges();
        switch (selectedNodesCount)
        {
            case 0:
                var empty_text = combo.get_emptyMessage() != null ? combo.get_emptyMessage() : '';
                combo.set_text(empty_text);
                combo.set_value('');
            break;
            case 1:
                combo.set_text(selectedNodes[0].get_attributes().getAttribute("FullPath"));
                combo.set_value(selectedNodes[0].get_value());
            break;
            default:
                combo.set_text(selectedNodesCount + " Org Locations selected");
                combo.set_value(selectedNodesCount + 'items');
            break;
        }
        combo.commitChanges();
    }
    
    function FinishSelect_<%=this.ClientID %>()
    {
        var combo = $find("<%= ddlOrgLocation.ClientID %>");
        combo.hideDropDown();
        return false;
    }
    
    function ClearSelect_<%=this.ClientID %>()
    {        
        var ddlOrgLocation = $find("<%= ddlOrgLocation.ClientID %>");
        var tvOrgLocations = ddlOrgLocation.get_items().getItem(0).findControl("tvOrgLocations");
        tvOrgLocations.unselectAllNodes();
        updateComboText(ddlOrgLocation, tvOrgLocations);
        ddlOrgLocation.hideDropDown();
        return false;
    } 
</script>
<telerik:RadComboBox ID="ddlOrgLocation" EnableLoadOnDemand="false" Height="310" Width="243" DropDownWidth="406" runat="server" AllowCustomText="false" DropDownCssClass="WSAClass"
    OnSelectedIndexChanged="ddlOrgLocation_SelectedIndexChanged"
    OnClientDropDownClosed="comboClientDropDownClosed"
    OnClientDropDownClosing="comboClientDropDownClosing"
    OnClientLoad="comboClientLoad"
    ToolTip="Use &lt;Ctrl&gt;/&lt;Shift&gt; to select several Org Locations">
<Items>
    <telerik:RadComboBoxItem runat="server" Text="" />
</Items>
<ItemTemplate>
    <table cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td colspan="4">
                <telerik:RadTreeView runat="server" ID="tvOrgLocations"
                    DataTextField="Name" DataFieldParentID="ParentEntityNodeId" DataValueField="EntityNodeId" DataFieldID="EntityNodeId"
                    OnClientNodeClicked="tvClientNodeClicked"
                    OnClientLoad="tvClientLoad"
                    CollapseAnimation-Type="None" ExpandAnimation-Type="None" Height="218px" Width="400px" BorderWidth="1px" BorderColor="Gray"
                    MultipleSelect="true"
                    OnNodeDataBound="tvOrgLocations_NodeDataBound" BackColor="White">
                </telerik:RadTreeView></td></tr>
        <tr>
            <td colspan="4" style="padding:2px 5px 15px 5px;">Select Multiple Org Locations. <br />
                Hold down the -ctrl- or -command- key to select multiple ones.</td></tr>
        <tr>
            <td align="right"><asp:Button ID="btnFinish" runat="server" Text="Finish" Width="70px"/></td><td>&nbsp;</td>
            <td align="left"><asp:Button ID="btnClear" runat="server" Text="Clear" Width="70px"/></td><td>&nbsp;&nbsp;<br /><br /></td></tr>
    </table>
    </div>
</ItemTemplate>
</telerik:RadComboBox>
</div>