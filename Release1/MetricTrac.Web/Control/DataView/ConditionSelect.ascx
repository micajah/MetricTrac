<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConditionSelect.ascx.cs" Inherits="MetricTrac.Control.DataView.ConditionSelect" %>
<telerik:RadComboBox runat="server" ID="rcbCondition" Width="111">
	<Items>
		<telerik:RadComboBoxItem Value="1" Text="Less"  Visible="false"/>
		<telerik:RadComboBoxItem Value="2" Text="Less or Equal"  Visible="false"/>
		<telerik:RadComboBoxItem Value="3" Text="Equal"  Visible="false"/>
		<telerik:RadComboBoxItem Value="4" Text="Like"  Visible="false"/>
		<telerik:RadComboBoxItem Value="5" Text="Great or Equal"  Visible="false"/>
		<telerik:RadComboBoxItem Value="6" Text="Great"  Visible="false"/>
	</Items>
</telerik:RadComboBox>