﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScoreCardMetricEdit.aspx.cs" Inherits="MetricTrac.ScoreCardMetricEdit" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="../Control/MetricFilter.ascx" tagprefix="uc" tagname="MetricFilter" %>
<%@ Register src="../Control/orgLocationSelect.ascx" tagname="orgLocationSelect" tagprefix="mts" %>
<asp:Content ID="cScoreCardMetric" ContentPlaceHolderID="PageBody" runat="server">
    <mt:MTDetailsView ObjectName="Score Card Metric" ID="mfScoreCardMetric" runat="server"
        AutoGenerateInsertButton="True" AutoGenerateDeleteButton="True" 
        AutoGenerateEditButton="True" DefaultMode="Insert"
        ShowCloseButtonSeparate="False" DataKeyNames="ScoreCardMetricID" AutoGenerateRows="false"
        RepeatColumns="1" DataSourceID="dsScoreCardMetric"
        OnAction="mfScoreCardMetric_Action" 
        OnItemDeleted="mfScoreCardMetric_ItemDeleted" 
        OnItemInserted="mfScoreCardMetric_ItemInserted" OnItemUpdated="mfScoreCardMetric_ItemUpdated"
        OnExtractRowValuesEvent="mfScoreCardMetric_ExtractRowValues"
        oniteminserting="mfScoreCardMetric_ItemInserting" 
        onitemupdating="mfScoreCardMetric_ItemUpdating" OnPreRender="mfScoreCardMetric_PreRender">
        <Fields>
			<mits:TemplateField HeaderText="">
				<ItemTemplate>
					<uc:MetricFilter runat="server" ID="cMericFilter" BaseDateVisible="false" MetricVisible="false" TwoRowMode="True" OnUse="cMericFilter_Use" FilterSectionVisible="true" />
				</ItemTemplate>
			</mits:TemplateField>
			<mits:TemplateField HeaderText="Metric">
				<ItemTemplate>
					<asp:Panel runat="server" ID="pMetric">
						<asp:DropDownList runat="server" ID="ddlMetric" DataTextField="Name" DataValueField="MetricID" Width="531" AutoPostBack="true" OnSelectedIndexChanged="ddlMetric_SelectedIndexChanged"/>
					</asp:Panel>
				</ItemTemplate>
			</mits:TemplateField>
			<mits:TemplateField HeaderText="Location">
				<ItemTemplate>
					<asp:Panel runat="server" ID="pLocation">
						<mts:orgLocationSelect ID="orgLocationSelect" runat="server" Width="531" RooOrgLocationEqualToNull="true" ShowNullAsRooOrgLocation="true" /></asp:Panel></ItemTemplate>
			</mits:TemplateField>
            <mt:MTDropDownListField HtmlDecode="true" DataSourceId="dsScoreCardPeriod" HeaderText="Period" DataTextField="Name" DataValueField="ScoreCardPeriodID" DataField="ScoreCardPeriodID"/>
            <mits:TextBoxField HeaderText="Minimum Value" DataField="MinValue" ValidationType="Double" ConvertEmptyStringToNull="true" ValidationErrorMessage="Invalid Value"  DataFormatString="{0:0.##########}"/>
        	<mits:TextBoxField HeaderText="Maximum Value" DataField="MaxValue" ValidationType="Double" ConvertEmptyStringToNull="true" ValidationErrorMessage="Invalid Value"  DataFormatString="{0:0.##########}"/>
			<mits:TextBoxField HeaderText="Baseline Value" DataField="BaselineValue"  ValidationType="Double" ConvertEmptyStringToNull="true" ValidationErrorMessage="Invalid Value" DataFormatString="{0:0.##########}" />
			<mits:TextBoxField HeaderText="Baseline Value Label" DataField="BaselineValueLabel" MaxLength="25" ConvertEmptyStringToNull="true" ValidationErrorMessage="Invalid Value" />
			<mits:TextBoxField HeaderText="Breakpoint #1 Value" DataField="Breakpoint1Value" ValidationType="Double"  ConvertEmptyStringToNull="true" ValidationErrorMessage="Invalid Value" DataFormatString="{0:0.##########}"/>
			<mits:TextBoxField HeaderText="Breakpoint #1 Label" DataField="Breakpoint1ValueLabel" MaxLength="25" ConvertEmptyStringToNull="true" ValidationErrorMessage="Invalid Value" />
			<mits:TextBoxField HeaderText="Breakpoint #2 Value" DataField="Breakpoint2Value" ValidationType="Double" ConvertEmptyStringToNull="true" ValidationErrorMessage="Invalid Value" DataFormatString="{0:0.##########}" />
			<mits:TextBoxField HeaderText="Breakpoint #2 Label" DataField="Breakpoint2ValueLabel" MaxLength="25" ConvertEmptyStringToNull="true" ValidationErrorMessage="Invalid Value" />
        </Fields>
    </mt:MTDetailsView>
    <telerik:RadAjaxLoadingPanel runat="server" ID="ralpMetric" Wrap="true" Transparency="33" BackColor="LightGray" ><table style="height:100%; width:100%">
        <tr><td valign="middle" align="center"><img alt="Loading ..." src="../images/loading4.gif" /></td></tr>
    </table></telerik:RadAjaxLoadingPanel>
    <bll:BllDataSource runat="server" ID="dsScoreCardMetric" TableName="ScoreCardMetric" EnableAllChange="true" />
    <bll:BllDataSource runat="server" ID="dsScoreCardPeriod" TableName="ScoreCardPeriod" />
    
</asp:Content>