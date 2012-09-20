<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MetricFilter.ascx.cs" Inherits="MetricTrac.Control.MetricFilter" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register src="~/Control/OrglocationMultipick.ascx" tagname="OrgLocationSelect" tagprefix="mts" %>
<%@ Register Src="~/Control/GCASelect.ascx" TagPrefix="uc" TagName="GCASelect" %>
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
    <telerik:RadWindow runat="server" ID="rwSaveFilter" Modal="true" VisibleStatusbar="false" Height="500px" Width="600px" DestroyOnClose="false" OnClientClose="clientCloseSaveFilter"/>
    <script type="text/javascript">
        function OpenDialogWindow() {
            var oWnd = $find("<%=rwSaveFilter.ClientID%>");
            oWnd.argument = null;
            oWnd.setUrl('SaveFilter.aspx');
            oWnd.Center();
            oWnd.show();
        }
        
        // update after popup window close
        function clientCloseSaveFilter(sender, args) {
            if (sender.argument != "Save") return;
            var rapSaved = $find("<%= rapSaved.ClientID %>");
            rapSaved.ajaxRequest();
        }
        
        // update metric list
        function MetricSelectAjax() {            
            var rapMetricSelect = $find("<%= rapMetricSelect.ClientID %>");
            if (rapMetricSelect == null) return;
            try {
                rapMetricSelect.ajaxRequest();
            } catch (ex) { }
        }

        function MetricFilterSelect() {
        	var hfSaved = $get("<%= hfSaved.ClientID %>");
        	hfSaved.value = "1";
        	SavedCallBack();
        }
        
        // select saved filter 
        function SavedCallBack() {            
            __doPostBack('<%=lFilter.UniqueID%>', '');
        }
        
        // change group by mode
        var cookieName = '__MetricsAndGroupBy';
        function SaveOnClient() {
        	var ddlGroup = document.getElementById('<%=ddlGroup.ClientID%>');
        	if (ddlGroup != null) document.cookie = cookieName + "=" + ddlGroup.options[ddlGroup.selectedIndex].value;
        }
    </script>
