<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataViewQuery.ascx.cs" Inherits="MetricTrac.Control.DataView.DataViewQuery" %>
<div class="metric">
<asp:Repeater runat="server" ID="rMasterGroup" OnItemDataBound="rMasterGroup_ItemDataBound">
	<HeaderTemplate>
		<table>
	</HeaderTemplate>
	<ItemTemplate>
		<tr>
			<asp:Repeater runat="server" ID="rMasterGroupTree"><ItemTemplate>
					
					<th colspan="99" class="title" style="border-bottom-style:none"><%#Container.ItemIndex==0?"<br>":""%><i style="font-weight:normal"><%#Eval("Header")%>: </i><%#Eval("Value")%></th>
				</tr>
				<tr>
					<td style="border-right-style:none;border-bottom-style:none" rowspan='<%#((int)Eval("SubGroupCount"))+ ((int)Eval("RecordCount"))+((int)Eval("HeaderCount"))%>'>&nbsp;&nbsp;&nbsp;</td>
			</ItemTemplate></asp:Repeater>

			<asp:Repeater runat="server" ID="rMasterRecordHeader"><ItemTemplate>
				<td><i><%#Eval("Header") %>:</i></td>
			</ItemTemplate></asp:Repeater>
			<asp:Repeater runat="server" ID="rSlaveGroup">
				<ItemTemplate>
					<td style="background-color:#F0F0F0"><%#Eval("Group")%></td>
				</ItemTemplate>
			</asp:Repeater>
		</tr>
		<asp:Repeater runat="server" ID="rMasterRecord" OnItemDataBound="rMasterRecord_ItemDataBound">
			<ItemTemplate>
				<tr>
					<asp:Repeater runat="server" ID="rMasterValue"><ItemTemplate>
						<td style="background-color:#F0F0F0" align="left"><%#Eval("Value")%></td>
					</ItemTemplate></asp:Repeater>
					
					<asp:Repeater runat="server" ID="rSlaveRecord" OnItemDataBound="rSlaveRecord_ItemDataBound"><ItemTemplate>
						<asp:Repeater runat="server" ID="rSlaveVlue"><ItemTemplate>
							<td class='cal-metric<%#string.IsNullOrEmpty((string)Eval("Value"))?" empty":""%>'><%#Eval("Value")%></td>
						</ItemTemplate></asp:Repeater>
					</ItemTemplate></asp:Repeater>					
				</tr>
			</ItemTemplate>
		</asp:Repeater>
	</ItemTemplate>
	<FooterTemplate>
		</table>
	</FooterTemplate>
</asp:Repeater></div>