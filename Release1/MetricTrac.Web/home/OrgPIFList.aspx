<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrgPIFList.aspx.cs" Inherits="MetricTrac.OrgPIFList" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="../Control/PIFList.ascx" tagname="PIFList" tagprefix="uc" %>
<asp:Content ID="cntPIF" ContentPlaceHolderID="PageBody" runat="server">
    <asp:Label runat="server" ID="lbOrg"/>
    <uc:PIFList ID="cPIFList" runat="server" OrgMode="True"/>
</asp:Content>