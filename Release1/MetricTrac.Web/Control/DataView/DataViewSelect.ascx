<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataViewSelect.ascx.cs" Inherits="MetricTrac.Control.DataView.DataViewSelect" %>
<table><tr>
	<td><asp:DropDownList runat="server" ID="ddlDataView" DataTextField="Name" DataValueField="DataViewListID" Width="300" /></td>
	<td><nobr><span style="font-weight:bold"><asp:Button runat="server" ID="bApply" Text="Go!" OnClick="bApply_Click" /></span>&nbsp;&nbsp;&nbsp;</nobr></td>
	<td><nobr><asp:LinkButton runat="server" ID="lbEdit" OnClick="lbEdit_Click">Edit</asp:LinkButton>&nbsp;&nbsp;&nbsp;</nobr></td>
	<td><nobr><asp:LinkButton runat="server" ID="lbAdd" OnClick="lbAdd_Click">Add New</asp:LinkButton></nobr></td>
</tr></table>
