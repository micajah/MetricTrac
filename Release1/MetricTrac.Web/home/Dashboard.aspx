<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="MetricTrac.Dashboard" MasterPageFile="~/MasterPage.master"%>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Charting" tagprefix="telerik" %>
<%@ Register TagPrefix="dgwc" Namespace="Dundas.Gauges.WebControl" Assembly="DundasWebGauge" %>
<%@ Register src="../Control/DataValueChart.ascx" tagname="DataValueChart" tagprefix="uc" %>
<%@ Register src="../Control/ScoreCardList.ascx" tagname="ScoreCardList" tagprefix="uc" %>
<asp:Content ID="cntMetric" ContentPlaceHolderID="PageBody" runat="server">
    <style type="text/css">
    .GaugeHeight
    {
      width:150px;
      height:150px;
      overflow:hidden;
    }
    </style>   
    <div runat="server">
	<telerik:RadAjaxPanel runat="server" ID="rapDasboard" LoadingPanelID="ralpDashboard" EnableEmbeddedScripts="true" EnableAJAX="true">
		<table>
			<tr><td colspan="2"><uc:ScoreCardList runat="server" ID="scl" MyDashboardMode="true" OnSelectedIndexChanged="scl_SelectedIndexChanged" Width="1000px"/></td></tr>
			<tr>
				<td>
					<uc:DataValueChart ID="dataValueChart" runat="server" />
				</td>
				<td width="150" valign="top">
					<table>
						<tr><td>
							<div runat="server" id="divCurrent" class="GaugeHeight">
								<asp:PlaceHolder runat="server" ID="phCurrent">
									Current Value: <b><asp:Label runat="server" ID="lbCurrent"/></b>
									<dgwc:GaugeContainer runat="server" ID="dgwcCurrent" Height="330" Width="150">
										<CircularGauges>
											<dgwc:CircularGauge name="Default">
												<Scales>
													<dgwc:CircularScale FillGradientType="TopBottom" FillGradientEndColor="Red" ShadowOffset="0" Width="0"
														FillColor="Green" StartAngle="90" SweepAngle="180" Radius="38" Name="Default">
														<LabelStyle Font="Arial, 12.75pt" TextColor="DimGray" RotateLabels="False"></LabelStyle>
														<MajorTickMark Width="2" Placement="Inside" BorderWidth="0" Shape="Rectangle" FillColor="DimGray"></MajorTickMark>
														<MinorTickMark Width="2" Placement="Inside" BorderWidth="0" FillColor="DimGray"></MinorTickMark>
													</dgwc:CircularScale>
												</Scales>
												<Ranges>
													<dgwc:CircularRange StartValue="0" EndValue="33" FillColor="Green" Placement="Inside" DistanceFromScale="-5" StartWidth="15" EndWidth="15" BorderStyle="NotSet" FillGradientType="None"/>
													<dgwc:CircularRange StartValue="33" EndValue="66" FillColor="Yellow" Placement="Inside" DistanceFromScale="-5" StartWidth="15" EndWidth="15" BorderStyle="NotSet" FillGradientType="None"/>
													<dgwc:CircularRange StartValue="66" EndValue="100" FillColor="Red" Placement="Inside" DistanceFromScale="-5" StartWidth="15" EndWidth="15" BorderStyle="NotSet" FillGradientType="None"/>
												</Ranges>
												<BackFrame FrameWidth="10" Style="Edged" BackGradientEndColor="White" Shape="AutoShape" FrameGradientType="DiagonalRight"
													FrameColor="SlateGray" BackColor="LightSteelBlue" FrameGradientEndColor="White"></BackFrame>
												<Size Height="100" Width="100"></Size>
												<PivotPoint Y="0" X="50"></PivotPoint>
												<Pointers>
													<dgwc:CircularPointer Value="33" />
												</Pointers>
											</dgwc:CircularGauge>
										</CircularGauges>
									</dgwc:GaugeContainer>
								</asp:PlaceHolder>
							</div>
						</td></tr>
						<tr><td>
							<div runat="server" id="divPrevious" class="GaugeHeight">							
								<asp:PlaceHolder runat="server" ID="phPrevious">
									Previous Value: <b><asp:Label runat="server" ID="lbPrevious"/></b>
									<dgwc:GaugeContainer runat="server" ID="dgwcPrevious" Height="330" Width="150">
										<CircularGauges>
											<dgwc:CircularGauge name="Default">
												<Scales>
													<dgwc:CircularScale FillGradientType="TopBottom" FillGradientEndColor="Red" ShadowOffset="0" Width="0"
														FillColor="Green" StartAngle="90" SweepAngle="180" Radius="38" Name="Default">
														<LabelStyle Font="Arial, 12.75pt" TextColor="DimGray" RotateLabels="False"></LabelStyle>
														<MajorTickMark Width="2" Placement="Inside" BorderWidth="0" Shape="Rectangle" FillColor="DimGray"></MajorTickMark>
														<MinorTickMark Width="2" Placement="Inside" BorderWidth="0" FillColor="DimGray"></MinorTickMark>
													</dgwc:CircularScale>
												</Scales>
												<Ranges>
													<dgwc:CircularRange StartValue="0" EndValue="33" FillColor="Green" Placement="Inside" DistanceFromScale="-5" StartWidth="15" EndWidth="15" BorderStyle="NotSet" FillGradientType="None"/>
													<dgwc:CircularRange StartValue="33" EndValue="66" FillColor="Yellow" Placement="Inside" DistanceFromScale="-5" StartWidth="15" EndWidth="15" BorderStyle="NotSet" FillGradientType="None"/>
													<dgwc:CircularRange StartValue="66" EndValue="100" FillColor="Red" Placement="Inside" DistanceFromScale="-5" StartWidth="15" EndWidth="15" BorderStyle="NotSet" FillGradientType="None"/>
												</Ranges>
												<BackFrame FrameWidth="10" Style="Edged" BackGradientEndColor="White" Shape="AutoShape" FrameGradientType="DiagonalRight"
													FrameColor="SlateGray" BackColor="LightSteelBlue" FrameGradientEndColor="White"></BackFrame>
												<Size Height="100" Width="100"></Size>
												<PivotPoint Y="0" X="50"></PivotPoint>
												<Pointers>
													<dgwc:CircularPointer Value="33" />
												</Pointers>
											</dgwc:CircularGauge>
										</CircularGauges>
									</dgwc:GaugeContainer>
								</asp:PlaceHolder>
							</div>
						</td></tr>
						<tr><td>
							<div runat="server" id="divChange" class="GaugeHeight">
								<asp:PlaceHolder runat="server" ID="phChange">
									Value Change: <b><asp:Label runat="server" ID="lbChange"/></b>
									<dgwc:GaugeContainer runat="server" ID="dgwcChange" Height="330" Width="150">
										<CircularGauges>
											<dgwc:CircularGauge name="Default">
												<Scales>
													<dgwc:CircularScale FillGradientType="TopBottom" FillGradientEndColor="Red" ShadowOffset="0" Width="0"
														FillColor="Green" StartAngle="90" SweepAngle="180" Radius="38" Name="Default" Minimum="-100" Maximum="100">
														<LabelStyle Font="Arial, 12.75pt" TextColor="DimGray" RotateLabels="False"></LabelStyle>
														<MajorTickMark Width="2" Placement="Inside" BorderWidth="0" Shape="Rectangle" FillColor="DimGray"></MajorTickMark>
														<MinorTickMark Width="2" Placement="Inside" BorderWidth="0" FillColor="DimGray"></MinorTickMark>
													</dgwc:CircularScale>
												</Scales>
												<Ranges>
													<dgwc:CircularRange StartValue="0" EndValue="33" FillColor="Green" Placement="Inside" DistanceFromScale="-5" StartWidth="15" EndWidth="15" BorderStyle="NotSet" FillGradientType="None"/>
													<dgwc:CircularRange StartValue="33" EndValue="66" FillColor="Yellow" Placement="Inside" DistanceFromScale="-5" StartWidth="15" EndWidth="15" BorderStyle="NotSet" FillGradientType="None"/>
													<dgwc:CircularRange StartValue="66" EndValue="100" FillColor="Red" Placement="Inside" DistanceFromScale="-5" StartWidth="15" EndWidth="15" BorderStyle="NotSet" FillGradientType="None"/>
												</Ranges>
												<BackFrame FrameWidth="10" Style="Edged" BackGradientEndColor="White" Shape="AutoShape" FrameGradientType="DiagonalRight"
													FrameColor="SlateGray" BackColor="LightSteelBlue" FrameGradientEndColor="White"></BackFrame>
												<Size Height="100" Width="100"></Size>
												<PivotPoint Y="0" X="50"></PivotPoint>
												<Pointers>
													<dgwc:CircularPointer Value="33" />
												</Pointers>
											</dgwc:CircularGauge>
										</CircularGauges>
									</dgwc:GaugeContainer>
								</asp:PlaceHolder>
							</div>
						</td></tr>
					</table>
				</td>
			</tr>
        </table>
		<telerik:RadAjaxLoadingPanel runat="server" ID="ralpDashboard" Wrap="true" Transparency="33" BackColor="LightGray" ><table style="height:100%; width:100%">
			<tr><td valign="middle" align="center"><img alt="Loading ..." src="../images/loading6.gif" /></td></tr>
		</table></telerik:RadAjaxLoadingPanel>
        </telerik:RadAjaxPanel>
    </div>
</asp:Content>
