<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ScoreCardList.ascx.cs" Inherits="MetricTrac.Control.ScoreCardList" %>
<telerik:RadWindowManager ID="rwmEditValue" runat="server" Modal="true" VisibleStatusbar="false" Height="525px" Width="897px" DestroyOnClose="true"  style="z-index: 90000"/>
<div id="Div1" runat="server">
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
    </script>            
</div>


		
<asp:Button runat="server" ID="btAjax" Text="Ajax" style="display:none; visibility:hidden" />		
<table cellpadding="0" cellspacing="9">
<tr>
    <td align="left"><telerik:RadDatePicker Visible="false" runat="server" ID="rdpDate" AutoPostBack="true" Skin="Telerik" /></td>
	<td align="right"><a class="Cgv_AddNew" href="~/home/ScoreCardEdit.aspx" runat="server" id="aNewCard"><b>Create New Scorecard</b></a></td>
</tr>
<asp:PlaceHolder ID="phMyScoreCards" runat="server" Visible="false">
    <tr><td colspan="2" style="font-weight:bold;font-size:14pt;">My Score Cards</td></tr>
</asp:PlaceHolder>
<asp:Repeater runat="server" ID="rScoreCard" DataSourceID="dsScoreCard" OnItemDataBound="rScoreCard_ItemDataBound">
    <ItemTemplate>
        <asp:PlaceHolder ID="phPublicScoreCards" runat="server" Visible="false">
            <tr><td colspan="2" style="font-weight:bold;font-size:14pt;">Public Score Cards </td></tr>
            <tr><td  colspan="2">
                <asp:PlaceHolder ID="phViewAll" runat="server" Visible="false">
                    <asp:LinkButton ID="lbViewAll" runat="server" Font-Size="Small" oncommand="lbViewAll_Command" >View All Hidden Scorecards</asp:LinkButton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                </asp:PlaceHolder>
                <asp:LinkButton ID="lbHideAll" runat="server" Font-Size="Small" oncommand="lbHideAll_Command">Hide All Public Scorecards</asp:LinkButton></td></tr>
        </asp:PlaceHolder>
        <tr><td colspan="2">        
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
                                    <mits:TextField HeaderText="Period" DataField="ScoreCardPeriodName" HeaderStyle-Width="135" />
                                    <mits:TextField HeaderText="Current" DataField="CurrentValueStr" ItemStyle-Font-Size="Larger"/>
                                    <mits:TextField HeaderText="Previous" DataField="PreviousValueStr" ItemStyle-Font-Size="Larger"/>
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
<tr><td colspan="2"><asp:LinkButton ID="lbViewAllAlternative" runat="server" Font-Size="Small" oncommand="lbViewAll_Command" Visible="false">View All Hidden Scorecards</asp:LinkButton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td></tr>
</table>
<asp:LinqDataSource runat="server" ID="dsScoreCard" TableName="ScoreCard" OnSelecting="dsScoreCard_Selecting"/>