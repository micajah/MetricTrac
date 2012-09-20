<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectMonth.ascx.cs" Inherits="MetricTrac.Control.SelectMonth" %>
<%@ Register src="~/Control/Select/SelectYear.ascx" tagname="SelectYear" tagprefix="mts" %>
<mts:SelectYear runat="server" ID="cSelectYear"/>
<asp:DropDownList runat="server" ID="ddlMonth" />