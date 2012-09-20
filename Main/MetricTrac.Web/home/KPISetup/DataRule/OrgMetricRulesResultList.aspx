<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrgMetricRulesResultList.aspx.cs" Inherits="MetricTrac.OrgMetricRulesResultList" MasterPageFile="~/MasterPage.master"%>
<asp:Content ID="cntMetric" ContentPlaceHolderID="PageBody" runat="server">
    <mits:CommonGridView runat="server" ID="cgvRulesResult" 
        DataSourceID="dsRulesResult" Width="99%" ColorScheme="Gray" AutoGenerateColumns="False">
        <Columns>
            <mits:TextField DataField="OrgLocationName" HeaderText="Org Location" SortExpression="OrgLocationName" />
            <mits:TextField DataField="MetricName" HeaderText="Metric" SortExpression="MetricName" />            
            <mits:TextField DataField="PINames" HeaderText="PI" SortExpression="PINames" />
            <mits:TextField DataField="GCANames" HeaderText="GCA" SortExpression="GCANames" />            
            <mits:TextField DataField="CollectorName" HeaderText="Collector" SortExpression="CollectorUserName" />
            <mits:TextField DataField="ApproverName" HeaderText="Approver" SortExpression="ApproverUserName" />
        </Columns>
    </mits:CommonGridView>
    <bll:BllDataSource runat="server" ID="dsRulesResult" TableName="Metric" BllSelectMethod="ListRulesResult"/>
</asp:Content>
