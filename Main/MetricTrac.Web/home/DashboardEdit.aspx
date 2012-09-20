<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashboardEdit.aspx.cs" Inherits="MetricTrac.DashboardEdit" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="../Control/ScoreCardEdit.ascx" tagname="ScoreCardEdit" tagprefix="uc" %>
<asp:Content ID="cScoreCard" ContentPlaceHolderID="PageBody" runat="server">
    <uc:ScoreCardEdit runat="server" ID="sce" MyDashboardMode="true"/>
</asp:Content>