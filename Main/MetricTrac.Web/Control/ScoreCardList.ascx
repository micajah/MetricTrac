<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ScoreCardList.ascx.cs" Inherits="MetricTrac.Control.ScoreCardList" %>
<telerik:RadWindowManager ID="rwmEditValue" runat="server" Modal="true" VisibleStatusbar="false" Height="525px" Width="897px" DestroyOnClose="true"  style="z-index: 90000"/>
<telerik:RadAjaxPanel ID="apMain" runat="server" LoadingPanelID="lpMain" ClientEvents-OnResponseEnd="InitDdls">
<div runat="server">
    <script type="text/javascript">
    	function openRadWindow(url) {
    		var oWnd = radopen(url, "rwValueEdit");
    		oWnd.Center();
    	}

    	function RowClick(ScoreCardMetricID) {
    		__doPostBack('<%=btAjax.ClientID%>', ScoreCardMetricID);
    	}
    	function rollIn(tr) {
    		if (tr.style.backgroundColorBak == null) {    			
    			tr.style.backgroundColorBak = tr.style.backgroundColor;
    		}
    		tr.style.backgroundColor = "#CDCDCD";
    	}
    	function rollOut(tr) {
    		tr.style.backgroundColor = tr.style.backgroundColorBak;
    	}


    	function AddComparePeriod(show) {
    		var trComparePeriod = document.getElementById("<%=trComparePeriod.ClientID%>");
    		var AddCompPeriodHref = document.getElementById("<%=AddCompPeriodHref.ClientID%>");
    		var hfComparePeriod = document.getElementById("<%=hfComparePeriod.ClientID%>");
    		
    		/*var style_display = "";
    		var style_display_i = "none";
    		if (!show) {
    			style_display = "none";
    			style_display_i = "";  			
    		}
    		trComparePeriod.style.display = style_display;
    		AddCompPeriodHref.style.display = style_display_i;
    		hfComparePeriod.value = style_display;*/
    		var style_visibility = "visible";
    		var style_visibility_i = "hidden";
    		if (!show) {
    			style_visibility = "hidden";
    			style_visibility_i = "visible";
    		}
    		trComparePeriod.style.visibility = style_visibility;
    		AddCompPeriodHref.style.visibility = style_visibility_i;
    		hfComparePeriod.value = style_visibility;
    	}

    	function GetAttribute(tr, Name) {
    		if (tr == null) return null;
    		var attr = tr.attributes;
    		if (attr == null) return null;
    		attr = attr[Name];
    		if (attr == null) return null;
    		return attr.nodeValue;
    	}

    	var DdlChanceLockCount = 0;
    	function DdlChange(ddl, dpBegin, dpEnd) {
    		var li = ddl.options[ddl.selectedIndex];
    		var db = GetAttribute(li, "DateBegin");
    		if (db == null) {
    			DdlChanceLockCount = 0;
    		}
    		else {
    			DdlChanceLockCount = 2;
    			dpBegin.set_selectedDate(new Date(db));
    			db = GetAttribute(li, "DateEnd");
    			dpEnd.set_selectedDate(new Date(db));
    		}
    	}

    	function ddlDatePeriodChange() {
    		var ddlDatePeriod = document.getElementById("<%=ddlDatePeriod.ClientID%>");
    		var dpDateBegin = $find("<%=dpDateBegin.ClientID%>");
    		var dpDateEnd = $find("<%=dpDateEnd.ClientID%>");
    		DdlChange(ddlDatePeriod, dpDateBegin, dpDateEnd);
    	}
    	function ddlCompDatePeriodChange() {
    		var ddlCompDatePeriod = document.getElementById("<%=ddlCompDatePeriod.ClientID%>");
    		var dpCompDateBegin = $find("<%=dpCompDateBegin.ClientID%>");
    		var dpCompDateEnd = $find("<%=dpCompDateEnd.ClientID%>");
    		DdlChange(ddlCompDatePeriod, dpCompDateBegin, dpCompDateEnd);
    	}
    	function InitDdls() {
    		var ddlDatePeriod = $("#<%=ddlDatePeriod.ClientID%>");
    		ddlDatePeriod.change(ddlDatePeriodChange);
    		var ddlCompDatePeriod = $("#<%=ddlCompDatePeriod.ClientID%>")
    		ddlCompDatePeriod.change(ddlCompDatePeriodChange);
    		DdlChanceLockCount = 0;
    	}
    	function dpDateChange() {
    		//alert(DdlChanceLockCount);
    		if (DdlChanceLockCount > 0) {
    			DdlChanceLockCount = DdlChanceLockCount - 1;
    			return;
    		}
    		var ddlDatePeriod = document.getElementById("<%=ddlDatePeriod.ClientID%>");
    		ddlDatePeriod.selectedIndex = 0;
    	}
    	function dpCompDateChange() {
    		//alert(DdlChanceLockCount);
    		if (DdlChanceLockCount > 0) {
    			DdlChanceLockCount = DdlChanceLockCount - 1;
    			return;
    		}
    		var ddlCompDatePeriod = document.getElementById("<%=ddlCompDatePeriod.ClientID%>");
    		ddlCompDatePeriod.selectedIndex = 0;
    	}
    	$(InitDdls);
    </script>            
