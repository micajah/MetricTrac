<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectSemiYear.ascx.cs" Inherits="MetricTrac.Control.Select.SelectSemiYear" %>
<%@ Register src="~/Control/Select/SelectYear.ascx" tagname="SelectYear" tagprefix="mts" %>
<mts:SelectYear runat="server" ID="cSelectYear"/>
<asp:DropDownList runat="server" ID="ddlSemi">
	<asp:ListItem Text="S1" Value="1" />
	<asp:ListItem Text="S2" Value="2" />
</asp:DropDownList>