</div>
    <asp:PlaceHolder runat="server" ID="phGroup" Visible="false">
         <table cellspacing="0" cellpadding="3"  class="filters"><tr>
            <td style="color:#989898;font-weight:bold;height:33"><nobr>Group by</nobr></td>
                        <td><asp:DropDownList runat="server" ID="ddlGroup" AutoPostBack="true" OnSelectedIndexChanged="ddlGroup_SelectedIndexChanged"  >
                            <asp:ListItem Text="Location" Value="False" />
                            <asp:ListItem Text="Metric" Value="True" />
                        </asp:DropDownList></td>
                        <td style="background-color:White"><nobr>&nbsp; &nbsp;</nobr></td>
         </tr></table>
    </asp:PlaceHolder>            
    <asp:PlaceHolder runat="server" ID="phFilter">
            <table cellspacing="0" cellpadding="3" class="filters" style="display:inline-table; float:left" >
                <tr>
                    <td style="color:#989898;font-weight:bold"><nobr>Filters</nobr></td>
                    <td><table><tr>
                        <td><telerik:RadAjaxPanel runat="server" ID="rapSaved" LoadingPanelID="lpSaved" EnableEmbeddedScripts="true" EnableAJAX="true" OnAjaxRequest="rapSaved_AjaxRequest">
                            <telerik:RadComboBox runat="server" ID="rcbSaved" DataSourceID="dsMetricForm" DataTextField="Name" DataValueField="MetricFilterID" Width="105" DropDownWidth="300" EmptyMessage="Saved" ToolTip="Saved Filters" OnClientSelectedIndexChanged="MetricFilterSelect" OnDataBound="rcbSaved_DataBound"/>
                            <asp:HiddenField runat="server" ID="hfSaved" />
                            <telerik:RadAjaxLoadingPanel runat="server" ID="lpSaved" Wrap="true" Transparency="33" BackColor="LightGray" ><table style="height:100%; width:100%">
                                <tr><td valign="middle" align="center"><img alt="Loading ..." src="../images/loading4.gif" /></td></tr>
                            </table></telerik:RadAjaxLoadingPanel>
                        </telerik:RadAjaxPanel></td>
                        <td>                            
                            <mts:OrgLocationSelect ID="multiOrgLocationSelect" runat="server" Width="105px" OnClientOrgLocationChange="MetricSelectAjax" EmptyMessage="Location" />
                        </td>
                        <td><uc:GCASelect Width="105" runat="server" ID="sGCA" OnClientGCAChange="MetricSelectAjax" EmptyMessage="G.C.A." ToolTip="Group Category Aspect" /></td>
                        <asp:PlaceHolder runat="server" ID="phTwoRow1" Visible="false"></tr><tr></asp:PlaceHolder>
                        <td><telerik:RadComboBox runat="server" ID="rcbPIForm" DataSourceID="dsPIForm" DataTextField="Name" DataValueField="PerformanceIndicatorFormID" AppendDataBoundItems="true" OnClientSelectedIndexChanged="MetricSelectAjax" Width="105" EmptyMessage="P.I. Form" ToolTip="Performance Indicator Form" DropDownWidth="500">
                            <Items><telerik:RadComboBoxItem /></Items>
                        </telerik:RadComboBox></td>
                        <td><telerik:RadComboBox runat="server" ID="rcbPI" DataSourceID="dsPI" DataTextField="Name" DataValueField="PerformanceIndicatorID" AppendDataBoundItems="true" OnClientSelectedIndexChanged="MetricSelectAjax" EmptyMessage="P.I." ToolTip="Performance Indicator" Width="105" DropDownWidth="500">
                            <Items>
                                <telerik:RadComboBoxItem />
                                <telerik:RadComboBoxItem Value="00000000-0000-0000-0000-000000000000" Text="<<< Not Assigned to Any PI >>>" Visible="false" />
                            </Items>
                        </telerik:RadComboBox></td>
                        <td runat="server" id="tdMetric">
                            <telerik:RadAjaxPanel runat="server" ID="rapMetricSelect" LoadingPanelID="lpMetricSelect" EnableEmbeddedScripts="true" EnableAJAX="true" >
                                <telerik:RadComboBox runat ="server" ID="rcbMetric" Width="105px" DataTextField="Name" DataValueField="MetricID" EmptyMessage="Metric" ToolTip="Metric" DropDownWidth="500"/>
                            </telerik:RadAjaxPanel>
                        </td>
                        <td><telerik:RadComboBox runat="server" ID="rcbUser" DataSourceID="dsUser" DataTextField="FullName" DataValueField="UserID" OnClientSelectedIndexChanged="MetricSelectAjax" AppendDataBoundItems="true" EmptyMessage="Collector" ToolTip="Collector" Width="105" DropDownWidth="300">
                                <Items><telerik:RadComboBoxItem /></Items>
                        </telerik:RadComboBox></td>
                        <td runat="server" id="tdBaseDate"><telerik:RadDatePicker runat="server" ID="dpBaseDate" DateInput-DateFormat="MM/dd/yyyy" Width="81" DatePopupButton-ImageUrl="~/images/buttons/RightDown.gif" DatePopupButton-HoverImageUrl="~/images/buttons/RightDownSelected.gif"/> </td>
                    </tr></table></td>
                    <td><nobr><span style="font-weight:bold"><asp:Button runat="server" ID="lFilter" Text="Apply" OnClick="lFilter_Click" /></span></nobr></td>
                    <td><nobr><asp:LinkButton runat="server" ID="lbClear" OnClick="lbClear_Click">Clear</asp:LinkButton></nobr></td>
                    <td><nobr><asp:LinkButton runat="server" ID="lbSave" OnClick="lbSave_Click">Save</asp:LinkButton>&nbsp;</nobr></td>
                    <td runat="server" id="tdExport" visible="false"><nobr>
						<asp:HyperLink runat="server" ID="hlExport" NavigateUrl="~/home/ExportExcel.aspx">Export</asp:HyperLink>
					</td>
                </tr>
            </table>
    </asp:PlaceHolder>
<telerik:RadAjaxLoadingPanel runat="server" ID="lpMetricSelect" Wrap="true" Transparency="33" BackColor="LightGray" ><table style="height:100%; width:100%">
    <tr><td valign="middle" align="center"><img alt="Loading ..." src="../images/loading4.gif" /></td></tr>
</table></telerik:RadAjaxLoadingPanel>
<bll:BllDataSource runat="server" ID="dsUser" TableName="Mc_User" BllSelectMethod="List"/>
<bll:BllDataSource runat="server" ID="dsPIForm" TableName="PerformanceIndicatorForm" OrderBy="Name"/>
<bll:BllDataSource runat="server" ID="dsPI" TableName="PerformanceIndicator" OrderBy="Name"/>
<bll:BllDataSource runat="server" ID="dsMetricForm" TableName="MetricFilter" BllSelectMethod="List" />
