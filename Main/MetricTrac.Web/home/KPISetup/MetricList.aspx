<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MetricList.aspx.cs" Inherits="MetricTrac.MetricList" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="~/Control/MetricList.ascx" tagname="MetricList" tagprefix="uc" %>
<%@ Register src="~/Control/MetricFilter.ascx" tagprefix="uc" tagname="MetricFilter" %>
<%@ Register src="~/Control/MetricCategorySelect.ascx" tagname="MetricCategorySelect" tagprefix="uc" %>
<asp:Content ID="cntMetric" ContentPlaceHolderID="PageBody" runat="server">
<div runat="server">
    <telerik:RadAjaxPanel runat="server" ID="rapMetricList" EnableEmbeddedScripts="true" EnableAJAX="true">
        <asp:Panel ID="pnlFilter" runat="server" DefaultButton="btnFilter">
            <table runat="server" id="tblListFilter" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td class="GridHeader">Metric Category</td><td style="width:10px;"></td>
                    <td class="GridHeader">Name/Description</td><td></td>
                </tr>        
                <tr>
                    <td>
                        <uc:MetricCategorySelect ID="mcs" runat="server" EmptyMessage="" Width="300px" AutoPostBack="false"/></td><td></td>
                    <td>
                        <asp:TextBox ID="txtSearch" runat="server" /></td>
                    <td style="padding-left:10px;">
                        <asp:Button ID="btnFilter" runat="server" Text="Filter" onclick="btnFilter_Click" Width="100px" UseSubmitBehavior="true" /></td>
                </tr>
            </table>
        </asp:Panel>
        <table cellpadding="0" cellspacing="0">
            <tr runat="server" id="trFilter" visible="false">
                <td>
                    <uc:MetricFilter runat="server" ID="cMericFilter" MetricVisible="false" OnUse="cMericFilter_Use" AddUnassigned="true" TwoRowMode="True" /><br /><br /><br /></td></tr>
            <tr>
                <td>
                    <div runat="server" id="divScroll">
                      <uc:MetricList ID="cMetricList" runat="server" Mode="List" /></div></td></tr>
            <tr runat="server" id="trButtons" visible="false">
                <td><br />
                    <asp:Button runat="server" ID="btSave" ForeColor="Black" Font-Bold="true" Text="Save Performance Indicator Metrics &amp; Close" onclick="btSave_Click" />
                    or
                    <a class="Mf_Cb" href="javascript:CloseOnReload(true)" style="font-weight:bold;">Cancel</a></td></tr>
        </table>
    </telerik:RadAjaxPanel>
</div>
</asp:Content>
