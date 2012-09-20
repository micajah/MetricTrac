<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MetricInputList.ascx.cs" Inherits="MetricTrac.Control.MetricInputList" %>
<%@ Register src="MetricFilter.ascx" tagprefix="uc" tagname="MetricFilter" %>
<%@ Register src="UnderReviewQueue.ascx" tagname="UnderReviewQueue" tagprefix="uc" %>
<%@ Register src="GroupByList.ascx" tagname="GroupByList" tagprefix="uc" %>
<div id="Div1" runat="server">
<telerik:RadAjaxPanel runat="server" ID="rapFrequency" LoadingPanelID="RadAjaxLoadingPanel1" EnableEmbeddedScripts="true" EnableAJAX="true" OnAjaxRequest="rapFrequency_AjaxRequest" 
        ClientEvents-OnRequestStart="rapFrequency_RequestStart">
    <telerik:RadWindowManager ID="rwmEditValue" runat="server" Modal="true" VisibleStatusbar="false" Height="525px" Width="775px" DestroyOnClose="true" style="z-index: 90000" />
        <div runat="server">
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
        
        <div style="width:100%">
            <uc:GroupByList ID="cGroupBy" runat="server" ExtendedMode="True" />
            <uc:MetricFilter runat="server" ID="MF" OnUse="MF_Use" ExportVisible="true" BaseDateVisible="true" />
        </div>        
        <asp:Panel ID="pnlAlert" runat="server"><br />
            <uc:UnderReviewQueue ID="AlertQueue" runat="server" />        
        </asp:Panel>        
        
        <div class="metric"><asp:Label runat="server" style="display:none" ID="lbDebugInfo"/>
        <table cellspacing="0" id="tFrequency">            
        <asp:Repeater runat="server" ID="rFrequency" OnItemDataBound="rFrequency_ItemDataBound" EnableViewState="false">
            <ItemTemplate>
                <tr><th colspan='4' class="flagcell"><span class="flag"><%#Eval("Name")%></span></th></tr>
                <asp:Repeater runat="server" ID="rMetric" OnItemDataBound="rpMetric_ItemDataBound">
                    <ItemTemplate>
                        <asp:PlaceHolder runat="server" Visible='<%#IsNewGroup(Container)%>'>
                            <tr><th colspan="34" class="empty" style="height:27px"></th></tr>
                            <asp:PlaceHolder ID="phCategory" runat="server" Visible='<%#IsNewMetricCategory(Container)%>'>
                                <tr><td colspan="34" style="border:none; text-align:left; font-size:110%; color:Gray;"><%#Eval("MetricCategoryName")%></td></tr>
                            </asp:PlaceHolder>
                            <tr>
                                <th class="empty">&nbsp;</th>
                                <th class="title"><%#GroupByMetric?Eval("Name"):Eval("OrgLocationFullName")%>&nbsp;
                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#(GroupByMetric && ((int)Eval("MetricTypeID") != 1))%>'><a href='<%#GetRelatedUrl(Container)%>' class="cursor">(Calc&nbsp;Metric)</a></asp:PlaceHolder>
                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#(GroupByMetric && DataMode == Mode.Input && ((int)Eval("MetricTypeID") == 1) && ((bool)Eval("AllowCustomNames")) && !String.IsNullOrEmpty((String)Eval("MetricOrgLocationAlias")) )%>'>&nbsp;&nbsp;<span style="font-size:80%;font-weight:normal;color:#666666;">Alias</span>&nbsp;<%#Eval("MetricOrgLocationAlias")%>&nbsp;</asp:PlaceHolder>
                                    <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#(GroupByMetric && DataMode == Mode.Input && ((int)Eval("MetricTypeID") == 1) && ((bool)Eval("AllowCustomNames")) && !String.IsNullOrEmpty((String)Eval("MetricOrgLocationCode")) )%>'>&nbsp;&nbsp;<span style="font-size:80%;font-weight:normal;color:#666666;">Code</span>&nbsp;<%#Eval("MetricOrgLocationCode")%>&nbsp;</asp:PlaceHolder></th>
                                <th align="right"><asp:PlaceHolder ID="phBulkEdit" runat="server" Visible='<%#DataMode != Mode.View %>'><a href='<%# (DataMode == Mode.Input ? "MetricBulkInput" : "MetricBulkApprove") + ".aspx?FrequencyID=" + Eval("FrequencyID") + "&" + (GroupByMetric ? "MetricID=" + Eval("MetricID") : "OrgLocationID=" + Eval("OrgLocationID"))%>' class="quickedit" >Bulk&nbsp;<%#DataMode == Mode.Input ? "Edit" : "Approve" %></a></asp:PlaceHolder></th>
                                <th class="arrow">
                                    <asp:HyperLink runat="server" ID="hlNavL1" NavigateUrl='<%#"javascript:DateNavigation(\"L1"+Eval("FrequencyID")+"\")"%>' EnableViewState="false">
                                        <asp:Image runat="server" ID="iNavL1" ImageUrl="~/images/buttons/arrow-left.png" ToolTip='<%#GetTitle((int)Eval("FrequencyID"),"L1")%>' AlternateText='<%#GetTitle((int)Eval("FrequencyID"),"L1")%>' EnableViewState="false" />
                                    </asp:HyperLink>
                                </th>
                                <asp:Repeater runat="server" ID="rDate" OnItemDataBound="rDate_ItemDataBound">
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
                            <td colspan="2" class="location" align="right">
                                <asp:PlaceHolder ID="tMetricAlias" runat="server" Visible='<%#(!GroupByMetric && DataMode == Mode.Input && ((int)Eval("MetricTypeID") == 1) && ((bool)Eval("AllowCustomNames")) && (!String.IsNullOrEmpty((String)Eval("MetricOrgLocationAlias")) || !String.IsNullOrEmpty((String)Eval("MetricOrgLocationCode"))) )%>'>
                                    <div>
                                        <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#(!String.IsNullOrEmpty((String)Eval("MetricOrgLocationAlias")) )%>'><%#Eval("MetricOrgLocationAlias")%></asp:PlaceHolder>
                                        <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#(!String.IsNullOrEmpty((String)Eval("MetricOrgLocationAlias")) && !String.IsNullOrEmpty((String)Eval("MetricOrgLocationCode")))%>'>&nbsp;-&nbsp;</asp:PlaceHolder>
                                        <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#(!String.IsNullOrEmpty((String)Eval("MetricOrgLocationCode")) )%>'><%#Eval("MetricOrgLocationCode")%></asp:PlaceHolder>
                                        &nbsp;<a href='<%#GetChartUrl(Container)%>' onclick="openRadWindow('<%#GetChartUrl(Container)%>');return false;" class="cursor"><img style="vertical-align:middle" border="0" src="../images/buttons/Chart.gif" width="16" height="16" alt="Chart" title="Open Chart" /></a>
                                    </div>
                                    <div style="padding-right:24px;">
                                        <span style="color:#aaaaaa;">Metric</span>&nbsp;<%#(Eval("Name")) %>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="phetricAlias" runat="server" Visible='<%#!(!GroupByMetric && DataMode == Mode.Input && ((int)Eval("MetricTypeID") == 1) && ((bool)Eval("AllowCustomNames")) && (!String.IsNullOrEmpty((String)Eval("MetricOrgLocationAlias")) || !String.IsNullOrEmpty((String)Eval("MetricOrgLocationCode"))) )%>'>                                
                                    <%#GroupByMetric ? Eval("OrgLocationFullName") : (((int)Eval("MetricTypeID")==2?"<b>":"")+Eval("Name")) %>
                                    &nbsp;
                                    <asp:PlaceHolder ID="phChart" runat="server" Visible='<%#!IsTotalMetric(Container)%>'>
                                        <a href='<%#GetChartUrl(Container)%>' onclick="openRadWindow('<%#GetChartUrl(Container)%>');return false;" class="cursor"><img style="vertical-align:middle" border="0" src="../../images/buttons/Chart.gif" width="16" height="16" alt="Chart" title="Open Chart" /></a>
                                    </asp:PlaceHolder>
                                </asp:PlaceHolder>
                            </td>
                            <td runat="server" id="tdUnitLeft" class="unit" title='<%#Eval("InputUnitOfMeasureNameTooltip")%>' visible="false"><%#Eval("InputUnitOfMeasureName")%></td>
                            <asp:Repeater runat="server" ID="rMericValue" OnItemDataBound="rMericValue_ItemDataBound">
                                <ItemTemplate>
                                    <td class="cal-metric<%#(IsEmptyValue(Container))? ((DataMode == Mode.Input && !IsEditValue(Container)) ? " empty-disabled" : " empty"):""%>" title='<%#GetValueTitle(Container)%>'>                                    
                                        <asp:PlaceHolder runat="server" Visible='<%#IsEditValue(Container)%>'><a title='<%#GetValueTitle(Container)%>' href='<%#GetEditUrl(Container)%>' <%#GetOnClickHandler(Container)%> class="cursor"></asp:PlaceHolder>                                            
                                                <big <%#GetValueCss(Container)%> <%#GetCalcStatusStyle(Container)%>><%#GetValueCell(Container)%></big>
                                        <asp:PlaceHolder runat="server" Visible='<%#IsEditValue(Container)%>'></a></asp:PlaceHolder>
                                    </td>
                                </ItemTemplate>
                            </asp:Repeater>
                            <td runat="server" id="tdUnitRight" class="unit" title='<%#Eval("InputUnitOfMeasureNameTooltip")%>' visible="false"><%#Eval("InputUnitOfMeasureName")%></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </ItemTemplate>
        </asp:Repeater>  
        </table>
        </div>
    <telerik:RadAjaxLoadingPanel runat="server" ID="RadAjaxLoadingPanel1" Wrap="true" Transparency="33" BackColor="LightGray" ><table style="height:100%; width:100%">
            <tr><td valign="top" align="center"><img alt="Loading ..." src="../images/loading6.gif" /></td></tr>
    </table></telerik:RadAjaxLoadingPanel>
</telerik:RadAjaxPanel>
</div>