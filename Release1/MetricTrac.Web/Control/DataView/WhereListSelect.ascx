<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WhereListSelect.ascx.cs" Inherits="MetricTrac.Control.DataView.WhereListSelect" %>
<%@ Register src="ColumnSelect.ascx" tagname="ColumnSelect" tagprefix="uc" %>
<%@ Register src="~/Control/OrgLocationSelect.ascx" tagname="OrgLocationSelect" tagprefix="uc" %>
<%@ Register src="ListManager.ascx" tagname="ListManager" tagprefix="uc" %>
<%@ Register src="ConditionSelect.ascx" tagname="ConditionSelect" tagprefix="uc" %>
<asp:Panel runat="server" ID="pWhere">
	<table>
		<asp:Repeater runat="server" ID="rpWhere" DataSourceID="dsWhere" OnPreRender="rpWhere_PreRender">
			<ItemTemplate>
				<tr runat="server" id="trWhere">
					<td><uc:ListManager runat="server" ID="lmWhere" First='<%#Eval("First")%>' Last='<%#Eval("Last")%>'/></td>
					<td><%#Eval("Name")%>.&nbsp;</td>
					<td><uc:ColumnSelect ID="csWhere" runat="server" DataViewTypeID='<%#DataViewTypeID%>' AutoPostBack="true" HideNameColumn="true" /></td>
					<td runat="server" id="tdCondition">
						<asp:Panel runat="server" ID="pCondition">
							<uc:ConditionSelect runat="server" ID="Condition" />
						</asp:Panel>
					</td>
					<td>
						<asp:Panel runat="server" ID="pValue">
							<asp:TextBox runat="server" ID="tbValue" Width="300"/>
							<telerik:RadComboBox runat="server" ID="rcbValue" Width="300" DataValueField="ID" DataTextField="Name"  Visible="false"/>
							<telerik:RadNumericTextBox runat="server" Type="Number" ID="rnValue" Width="300" Visible="false"></telerik:RadNumericTextBox>
							<asp:PlaceHolder runat="server" ID="phValue" OnLoad="phValue_Load"/>
						</asp:Panel>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
	</table>

</asp:Panel>

	<telerik:RadAjaxLoadingPanel runat="server" ID="lpWhere" Wrap="true" Transparency="33" BackColor="LightGray" ><table style="height:100%; width:100%">
		<tr><td valign="middle" align="center"><asp:Image runat="server" ImageUrl="~/images/loading4.gif" /></td></tr>
	</table></telerik:RadAjaxLoadingPanel>
	
Advanced Conditions:<br />
<asp:TextBox runat="server" ID="tbCondition" Width="681" />
<asp:ObjectDataSource runat="server" ID="dsWhere" TypeName="MetricTrac.Control.DataView.WhereListSelect" SelectMethod="GetWhereData" />