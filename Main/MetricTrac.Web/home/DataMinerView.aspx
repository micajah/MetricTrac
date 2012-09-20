<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataMinerView.aspx.cs" Inherits="MetricTrac.DataMinerView" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="../Control/DataView/DataViewList.ascx" tagname="DataViewList" tagprefix="uc1" %>
<asp:Content ID="cntGCA" ContentPlaceHolderID="PageBody" runat="server">
	<uc1:DataViewList ID="DataViewList1" runat="server" DataViewTypeID="00000000-0000-0000-0000-000000000001" RedirectEditUrl="DataMinerEdit.aspx"/>
</asp:Content>