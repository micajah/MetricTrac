<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroupByList.ascx.cs" Inherits="MetricTrac.Control.GroupByList" %>
<table cellspacing="0" cellpadding="3" class="filters">
<tr>
    <td style="color:#989898;font-weight:bold;height:33">
        <nobr>Group by</nobr></td>
    <td>
        <asp:DropDownList runat="server" ID="ddlGroup" AutoPostBack="true">
            <asp:ListItem Text="Location" Value="Location" />
            <asp:ListItem Text="Metric" Value="Metric" />            
        </asp:DropDownList></td>
    <td style="background-color:White">
        <nobr>&nbsp; &nbsp;</nobr></td>
</tr></table>