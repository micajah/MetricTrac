<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RelatedInputValues.aspx.cs" Inherits="MetricTrac.RelatedInputValues" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="../Control/RelatedInputValues.ascx" tagname="RelatedInputValues" tagprefix="uc" %>
<asp:Content ID="cntGCA" ContentPlaceHolderID="PageBody" runat="server">    
    <uc:RelatedInputValues ID="RelatedInputValues1" runat="server" DataMode="View" />    
</asp:Content>