<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataApproverList.aspx.cs" Inherits="MetricTrac.DataApproverList" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="~/Control/DataRuleList.ascx" tagname="DataRuleList" tagprefix="uc" %>
<asp:Content ID="cntGCA" ContentPlaceHolderID="PageBody" runat="server">    
    <uc:DataRuleList ID="DataRuleList1" runat="server" DataRuleTypeID="2" RedirectEditUrl="~/home/KPISetup/DataRule/DataApproverEdit.aspx" />    
</asp:Content>