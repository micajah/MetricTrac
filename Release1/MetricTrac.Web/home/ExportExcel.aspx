<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExportExcel.aspx.cs" Inherits="MetricTrac.home.ExportExcel"  MasterPageFile="~/MasterPage.master"%>
<%@ Register src="~/Control/MetricFilter.ascx" tagprefix="uc" tagname="MetricFilter" %>
<%@ Register src="~/Control/Select/SelectWeek.ascx" tagname="SelectWeek" tagprefix="mts" %>
<%@ Register src="~/Control/Select/SelectMonth.ascx" tagname="SelectMonth" tagprefix="mts" %>
<%@ Register src="~/Control/Select/SelectQuarter.ascx" tagname="SelectQuarter" tagprefix="mts" %>
<%@ Register src="~/Control/Select/SelectSemiYear.ascx" tagname="SelectSemiYear" tagprefix="mts" %>
<%@ Register src="~/Control/Select/SelectYear.ascx" tagname="SelectYear" tagprefix="mts" %>
<asp:Content ID="cntFormJunc" ContentPlaceHolderID="PageBody" runat="server">
    <div id="Div1" runat="server">
		<style type="text/css">
			.TdName
			{
				width:<%=NameWidth%>;
				min-width:<%=NameWidth%>;
				max-width:<%=NameWidth%>;
				height:<%=CellHeight%>;
				min-height:<%=CellHeight%>;
				max-height:<%=CellHeight%>;
				overflow:hidden;
			}
			.TdFrom
			{
				width:<%=FromWidth%>;
				min-width:<%=FromWidth%>;
				max-width:<%=FromWidth%>;
				height:<%=CellHeight%>;
				min-height:<%=CellHeight%>;
				max-height:<%=CellHeight%>;
				overflow:hidden;
			}
			.TdTo
			{
				width:<%=ToWidth%>;
				min-width:<%=ToWidth%>;
				max-width:<%=ToWidth%>;
				height:<%=CellHeight%>;
				min-height:<%=CellHeight%>;
				max-height:<%=CellHeight%>;
				overflow:hidden;
			}
			.TdInput
			{
				width:<%=InputWidth%>;
				min-width:<%=InputWidth%>;
				max-width:<%=InputWidth%>;
				height:<%=CellHeight%>;
				min-height:<%=CellHeight%>;
				max-height:<%=CellHeight%>;
				overflow:hidden;
			}
		</style>
        <script type="text/javascript">
        	function GetRadWindow() {//123
        		var oWindow = null;
        		if (window.radWindow)
        			oWindow = window.radWindow;
        		else if (window.frameElement.radWindow)
        			oWindow = window.frameElement.radWindow;
        		return oWindow;
        	}

        	function CloseOnReload(IsCancel) {
        		var w = GetRadWindow();
        		if (IsCancel) w.argument = null;
        		else w.argument = "Save";
        		w.close();
        	}
        	
        	var RbIntervalChangeTime = 0;
        	function RbIntervalChange() {
        		var rbIntervalCommon = $get("<%=rbIntervalCommon.ClientID%>");
        		var rbIntervalIndividual = $get("<%=rbIntervalIndividual.ClientID%>");
        		var CommonElements = $('div[ShowType="Common"]');
        		var IndividualElements = $('div[ShowType="Individual"]');


        		var dpCommonFrom = $find("<%=dpCommonFrom.ClientID%>");

        		if (rbIntervalCommon.checked) {
        			CommonElements.show(RbIntervalChangeTime);
        			IndividualElements.hide(RbIntervalChangeTime);
        		}
        		else if (rbIntervalIndividual.checked) {
        			IndividualElements.show(RbIntervalChangeTime);
        			CommonElements.hide(RbIntervalChangeTime);
        		}
        		else {
        			IndividualElements.hide(RbIntervalChangeTime);
        			CommonElements.hide(RbIntervalChangeTime);
        		}
        		RbIntervalChangeTime = 1500;
        	}

        	function InitSettings() {
        		var cbInterval = $("#<%=rbIntervalCommon.ClientID%>").add("#<%=rbIntervalIndividual.ClientID%>").add("#<%=rbIntervalFull.ClientID%>");
        		cbInterval.change(RbIntervalChange);
        		RbIntervalChange();
        	}
        	$(InitSettings);
        </script>
        
       <uc:MetricFilter runat="server" ID="MF" GroupByVisible="false" FilterSectionVisible="true" ExportVisible="false" />
       <br /><br /><br /><br />
       <table id="ExportTable" cellpadding="0" cellspacing="0" >
			<tr style="background-color:#F0F0F0">
				<td colspan="6" class="TdName">Time interval:</td>
			</tr>
			<tr>
				<td align="right">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:RadioButton runat="server" GroupName="TimeInterval" ID="rbIntervalCommon" Checked="true"/></td>
				<td class="TdName">Common</td>
				<td class="TdFrom" align="right"><div ShowType="Common">From: &nbsp;</div></td>
				<td class="TdInput"><div ShowType="Common"><telerik:RadDatePicker runat="server" ID="dpCommonFrom"/></div></td>
				<td class="TdTo" align="right"><div ShowType="Common">To: &nbsp;</div></td>
				<td class="TdInput"><div ShowType="Common"><telerik:RadDatePicker runat="server" ID="dpCommonTo" /></div></td>
			</tr>
			<tr style="background-color:#F0F0F0">
				<td align="right"><asp:RadioButton runat="server" GroupName="TimeInterval" ID="rbIntervalIndividual" /></td>
				<td class="TdName">Individual</td>
				<td colspan="5"></td>
			</tr>
			<tr>
				<td align="right"><asp:RadioButton runat="server" GroupName="TimeInterval" ID="rbIntervalFull" /></td>
				<td class="TdName">Full</td>
				<td colspan="5"></td>
			</tr>
			<tr style="background-color:#F0F0F0">
				<td colspan="3" class="TdName">Export Metrics</td>
				<td colspan="5"></td>
			</tr>
			<asp:PlaceHolder runat="server" ID="phDaily">
				<tr <%=BackgroundColor%>>
					<td align="right"><asp:CheckBox Checked="true" runat="server" ID="cbDaily" /></td>
					<td class="TdName">Daily</td>
					<td class="TdFrom" align="right"><div  ShowType="Individual">From: &nbsp;</div></td>
					<td class="TdInput"><div  ShowType="Individual"><telerik:RadDatePicker runat="server" ID="dpSelectDayFrom"/></div></td>
					<td class="TdTo" align="right"><div  ShowType="Individual">To: &nbsp;</div></td>
					<td class="TdInput"><div  ShowType="Individual"><telerik:RadDatePicker runat="server" ID="dpSelectDayTo" /></div></td>
				</tr>
			</asp:PlaceHolder>
			<asp:PlaceHolder runat="server" ID="phWeekly">
				<tr <%=BackgroundColor%>>
					<td align="right"><asp:CheckBox Checked="true" runat="server" ID="cbWeekly" /></td>
					<td class="TdName">Weekly</td>
					<td class="TdFrom" align="right"><div  ShowType="Individual">From: &nbsp;</div></td>
					<td class="TdInput"><div  ShowType="Individual"><mts:SelectWeek runat="server" ID="cSelectWeekFrom" /></div></td>
					<td class="TdTo" align="right"><div  ShowType="Individual">To: &nbsp;</div></td>
					<td class="TdInput"><div  ShowType="Individual"><mts:SelectWeek runat="server" ID="cSelectWeekTo" /></div></td>
				</tr>
			</asp:PlaceHolder>
			<asp:PlaceHolder runat="server" ID="phMonthly">
				<tr <%=BackgroundColor%>>
					<td align="right"><asp:CheckBox Checked="true" runat="server" ID="cbMonthly" /></td>
					<td class="TdName">Monthly</td>
					<td class="TdFrom" align="right"><div  ShowType="Individual">From: &nbsp;</div></td>
					<td class="TdInput"><div  ShowType="Individual"><mts:SelectMonth runat="server" ID="cSelectMonthFrom" /></div></td>
					<td class="TdTo" align="right"><div  ShowType="Individual">To: &nbsp;</div></td>
					<td class="TdInput"><div  ShowType="Individual"><mts:SelectMonth runat="server" ID="cSelectMonthTo" /></div></td>
				</tr>
			</asp:PlaceHolder>
			<asp:PlaceHolder runat="server" ID="phQtrly">
				<tr <%=BackgroundColor%>>
					<td align="right"><asp:CheckBox Checked="true" runat="server" ID="cbQtrly" /></td>
					<td class="TdName">Quarterly</td>
					<td class="TdFrom" align="right"><div  ShowType="Individual">From: &nbsp;</td>
					<td class="TdInput"><div  ShowType="Individual"><mts:SelectQuarter runat="server" ID="cSelectQuarterFrom" /></td>
					<td class="TdTo" align="right"><div  ShowType="Individual">To: &nbsp;</td>
					<td class="TdInput"><div  ShowType="Individual"><mts:SelectQuarter runat="server" ID="cSelectQuarterTo" /></td>
				</tr>
			</asp:PlaceHolder>
			<asp:PlaceHolder runat="server" ID="phSemiAnnual">
				<tr <%=BackgroundColor%>>
					<td align="right"><asp:CheckBox Checked="true" runat="server" ID="cbSemiAnnual" /></td>
					<td class="TdName">Semiannual</td>
					<td class="TdFrom" align="right"><div  ShowType="Individual">From: &nbsp;</div></td>
					<td class="TdInput"><div  ShowType="Individual"><mts:SelectSemiYear runat="server" ID="cSelectSemiYearFrom" /></div></td>
					<td class="TdTo" align="right"><div  ShowType="Individual">To: &nbsp;</div></td>
					<td class="TdInput"><div  ShowType="Individual"><mts:SelectSemiYear runat="server" ID="cSelectSemiYearTo" /></div></td>
				</tr>
			</asp:PlaceHolder>
			<asp:PlaceHolder runat="server" ID="phAnnual">
				<tr <%=BackgroundColor%>>
					<td align="right"><asp:CheckBox Checked="true" runat="server" ID="cbAnnual" /></td>
					<td class="TdName">Annual</td>
					<td class="TdFrom" align="right"><div  ShowType="Individual">From: &nbsp;</div></td>
					<td class="TdInput"><div  ShowType="Individual"><mts:SelectYear runat="server" ID="cSelectYearFrom" /></div></td>
					<td class="TdTo" align="right"><div  ShowType="Individual">To: &nbsp;</div></td>
					<td class="TdInput"><div  ShowType="Individual"><mts:SelectYear runat="server" ID="cSelectYearTo" /></div></td>
				</tr>
			</asp:PlaceHolder>
			<asp:PlaceHolder runat="server" ID="phBiAnnual">
				<tr <%=BackgroundColor%>>
					<td align="right"><asp:CheckBox Checked="true" runat="server" ID="cbBiAnnual" /></td>
					<td class="TdName">Biannual</td>
					<td class="TdFrom" align="right"><div  ShowType="Individual">From: &nbsp;</div></td>
					<td class="TdInput"><div  ShowType="Individual"><mts:SelectYear runat="server" ID="cSelectBiYearFrom" BiAnnual="True"/></div></td>
					<td class="TdTo" align="right"><div  ShowType="Individual">To: &nbsp;</div></td>
					<td class="TdInput"><div  ShowType="Individual"><mts:SelectYear runat="server" ID="cSelectBiYearTo" BiAnnual="True"/></div></td>
				</tr>
			</asp:PlaceHolder>
			<asp:PlaceHolder runat="server" ID="phFiscalQtrly">
				<tr <%=BackgroundColor%>>
					<td align="right"><asp:CheckBox Checked="true" runat="server" ID="cbFiscalQtrly" /></td>
					<td class="TdName">Fiscal Quarterly</td>
					<td class="TdFrom" align="right"><div  ShowType="Individual">From: &nbsp;</div></td>
					<td class="TdInput"><div  ShowType="Individual"><mts:SelectQuarter runat="server" ID="cSelectFiscalQuarterFrom" /></div></td>
					<td class="TdTo" align="right"><div  ShowType="Individual">To: &nbsp;</div></td>
					<td class="TdInput"><div  ShowType="Individual"><mts:SelectQuarter runat="server" ID="cSelectFiscalQuarterTo" /></div></td>
				</tr>
			</asp:PlaceHolder>
			<asp:PlaceHolder runat="server" ID="phFiscalSemiAnnual">
				<tr <%=BackgroundColor%>>
					<td align="right"><asp:CheckBox Checked="true" runat="server" ID="CheckBox1" /></td>
					<td class="TdName">Fiscal Semiannual</td>
					<td class="TdFrom" align="right"><div  ShowType="Individual">From: &nbsp;</div></td>
					<td class="TdInput"><div  ShowType="Individual"><mts:SelectSemiYear runat="server" ID="cSelectFiscalSemiYearFrom" /></div></td>
					<td class="TdTo" align="right"><div  ShowType="Individual">To: &nbsp;</div></td>
					<td class="TdInput"><div  ShowType="Individual"><mts:SelectSemiYear runat="server" ID="cSelectFiscalSemiYearTo" /></div></td>
				</tr>
			</asp:PlaceHolder>
			<asp:PlaceHolder runat="server" ID="phFiscalAnnual">
				<tr <%=BackgroundColor%>>
					<td align="right"><asp:CheckBox Checked="true" runat="server" ID="cbFiscalAnnual" /></td>
					<td class="TdName">Annual</td>
					<td class="TdFrom" align="right"><div  ShowType="Individual">From: &nbsp;</div></td>
					<td class="TdInput"><div  ShowType="Individual"><mts:SelectYear runat="server" ID="cSelectFiscalYearFrom" /></div></td>
					<td class="TdTo" align="right"><div  ShowType="Individual">To: &nbsp;</div></td>
					<td class="TdInput"><div  ShowType="Individual"><mts:SelectYear runat="server" ID="cSelectFiscalYearTo" /></div></td>
				</tr>
			</asp:PlaceHolder>
			<asp:PlaceHolder runat="server" ID="phFiscalBiAnnual">
				<tr <%=BackgroundColor%>>
					<td align="right"><asp:CheckBox Checked="true" runat="server" ID="cbFiscalBiAnnual" /></td>
					<td class="TdName">Fiscal Biannual</td>
					<td class="TdFrom" align="right"><div  ShowType="Individual">From: &nbsp;</div></td>
					<td class="TdInput"><div  ShowType="Individual"><mts:SelectYear runat="server" ID="cSelectFiscalBiYearFrom" BiAnnual="True"/></div></td>
					<td class="TdTo" align="right"><div  ShowType="Individual">To: &nbsp;</div></td>
					<td class="TdInput"><div  ShowType="Individual"><mts:SelectYear runat="server" ID="cSelectFiscalBiYearTo" BiAnnual="True"/></div></td>
				</tr>
			</asp:PlaceHolder>
			<tr><td>&nbsp;</td></tr>
			<tr>
				<td>&nbsp;</td>
				<td colspan="5" class="Mf_B">
					<asp:Button runat="server" Text="Export to Excel" ID="bExport" OnClick="bExport_Click" />
					&nbsp; &nbsp;or&nbsp; &nbsp;
					<asp:HyperLink runat="server" CssClass="Mf_Cb" NavigateUrl="~/home/MetricDataList.aspx">Cancel</asp:HyperLink>
				</td>
			</tr>
       </table>
    </div>
</asp:Content>
