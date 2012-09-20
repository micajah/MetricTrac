<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApproveWorkList.aspx.cs" Inherits="MetricTrac.ApproveWorkList" MasterPageFile="~/MasterPage.master"%>
<%@ Register src="~/Control/GroupByList.ascx" tagname="GroupByList" tagprefix="uc" %>
<asp:Content ID="cntMetric" ContentPlaceHolderID="PageBody" runat="server">    
<div id="Div1" runat="server">
    <telerik:RadWindowManager ID="rwmEditValue" runat="server" Modal="true" VisibleStatusbar="false" Height="525px" Width="775px" DestroyOnClose="true"  style="z-index: 90000"/>
        <div id="Div2" runat="server">
            <script type="text/javascript">
                function openRadWindow(url) {
                    var oWnd = radopen(url, "rwValueEdit");
                    oWnd.Center();
                }

                function OnRequestStart(sender, args) {
                    document.body.style.cursor = "wait";
                }

                function OnResponseEnd(sender, args) {
                    document.body.style.cursor = "default";
                }
            </script>            
        </div>        
        
        <div style="width:100%">
            <uc:GroupByList ID="GroupBy" runat="server" />
        </div>
        <div class="metric">
        <table cellspacing="0" border="0">
            <asp:Repeater runat="server" ID="rMetric" OnItemDataBound="rpMetric_ItemDataBound" OnPreRender="rMetric_PreRender"> 
                <ItemTemplate>
                    <tr><th colspan="3" class="empty" style="height:20px"></th></tr>
                    <tr>
                        <th class="empty">&nbsp;</th>                            
                        <th class="title">                                
                            <%# Eval("EntityName")%>
                            &nbsp;
                            <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#(GroupByMetric && ((int)Eval("MetricTypeID") != 1))%>'>
                                <a href='<%#"RelatedInputValuesA.aspx?MetricID=" + Eval("EntityID")%>' class="cursor">(Calc&nbsp;Metric)</a>
                            </asp:PlaceHolder></th>
                        <th align="right">
                            <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#GroupByMetric%>'>
                            <a href='<%# "MetricBulkApprove.aspx?FrequencyID=" + Eval("FrequencyID") + "&MetricID=" + Eval("EntityID")%>' class="quickedit" >
                                Bulk&nbsp;Approve
                            </a>
                            </asp:PlaceHolder></th>
                    </tr>
                    <tr>
                        <th class="empty">&nbsp;</th> 
                        <td colspan="2">
                            <asp:Panel ID="pnlUpdate" runat="server">                                 
                                <mt:MTGridView runat="server" ID="cgvMetricValue" DataKeyNames="MetricValueID" ColorScheme="Gray" AutoGenerateColumns="False">
                                    <Columns>                                    
                                        <mits:TemplateField>
                                            <HeaderTemplate>
                                                <%#GroupByMetric ? "Org Location" : "Metric" %>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <span title='<%#GroupByMetric ? "" : ("Metric Category: " + Eval("MetricCategoryName"))%>'><%#GroupByMetric ? Eval("OrgLocationFullName") : Eval("MetricName")%></span>                                                    
                                            </ItemTemplate>
                                        </mits:TemplateField>
                                        <mits:TemplateField HeaderText="Status" ItemStyle-Width="100px">                                            
                                            <ItemTemplate>
                                                <div title='<%#GetValueTitle(Container)%>'><%#Eval("ApprovalStatus")%></div>                                                    
                                            </ItemTemplate>
                                        </mits:TemplateField>
                                        <mits:TextField HeaderText="Frequency" DataField="ValueFrequencyName" ItemStyle-Width="80px" />
                                        <mits:TextField HeaderText="Period" DataField="Period" ItemStyle-Width="50px" />
                                        <mits:TemplateField HeaderText="Value" ItemStyle-Width="110px">
                                            <ItemStyle HorizontalAlign="Right" />
                                            <ItemTemplate>
                                                <a href='<%#GetEditUrl(Container)%>' <%#GetOnClickHandler(Container)%> class="cursor">                                                    
                                                    <span style="font-size:10pt;"><%#GetValueCell(Container)%></span>
                                                </a>                                                
                                            </ItemTemplate>
                                        </mits:TemplateField>
                                        <mits:TextField HeaderText="Notes" DataField="Notes" ItemStyle-Width="200px" />
                                    </Columns>
                                </mt:MTGridView>
                                <asp:Panel ID="pnlPager" runat="server">
                                </asp:Panel>
                                <asp:HiddenField ID="hfLinksMemory" runat="server" />   
                            </asp:Panel>                                                                
                         </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>          
        </table>
        </div>
    <telerik:RadAjaxLoadingPanel runat="server" ID="ralGrid" Wrap="true" Transparency="33" BackColor="LightGray" ><table style="height:100%; width:100%">
            <tr><td valign="top" align="center"><img alt="Loading ..." src="../images/loading6.gif" /></td></tr>
    </table></telerik:RadAjaxLoadingPanel>
</div>
    
</asp:Content>