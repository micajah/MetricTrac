<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PerformanceIndicatorInfo.aspx.cs" Inherits="MetricTrac.PerformanceIndicatorInfo" MasterPageFile="~/MasterPage.master" %>
<%@ Register Src="~/Control/MetricList.ascx" TagName="MetricList" TagPrefix="c" %>
<asp:Content ID="cntMetricValueInput" ContentPlaceHolderID="PageBody" runat="server">
    <table border="0" cellpadding="0" cellspacing="8">
    <tr>
        <td colspan="2"><asp:Button ID="btnClose1" runat="server" Text="Close" class="SaveButtonCaption" Width="150px" OnClientClick="CloseInfoWindow(); return false;"  />      </td></tr>
    <tr>
        <td colspan="2" style="font-size:11pt; font-weight:bolder;">Performance Indicator Info</td></tr>
    <tr>
        <th class="TableHeaders">
            Name</th>
        <td>
            <asp:Label ID="lblName" runat="server" CssClass="TableFields" /></td></tr>
    <tr>
        <th class="TableHeaders">
            Alias</th>
        <td>
            <asp:Label ID="lblAlias" runat="server"  CssClass="TableFields"/></td></tr>
    <tr>
        <th class="TableHeaders">
            Code</th>
        <td>
            <asp:Label ID="lblCode" runat="server" CssClass="TableFields" /></td></tr>
    <tr>
        <th class="TableHeaders">
            GCA</th>
        <td>
            <asp:Label ID="lblGCA" runat="server" CssClass="TableFields" /></td></tr>
    <tr>
        <th class="TableHeaders">
            Sector</th>
        <td>
            <asp:Label ID="lblSector" runat="server" CssClass="TableFields" /></td></tr>
    <tr>
        <th class="TableHeaders">
            Requirement</th>
        <td>
            <asp:Label ID="lblRequirement" runat="server" CssClass="TableFields" /></td></tr>
    <tr>
        <th class="TableHeaders">
            Description</th>
        <td>
            <asp:Label ID="lblDescription" runat="server" CssClass="TableFields" /></td></tr>
    <tr>
        <th class="TableHeaders">
            Help Text</th>
        <td>
            <asp:Label ID="lblHelp" runat="server" CssClass="TableFields" /></td></tr>    
    </table><br />
    <table cellpadding="0" cellspacing="8" width="730">
        <tr>
            <td class="Mf_Cpt">Metrics</td></tr>
        <tr><td>
            <c:MetricList ID="cMetricList" runat="server" Mode="PIEdit" ShowDeleteColumn="false" /></td></tr>
        <tr><td>&nbsp;</td></tr>
        <tr>
            <td class="Mf_Cpt">Related Input Metrics</td></tr>
        <tr>
            <td><c:MetricList ID="RelatedMetric" runat="server" Mode="PiRef" /></td></tr>
        <tr>
            <td colspan="2"><asp:Button ID="btnClose2" runat="server" Text="Close" class="SaveButtonCaption" Width="150px" OnClientClick="CloseInfoWindow(); return false;"  />      </td></tr>
    </table>    
</asp:Content>