<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectQuarter.ascx.cs" Inherits="MetricTrac.Control.Select.SelectQuarter" %>
<%@ Register src="~/Control/Select/SelectYear.ascx" tagname="SelectYear" tagprefix="mts" %>
<mts:SelectYear runat="server" ID="cSelectYear"/>
<asp:DropDownList runat="server" ID="ddlQuarter">
	<asp:ListItem Text="Q1" Value="1" />
	<asp:ListItem Text="Q2" Value="2" />
	<asp:ListItem Text="Q3" Value="3" />
	<asp:ListItem Text="Q4" Value="4" />
</asp:DropDownList>