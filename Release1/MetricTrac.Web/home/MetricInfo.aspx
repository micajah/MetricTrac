<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MetricInfo.aspx.cs" Inherits="MetricTrac.MetricInfo" MasterPageFile="~/MasterPage.master" %>
<asp:Content ID="cntMetricValueInput" ContentPlaceHolderID="PageBody" runat="server">
<script type="text/javascript" language="javascript">    
    function GetRadWindow()
    {
         var oWindow = null;
         if (window.radWindow)
            oWindow = window.radWindow;     
         else if (window.frameElement.radWindow)
           oWindow = window.frameElement.radWindow;   
         return oWindow;
    }

    function CloseInfoWindow() {        
        var w = GetRadWindow();        
        w.close();
    }
</script>    
    <table border="0" cellpadding="0" cellspacing="8">
    <tr>
        <td colspan="2" style="font-size:11pt; font-weight:bolder;">Metric Info</td></tr>
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
            Category</th>
        <td>
            <asp:Label ID="lblCategory" runat="server" CssClass="TableFields" /></td></tr>
    <tr>
        <th class="TableHeaders">
            Metric Type</th>
        <td>
            <asp:Label ID="lblMetricType" runat="server" CssClass="TableFields" /></td></tr>
    <tr runat="server" id="rowDataType">
        <th class="TableHeaders">
            Data Type</th>
        <td>
            <asp:Label ID="lblDataType" runat="server" CssClass="TableFields" /></td></tr>    
    <tr runat="server" id="rowUoM">
        <th class="TableHeaders">
            Output Unit Of Measure</th>
        <td>
            <asp:Label ID="lblUoM" runat="server" CssClass="TableFields" /></td></tr>
    <tr runat="server" id="rowInputUoM">
        <th class="TableHeaders">
            Input Unit Of Measure</th>
        <td>
            <asp:Label ID="lblInputUoM" runat="server" CssClass="TableFields" /></td></tr>
    <tr runat="server" id="rowDecPlaces">
        <th class="TableHeaders">
            Decimal Places</th>
        <td>
            <asp:Label ID="lblDecPlaces" runat="server" CssClass="TableFields" /></td></tr>
    <tr runat="server" id="rowMinValue">
        <th class="TableHeaders">
            Minimum Value</th>
        <td>
            <asp:Label ID="lblMinValue" runat="server" CssClass="TableFields" /></td></tr>
    <tr runat="server" id="rowMaxValue">
        <th class="TableHeaders">
            Maximum Value</th>
        <td>
            <asp:Label ID="lblMaxValue" runat="server" CssClass="TableFields" /></td></tr>   
    <tr runat="server" id="rowFormula">
        <th class="TableHeaders">
            Formula</th>
        <td>
            <asp:Label ID="lblFormula" runat="server" CssClass="TableFields" /></td></tr>   
    <tr>
        <th class="TableHeaders">
            Frequency</th>
        <td>
            <asp:Label ID="lblFrequency" runat="server" CssClass="TableFields" /></td></tr>
    <tr>
        <th class="TableHeaders" valign="top">
            Description</th>
        <td>
            <asp:Label ID="lblDescription" runat="server" CssClass="TableFields" /></td></tr>
    <tr>
        <th class="TableHeaders" valign="top">
            Definition</th>
        <td>
            <asp:Label ID="lblDefinition" runat="server" CssClass="TableFields" /></td></tr>    
            <tr>
    <th class="TableHeaders" valign="top">
            Documentation</th>
        <td>
            <asp:Label ID="lblDocumentation" runat="server" CssClass="TableFields" /></td></tr>
    <tr>
        <th class="TableHeaders" valign="top">
            References</th>
        <td>
            <asp:Label ID="lblReferences" runat="server" CssClass="TableFields" /></td></tr>    
    <tr>
        <td colspan="2"><asp:Button ID="btnClose2" runat="server" Text="Close" class="SaveButtonCaption" Width="150px" OnClientClick="CloseInfoWindow(); return false;"  />      </td></tr>
    </table>
</asp:Content>