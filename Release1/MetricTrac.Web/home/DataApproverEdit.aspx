<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataApproverEdit.aspx.cs" Inherits="MetricTrac.DataApproverEdit" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="../Control/DataRuleEdit.ascx" tagname="DataRuleEdit" tagprefix="uc" %>
<asp:Content ID="DataCollectorEdit" ContentPlaceHolderID="PageBody" runat="server">
    <uc:DataRuleEdit ID="DataRuleEdit1" runat="server" DataRuleTypeID="2" RedirectListUrl="~/home/DataApproverList.aspx" />
</asp:Content>