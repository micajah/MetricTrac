<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RelatedInputValuesA.aspx.cs" Inherits="MetricTrac.RelatedInputValuesA" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="../Control/RelatedInputValues.ascx" tagname="RelatedInputValues" tagprefix="uc" %>
<asp:Content ID="cntGCA" ContentPlaceHolderID="PageBody" runat="server">    
    <uc:RelatedInputValues ID="RelatedInputValues1" runat="server" DataMode="Approve" />    
</asp:Content>