<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MetricEdit.aspx.cs" Inherits="MetricTrac.MetricEdit" MasterPageFile="~/MasterPage.master"%>
<%@ Register Namespace="MetricTrac.MTControls" TagPrefix="mt" Assembly="MetricTrac.Web"%>
<%@ Register src="../Control/MetricCategorySelect.ascx" tagname="MetricCategorySelect" tagprefix="mts" %>
<asp:Content ID="cntGCA" ContentPlaceHolderID="PageBody" runat="server">    
    <div runat="server">
    <script type="text/javascript">
        function clientShow(sender, eventArgs) {
            var txtInput = document.getElementById("<%=this.FormulaInputClientID %>");
            var GetBeginDateSaver = document.getElementById("<%=this.BeginDateClientID %>");
            var GetEndDateSaver = document.getElementById("<%=this.EndDateClientID %>");
            var arg = GetBeginDateSaver.value + '|' + GetEndDateSaver.value + '|' + txtInput.value;
            sender.argument = arg;      
        }
        
        
        function clientClose(sender, args) {
            if (args.get_argument() != null) {
                var txtInput = document.getElementById("<%=this.FormulaInputClientID %>");
                var GetBeginDateSaver = document.getElementById("<%=this.BeginDateClientID %>");
                var GetEndDateSaver = document.getElementById("<%=this.EndDateClientID %>");
                var receivedData = args.get_argument();
                var n1 = receivedData.indexOf('|');
                var beginDate = receivedData.substr(0, n1);
                var n2 = receivedData.indexOf('|', n1 + 1);
                var endDate = receivedData.substring(n1 + 1, n2);
                var formula = receivedData.substring(n2 + 1);
                
                GetBeginDateSaver.value = beginDate;
                GetEndDateSaver.value = endDate;
                txtInput.value = formula;

                var lblBegin = document.getElementById("<%=this.lblBeginDateClientID %>")
                var lblEnd = document.getElementById("<%=this.lblEndDateClientID %>")

                lblBegin.innerHTML = beginDate;
                lblEnd.innerHTML = endDate == '' ? 'Non given' : endDate;
            }
        }

        function openRadWindow(metric) {
            var ddlMetricCategory = $find("<%= this.ddlMetricCategorySelect.ddlMetricCategoryClientID %>");
            var ddlFrequency = document.getElementById("<%= this.ddlInputPeriod.ClientID %>");
            var SelectedMetricCategory = ddlMetricCategory.get_text();
            var SelectedFrequency = ddlFrequency.options[ddlFrequency.selectedIndex].value;            
            var oWnd = radopen("MetricFormulaEdit.aspx?MetricID=" + metric + "&MetricCategory=" + SelectedMetricCategory + "&FrequencyID=" + SelectedFrequency, "rwFormulaEdit");            
            oWnd.Center();
        }

        function ShowRow(NumRow) {
            var tableObject = document.getElementById("<%= mfMetric.ClientID %>");
            tableObject.rows[NumRow].style.visibility = "visible";
            tableObject.rows[NumRow].style.display = "";
        }
        function HideRow(NumRow) {
            var tableObject = document.getElementById("<%= mfMetric.ClientID %>");
            tableObject.rows[NumRow].style.visibility = "hidden";
            tableObject.rows[NumRow].style.display = "none";
        }

        function HideShowType(IsShow) {
            var tableObject = document.getElementById("<%= mfMetric.ClientID %>");
            if (tableObject == null) return;
            if (IsShow) {
                ShowRow(8); // coll start date
                ShowRow(9); // coll end date
                ShowRow(10);                
            	HideRow(15);
            }
            else {
                HideRow(8); // coll start date
                HideRow(9); // coll end date
            	HideRow(10);
            	HideRow(11);
            	ShowRow(12);
            	HideRow(13);
            	HideRow(14);
            	ShowRow(6);
            	ShowRow(15);
            }
        }

        function HideShowNumericOptions(IsShow) {
            var tableObject = document.getElementById("<%= mfMetric.ClientID %>");
            if (tableObject == null) return;
            if (IsShow) {
            	ShowRow(6);
            	ShowRow(11);
            	ShowRow(12);
            	ShowRow(13);
            	ShowRow(14);
            }
            else {
            	HideRow(6);
            	HideRow(11);
            	HideRow(12);
            	HideRow(13);
            	HideRow(14);
            }
        }
        
        function fDataTypeChange(ddlDataType) {
            var sender = document.getElementById(ddlDataType);
            if (sender != null)
                HideShowNumericOptions(sender.options[sender.selectedIndex].value == '1');            
        }
        
        function fMetricTypeChange(ddlMetricType, ddlDataType) {
            var sender = document.getElementById(ddlMetricType);
            fDataTypeChange(ddlDataType);
            if (sender != null)
                HideShowType(sender.options[sender.selectedIndex].value == '1');
        }       

        function PutSubmitInput() {
            var primeSubmit = document.getElementById('ctl00_PageBody_mfMetric_btnUpdateClose');
            if (primeSubmit == null)
                primeSubmit = document.getElementById('ctl00_PageBody_mfMetric_btnInsertClose');
            if (primeSubmit == null) return;
            var mainInput = primeSubmit.parentNode;
            var inht = mainInput.innerHTML;
            var i = inht.indexOf('&nbsp;&nbsp;or&nbsp;&nbsp;');
            var part1 = inht.substr(0, i);
            var part2 = inht.substring(i, inht.length);
            var newhtml = (part1
                + '&nbsp;&nbsp;or&nbsp;&nbsp;<a id=\'__etounikalniyid\' style=\'color: Black;font-size:12px;\' href=\'javascript:__doPostBack(\"<%= this.ClientID %>\", \"SaveCopy\");\'>Save Metric & Create Copy</a>'
                + '&nbsp;&nbsp;or&nbsp;&nbsp;<a id=\'__etomegaunikalniyid\' style=\'color: Black;font-size:12px;\' href=\'javascript:__doPostBack(\"<%= this.ClientID %>\", \"SaveCreatePI\");\'>Save Metric & Create PI</a>'
                + part2);            
            mainInput.innerHTML = newhtml;
        }

        function fClearFormulaBox() {
            var txtInput = document.getElementById("<%=this.FormulaInputClientID %>");
            if (txtInput != null)
                txtInput.value = '';
        }
    </script>
    </div>    
    <telerik:RadAjaxLoadingPanel runat="server" ID="ralpOutputUoM" Wrap="true" Transparency="33" BackColor="LightGray" >
        <table style="height:100%; width:305px;">
            <tr><td valign="middle" align="center"><img alt="Loading ..." src="../images/loading4.gif" /></td></tr></table>
    </telerik:RadAjaxLoadingPanel> 
    <telerik:RadWindowManager ID="rwmEditFormula" runat="server" 
        Modal="true" VisibleStatusbar="false"
        Height="525px" Width="750px" DestroyOnClose="true"
        OnClientShow="clientShow"
        OnClientClose="clientClose" style="z-index: 90000"/>
    <mits:MagicForm ObjectName="Metric" ID="mfMetric" runat="server" AutoGenerateInsertButton="True"
        AutoGenerateDeleteButton="True" AutoGenerateEditButton="True" 
        DefaultMode="Insert" DataKeyNames="MetricID" AutoGenerateRows="False" 
        DataSourceID="ldsMetric"
        OnAction="mfMetric_Action" OnItemDeleted="mfMetric_ItemDeleted" 
        OnItemInserted="mfMetric_ItemInserted" OnItemUpdated="mfMetric_ItemUpdated" 
        oniteminserting="mfMetric_ItemInserting" 
        onitemupdating="mfMetric_ItemUpdating" onprerender="mfMetric_PreRender"        
        Caption="Create&amp;nbsp;Metric" CellPadding="0" CssClass="Mf_T" 
        GridLines="None" >
