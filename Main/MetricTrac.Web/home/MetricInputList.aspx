<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MetricInputList.aspx.cs" Inherits="MetricTrac.MetricInputList" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="../Control/MetricInputList.ascx" tagname="MetricInputList" tagprefix="uc" %>
<asp:Content ID="cntGCA" ContentPlaceHolderID="PageBody" runat="server">    
    <uc:MetricInputList ID="MetricInputList1" runat="server" DataMode="Input" />    
</asp:Content>
