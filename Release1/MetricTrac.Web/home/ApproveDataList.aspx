<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApproveDataList.aspx.cs" Inherits="MetricTrac.ApproveDataList" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="../Control/MetricInputList.ascx" tagname="MetricInputList" tagprefix="uc" %>
<asp:Content ID="cntMetric" ContentPlaceHolderID="PageBody" runat="server">
    <uc:MetricInputList ID="MetricInputList1" runat="server" DataMode="Approve" />
</asp:Content>
