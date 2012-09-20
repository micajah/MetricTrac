﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScoreCardDashboardEdit.aspx.cs" Inherits="MetricTrac.ScoreCardDashboardEdit" MasterPageFile="~/MasterPage.master"%>
<asp:Content ID="cScoreCardDashboard" ContentPlaceHolderID="PageBody" runat="server">
    <mt:MTDetailsView ObjectName="Dashboard Metric" ID="mfScoreCardDashboard" runat="server"
        AutoGenerateInsertButton="True" AutoGenerateDeleteButton="True" AutoGenerateEditButton="True" DefaultMode="Insert"
        ShowCloseButtonSeparate="False" AutoGenerateRows="false"
        RepeatColumns="1" DataSourceID="dsScoreCardDashboard" DataKeyNames="ScoreCardDashboardID"
        OnAction="mfScoreCardDashboard_Action" OnItemDeleted="mfScoreCardDashboard_ItemDeleted" OnItemInserted="mfScoreCardDashboard_ItemInserted" OnItemUpdated="mfScoreCardDashboard_ItemUpdated"
        OnItemInserting="mfScoreCardDashboard_ItemInserting" OnItemUpdating="mfScoreCardDashboard_ItemUpdating"
        OnExtractRowValuesEvent="mfScoreCardDashboard_ExtractRowValues">
        <Fields>
            <mits:TemplateField HeaderText="Score Card">
                <HeaderTemplate>
                    <table>
                        <tr><td class="Mf_R"><span style="position:relative; left:-9">Score Card</span></td></tr>
                        <tr><td class="Mf_R"><span style="position:relative; left:-9; top:5">Metric</span></td></tr>
                    </table>
                </HeaderTemplate>
                <EditItemTemplate>
                    <div runat="server" id="divMain"><telerik:RadAjaxPanel runat="server" ID="rapDashboard" LoadingPanelID="ralpDashboard" EnableEmbeddedScripts="true" EnableAJAX="true">
                        <table>
                            <tr><td><asp:DropDownList runat="server" ID="ddlScoreCard" DataSourceID="dsScoreCard" DataValueField="ScoreCardID" DataTextField="Name" Width="500" AutoPostBack="true" /></td></tr>
                            <tr><td><telerik:RadComboBox runat="server" ID="rcbMetricOrg" DataValueField="ScoreCardMetricID" DataTextField="MetricName" Width="500" HighlightTemplatedItems="true" MarkFirstMatch="true" AllowCustomText="false" ShowDropDownOnTextboxClick="true" EmptyMessage="Please select Metric">
								<HeaderTemplate>
									<table width="100%"><tr>
										<td style="width:33%;overflow:hidden">Metric</td>
										<td style="width:33%;overflow:hidden">Org Location</td>
									</tr></table>
								</HeaderTemplate>
								<ItemTemplate>
									<table width="100%"><tr>
										<td style="width:33%;overflow:hidden"><%#Eval("MetricName")%></td>
										<td style="width:33%;overflow:hidden"><%#Eval("OrgLocationName")%></td>
									</tr></table>
								</ItemTemplate>
							</telerik:RadComboBox></td></tr>
                        </table>
                        <telerik:RadAjaxLoadingPanel runat="server" ID="ralpDashboard" Wrap="true" Transparency="33" BackColor="LightGray" ><table style="height:100%; width:100%">
                            <tr><td valign="middle" align="center"><img alt="Loading ..." src="../images/loading6.gif" /></td></tr>
                        </table></telerik:RadAjaxLoadingPanel>
                    </telerik:RadAjaxPanel></div>
                </EditItemTemplate>
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
    <bll:BllDataSource runat="server" ID="dsScoreCardDashboard" TableName="ScoreCardDashboard" EnableAllChange="true" />
    <bll:BllDataSource runat="server" ID="dsScoreCard" TableName="ScoreCard" BllSelectMethod="List" />
    <bll:BllDataSource runat="server" ID="dsScoreCardPeriod" TableName="ScoreCardPeriod" />
</asp:Content>