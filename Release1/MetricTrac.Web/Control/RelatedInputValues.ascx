<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RelatedInputValues.ascx.cs" Inherits="MetricTrac.Control.RelatedInputValues" %>
<%@ Register src="MetricFilter.ascx" tagprefix="uc" tagname="MetricFilter" %>
<div id="Div1" runat="server">
<telerik:RadAjaxPanel runat="server" ID="rapFrequency" LoadingPanelID="RadAjaxLoadingPanel1" EnableEmbeddedScripts="true" EnableAJAX="true" OnAjaxRequest="rapFrequency_AjaxRequest" 
        ClientEvents-OnRequestStart="rapFrequency_RequestStart">
    <telerik:RadWindowManager ID="rwmEditValue" runat="server" Modal="true" VisibleStatusbar="false" Height="525px" Width="775px" DestroyOnClose="true"  style="z-index: 90000"/>
        <div id="Div2" runat="server">
            <script type="text/javascript">
                function openRadWindow(url) {
                    var oWnd = radopen(url, "rwValueEdit");
                    oWnd.Center();
                }
                function DateNavigation(arg) {
                    var rapFrequency = $find("<%=rapFrequency.ClientID%>");
                    rapFrequency.ajaxRequest(arg);
                }                
                function rapFrequency_RequestStart() {
                    var p = $find("<%= RadAjaxLoadingPanel1.ClientID %>");
                    p.show("<%= rapFrequency.ClientID %>");
                }
                function rapFrequency_ResponseEnd() {
                    var p = $find("<%= RadAjaxLoadingPanel1.ClientID %>");
                    p.hide("<%= rapFrequency.ClientID %>");
                }
            </script>            
        </div>        
        
        <div style="width:100%;">
        <table border="0" cellpadding="0" cellspacing="0"><tr>
            <td align="left"><uc:MetricFilter runat="server" ID="MF" GroupByVisible="true" FilterSectionVisible="false" /></td>
            <td align="left"><asp:HyperLink ID="hlBack" runat="server" Font-Size="Larger" /></td></tr>
        </table></div>
        <div class="metric">        
        <table cellspacing="0" id="tFrequency" border="0">        
            <tr><th colspan='4' class="flagcell"><asp:Label ID="lblFrequency" runat="server" CssClass="flag" /></th>
            <asp:Repeater runat="server" ID="rFormula">
                <ItemTemplate>
                    <th colspan='<%#Eval("ColGroup")%>' class="for-head"><%#Eval("sFormula")%></th>
                </ItemTemplate>
            </asp:Repeater></tr>
            <asp:Repeater runat="server" ID="rMetric" OnItemDataBound="rpMetric_ItemDataBound">
                <ItemTemplate>
                    
                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#IsNewGroup(Container)%>'>
                        <tr><th colspan="34" class="empty" style="height:27px"></th></tr>
                        <tr>
                            <th class="empty">&nbsp;</th>
                            <th class="title"><%#GroupByMetric?Eval("Name"):Eval("OrgLocationFullName")%></th>                            
                            <th class="arrow">
                                <asp:HyperLink runat="server" ID="hlNavL1" NavigateUrl='<%#"javascript:DateNavigation(\"L1"+Eval("FrequencyID")+"\")"%>' EnableViewState="false">
                                    <asp:Image runat="server" ID="iNavL1" ImageUrl="~/images/buttons/arrow-left.png" ToolTip='<%#GetTitle((int)Eval("FrequencyID"),"L1")%>' AlternateText='<%#GetTitle((int)Eval("FrequencyID"),"L1")%>' EnableViewState="false" />
                                </asp:HyperLink>
                            </th>
                            <asp:Repeater runat="server" ID="rDate">
                                <ItemTemplate>
                                    <th class="cal-head"><%#Eval("sDate")%></th>
                                </ItemTemplate>
                            </asp:Repeater>
                            <th class="arrow">
                                <asp:HyperLink runat="server" ID="hlNavR1" NavigateUrl='<%#"javascript:DateNavigation(\"R1"+Eval("FrequencyID")+"\")"%>' EnableViewState="false">
                                    <asp:Image runat="server" ID="iNavR1" ImageUrl="~/images/buttons/arrow-right.png" ToolTip='<%#GetTitle((int)Eval("FrequencyID"),"R1")%>' AlternateText='<%#GetTitle((int)Eval("FrequencyID"),"R1")%>' EnableViewState="false" />
                                </asp:HyperLink>
                            </th>
                        </tr>
                    </asp:PlaceHolder>
                    <tr>
                        <th class="empty">&nbsp;</th>
                        <td class="location" align="right"><%#GroupByMetric ? Eval("OrgLocationFullName") : (((int)Eval("MetricTypeID")==2?"<b>":"")+Eval("Name")) %>&nbsp;<a href='<%#GetChartUrl(Container)%>' onclick="openRadWindow('<%#GetChartUrl(Container)%>');return false;" class="cursor"><img style="vertical-align:middle" border="0" src="../images/buttons/Chart.gif" width="16" height="16" alt="Chart" title="Open Chart" /></a></td>
                        <td runat="server" id="tdUnitLeft" class="unit" title='<%#Eval("InputUnitOfMeasureNameTooltip")%>' visible="false"><%#Eval("InputUnitOfMeasureName")%></td>
                        <asp:Repeater runat="server" ID="rMericValue">
                            <ItemTemplate>
                                <td class="cal-metric<%#GetEmptyStyle(Container)%>" title='<%#GetValueTitle(Container)%>'><nobr>
                                    <asp:PlaceHolder runat="server" Visible='<%#IsEditValue(Container)%>'><a title='<%#GetValueTitle(Container)%>' href='<%#GetEditUrl(Container)%>' <%#GetOnClickHandler(Container)%> class="cursor"></asp:PlaceHolder>
                                        <big <%#GetValueCss(Container)%> <%#GetCalcStatusStyle(Container)%>><%#GetValueCell(Container)%></big>
                                    <asp:PlaceHolder runat="server" Visible='<%#IsEditValue(Container)%>'></a></asp:PlaceHolder>
                                </nobr></td>
                            </ItemTemplate>
                        </asp:Repeater>
                        <td runat="server" id="tdUnitRight" class="unit" title='<%#Eval("InputUnitOfMeasureNameTooltip")%>' visible="false"><%#Eval("InputUnitOfMeasureName")%></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>            
        </table>
        </div>
    <telerik:RadAjaxLoadingPanel runat="server" ID="RadAjaxLoadingPanel1" Wrap="true" Transparency="33" BackColor="LightGray" ><table style="height:100%; width:100%">
            <tr><td valign="top" align="center"><img alt="Loading ..." src="../images/loading6.gif" /></td></tr>
    </table></telerik:RadAjaxLoadingPanel>
</telerik:RadAjaxPanel>
</div>