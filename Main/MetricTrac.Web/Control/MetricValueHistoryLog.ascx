﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MetricValueHistoryLog.ascx.cs" Inherits="MetricTrac.Control.MetricValueHistoryLog" %>
<span class="GridHeader">Metric Value History Log</span>
<mits:CommonGridView runat="server" ID="cgvLog" DataKeyNames="MetricValueChangeLogID"
    DataSourceID="ldsMetricValueChangeLog" Width="99%" ColorScheme="Gray" AutoGenerateColumns="False"
    AutoGenerateEditButton="False" AutoGenerateDeleteButton="False" 
    ShowAddLink="False" EmptyDataText="No registered changes">
    <Columns>            
        <mits:TextField DataField="CreatedTime" HeaderText="Date"  />
        <mits:TextField DataField="UserName" HeaderText="User" />        
        <mits:TextField DataField="TypeName" HeaderText="Change Type" />           
        <mits:TextField DataField="OldValue" HeaderText="Old Value" />           
        <mits:TextField DataField="NewValue" HeaderText="New Value" />
    </Columns>
</mits:CommonGridView>    
<asp:LinqDataSource runat="server" ID="ldsMetricValueChangeLog" 
    ContextTypeName="MetricTrac.Bll.LinqMicajahDataContext" TableName="MetricValueChangeLog"        
    onselecting="ldsMetricValueChangeLog_Selecting" />