<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PerformanceIndicatorList.aspx.cs" Inherits="MetricTrac.PerformanceIndicatorList" MasterPageFile="~/MasterPage.master"%>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register src="~/Control/PerformanceIndicatorList.ascx" tagname="PerformanceIndicatorList" tagprefix="mts" %>
<%@ Register Src="~/Control/GCASelect.ascx" TagName="GCASelect" TagPrefix="mts" %>
<asp:Content ID="cntGCA" ContentPlaceHolderID="PageBody" runat="server">
    <div runat="server">
    <telerik:RadAjaxPanel runat="server" ID="rapPIList" LoadingPanelID="ralpPIList" EnableEmbeddedScripts="true" EnableAJAX="true">
        <table cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td class="GridHeader">Group/Category/Aspect</td><td style="width:10px;"></td>
                <td class="GridHeader">Sector</td>
            </tr>        
            <tr>
                <td>
                    <mts:GCASelect ID="GCAFilter" runat="server" AutoPostBack="true" EmptyMessage="" Width="300px"/></td><td></td>
                <td>
                    <telerik:RadComboBox runat="server" ID="ddlSector" DataSourceID="ldsSector" DataTextField="Name" DataValueField="SectorID" AppendDataBoundItems="true" AutoPostBack="true" Width="300">
                        <Items>
                            <telerik:RadComboBoxItem Text="" Value="" />
                        </Items>
                    </telerik:RadComboBox></td>
            </tr>
        </table>
        <mts:PerformanceIndicatorList ID="MainPerformanceIndicatorList" runat="server"/>
        <asp:LinqDataSource runat="server" ID="ldsSector" ContextTypeName="MetricTrac.Bll.LinqMicajahDataContext" TableName="Sector"/>
    </telerik:RadAjaxPanel>
    </div>
</asp:Content>