<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScoreCardEdit.aspx.cs" Inherits="MetricTrac.ScoreCardEdit" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="../Control/ScoreCardEdit.ascx" tagname="ScoreCardEdit" tagprefix="uc" %>
<asp:Content ID="cScoreCard" ContentPlaceHolderID="PageBody" runat="server">
    <uc:ScoreCardEdit runat="server" ID="sce"/>
</asp:Content>