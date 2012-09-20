<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MetricFilter.ascx.cs" Inherits="MetricTrac.Control.MetricFilter" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register src="~/Control/OrglocationMultipick.ascx" tagname="OrgLocationSelect" tagprefix="mts" %>
<%@ Register Src="~/Control/GCASelect.ascx" TagPrefix="uc" TagName="GCASelect" %>
<%@ Register src="~/Control/MultiSelectList.ascx" tagname="MultiSelectList" tagprefix="uc" %>
<style type="text/css">
    .RadComboBox_Default .rcbInputCell .rcbEmptyMessage {
        font-style:normal;
    }
    .RadPicker_Default table.rcTable .rcInputCell {
        padding:0;
    }
    .RadPicker_Default td a {
        margin:0;
    }
</style>
<style type="text/css">
    .riTextBox 
    {
    	-x-system-font:none;
    	font-family:"Segoe UI",Arial,sans-serif;
        font-size:12px;
        font-size-adjust:none;
        font-stretch:normal;
        font-style:normal;
        font-variant:normal;
        font-weight:normal;
        line-height:normal;
        text-align:left;
        
        border-style:hidden;
        border-width:0;
        margin-left:3;
    }
    .rcInputCell
    {
    	background:transparent url(../images/buttons/border.gif) no-repeat scroll 0 0;
    }
</style>
<div runat="server">
    <telerik:RadWindow runat="server" ID="rwSaveFilter" Modal="true" VisibleStatusbar="false" Height="500px" Width="600px" DestroyOnClose="false" OnClientClose="clientCloseSaveFilter" Behaviors="Resize, Close, Move"/>
    <script type="text/javascript">
        function OpenDialogWindow() {
            var oWnd = $find("<%=rwSaveFilter.ClientID%>");
            oWnd.argument = null;
            oWnd.setUrl('SaveFilter.aspx');
            oWnd.Center();
            oWnd.show();
        }
        
        // update filter list after popup window close
        function clientCloseSaveFilter(sender, args) {
            if (sender.argument != null) {
                //setTimeout(function() { __doPostBack("<%= this.UniqueID %>", sender.argument); }, 500);
                setTimeout(function() {
                    var combo = $find("<%= rcbSaved.ClientID %>");
                    var ex_item = combo.findItemByValue(sender.argument);
                    if (ex_item != null) {
                        ex_item.select();
                    }
                    else {
                        var items = combo.get_items();
                        combo.trackChanges();
                        var comboItem = new Telerik.Web.UI.RadComboBoxItem();
                        comboItem.set_text("New Item");
                        comboItem.set_value(sender.argument);
                        items.add(comboItem);
                        combo.commitChanges();
                        comboItem.select();
                    }
                }, 500);
            }
        }
        
        // update metric list
        function MetricSelectAjax() {            
            var rapMetricSelect = $find("<%= rapMetricSelect.ClientID %>");
            if (rapMetricSelect == null) return;
            try {
                rapMetricSelect.ajaxRequest();
            } catch (ex) { }
        }
    </script>
</div>             
<table cellspacing="0" cellpadding="3" class="filters" style="display:inline-table; float:left" >
    <tr>
        <td style="color:#989898;font-weight:bold"><nobr>Filters</nobr></td>
        <td><table><tr>
            <td>
                <telerik:RadComboBox runat="server" ID="rcbSaved" DataSourceID="dsMetricForm" 
                    DataTextField="Name" DataValueField="MetricFilterID" Width="105" 
                    DropDownWidth="300" EmptyMessage="Saved" ToolTip="Saved Filters" AutoPostBack="true"
                    onselectedindexchanged="rcbSaved_SelectedIndexChanged"/></td>
            <td>                            
                <mts:OrgLocationSelect ID="multiOrgLocationSelect" runat="server" Width="105px" OnClientOrgLocationChange="MetricSelectAjax" EmptyMessage="Location"/>
            </td>
            <td><uc:GCASelect Width="105" runat="server" ID="sGCA" OnClientGCAChange="MetricSelectAjax" EmptyMessage="G.C.A." ToolTip="Group Category Aspect" /></td>
            <asp:PlaceHolder runat="server" ID="phTwoRowMode" Visible="false"></tr><tr></asp:PlaceHolder>                        
            <td>
                <uc:MultiSelectList ID="mslPI" runat="server" EntitiesName="Performance Indicators" OnClientSelectedIndexChanged="MetricSelectAjax" EmptyMessage="P.I." Width="105" AutoPostBack="false" ToolTip="Use &lt;Ctrl&gt;/&lt;Shift&gt; to select several Performance Indicators" /></td>
            <td runat="server" id="tdMetric">
                <telerik:RadAjaxPanel runat="server" ID="rapMetricSelect" LoadingPanelID="lpMetricSelect" EnableEmbeddedScripts="true" EnableAJAX="true" >
                    <uc:MultiSelectList ID="multiMetric" runat="server" Width="105" EntitiesName="Metrics" EmptyMessage="Metric" AutoPostBack="False" ToolTip="Use &lt;Ctrl&gt;/&lt;Shift&gt; to select several Metrics"/>                                
                </telerik:RadAjaxPanel>
            </td>
            <td><telerik:RadComboBox runat="server" ID="rcbUser" DataSourceID="dsUser" DataTextField="FullName" DataValueField="UserID" OnClientSelectedIndexChanged="MetricSelectAjax" AppendDataBoundItems="true" EmptyMessage="Collector" ToolTip="Collector" Width="105" DropDownWidth="300">
                    <Items><telerik:RadComboBoxItem /></Items>
            </telerik:RadComboBox></td>
            <td runat="server" id="tdBaseDate" visible="False"><telerik:RadDatePicker runat="server" ID="dpBaseDate" DateInput-DateFormat="MM/dd/yyyy" Width="81" DatePopupButton-ImageUrl="~/images/buttons/RightDown.gif" DatePopupButton-HoverImageUrl="~/images/buttons/RightDownSelected.gif" DateInput-EnableEmbeddedBaseStylesheet="false" DateInput-EnableEmbeddedSkins="false"/> </td>
        </tr></table></td>
        <td><nobr><span style="font-weight:bold"><asp:Button runat="server" ID="lFilter" Text="Apply" OnClick="lFilter_Click" /></span></nobr></td>
        <td><nobr><asp:LinkButton runat="server" ID="lbClear" OnClick="lbClear_Click">Clear</asp:LinkButton></nobr></td>
        <td><nobr><asp:LinkButton runat="server" ID="lbSave" OnClick="lbSave_Click">Save</asp:LinkButton>&nbsp;</nobr></td>
        <td runat="server" id="tdExport" visible="false"><nobr>
            <asp:LinkButton runat="server" ID="lbExport" OnClick="lbExport_Click">Export</asp:LinkButton>&nbsp;</td>
    </tr>
</table>
    
<telerik:RadAjaxLoadingPanel runat="server" ID="lpMetricSelect" Wrap="true" Transparency="33" BackColor="LightGray" ><table style="height:100%; width:100%">
    <tr><td valign="middle" align="center"><img alt="Loading ..." src="../images/loading4.gif" /></td></tr>
</table></telerik:RadAjaxLoadingPanel>
<bll:BllDataSource runat="server" ID="dsUser" TableName="Mc_User" BllSelectMethod="List"/>
<bll:BllDataSource runat="server" ID="dsMetricForm" TableName="MetricFilter" BllSelectMethod="List" />
