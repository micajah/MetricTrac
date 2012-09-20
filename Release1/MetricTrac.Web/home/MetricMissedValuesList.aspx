<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MetricMissedValuesList.aspx.cs" Inherits="MetricTrac.MetricMissedValuesList" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="../Control/MissedInputValues.ascx" tagname="MissedInputValues" tagprefix="uc" %>
<asp:Content ID="cntGCA" ContentPlaceHolderID="PageBody" runat="server">    
    <uc:MissedInputValues ID="MissedValues" runat="server" />
</asp:Content>