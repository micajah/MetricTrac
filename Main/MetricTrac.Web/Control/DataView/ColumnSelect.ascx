<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ColumnSelect.ascx.cs" Inherits="MetricTrac.Control.DataView.ColumnSelect" %>
<asp:Label runat="server" ID="lbTest" Visible="false" />
<telerik:RadComboBox runat="server" ID="rcbColumn" Width="300" HighlightTemplatedItems="true" DataValueField="id" DataTextField="text" AllowCustomText="false" OnSelectedIndexChanged="rcbColumn_SelectedIndexChanged" DropDownCssClass="TopZIndex">
</telerik:RadComboBox>