<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataCollectorList.aspx.cs" Inherits="MetricTrac.DataCollectorList" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="../Control/DataRuleList.ascx" tagname="DataRuleList" tagprefix="uc" %>
<asp:Content ID="cntGCA" ContentPlaceHolderID="PageBody" runat="server">    
    <uc:DataRuleList ID="DataRuleList1" runat="server" DataRuleTypeID="1" RedirectEditUrl="~/home/DataCollectorEdit.aspx" />    
</asp:Content>