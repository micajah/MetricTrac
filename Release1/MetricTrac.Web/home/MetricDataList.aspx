<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MetricDataList.aspx.cs" Inherits="MetricTrac.MetricDataList" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="../Control/MetricInputList.ascx" tagname="MetricInputList" tagprefix="uc1" %>
<asp:Content ID="cntGCA" ContentPlaceHolderID="PageBody" runat="server">
    <uc1:MetricInputList ID="MetricInputList1" runat="server" DataMode="View" />
</asp:Content>
