<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataCollectorEdit.aspx.cs" Inherits="MetricTrac.DataCollectorEdit" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="../Control/DataRuleEdit.ascx" tagname="DataRuleEdit" tagprefix="uc" %>
<asp:Content ID="DataCollectorEdit" ContentPlaceHolderID="PageBody" runat="server">
    <uc:DataRuleEdit ID="DataRuleEdit1" runat="server" DataRuleTypeID="1" RedirectListUrl="~/home/DataCollectorList.aspx" />
</asp:Content>