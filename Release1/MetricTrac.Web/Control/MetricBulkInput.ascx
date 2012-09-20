<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MetricBulkInput.ascx.cs" Inherits="MetricTrac.Control.MetricBulkInput" %>
<asp:Panel ID="pnlUpdate" runat="server"> 
<div id="Div1" runat="server">
        <telerik:RadWindowManager ID="rwmEditValue" runat="server" Modal="true" VisibleStatusbar="false" Height="525px" Width="775px" DestroyOnClose="true" OnClientClose="clientClose"  style="z-index: 90000"/>
        <div id="Div2" runat="server">
            <script type="text/javascript">
                function window_onbeforeunload() {
                    if (IsDataChanged && !DisableWarning)
                        return "There are unsaved metric values on this page!";
                }
                window.onbeforeunload = window_onbeforeunload;
                var IsDataChanged = false;
                var DisableWarning = false;

                function PageChanged() {
                    if (IsDataChanged) {
                        var ConfirmResult = confirm("There are unsaved values, cancel changes?");
                        if (ConfirmResult)
                            DisableWarning = true;
                        return ConfirmResult;
                    }
                    else
                        return true;
                }

                function OnRequestStart(sender, args) {
                    if (IsDataChanged && args.get_eventTargetElement().id != "<%= lbSave1.ClientID %>")
                        if (!confirm("There are unsaved values, cancel changes?")) 
                        {
                            args.set_cancel(true);
                            return;
                        }
                        document.body.style.cursor = "wait";                        
                }

                function OnResponseEnd(sender, args) {
                    document.body.style.cursor = "default";
                    IsDataChanged = false;
                }
                
                function openRadWindow(url) {
                    var oWnd = radopen(url, "rwValueEdit");
                    oWnd.Center();
                }

                function valueChanged() {
                    IsDataChanged = true;
                }

                var ListsStringArray = '<%= ListsStringArray %>';
                var activeEditElement = '';
                var activeSelectElement = '';
                var activeEditElementType = 1;

                function clientClose(sender, args) {
                    if (args.get_argument() != null && activeEditElement != '') 
                    {
                        var s = args.get_argument();
                        var ar = s.split('|', 2);
                        if (ar.length != 2) return;
                        var value = ar[0];                        
                        var status = ar[1];                        
                        var element = $find(activeEditElement);                        
                        switch (activeEditElementType) {
                            case 2:
                                element.value = value;
                                break;
                            case 3:
                                element.checked = (value == 'true');
                                break;
                            case 4:
                            case 1:
                            default:
                                element.set_value(value);
                                break;
                        }
                        var select = document.getElementById(activeSelectElement);
                        if (select != null)
                            for (var i = 0; i < select.options.length; i++)
                                if (select.options[i].value == status)
                                {
                                    select.options[i].selected = true;
                                    break;
                                }
                            
                    }
                }

                function SelectAll(sourceStatus, destStatus) {                    
                    var ddlClientIdArray = ListsStringArray.split('|');
                    for (var i = 0; i < ddlClientIdArray.length; i++)
                        if (ddlClientIdArray != '')
                        {
                            var ddlObject = document.getElementById(ddlClientIdArray[i]);
                            if (ddlObject != null)
                            {
                                var IsEditDDL = sourceStatus == null;
                                if (sourceStatus != null)
                                    if (ddlObject.options[ddlObject.selectedIndex].value == sourceStatus)
                                        IsEditDDL = true;
                                if (IsEditDDL)
                                    for (var j = 0; j < ddlObject.options.length; j++)
                                        if (ddlObject.options[j].value == destStatus)
                                        {
                                            ddlObject.options[j].selected = true;
                                            valueChanged();
                                            break;
                                        }
                            }                            
                        }
                }
            </script>            
        </div>               
        <div class="metric">
        <asp:HiddenField ID="hfSwitch" runat="server" Value="0" />
        <table cellspacing="0" id="tFrequency" border="0">                                                    
            <asp:Repeater runat="server" ID="rMetric" OnItemDataBound="rpMetric_ItemDataBound">
                <ItemTemplate>
                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#(LastMetricMetricValue == null)%>'>
                        <tr><th colspan="33" class="empty" style="height:27px"></th></tr>
                        <tr><th colspan="33" class="title" style="border-bottom-style:none;" >
                                <%#GroupByMetric?Eval("Name"):Eval("OrgLocationFullName")%>&nbsp;&nbsp;&nbsp;<asp:LinkButton ID="btnSwitch" runat="server" onclick="btnSwitch_Click" Text='<%#GetSwitchTitle()%>' OnClientClick="return PageChanged();" Font-Size="10pt" />
                                <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#(GroupByMetric && DataMode == Mode.Input && ((int)Eval("MetricTypeID") == 1) && ((bool)Eval("AllowCustomNames")) && !String.IsNullOrEmpty((String)Eval("MetricOrgLocationAlias")) )%>'>&nbsp;&nbsp;<span style="font-size:80%;font-weight:normal;color:#666666;">Alias</span>&nbsp;<%#Eval("MetricOrgLocationAlias")%>&nbsp;</asp:PlaceHolder>
                                <asp:PlaceHolder ID="PlaceHolder3" runat="server" Visible='<%#(GroupByMetric && DataMode == Mode.Input && ((int)Eval("MetricTypeID") == 1) && ((bool)Eval("AllowCustomNames")) && !String.IsNullOrEmpty((String)Eval("MetricOrgLocationCode")) )%>'>&nbsp;&nbsp;<span style="font-size:80%;font-weight:normal;color:#666666;">Code</span>&nbsp;<%#Eval("MetricOrgLocationCode")%>&nbsp;</asp:PlaceHolder>
                            </th></tr>                        
                        <tr>                            
                            <th class="title"></th>                            
                            <th class="arrow">                                
                                <asp:LinkButton runat="server" ID="hlNavL1" OnClientClick="return PageChanged();" CommandName="Navigation" CommandArgument="L1" EnableViewState="false" oncommand="hlNavR1_Command">
                                    <asp:Image runat="server" ID="iNavL1" ImageUrl="~/images/buttons/arrow-left.png" ToolTip='<%#GetTitle("L1")%>' AlternateText='<%#GetTitle("L1")%>' EnableViewState="false" />
                                </asp:LinkButton>
                            </th>
                            <asp:Repeater runat="server" ID="rDate">
                                <ItemTemplate>
                                    <th class="cal-head"><%#Eval("sDate")%></th>
                                </ItemTemplate>
                            </asp:Repeater>
                            <th class="arrow">
                                <asp:LinkButton runat="server" ID="hlNavR1" OnClientClick="return PageChanged();" CommandName="Navigation" CommandArgument="R1" EnableViewState="false" oncommand="hlNavR1_Command">
                                    <asp:Image runat="server" ID="iNavR1" ImageUrl="~/images/buttons/arrow-right.png" ToolTip='<%#GetTitle("R1")%>' AlternateText='<%#GetTitle("R1")%>' EnableViewState="false" />
                                </asp:LinkButton>
                            </th>
                        </tr>
                    </asp:PlaceHolder>
                    <tr>                        
                        <td class="location" align="right">
                            <asp:PlaceHolder ID="tMetricAlias" runat="server" Visible='<%#(!GroupByMetric && DataMode == Mode.Input && ((int)Eval("MetricTypeID") == 1) && ((bool)Eval("AllowCustomNames")) && (!String.IsNullOrEmpty((String)Eval("MetricOrgLocationAlias")) || !String.IsNullOrEmpty((String)Eval("MetricOrgLocationCode"))) )%>'>
                                <div>
                                    <asp:PlaceHolder ID="PlaceHolder4" runat="server" Visible='<%#(!String.IsNullOrEmpty((String)Eval("MetricOrgLocationAlias")) )%>'><%#Eval("MetricOrgLocationAlias")%></asp:PlaceHolder>
                                    <asp:PlaceHolder ID="PlaceHolder6" runat="server" Visible='<%#(!String.IsNullOrEmpty((String)Eval("MetricOrgLocationAlias")) && !String.IsNullOrEmpty((String)Eval("MetricOrgLocationCode")))%>'>&nbsp;-&nbsp;</asp:PlaceHolder>
                                    <asp:PlaceHolder ID="PlaceHolder5" runat="server" Visible='<%#(!String.IsNullOrEmpty((String)Eval("MetricOrgLocationCode")) )%>'><%#Eval("MetricOrgLocationCode")%></asp:PlaceHolder>
                                </div>
                                <div>
                                    <span style="font-size:85%;color:#aaaaaa;">Metric</span>&nbsp;<%#(Eval("Name")) %>
                                </div>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phetricAlias" runat="server" Visible='<%#!(!GroupByMetric && DataMode == Mode.Input && ((int)Eval("MetricTypeID") == 1) && ((bool)Eval("AllowCustomNames")) && (!String.IsNullOrEmpty((String)Eval("MetricOrgLocationAlias")) || !String.IsNullOrEmpty((String)Eval("MetricOrgLocationCode"))) )%>'>                                
                                <%#GroupByMetric ? Eval("OrgLocationFullName") : (((int)Eval("MetricTypeID")==2?"<b>":"")+Eval("Name")) %>                                
                            </asp:PlaceHolder></td>
                        <td runat="server" id="tdUnitLeft" class="unit">
                            <asp:DropDownList ID="ddlInputUnitOfMeasure" runat="server" Width="100px"                                
                                DataTextField="SingularFullName" 
                                DataValueField="MeasureUnitId"
                                AppendDataBoundItems="true" >                                
                            </asp:DropDownList>
                            <asp:Label ID="lblUoM" runat="server" Text='<%#Eval("InputUnitOfMeasureName")%>' />
                            <asp:HiddenField ID="hfUoM" runat="server" Value='<%#Eval("InputUnitOfMeasureID")%>' />
                            <asp:HiddenField ID="hfMetric" runat="server" Value='<%#Eval("MetricID")%>' />
                            <asp:HiddenField ID="hfOrgLocation" runat="server" Value='<%#Eval("OrgLocationID")%>' /></td>                            
                        <asp:Repeater runat="server" ID="rMericValue" OnItemDataBound="rpMetricValue_ItemDataBound">
                            <ItemTemplate>
                                <td class="cal-metric<%#IsEditValue(Container)?"":" empty"%>"">
                                    <asp:PlaceHolder ID="PlaceHolder2" runat="server" Visible='<%#IsEditValue(Container)%>'>
                                    <telerik:RadNumericTextBox ID="rntValue" runat="server" Type="Number" Culture="English (United States)" Width="50px" ClientEvents-OnValueChanged="valueChanged" />
                                    <asp:TextBox ID="tbValue" runat="server" Columns="38" MaxLength="500"  />                    
                                    <asp:CheckBox ID="chbValue" runat="server" />                    
                                    <telerik:RadDatePicker ID="rdpDateValue" runat="server" Culture="English (United States)" AllowEmpty="true" Width="50px" MinDate="02.01.1900 0:00:00" MaxDate="05.06.2079 0:00:00" Calendar-RangeMinDate="02.01.1900 0:00:00" Calendar-RangeMaxDate="05.06.2079 0:00:00" />
                                    <a href='<%#GetEditUrl(Container)%>' <%#GetOnClickHandler(Container)%> class="cursor"><img src="../images/icons/pencil.png" alt="Edit Value" style="border:none 0px;" /></a>                                    
                                    <asp:HiddenField ID="hfDate" runat="server" Value='<%#Eval("Date")%>' /> 
                                    <asp:PlaceHolder ID="PlaceHolder1" runat="server" Visible='<%#DataMode == Mode.Approve%>'>
                                        <div style="margin-top:1px;">
                                            <asp:DropDownList ID="ddlApprovalStatus" runat="server" Width="67px" Font-Size="XX-Small" SelectedValue='<%#Eval("Approved") %>' >
                                                <asp:ListItem Value="False" Text="Pending" />
                                                <asp:ListItem Value="" Text="Under Review" />
                                                <asp:ListItem Value="True" Text="Approved" />
                                            </asp:DropDownList>   
                                        </div>
                                    </asp:PlaceHolder>
                                    </asp:PlaceHolder>
                                </td>
                            </ItemTemplate>
                        </asp:Repeater>
                        <td runat="server" id="tdUnitRight" class="unit">&nbsp;</td>
                    </tr>
                </ItemTemplate>                
            </asp:Repeater>
            <asp:PlaceHolder ID="PlaceHolder3" runat="server">
            <tr>
                <th style="border-bottom-style:none;" align="right"></th>
                <td colspan="33" class="select">
                    Select All:
                        <a href="javascript:void(SelectAll(null, 'False'));">Pending</a>,
                        <a href="javascript:void(SelectAll(null, ''));">Under Review</a>,
                        <a href="javascript:void(SelectAll(null, 'True'));">Approved</a>&nbsp;&nbsp;&nbsp;&nbsp;
                    Change:
                        <a href="javascript:void(SelectAll('False', ''));">Pending -> Under Review</a>,
                        <a href="javascript:void(SelectAll('', 'True'));">Under Review -> Approved</a>                 
                </td>
            </tr>            
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phInputButtons" runat="server">
            <tr>
                <th class="button"><asp:Button ID="lbSave1" runat="server" onclick="lbSave2_Click" style="font-size:larger; font-weight:bolder;" Text="Save Values" OnClientClick="DisableWarning = true;"/></th>
                <th style="border-bottom-style:none;" colspan="33" align="left">&nbsp;&nbsp;or&nbsp;&nbsp;
                    <asp:LinkButton ID="lbSave2" runat="server" onclick="lbSave1_Click" style="font-size:larger; font-weight:bolder;" Text="Save Values & Back" OnClientClick="DisableWarning = true;"/>
                    <asp:Label ID="lbDebug" Visible="false" runat="server" Text="Label"></asp:Label></th>
            </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phApproveButtons" runat="server">
            <tr>
                <th class="button"><asp:Button ID="lbSave3" runat="server" onclick="lbSave1_Click" style="font-size:larger; font-weight:bolder;" Text="Save Values & Back" OnClientClick="DisableWarning = true;"/></th>
                <th style="border-bottom-style:none;" colspan="33" align="left"><asp:Label ID="Label1" Visible="false" runat="server" Text="Label"></asp:Label></th>
            </tr>
            </asp:PlaceHolder>
        </table>        
        </div>        
    <telerik:RadAjaxLoadingPanel runat="server" ID="ralGrid" Wrap="true" Transparency="33" BackColor="LightGray" ><table style="height:100%; width:100%">
            <tr><td valign="top" align="center"><img alt="Loading ..." src="../images/loading6.gif" /></td></tr>
    </table></telerik:RadAjaxLoadingPanel>
</div>
</asp:Panel>