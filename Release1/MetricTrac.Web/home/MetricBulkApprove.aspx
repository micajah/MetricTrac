<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MetricBulkApprove.aspx.cs" Inherits="MetricTrac.MetricBulkApprove" MasterPageFile="~/MasterPage.master" %>
<%@ Register src="../Control/MetricBulkInput.ascx" tagname="MetricBulkInput" tagprefix="uc" %>
<asp:Content ID="cntGCA" ContentPlaceHolderID="PageBody" runat="server">    
    <uc:MetricBulkInput ID="mbi" runat="server" DataMode="Approve" />    
</asp:Content>