<FooterStyle CssClass="Mf_F"></FooterStyle>
        <Fields>
            <mits:TextBoxField HeaderText="Name" DataField="Name" Columns="55" Required="true" MaxLength="50"/>
            <mits:TextBoxField HeaderText="Alias" DataField="Alias" Columns="55" Required="false" MaxLength="50"/>
            <mits:TextBoxField HeaderText="Code" DataField="Code" Columns="55" Required="false" MaxLength="25"/>
            <mits:TemplateField>
                <EditItemTemplate>
                    <mts:MetricCategorySelect ID="mcs" runat="server"  Width="305px" MetricCategoryID='<%#Bind("MetricCategoryID")%>' OnClientTextChange="fClearFormulaBox" />
                </EditItemTemplate>
                <HeaderTemplate>Category</HeaderTemplate>
            </mits:TemplateField>
            <mits:TemplateField HeaderText="Frequency">
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlInputPeriod" runat="server" Width="305px"
                        DataSourceID="ldsFrequency" DataTextField="Name" DataValueField="FrequencyID" AppendDataBoundItems="true" SelectedValue='<%#Bind("FrequencyID") %>'>
                    </asp:DropDownList>
                </EditItemTemplate>
            </mits:TemplateField>
            <mits:TemplateField>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlUnitOfMeasure" runat="server" Width="305px" AutoPostBack="true"
                        DataSourceID="ldsUnitOfMeasure" DataTextField="SingularFullName" 
                        DataValueField="MeasureUnitId" AppendDataBoundItems="true" 
                        SelectedValue='<%#Bind("UnitOfMeasureID") %>' 
                        onselectedindexchanged="ddlUnitOfMeasure_SelectedIndexChanged1">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                    </asp:DropDownList>
                </EditItemTemplate>
                <HeaderTemplate>Output Unit of Measure</HeaderTemplate>
            </mits:TemplateField>
            <mits:TemplateField>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlMetricType" runat="server" Width="305px"
                        DataSourceID="ldsMetricType" DataTextField="Name" DataValueField="MetricTypeID" AppendDataBoundItems="true" SelectedValue='<%#Bind("MetricTypeID") %>'>
                    </asp:DropDownList>                    
                </EditItemTemplate>
                <HeaderTemplate>Metric Type</HeaderTemplate>
            </mits:TemplateField>
            <mt:MTDatePickerField HeaderText="Collection Start Date" DataField="CollectionStartDate" Required="false" NullDisplayText="" MaxDate="01/01/2100" MinDate="01/01/2005" Type="DatePicker" DateFormat="MM/dd/yyyy" />
            <mt:MTDatePickerField HeaderText="Collection End Date" DataField="CollectionEndDate" Required="false" NullDisplayText="" MaxDate="01/01/2100" MinDate="01/01/2005" Type="DatePicker" DateFormat="MM/dd/yyyy" />
            <mits:TemplateField>
                <EditItemTemplate>
                    <asp:DropDownList ID="ddlDataType" runat="server" Width="305px"
                        DataSourceID="ldsDataType" DataTextField="Name" DataValueField="MetricDataTypeID" AppendDataBoundItems="true" SelectedValue='<%#Bind("MetricDataTypeID") %>'>
                    </asp:DropDownList>
                </EditItemTemplate>
                <HeaderTemplate>Data Type</HeaderTemplate>
            </mits:TemplateField>            
            <mits:TemplateField>
                <EditItemTemplate>                    
                    <asp:DropDownList ID="ddlInputUnitOfMeasure" runat="server" Width="305px"
                        DataSourceID="ldsInputUnitOfMeasure" DataTextField="SingularFullName" DataValueField="MeasureUnitId" AppendDataBoundItems="true"  >
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                    </asp:DropDownList>                    
                </EditItemTemplate>
                <HeaderTemplate>Input Unit of Measure</HeaderTemplate>
            </mits:TemplateField> 
            <mits:TextBoxField HeaderText="Decimal Places" DataField="NODecPlaces" Columns="55" ConvertEmptyStringToNull="false" MaxLength="2" MinimumValue="0" MaximumValue="10" TextType="Text" TextMode="SingleLine" ValidationType="Integer" ValidationErrorMessage="Invalid integer value" DefaultText="0"/>
            <mits:TextBoxField HeaderText="Minimum Value" DataField="NOMinValue" Columns="55" ConvertEmptyStringToNull="false" MaxLength="29" TextType="Text" TextMode="SingleLine" ValidationType="Currency" ValidationErrorMessage="Invalid Minimum Value" DataFormatString="{0:f2}"/>
            <mits:TextBoxField HeaderText="Maximum Value" DataField="NOMaxValue" Columns="55" ConvertEmptyStringToNull="false" MaxLength="29" TextType="Text" TextMode="SingleLine" ValidationType="Currency" ValidationErrorMessage="Invalid Maximum Value" DataFormatString="{0:f2}"/>
            <mits:TemplateField>
                <EditItemTemplate>
                    <table cellpadding="0" cellspacing="0" border="0" style="margin-left:-5px;">
                    <tr style="visibility:hidden;display:none;">
                        <td colspan="2" style="padding-left:5px;">
                        Formula Begin Date: <asp:Label ID="lblBeginDate" runat="server" Text='<%# MetricFormula.BeginDate.ToShortDateString()%>' />
                        &nbsp;&nbsp;&nbsp;&nbsp;End Date: <asp:Label ID="lblEndDate" runat="server" Text='<%# MetricFormula.EndDate == null ? "Non given" : ((DateTime)MetricFormula.EndDate).ToShortDateString()%>' />
                        </td>                        
                    </tr>
                    <tr>
                        <td>
                            <mits:TextBox ID="heExpression" runat="server" Text='<%# MetricFormula.Formula %>' Columns="57" Rows="5" TextMode="MultiLine" /></td>
                        <td>
                            &nbsp;&nbsp;&nbsp;<a onclick="openRadWindow('<%#Eval("MetricID")%>'); return false;" href="MetricFormulaEdit.aspx">Edit Formula</a></td>
                    </tr>
                    </table>   
                    <asp:HiddenField ID="hfFormulaID" runat="server" Value='<%# MetricFormula.MetricFormulaID %>' />
                    <asp:HiddenField ID="hfBeginDate" runat="server" Value='<%# MetricFormula.BeginDate.ToShortDateString()%>' />
                    <asp:HiddenField ID="hfEndDate" runat="server" Value='<%# MetricFormula.EndDate == null ? String.Empty : ((DateTime)MetricFormula.EndDate).ToShortDateString()%>' />                    
                </EditItemTemplate>
                <HeaderTemplate>Formula</HeaderTemplate>
            </mits:TemplateField>
            <mits:TemplateField HeaderText="Trend Up is">
				<ItemTemplate>
					<telerik:RadComboBox runat="server" ID="rcbUpGood" AllowCustomText="false" ShowDropDownOnTextboxClick="true" MarkFirstMatch="true" SelectedValue='<%#Bind("GrowUpIsGood")%>'>
						<Items>
							<telerik:RadComboBoxItem Text="" Value="" />
							<telerik:RadComboBoxItem Text="Good" Value="True" ImageUrl="../images/buttons/UpGreen.gif" />
							<telerik:RadComboBoxItem Text="Bad" Value="False" ImageUrl="../images/buttons/UpRed.gif"  />
						</Items>
					</telerik:RadComboBox>
				</ItemTemplate>
            </mits:TemplateField>
            <mits:CheckBoxField DataField="AllowCustomNames" HeaderText="Allow Custom Names" DefaultChecked="false" ToolTip="If enabled the data input user will be allowed to input a custom name and code for this metric per location" />
            <mt:MTHtmlEditorField HeaderText="Description"  DataField="Notes" 
                ToolsFile="~/includes/ToolsFiles/BasicTools.xml" ControlStyle-Height="180px" >            
