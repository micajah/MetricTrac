<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScoreCardList.aspx.cs" Inherits="MetricTrac.ScoreCardList" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="../Control/ScoreCardList.ascx" tagname="ScoreCardList" tagprefix="uc" %>
<asp:Content ID="cntScoreCardList" ContentPlaceHolderID="PageBody" runat="server">
	<uc:ScoreCardList runat="server" ID="scl"/>
</asp:Content>
