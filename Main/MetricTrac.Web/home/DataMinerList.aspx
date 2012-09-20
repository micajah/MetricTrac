<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DataMinerList.aspx.cs" Inherits="MetricTrac.DataMinerList" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="../Control/DataView/DataViewSelect.ascx" tagname="DataViewSelect" tagprefix="uc1" %>
<%@ Register src="../Control/DataView/DataViewQuery.ascx" tagname="DataViewQuery" tagprefix="uc2" %>
<asp:Content ID="cntGCA" ContentPlaceHolderID="PageBody" runat="server">
	<table>
		<tr><td>
			<uc1:DataViewSelect ID="DataViewSelect1" runat="server" DataViewTypeID="00000000-0000-0000-0000-000000000001" ListUrl="DataMinerView.aspx" EditUrl="DataMinerEdit.aspx"/>
		</td></tr>
		<tr><td>
			<uc2:DataViewQuery ID="DataViewQuery1" runat="server" DataViewTypeID="00000000-0000-0000-0000-000000000001"/>	
		</td></tr>
	</table>
</asp:Content>