<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MetricFormulaEdit.aspx.cs" Inherits="MetricTrac.MetricFormulaEdit" MasterPageFile="~/MasterPage.master"%>
<%@ Register Namespace="MetricTrac.MTControls" TagPrefix="mt" Assembly="MetricTrac.Web"%>
<%@ Register src="../Control/FormulaBuilder.ascx" tagname="FormulaBuilder" tagprefix="mts" %>
<asp:Content ID="cntMetricFormulaEdit" ContentPlaceHolderID="PageBody" runat="server">
<div runat="server">
<script type="text/javascript" language="javascript">
    function GetInput() {        
        return $find("<%=fbExpression.InputClientID%>")
    }

    function GetBeginDateInput() {
        return $find("<%=rdpBeginDate.ClientID%>")
    }

    function GetEndDateInput() {
        return $find("<%=rdpEndDate.ClientID%>")
    }

    function GetCommentInput() {
        return document.getElementById('txtComment');
    }

    var cookieName = 'FormulaComment';
    function getCookieValue() {
        var pos = document.cookie.indexOf(cookieName + '=');
        if (pos == -1)
            return '';
        else {
            var pos2 = document.cookie.indexOf(';', pos);
            if (pos2 == -1)
                return unescape(document.cookie.substring(pos + cookieName.length + 1));
            else
                return unescape(document.cookie.substring(pos + cookieName.length + 1, pos2));
        }
    }
    
    function GetRadWindow()
    {
         var oWindow = null;
         if (window.radWindow)
            oWindow = window.radWindow;     
         else if (window.frameElement.radWindow)
           oWindow = window.frameElement.radWindow;  
         return oWindow;
    }

    function CloseOnCancel() { //cancel        
        var oWindow = GetRadWindow();
        oWindow.argument = null;
        oWindow.close();
    }
    function DateToString(date)
    {
        return (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear();
    }

    function returnArg() { // save
        var txtComment = GetCommentInput();
        document.cookie = cookieName + '=' + escape(txtComment.value);
        var oWnd = GetRadWindow();
        var txtInput = GetInput();
        var beginDatePicker = GetBeginDateInput();
        var endDatePicker = GetEndDateInput();
        if (beginDatePicker.isEmpty()) return;
        var startDate = beginDatePicker.get_selectedDate();
        var argument = DateToString(startDate);
        if (!endDatePicker.isEmpty()) {
            var endDate = endDatePicker.get_selectedDate();
            if (startDate > endDate) return;
            argument += '|' + DateToString(endDate) + '|';
        }
        else
            argument += '||';
        argument += txtInput.get_text();
        oWnd.close(argument);
    }

    function receiveArg() {
        var txtComment = GetCommentInput();
        var commentValue = getCookieValue();
        txtComment.value = commentValue;
        var txtInput = GetInput();
        var beginDatePicker = GetBeginDateInput();
        var endDatePicker = GetEndDateInput();              
        var currentWindow = GetRadWindow();
        var receivedData = currentWindow.argument;
        
        var n1 = receivedData.indexOf('|');
        var beginDate = receivedData.substr(0, n1);
        var n2 = receivedData.indexOf('|', n1+1);
        var endDate = receivedData.substring(n1+1, n2);
        var formula = receivedData.substring(n2+1);

        beginDatePicker.set_selectedDate(new Date(beginDate));
        if (endDate != '')
            endDatePicker.set_selectedDate(new Date(endDate));
        else endDatePicker.clear();
        txtInput.set_html(formula);
    }
</script>
</div>
        
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
    </table><br />
    <span class="GridHeader">Calculation Editor</span>
    <mts:FormulaBuilder ID="fbExpression" runat="server" /><br />
    <span class="GridHeader">Comment</span><br />    
    <textarea id="txtComment" name="txtComment" rows="5" style="width:530px;"></textarea><br /><br />
    <asp:Button ID="btnSave" runat="server" OnClientClick="returnArg(); return false;" Text="Save Formula" class="SaveButtonCaption" Width="180px"  />&nbsp;&nbsp;&nbsp;&nbsp;or&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:LinkButton ID="lnkCancel" runat="server" OnClientClick="CloseOnCancel(); return false;" CssClass="Mf_Cb">Cancel</asp:LinkButton><br /><br />
    <span class="GridHeader">Calculation History</span>
    <mits:CommonGridView runat="server" ID="cgvFormula" DataKeyNames="MetricFormulaID"
        DataSourceID="ldsMetricFormula" Width="99%" ColorScheme="Gray" AutoGenerateColumns="False"
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
    <asp:LinqDataSource runat="server" ID="ldsMetricFormula" 
        ContextTypeName="MetricTrac.Bll.LinqMicajahDataContext" TableName="MetricFormula"        
        onselecting="ldsMetricFormula_Selecting" />
</asp:Content>