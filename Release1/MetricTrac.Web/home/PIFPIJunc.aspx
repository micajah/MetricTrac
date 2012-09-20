<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PIFPIJunc.aspx.cs" Inherits="MetricTrac.PIFPIJunc" MasterPageFile="~/MasterPage.master"%>
<%@ Register Src="~/Control/GCASelect.ascx" TagName="GCASelect" TagPrefix="mts" %>
<%@ Register src="~/Control/PerformanceIndicatorList.ascx" tagname="PerformanceIndicatorList" tagprefix="mts" %>
<asp:Content ID="cntFormJunc" ContentPlaceHolderID="PageBody" runat="server">    
    <span class="GridHeader">Group/Category/Aspect</span><br />
    <mts:GCASelect ID="GCAFilter" runat="server" AutoPostBack="true" /><br /><br />
    <span class="GridHeader">Select Performance Indicators to add</span>
    <mts:PerformanceIndicatorList ID="mainPerformanceIndicatorList" runat="server" ListMode="FormSelect"/><br />
    <asp:Button ID="btnAdd" runat="server" Text="Add Performance Indicators To Form &amp; Back" class="SaveButtonCaption" Width="280px" onclick="btnAdd_Click" /><span style="width:10px;"></span>or<span style="width:10px;"></span>
    <asp:LinkButton ID="lnkCancel" runat="server" onclick="lnkCancel_Click" CssClass="Mf_Cb">Cancel</asp:LinkButton>
</asp:Content>
