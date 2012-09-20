<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataValueChart.aspx.cs" Inherits="MetricTrac.DataValueChart" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="../Control/DataValueChart.ascx" tagname="DataValueChart" tagprefix="uc" %>
<asp:Content ID="cntMetric" ContentPlaceHolderID="PageBody" runat="server">
    <uc:DataValueChart ID="dataValueChart" runat="server" />
</asp:Content>