<ControlStyle Height="180px"></ControlStyle>
            </mt:MTHtmlEditorField>
            <mt:MTHtmlEditorField HeaderText="Definition" DataField="Definition" 
                ToolsFile="~/includes/ToolsFiles/BasicTools.xml" ControlStyle-Height="180px" >
<ControlStyle Height="180px"></ControlStyle>
            </mt:MTHtmlEditorField>
            <mt:MTHtmlEditorField HeaderText="Documentation" DataField="Documentation" 
                ToolsFile="~/includes/ToolsFiles/BasicTools.xml" ControlStyle-Height="180px" >            
<ControlStyle Height="180px"></ControlStyle>
            </mt:MTHtmlEditorField>
            <mt:MTHtmlEditorField HeaderText="References" DataField="MetricReferences" 
                ToolsFile="~/includes/ToolsFiles/BasicTools.xml" ControlStyle-Height="180px" >                        
<ControlStyle Height="180px"></ControlStyle>
            </mt:MTHtmlEditorField>
            <mits:TextBoxField HeaderText="FormulaCode" DataField="FormulaCode" Columns="55" Required="false" MaxLength="200"/>
        </Fields>

<EmptyDataRowStyle CssClass="Mf_R"></EmptyDataRowStyle>

<EditRowStyle CssClass="Mf_R"></EditRowStyle>

