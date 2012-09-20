<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PerformanceIndicatorFormulaEdit.aspx.cs" Inherits="MetricTrac.PerformanceIndicatorFormulaEdit" MasterPageFile="~/MasterPage.master"%>
<%@ Register Namespace="MetricTrac.MTControls" TagPrefix="mt" Assembly="MetricTrac.Web"%>
<%@ Register src="~/Control/PIFormulaBuilder.ascx" tagname="FormulaBuilder" tagprefix="mts" %>
<asp:Content ID="cntPIFormulaEdit" ContentPlaceHolderID="PageBody" runat="server">
<div runat="server">
	<script type="text/javascript">
		function receiveArg() {
		}
		function GetRadWindow() {
			var oWindow = null;
			if (window.radWindow)
				oWindow = window.radWindow;
			else if (window.frameElement.radWindow)
				oWindow = window.frameElement.radWindow;
			return oWindow;
		}
		function GetDateStr(d) {
			if (d == null || d == undefined || d == "") return "";
			return (d.getMonth()+1) + "/" + d.getDate() + "/" + d.getFullYear();
		}
		function returnArg() {
			var rdpBeginDate = $find("<%=rdpBeginDate.ClientID%>");
			var rdpEndDate = $find("<%=rdpEndDate.ClientID%>");
			var txtComment = document.getElementById("<%=txtComment.ClientID%>");
			var rbSimpleFormula = document.getElementById("<%=rbSimpleFormula.ClientID%>");

			if (rbSimpleFormula.checked) {
				var fbExpression = document.getElementById("<%=ddlSimpleFormula.ClientID%>");
				if (fbExpression.selectedIndex >= 0) {
					fbExpression = fbExpression.options[fbExpression.selectedIndex].value;
				} else {
					fbExpression = "";
				}
			} else {
				var fbExpression = $find("<%=fbExpression.InputClientID%>");
				fbExpression = fbExpression.get_text();
			}

			rdpBeginDate = rdpBeginDate.get_selectedDate();
			rdpEndDate = rdpEndDate.get_selectedDate();
			txtComment = txtComment.value;


			var RespParam = encodeURIComponent(fbExpression) + "&" +
				encodeURIComponent(GetDateStr(rdpBeginDate)) + "&" +
				encodeURIComponent(GetDateStr(rdpEndDate)) + "&" +				
				encodeURIComponent(txtComment);

			var oWnd = GetRadWindow();
			oWnd.close(RespParam);
		}
		function CloseOnCancel() { //cancel        
			var oWindow = GetRadWindow();
			oWindow.argument = null;
			oWindow.close();
		}
	</script>
</div>
<table>
<tr><td colspan="3">
	<table cellpadding="0" cellspacing="0" border="0">
	<tr>
		<td>
			<span style="color:Red;">*</span><span class="GridHeader">Calculation Begin Date</span></td><td style="width:5px;"></td>            
		<td>
			<span class="GridHeader">Calculation End Date</span></td>
	</tr>
	<tr>
		<td>
			<telerik:RadDatePicker id="rdpBeginDate" runat="server" Width="190px" Culture="English (United States)" MinDate="02.01.1900 0:00:00" MaxDate="05.06.2079 0:00:00" Calendar-RangeMinDate="02.01.1900 0:00:00" Calendar-RangeMaxDate="05.06.2079 0:00:00" AllowEmpty="false" />
			<asp:RequiredFieldValidator runat="server" ID="vldStart"
				ControlToValidate="rdpBeginDate"
				ErrorMessage="Begin Date is required.">
			</asp:RequiredFieldValidator></td><td></td>            
		<td>
			<telerik:RadDatePicker id="rdpEndDate" runat="server" Width="190px" Culture="English (United States)" MinDate="02.01.1900 0:00:00" MaxDate="05.06.2079 0:00:00" Calendar-RangeMinDate="02.01.1900 0:00:00" Calendar-RangeMaxDate="05.06.2079 0:00:00" AllowEmpty="true" />
			<asp:CompareValidator ID="dateCompareValidator" runat="server"
				ControlToValidate="rdpEndDate"
				ControlToCompare="rdpBeginDate"
				Operator="GreaterThanEqual"
				Type="Date"
				ErrorMessage="The end date must be after the start one.">
			</asp:CompareValidator></td>
	</tr>
	</table>
</td></tr><tr><td colspan="3">
	<br /><span class="GridHeader">Calculation Editor</span><br />
</td></tr><tr>
	<td style="width:21px"><asp:RadioButton runat="server" ID="rbSimpleFormula" GroupName="rbFormulaType" /></td>
	<td><asp:DropDownList runat="server" ID="ddlSimpleFormula">
		<asp:ListItem Text="Sum of all included metrics" Value="Sum" />
        <asp:ListItem Text="Average of all included metrics" Value="Average" />
        <asp:ListItem Text="Root-Mean-Square (standard deviation) of all included metrics" Value="RMS" />
	</asp:DropDownList></td>
</tr><tr>
	<td><asp:RadioButton runat="server" ID="rbCustomFormula" GroupName="rbFormulaType" /></td>
	<td><mts:FormulaBuilder ID="fbExpression" runat="server" /></td>
</tr><tr><td colspan="3">
	<span class="GridHeader">Comment</span><br />
	<textarea runat="server" id="txtComment" name="txtComment" rows="5" style="width:530px;"></textarea>
</td></tr><tr><td colspan="3">
	<asp:Button ID="btnSave" runat="server" OnClientClick="returnArg(); return false;" Text="Save Formula" class="SaveButtonCaption" Width="180px"  />&nbsp;&nbsp;&nbsp;&nbsp;or&nbsp;&nbsp;&nbsp;&nbsp;
	<asp:LinkButton ID="lnkCancel" runat="server" OnClientClick="CloseOnCancel(); return false;" CssClass="Mf_Cb">Cancel</asp:LinkButton>
</td></tr><tr><td colspan="3">
	<span class="GridHeader">Calculation History</span>
	<mits:CommonGridView runat="server" ID="cgvFormula" DataKeyNames="PerformanceIndicatorFomulaID"
		DataSourceID="ldsPIFormula" Width="99%" ColorScheme="Gray" AutoGenerateColumns="False"
		AutoGenerateEditButton="False" AutoGenerateDeleteButton="False" 
		ShowAddLink="False" EmptyDataText="No defined formulas" 
		onrowdatabound="cgvFormula_RowDataBound">
		<Columns>            
			<mits:TextField DataField="BeginDate" HeaderText="Begin Date"  />
			<mits:TextField DataField="EndDate" HeaderText="End Date" NullDisplayText="None given" />
			<mits:TextField DataField="Formula" HeaderText="Formula" />            
			<mits:TextField DataField="UserName" HeaderText="Updated By" />
			<mits:TextField DataField="ChangeDate" HeaderText="Updated" />
			<mits:TextField DataField="Comment" HeaderText="Comment" />
		</Columns>
	</mits:CommonGridView>
</td></tr>
</table>
<asp:LinqDataSource runat="server" ID="ldsPIFormula" ContextTypeName="MetricTrac.Bll.LinqMicajahDataContext" TableName="PerformanceIndicatorFormula" onselecting="ldsPIFormula_Selecting" />
</asp:Content>