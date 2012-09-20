<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MetricBulkInput.aspx.cs" Inherits="MetricTrac.MetricBulkInput" MasterPageFile="~/MasterPage.master" %>
<%@ Register src="../Control/MetricBulkInput.ascx" tagname="MetricBulkInput" tagprefix="uc" %>
<asp:Content ID="cntGCA" ContentPlaceHolderID="PageBody" runat="server">    
    <uc:MetricBulkInput ID="mbi" runat="server" DataMode="Input" />    
</asp:Content>