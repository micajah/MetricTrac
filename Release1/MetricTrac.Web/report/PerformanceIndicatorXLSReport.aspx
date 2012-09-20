<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PerformanceIndicatorXLSReport.aspx.cs" Inherits="MetricTrac.PerformanceIndicatorXLSReport" MasterPageFile="~/MasterPage.master" %>
<%@ Register Src="~/Control/OrgLocationSelect.ascx" TagPrefix="uc" TagName="OrgLocationSelect" %>
<asp:Content ID="cntGCA" ContentPlaceHolderID="PageBody" runat="server">
<table border="0" cellpadding="0" cellspacing="12" style="background-color:InfoBackground;">
    <tr>
        <td align="right" style="width:100px;">
            Title:</td>
        <td style="width:235px;">
            <asp:TextBox ID="txtTitle" runat="server" Columns="39" MaxLength="255" />&nbsp;<span style="color:Red; vertical-align:top;">*</span><br />
            <asp:RequiredFieldValidator ID="rfvTitle" ControlToValidate="txtTitle" SetFocusOnError="true" runat="server" ErrorMessage="Report title is required." Display="Dynamic"></asp:RequiredFieldValidator></td></tr>           
    <tr>
        <td align="right">
            Org Location:</td>
        <td>
            <uc:OrgLocationSelect runat="server" ID="sOrgLocation" Width="225" /></td></tr>
    <tr>
        <td align="right">
            Group By Location Type:</td>
        <td>        
            <asp:DropDownList ID="ddlLocationType" runat="server" Width="225"
                DataSourceID="ldsLocationType" DataTextField="Name" DataValueField="EntityNodeTypeId" AppendDataBoundItems="true">                
                <asp:ListItem Selected="True" Text="" Value="" />
            </asp:DropDownList></td></tr>
    <tr>
        <td align="right">
            Group:</td>
        <td>        
            <asp:DropDownList ID="ddlGroup" runat="server" Width="225"
                DataSourceID="ldsGroup" DataTextField="Name" DataValueField="GroupCategoryAspectID" AppendDataBoundItems="true">                
            </asp:DropDownList></td></tr>
    <tr>
        <td align="right">
            Frequency:</td>
        <td>
            <asp:DropDownList ID="ddlFrequency" runat="server" Width="225"
                DataSourceID="ldsFrequency" DataTextField="Name" DataValueField="FrequencyID" AppendDataBoundItems="true">                
            </asp:DropDownList></td></tr>
    <tr>
        <td align="right">
            Period:</td>
        <td>
            <table cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td>
                        <telerik:RadDatePicker id="rdpBeginDate" runat="server" Width="100px" Culture="English (United States)" MinDate="02.01.1900 0:00:00" MaxDate="05.06.2079 0:00:00" Calendar-RangeMinDate="02.01.1900 0:00:00" Calendar-RangeMaxDate="05.06.2079 0:00:00" AllowEmpty="false" /></td>
                    <td style="padding-left:10px;padding-right:11px;">
                    to</td>
                    <td>
                        <telerik:RadDatePicker id="rdpEndDate" runat="server" Width="100px" Culture="English (United States)" MinDate="02.01.1900 0:00:00" MaxDate="05.06.2079 0:00:00" Calendar-RangeMinDate="02.01.1900 0:00:00" Calendar-RangeMaxDate="05.06.2079 0:00:00" AllowEmpty="false" /></td>
                </tr>                
                
            </table>
            </td>
    </tr> 
    <tr>
        <td></td>
        <td>
            <asp:RequiredFieldValidator runat="server" ID="vldStart"
                ControlToValidate="rdpBeginDate"
                ErrorMessage="Begin Date is required." Display="Dynamic" >
            </asp:RequiredFieldValidator>
            <asp:RequiredFieldValidator runat="server" ID="vldStop"
                ControlToValidate="rdpEndDate"
                ErrorMessage="End Date is required." Display="Dynamic">
            </asp:RequiredFieldValidator>
            <asp:CompareValidator ID="dateCompareValidator" runat="server"
                ControlToValidate="rdpEndDate"
                ControlToCompare="rdpBeginDate"
                Operator="GreaterThanEqual"
                Type="Date"
                ErrorMessage="The end date must be after the start one." Display="Dynamic">
            </asp:CompareValidator></td>
    </tr>  
    <tr>
        <td></td>
        <td>
            <asp:Button ID="btnAdd" runat="server" Text="Create Report" 
                class="SaveButtonCaption" Width="226px" onclick="btnAdd_Click" /></td></tr>
</table>
<asp:LinqDataSource runat="server" ID="ldsLocationType" 
        ContextTypeName="MetricTrac.Bll.LinqMicajahDataContext" 
        TableName="Mc_EntityNodeType" onselecting="ldsLocationType_Selecting"/>

<bll:BllDataSource runat="server" ID="ldsGroup" TableName="GroupCategoryAspect" BllSelectMethod="SelectGroups"/>
<bll:BllDataSource runat="server" ID="ldsFrequency" TableName="Frequency"/>

</asp:Content>