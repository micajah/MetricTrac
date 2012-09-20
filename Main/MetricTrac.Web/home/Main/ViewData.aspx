<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewData.aspx.cs" Inherits="MetricTrac.ViewData" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="~/Control/MetricValueList.ascx" tagname="MetricValueList" tagprefix="uc1" %>
<asp:Content ID="cntGCA" ContentPlaceHolderID="PageBody" runat="server">
    <uc1:MetricValueList ID="MetricInputList1" runat="server" DataMode="View" />
</asp:Content>