<HeaderStyle CssClass="Mf_H"></HeaderStyle>

<AlternatingRowStyle CssClass="Mf_R"></AlternatingRowStyle>

<RowStyle CssClass="Mf_R"></RowStyle>
    </mits:MagicForm>
    
    <asp:LinqDataSource runat="server" ID="ldsMetric" 
        ContextTypeName="MetricTrac.Bll.LinqMicajahDataContext" TableName="Metric"
        AutoGenerateWhereClause="true" 
        EnableInsert="true" EnableDelete="true" EnableUpdate="true" 
        onselected="ldsMetric_Selected" oninserted="ldsMetric_Inserted" >
    </asp:LinqDataSource>
    <asp:LinqDataSource runat="server" ID="ldsInputUnitOfMeasure" 
    ContextTypeName="MetricTrac.Bll.LinqMicajahDataContext" 
        TableName="Mc_UnitsOfMeasure" onselecting="ldsInputUnitOfMeasure_Selecting"/>
    
    <asp:LinqDataSource runat="server" ID="ldsFrequency" ContextTypeName="MetricTrac.Bll.LinqMicajahDataContext" TableName="Frequency" />    
    <asp:LinqDataSource runat="server" ID="ldsScoreCardPeriod" ContextTypeName="MetricTrac.Bll.LinqMicajahDataContext" TableName="ScoreCardPeriod" />    
    <asp:LinqDataSource runat="server" ID="ldsDataType" ContextTypeName="MetricTrac.Bll.LinqMicajahDataContext" TableName="MetricDataType"/>
    <asp:LinqDataSource runat="server" ID="ldsMetricType" ContextTypeName="MetricTrac.Bll.LinqMicajahDataContext" TableName="MetricType"/>
    <asp:LinqDataSource runat="server" ID="ldsUnitOfMeasure" 
        ContextTypeName="MetricTrac.Bll.LinqMicajahDataContext" 
        TableName="Mc_UnitsOfMeasure" onselecting="ldsUnitOfMeasure_Selecting"/>
</asp:Content>