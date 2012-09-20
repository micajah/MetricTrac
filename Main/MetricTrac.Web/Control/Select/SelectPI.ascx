<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectPI.ascx.cs" Inherits="MetricTrac.Control.Select.SelectPI" %>
<asp:DropDownList runat="server" ID="ddlPI" DataSourceID="dsPI" DataTextField="Name" DataValueField="PerformanceIndicatorID"/>
<bll:BllDataSource runat="server" ID="dsPI" TableName="PerformanceIndicator" />