</div>


		
<asp:Button runat="server" ID="btAjax" Text="Ajax" style="display:none; visibility:hidden" />		
<table cellpadding="0" cellspacing="9">
<asp:PlaceHolder runat="server" ID="phSelectPeriod">
	<tr>
		<td>
			<asp:DropDownList runat="server" ID="ddlDatePeriod">
				<asp:ListItem Text="Custom" Value="c" />
			
				<asp:ListItem Text="Today" Value="t_d" />
				<asp:ListItem Text="This Week" Value="t_w" />
				<asp:ListItem Text="This Week-to-date" Value="t_w_d" />
				<asp:ListItem Text="This Month" Value="t_m" />
				<asp:ListItem Text="This Month-to-date" Value="t_m_d" Selected="True" />
				<asp:ListItem Text="This Quarter" Value="t_q" />
				<asp:ListItem Text="This Quarter-to-date" Value="t_q_d" />
				<asp:ListItem Text="This Fiscal Quarter" Value="t_fq" />
				<asp:ListItem Text="This Fiscal Quarter-to-date" Value="t_fq_d" />
				<asp:ListItem Text="This Year" Value="t_y" />
				<asp:ListItem Text="This Year-to-date" Value="t_y_d" />
				<asp:ListItem Text="This Fiscal Year" Value="t_fy" />
				<asp:ListItem Text="This Fiscal Year-to-date" Value="t_fy_d" />
				
				<asp:ListItem Text="Yesterday" Value="l_d" />
				<asp:ListItem Text="Last Week" Value="l_w" />
				<asp:ListItem Text="Last Week-to-date" Value="l_w_d" />
				<asp:ListItem Text="Last Month" Value="l_m" />
				<asp:ListItem Text="Last Month-to-date" Value="l_m_d" />
				<asp:ListItem Text="Last Quarter" Value="l_q" />
				<asp:ListItem Text="Last Quarter-to-date" Value="l_q_d" />
				<asp:ListItem Text="Last Fiscal Quarter" Value="l_fq" />
				<asp:ListItem Text="Last Fiscal Quarter-to-date" Value="l_fq_d" />
				<asp:ListItem Text="Last Year" Value="l_y" />
				<asp:ListItem Text="Last Year-to-date" Value="l_y_d" />
				<asp:ListItem Text="Last Fiscal Year" Value="l_fy" />
				<asp:ListItem Text="Last Fiscal Year-to-date" Value="l_fy_d" />
			</asp:DropDownList>
			&nbsp;Begin:<telerik:RadDatePicker runat="server" ID="dpDateBegin"><ClientEvents OnDateSelected="dpDateChange" /></telerik:RadDatePicker>
			&nbsp;End:<telerik:RadDatePicker runat="server" ID="dpDateEnd"><ClientEvents OnDateSelected="dpDateChange" /></telerik:RadDatePicker>
			&nbsp;<a runat="server" id="AddCompPeriodHref" href="javascript:AddComparePeriod(true)" title="Add Compare Period 2">Add Compare Period 2</a>
			&nbsp; &nbsp;
			
		</td>
		<td><asp:Button runat="server" ID="bApplyPeriod" Text="Apply" ToolTip="Apply Period Settings" onclick="bApplyPeriod_Click" /></td>
		<td align="right" rowspan="2" valign="bottom"><a class="Cgv_AddNew" href="~/home/ScoreCardEdit.aspx" runat="server" id="aNewCard"><b>Create New Scorecard</b></a></td>
	</tr>

	<tr runat="server" id="trComparePeriod">
		<td><asp:HiddenField runat="server" ID="hfComparePeriod" />
			<asp:DropDownList runat="server" ID="ddlCompDatePeriod">
				<asp:ListItem Text="Custom" Value="c" />
			
				<asp:ListItem Text="Today" Value="t_d" />
				<asp:ListItem Text="This Week" Value="t_w" />
				<asp:ListItem Text="This Week-to-date" Value="t_w_d" />
				<asp:ListItem Text="This Month" Value="t_m" />
				<asp:ListItem Text="This Month-to-date" Value="t_m_d" />
				<asp:ListItem Text="This Quarter" Value="t_q" />
				<asp:ListItem Text="This Quarter-to-date" Value="t_q_d" />
				<asp:ListItem Text="This Fiscal Quarter" Value="t_fc" />
				<asp:ListItem Text="This Fiscal Quarter-to-date" Value="t_fc_d" />
				<asp:ListItem Text="This Year" Value="t_y" />
				<asp:ListItem Text="This Year-to-date" Value="t_y_d" />
				<asp:ListItem Text="This Fiscal Year" Value="t_fy" />
				<asp:ListItem Text="This Fiscal Year-to-date" Value="t_fy_d" />
				
				<asp:ListItem Text="Yesterday" Value="l_d" />
				<asp:ListItem Text="Last Week" Value="l_w" />
				<asp:ListItem Text="Last Week-to-date" Value="l_w_d" />
				<asp:ListItem Text="Last Month" Value="l_m"  Selected="True"/>
				<asp:ListItem Text="Last Month-to-date" Value="l_m_d" />
				<asp:ListItem Text="Last Quarter" Value="l_q" />
				<asp:ListItem Text="Last Quarter-to-date" Value="l_q_d" />
				<asp:ListItem Text="Last Fiscal Quarter" Value="l_fc" />
				<asp:ListItem Text="Last Fiscal Quarter-to-date" Value="l_fc_d" />
				<asp:ListItem Text="Last Year" Value="l_y" />
				<asp:ListItem Text="Last Year-to-date" Value="l_y_d" />
				<asp:ListItem Text="Last Fiscal Year" Value="l_fy" />
				<asp:ListItem Text="Last Fiscal Year-to-date" Value="l_fy_d" />
			</asp:DropDownList>
			&nbsp;Begin:<telerik:RadDatePicker runat="server" ID="dpCompDateBegin"><ClientEvents OnDateSelected="dpCompDateChange" /></telerik:RadDatePicker>
			&nbsp;End:<telerik:RadDatePicker runat="server" ID="dpCompDateEnd"><ClientEvents OnDateSelected="dpCompDateChange" /></telerik:RadDatePicker>
			<a href="javascript:AddComparePeriod(false)">Remove Compare Period 2</a>
		</td>
	</tr>
