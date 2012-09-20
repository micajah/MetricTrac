<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DataValueChart.ascx.cs" Inherits="MetricTrac.Control.DataValueChart" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Charting" Assembly="Telerik.Web.UI" %>
<b><asp:Label runat="server" ID="lbTitle">Metric Data Chart</asp:Label></b>
<telerik:RadChart EnableEmbeddedSkins="true" runat="server" 
    ID="rcMetricOrgLocation" Width="822" Height="430" DefaultType="Line" 
    EnableViewState="false" IntelligentLabelsEnabled="true">
    <Appearance>
        <Border Visible="true" Width="1" Color="White" />
        <Shadow Color="White" Distance="0" />
    </Appearance>
    <ChartTitle Visible="false">
    </ChartTitle>
    <PlotArea Appearance-Border-Visible="false" >
        <XAxis AutoScale="false" MinValue="0" MaxValue="6" Step="1">
            <AxisLabel TextBlock-Text="Date"></AxisLabel>
            <Items>
                <telerik:ChartAxisItem Visible="true" Value="0" TextBlock-Visible="true">
                    <Appearance></Appearance>
                </telerik:ChartAxisItem>
                <telerik:ChartAxisItem Visible="true" Value="1" TextBlock-Visible="true">
                    <Appearance></Appearance>
                </telerik:ChartAxisItem>
                <telerik:ChartAxisItem Visible="true" Value="2" TextBlock-Visible="true">
                    <Appearance></Appearance>
                </telerik:ChartAxisItem>
                <telerik:ChartAxisItem Visible="true" Value="3" TextBlock-Visible="true">
                    <Appearance></Appearance>
                </telerik:ChartAxisItem>
                <telerik:ChartAxisItem Visible="true" Value="4" TextBlock-Visible="true">
                    <Appearance></Appearance>
                </telerik:ChartAxisItem>
                <telerik:ChartAxisItem Visible="true" Value="5" TextBlock-Visible="true">
                    <Appearance></Appearance>
                </telerik:ChartAxisItem>
            </Items>
        </XAxis>
        <YAxis AxisMode="Extended">
        </YAxis>            
    </PlotArea>
    <Series>
        <telerik:ChartSeries Type="Line" Name="Base Line Value" Visible="false">            
			<Appearance LegendDisplayMode="Nothing" LabelAppearance-Visible="true">			    
				<PointMark Visible="false"/>
                <LineSeriesAppearance Width="3" PenStyle="Dash"/>
                <FillStyle FillType="Solid" MainColor="#00c000"/>                
                <TextAppearance TextProperties-Color="#7d7d7d" TextProperties-Font="Verdana" />
			</Appearance>
        </telerik:ChartSeries>
        <telerik:ChartSeries Type="Line" Name="Base Line Value" Visible="false">
			<Appearance LegendDisplayMode="Nothing" LabelAppearance-Visible="true">
				<PointMark Visible="false"/>
                <LineSeriesAppearance Width="3" PenStyle="Dash"/>
                <FillStyle FillType="Solid" MainColor="#e0e000"/>
                <TextAppearance TextProperties-Color="#7d7d7d" TextProperties-Font="Verdana" />
			</Appearance>
        </telerik:ChartSeries>
        <telerik:ChartSeries Type="Line" Name="Base Line Value" Visible="false">            
			<Appearance LegendDisplayMode="Nothing" LabelAppearance-Visible="true" >
				<PointMark Visible="false"/>
                <LineSeriesAppearance Width="3" PenStyle="Dash"/>
                <FillStyle FillType="Solid" MainColor="#c00000"/>
                <TextAppearance TextProperties-Color="#7d7d7d" TextProperties-Font="Verdana" />
			</Appearance>
        </telerik:ChartSeries>        
        <telerik:ChartSeries Type="Line" Name="MetricValue"  >
            <Appearance LegendDisplayMode="Nothing">
                <PointMark Figure="Circle" Visible="true" Border-Color="Red" Border-Width="1" Dimensions-Height="25" Dimensions-Width="25">
                    <FillStyle FillType="Solid" MainColor="Yellow"></FillStyle>
                </PointMark>
                <EmptyValue  Mode="Approximation">                        
                    <Line PenStyle="Solid" Color="#d0f6ff" />
                </EmptyValue>
                <FillStyle FillType="Solid" MainColor="White"></FillStyle>
            </Appearance>
        </telerik:ChartSeries>        
        
    </Series>
    <PlotArea>
        <YAxis>
            <Appearance>
                <MinorGridLines Visible="false" />
            </Appearance>
        </YAxis>        
    </PlotArea>
</telerik:RadChart>
<bll:BllDataSource runat="server" ID="dsMetricOrgLocation" TableName="MetricValue" OnSelecting="dsMetricOrgLocation_Selecting"/>