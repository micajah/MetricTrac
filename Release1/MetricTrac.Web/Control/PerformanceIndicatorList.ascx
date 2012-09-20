<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PerformanceIndicatorList.ascx.cs" Inherits="MetricTrac.Control.PerformanceIndicatorList" %>
<div id="divScript" runat="server">
<telerik:RadWindowManager ID="rwmEditValue" runat="server" Modal="true" VisibleStatusbar="false" Height="525px" Width="790px" DestroyOnClose="true"  style="z-index: 90000"/>
<script language="javascript" type="text/javascript">
    function toggleCheckBoxes(chbHeaderID) {
        var chbHeader = document.getElementById(chbHeaderID);
        var checkBoxes = document.forms[0].elements;
        for (i = 0; i < checkBoxes.length; i++)
            if (checkBoxes[i].type == 'checkbox') checkBoxes[i].checked = chbHeader.checked;
    }
    function openRadWindow(url) {
        var oWnd = radopen(url, "rwPerformanceIndicator");
        oWnd.Center();
    }            
</script>
</div>

    <mits:CommonGridView runat="server" ID="cgvPerformanceIndicator" DataKeyNames="PerformanceIndicatorID"
        Width="99%" ColorScheme="Gray" AutoGenerateColumns="False" AllowSorting="true">
        <Columns>            
            <mits:TemplateField ItemStyle-Width="40px">
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
                <HeaderTemplate>
                    <asp:CheckBox ID="chbHeaderCheck" runat="server" Checked="False" EnableViewState="false" />                
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="chbProjectCheck" runat="server" Checked="False"/>
                    <asp:HiddenField id="hfComplexId" runat="server"></asp:HiddenField>
                </ItemTemplate>
            </mits:TemplateField>        
            <mits:HyperLinkField Text="Edit" DataNavigateUrlFormatString="~/home/PerformanceIndicatorEdit.aspx?PerformanceIndicatorID={0}" DataNavigateUrlFields="PerformanceIndicatorID" ItemStyle-CssClass="Cgv_B" />
            <mits:TextField DataField="GCAName" HeaderText="Group&nbsp;&nbsp;>&nbsp;&nbsp;Category&nbsp;&nbsp;>&nbsp;&nbsp;Aspect" SortExpression="GCAName" />
            <mits:TextField DataField="Code" HeaderText="Code" SortExpression="Code"/>
            <mits:TemplateField HeaderText="Performance Indicator" SortExpression="Name">
                <ItemTemplate>
                    <asp:Label ID="lblName" runat="server" Text='<%# Eval("Name")%>'></asp:Label>&nbsp;<a href='<%# "PerformanceIndicatorInfo.aspx?PerformanceIndicatorID=" + Eval("PerformanceIndicatorID")%>' onclick="openRadWindow('<%# "PerformanceIndicatorInfo.aspx?PerformanceIndicatorID=" + Eval("PerformanceIndicatorID")%>');return false;" class="cursor">&gt;&gt;</a>
                </ItemTemplate>
            </mits:TemplateField>        
            <mits:TextField DataField="SectorName" HeaderText="Sector" SortExpression="SectorName" ItemStyle-Width="10%" />
        </Columns>
    </mits:CommonGridView>
    <asp:LinqDataSource runat="server" ID="ldsPerformanceIndicator" 
        ContextTypeName="MetricTrac.Bll.LinqMicajahDataContext" TableName="PerformanceIndicator" 
        EnableDelete="true" OnSelecting="ldsPerformanceIndicator_Selecting" />    
    <asp:LinqDataSource runat="server" ID="ldsPerformanceIndicatorFormPerformanceIndicators" 
        ContextTypeName="MetricTrac.Bll.LinqMicajahDataContext" TableName="PerformanceIndicatorFormPerformanceIndicatorJunc" 
        EnableDelete="true" OnSelecting="ldsPerformanceIndicatorFormPerformanceIndicators_Selecting" 
        ondeleting="ldsPerformanceIndicatorFormPerformanceIndicators_Deleting" />

        <asp:Panel ID="pnlToolTips" runat="server">
        </asp:Panel>