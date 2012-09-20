<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PIFList.aspx.cs" Inherits="MetricTrac.PIFList" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="../Control/PIFList.ascx" tagname="PIFList" tagprefix="uc" %>
<asp:Content ID="cntPIF" ContentPlaceHolderID="PageBody" runat="server">   
    <div class="UpToHeader" />
    <uc:PIFList ID="PIFList1" runat="server" />
</asp:Content>
