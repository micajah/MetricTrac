<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataMinerEdit.aspx.cs" Inherits="MetricTrac.DataMinerEdit" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="../Control/DataView/DataViewEdit.ascx" tagname="DataViewEdit" tagprefix="uc1" %>
<asp:Content ID="cntGCA" ContentPlaceHolderID="PageBody" runat="server">
	<uc1:DataViewEdit ID="DataViewEdit1" runat="server" DataViewTypeID="00000000-0000-0000-0000-000000000001" RedirectListUrl="DataMinerList.aspx" />
</asp:Content>