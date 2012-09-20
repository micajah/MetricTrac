<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MissedInputValues.ascx.cs" Inherits="MetricTrac.Control.MissedInputValues" %>
<%@ Register src="GroupByList.ascx" tagname="GroupByList" tagprefix="uc" %>
<div id="Div1" runat="server">
<telerik:RadAjaxPanel runat="server" ID="rapFrequency" LoadingPanelID="RadAjaxLoadingPanel1" EnableEmbeddedScripts="true" EnableAJAX="true" OnAjaxRequest="rapFrequency_AjaxRequest" 
        ClientEvents-OnRequestStart="mrapFrequency_RequestStart">
    <telerik:RadWindowManager ID="rwmEditValue" runat="server" Modal="true" VisibleStatusbar="false" Height="525px" Width="775px" DestroyOnClose="true" style="z-index: 90000" />
        <div id="Div2" runat="server">
            <script type="text/javascript">
                function mopenRadWindow(url) {
                    var oWnd = radopen(url, "mrwValueEdit");
                    oWnd.Center();
                }
                function mDateNavigation(arg) {
                    var rapFrequency = $find("<%=rapFrequency.ClientID%>");
                    rapFrequency.ajaxRequest(arg);
                }                
                function mrapFrequency_RequestStart() {
                    var p = $find("<%= RadAjaxLoadingPanel1.ClientID %>");
                    p.show("<%= rapFrequency.ClientID %>");
                }
                function mrapFrequency_ResponseEnd() {
                    var p = $find("<%= RadAjaxLoadingPanel1.ClientID %>");
                    p.hide("<%= rapFrequency.ClientID %>");
                }
            </script>
        </div>        
        <mits:NoticeMessageBox ID="lblAlert" runat="server" MessageType="Error" Message="Missed Input Metric Values" Description="" Width="500px" /><br />
        <div style="width:100%">
            <uc:GroupByList ID="GroupBy" runat="server" />
        </div>        
        <div class="metric">
        <table cellspacing="0" id="tFrequency" style="border-bottom-color:#ff6666;border-bottom-width:2px; border-bottom-style:dashed;" >
        <asp:Repeater runat="server" ID="rFrequency" OnItemDataBound="rFrequency_ItemDataBound" EnableViewState="false">
            <ItemTemplate>
                <tr><th colspan='3' class="flagcell"><span class="flag"><%#Eval("Name")%></span></th></tr>
                <asp:Repeater runat="server" ID="rMetric" OnItemDataBound="rpMetric_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <th class="empty">&nbsp;</th>
                            <th>&nbsp;</th>
                            <th>&nbsp;</th>                            
                            <th class="arrow">                                    
                                <asp:HyperLink runat="server" ID="hlNavL1" NavigateUrl='<%#"javascript:mDateNavigation(\"L1|" + Eval("FrequencyID") + "|" + Eval("MetricID") + "|" + Eval("OrgLocationID") + "|" + Eval("PreviousDate") + "|" + Eval("NextDate") + "|" + Eval("IsNextValues") + "\")"%>' EnableViewState="false">
                                    <asp:Image runat="server" ID="iNavL1" ImageUrl="~/images/buttons/arrow-left.png" ToolTip='<%#GetTitle((int)Eval("FrequencyID"),"L1")%>' AlternateText='<%#GetTitle((int)Eval("FrequencyID"),"L1")%>' EnableViewState="false" />
                                </asp:HyperLink>                                    
                            </th>                                
                            <asp:Repeater runat="server" ID="rDate" >
                                <ItemTemplate>
                                    <th class="cal-head"><%#Eval("sDate")%></th>
                                </ItemTemplate>
                            </asp:Repeater>
                            <th class="arrow">                                    
                                <asp:HyperLink runat="server" ID="hlNavR1" NavigateUrl='<%#"javascript:mDateNavigation(\"R1|" + Eval("FrequencyID") + "|" + Eval("MetricID") + "|" + Eval("OrgLocationID") + "|" + Eval("PreviousDate") + "|" + Eval("NextDate") + "|" + Eval("IsNextValues") + "\")"%>' EnableViewState="false">
                                    <asp:Image runat="server" ID="iNavR1" ImageUrl="~/images/buttons/arrow-right.png" ToolTip='<%#GetTitle((int)Eval("FrequencyID"),"R1")%>' AlternateText='<%#GetTitle((int)Eval("FrequencyID"),"R1")%>' EnableViewState="false" />
                                </asp:HyperLink>                                
                            </th>
                        </tr>                        
                        <tr>
                            <th class="empty">&nbsp;</th>
                            <td class="title">
                                <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#(GroupByMetric && ((int)Eval("MetricTypeID") == 1) && ((bool)Eval("AllowCustomNames")) && (!String.IsNullOrEmpty((String)Eval("MetricOrgLocationAlias")) || !String.IsNullOrEmpty((String)Eval("MetricOrgLocationCode"))) )%>'>
                                    <div>
                                        <asp:PlaceHolder ID="PlaceHolder7" runat="server" Visible='<%#(!String.IsNullOrEmpty((String)Eval("MetricOrgLocationAlias")) )%>'><%#Eval("MetricOrgLocationAlias")%></asp:PlaceHolder>
                                        <asp:PlaceHolder ID="PlaceHolder8" runat="server" Visible='<%#(!String.IsNullOrEmpty((String)Eval("MetricOrgLocationAlias")) && !String.IsNullOrEmpty((String)Eval("MetricOrgLocationCode")))%>'>&nbsp;-&nbsp;</asp:PlaceHolder>
                                        <asp:PlaceHolder ID="PlaceHolder9" runat="server" Visible='<%#(!String.IsNullOrEmpty((String)Eval("MetricOrgLocationCode")) )%>'><%#Eval("MetricOrgLocationCode")%></asp:PlaceHolder>                                        
                                    </div>
                                    <div>
                                        <span style="color:#aaaaaa;">Metric</span>&nbsp;<%#(Eval("Name")) %>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="PlaceHolder10" runat="server" Visible='<%#!(GroupByMetric && ((int)Eval("MetricTypeID") == 1) && ((bool)Eval("AllowCustomNames")) && (!String.IsNullOrEmpty((String)Eval("MetricOrgLocationAlias")) || !String.IsNullOrEmpty((String)Eval("MetricOrgLocationCode"))) )%>'>                                
                                    <%#GroupByMetric ? Eval("Name") : Eval("OrgLocationFullName")%>
                                </asp:PlaceHolder>
                                
                            </td> 
                            <td class="location" align="right">
                                 <asp:PlaceHolder ID="tMetricAlias" runat="server" Visible='<%#(!GroupByMetric && ((int)Eval("MetricTypeID") == 1) && ((bool)Eval("AllowCustomNames")) && (!String.IsNullOrEmpty((String)Eval("MetricOrgLocationAlias")) || !String.IsNullOrEmpty((String)Eval("MetricOrgLocationCode"))) )%>'>
                                    <div>
                                        <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#(!String.IsNullOrEmpty((String)Eval("MetricOrgLocationAlias")) )%>'><%#Eval("MetricOrgLocationAlias")%></asp:PlaceHolder>
                                        <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#(!String.IsNullOrEmpty((String)Eval("MetricOrgLocationAlias")) && !String.IsNullOrEmpty((String)Eval("MetricOrgLocationCode")))%>'>&nbsp;-&nbsp;</asp:PlaceHolder>
                                        <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#(!String.IsNullOrEmpty((String)Eval("MetricOrgLocationCode")) )%>'><%#Eval("MetricOrgLocationCode")%></asp:PlaceHolder>                                        
                                    </div>
                                    <div>
                                        <span style="color:#aaaaaa;">Metric</span>&nbsp;<%#(Eval("Name")) %>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="phetricAlias" runat="server" Visible='<%#!(!GroupByMetric && ((int)Eval("MetricTypeID") == 1) && ((bool)Eval("AllowCustomNames")) && (!String.IsNullOrEmpty((String)Eval("MetricOrgLocationAlias")) || !String.IsNullOrEmpty((String)Eval("MetricOrgLocationCode"))) )%>'>                                
                                    <%#GroupByMetric ? Eval("OrgLocationFullName") : (((int)Eval("MetricTypeID") == 2 ? "<b>" : "") + Eval("Name"))%>
                                </asp:PlaceHolder>
                            </td>
                            <td runat="server" id="tdUnitLeft" title='<%#Eval("InputUnitOfMeasureNameTooltip")%>' class="unit"><%#Eval("InputUnitOfMeasureName")%></td>
                            <asp:Repeater runat="server" ID="rMericValue" >
                                <ItemTemplate>
                                    <td class="cal-metric<%#(IsEmptyValue(Container))? " empty" : (!IsCollectingValue(Container) ? " empty-disabled" : "")%>"><nobr>                                    
                                        <asp:PlaceHolder runat="server" Visible='<%#IsEditValue(Container)%>'><a href='<%#GetEditUrl(Container)%>' <%#GetOnClickHandler(Container)%> class="cursor"></asp:PlaceHolder>                                            
                                            <big <%#GetValueCss(Container)%>><%#GetValueCell(Container)%></big>
                                        <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#IsEditValue(Container)%>'></a></asp:PlaceHolder>
                                    </nobr></td>
                                </ItemTemplate>
                            </asp:Repeater>
                            <td runat="server" id="tdUnitRight" title='<%#Eval("InputUnitOfMeasureNameTooltip")%>' class="unit" ><%#Eval("UnitOfMeasureName")%></td>
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
&nbsp;</div>