</asp:PlaceHolder>

<asp:PlaceHolder ID="phMyScoreCards" runat="server" Visible="false">
    <tr><td colspan="5" style="font-weight:bold;font-size:14pt;">My Score Cards</td></tr>
</asp:PlaceHolder>
<asp:Repeater runat="server" ID="rScoreCard" DataSourceID="dsScoreCard" OnItemDataBound="rScoreCard_ItemDataBound">
    <ItemTemplate>
        <asp:PlaceHolder ID="phPublicScoreCards" runat="server" Visible="false">
            <tr><td colspan="5" style="font-weight:bold;font-size:14pt;">Public Score Cards </td></tr>
            <tr><td  colspan="5">
                <asp:PlaceHolder ID="phViewAll" runat="server" Visible="false">
                    <asp:LinkButton ID="lbViewAll" runat="server" Font-Size="Small" oncommand="lbViewAll_Command" >View All Hidden Scorecards</asp:LinkButton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                </asp:PlaceHolder>
                <asp:LinkButton ID="lbHideAll" runat="server" Font-Size="Small" oncommand="lbHideAll_Command">Hide All Public Scorecards</asp:LinkButton></td></tr>
        </asp:PlaceHolder>
        <tr><td colspan="5">       
            <div runat="server" id="divMain">
				<asp:Panel runat="server" ID="pMetric">
				<table cellpadding="0" cellspacing="0" width="900px">				    
                    <tr runat="server" id="trCardHeader" visible='<%#!MyDashboardMode %>' >
                        <td class="CardBorderTitleLeft" width="6"> </td>
                        <td class="CardBorderTitleTop" width="888"><%#Eval("Name")%><%#GetScoreCardUser(Container)%></td>
                        <td class="CardBorderTitleRight" width="6"> </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="border-left:1px solid #D3D3D3;border-right:1px solid #D3D3D3"><mt:MTGridView runat="server" ID="cgvMetric" DataKeyNames="ScoreCardMetricID" ColorScheme="Gray" AutoGenerateColumns="False" Width=<%#Width%> EmptyDataText="No Metrics Found. Click to Add New">
                                <Columns>
                                    <mits:TemplateField HeaderStyle-Width="21" HeaderText="&nbsp;" ItemStyle-Width="21" Visible="false">
                                        <ItemTemplate>
                                            <asp:ImageButton runat="server" ID="ibRefresh" ImageUrl="~/images/buttons/refresh.gif" CommandArgument='<%#Eval("ScoreCardMetricID")%>' CommandName="Refresh" OnCommand="ibRefresh_Command" />
                                        </ItemTemplate>
                                    </mits:TemplateField>
                                    <mits:TextField HeaderText="Metric" DataField="MetricName" ItemStyle-Font-Size="Larger" HeaderStyle-Width="180" />
                                    <mits:TextField HeaderText="Org Location" DataField="OrgLocationName" HeaderStyle-Width="180" />
                                    <mits:TextField HeaderText="Period" DataField="ScoreCardPeriodName" HeaderStyle-Width="180" />
                                    <mits:TextField HeaderText="Period 1" DataField="CurrentValueStr" ItemStyle-Font-Size="Larger"/>
                                    <mits:TextField HeaderText="Period 2" DataField="PreviousValueStr" ItemStyle-Font-Size="Larger"/>
                                    <mits:TemplateField HeaderText="Change">
										<ItemTemplate>
											<span style="font-size:larger;"><nobr><%#GetImageHtml(Eval("ChangeValue"), Eval("GrowUpIsGood"))%>&nbsp;<%#Eval("ChangeValueStr")%></nobr></span>
										</ItemTemplate>
                                    </mits:TemplateField>
                                    <mits:TemplateField HeaderStyle-Width="21" HeaderText="&nbsp;" ItemStyle-Width="21">
                                        <ItemTemplate>
                                            <a href='<%#GetChartUrl(Container)%>' onclick="openRadWindow('<%#GetChartUrl(Container)%>');return false;" class="cursor"><img style="vertical-align:middle" border="0" src="../images/buttons/Chart.gif" width="16" height="16" alt="Chart" title="Open Chart" /></a>
                                        </ItemTemplate>
                                    </mits:TemplateField>
                                </Columns>
                            </mt:MTGridView>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3" style="border:1px solid #D3D3D3; border-top-style:none; padding:2" align="right">
                            <table cellpadding="0" cellspacing="0" style="padding-bottom:5px;padding-top:5px;">
                                <tr>
                                <asp:PlaceHolder ID="phHideLinks" runat="server" Visible="false">
                                        <td>
                                            <asp:LinkButton ID="lbHide" runat="server" Font-Size="Small" CommandName="Hide" CommandArgument='<%#Eval("ScoreCardID")%>' oncommand="lbHide_Command">Hide</asp:LinkButton>&nbsp;&nbsp;&nbsp;&nbsp;</td>
                                </asp:PlaceHolder>
                                <td style="padding-top:4px;">
                                        <asp:ImageButton ImageUrl="~/images/buttons/refresh.gif" ID="inhg" runat="server" OnCommand="lbRefreshAll_Command" CommandArgument='<%#Eval("ScoreCardID")%>' AlternateText="Refresh All"/>&nbsp;&nbsp;</td>
                                <td>
                                    <asp:LinkButton runat="server" ID="lbRefresh" OnCommand="lbRefreshAll_Command" Font-Size="Larger" CommandArgument='<%#Eval("ScoreCardID")%>'>Refresh All</asp:LinkButton>&nbsp; &nbsp; &nbsp;</td>
                                <td>
                                    <a href='<%#MyDashboardMode?"DashboardEdit.aspx":"ScoreCardEdit.aspx?ScoreCardID="+Eval("ScoreCardID")%>' style="font-size:larger;">Set Up</a>&nbsp; &nbsp; &nbsp;</td></tr>
                            </table>
                        </td>
                    </tr>
                </table>
                
                <telerik:RadAjaxLoadingPanel runat="server" ID="ralpMetric" Wrap="true" Transparency="33" BackColor="LightGray" ><table style="height:100%; width:100%">
                    <tr><td valign="middle" align="center"><img alt="Loading ..." src="../images/loading6.gif" /></td></tr>
                </table></telerik:RadAjaxLoadingPanel>
            </asp:Panel></div>
        </td></tr>
    </ItemTemplate>
</asp:Repeater>
<tr><td colspan="3"><asp:LinkButton ID="lbViewAllAlternative" runat="server" Font-Size="Small" oncommand="lbViewAll_Command" Visible="false">View All Hidden Scorecards</asp:LinkButton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td></tr>
</table>
<telerik:RadAjaxLoadingPanel runat="server" ID="lpMain" Wrap="true" Transparency="33" BackColor="LightGray" ><table style="height:100%; width:100%">
    <tr><td valign="middle" align="center"><img alt="Loading ..." src="../images/loading6.gif" /></td></tr>
</table></telerik:RadAjaxLoadingPanel>
</telerik:RadAjaxPanel>
<asp:LinqDataSource runat="server" ID="dsScoreCard" TableName="ScoreCard" OnSelecting="dsScoreCard_Selecting"/>