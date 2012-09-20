<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MultiSelectList.ascx.cs" Inherits="MetricTrac.Control.MultiSelectList" %>
<style type="text/css">
.WSAClass
{
    background-color:#f3f3f3; 
}
.RadComboBoxDropDown .rcbItem, .RadComboBoxDropDown .rcbHovered, .RadComboBoxDropDown .rcbDisabled, .RadComboBoxDropDown .rcbLoading 
{
    padding-left:0px !important;
    padding-right:0px !important;
}
</style>
<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
<script language="javascript">
    function comboClientDropDownClosed_<%=this.ClientID %>(ddlEntities, eventArgs) {                
        <%if(!String.IsNullOrEmpty(OnClientSelectedIndexChanged)){ %>
            <%=OnClientSelectedIndexChanged%>();
        <%} %>
        <%if(AutoPostBack){ %>
            __doPostBack('<%=ddlEntities.ClientID%>','');
        <%} %>
    } 
    
    function FinishSelect_<%=this.ClientID %>()
    {
        var combo = $find("<%= ddlEntities.ClientID %>");        
        
        <%if(AutoPostBack){ %>
            __doPostBack('<%=ddlEntities.ClientID%>','');        
        <%} else { %>        
            combo.hideDropDown();
        <%} %>
        return false;
    }
    
    function ClearSelect_<%=this.ClientID %>()
    {
        var combo = $find("<%= ddlEntities.ClientID %>");        
        var listbox = document.getElementById("<%= lbEntities.ClientID %>");        
        for (var i = 0; i < listbox.options.length; i++)
            if (listbox.options[i].selected)
                listbox.options[i].selected = false;
        //combo.set_text('');
        var empty_text = combo.get_emptyMessage() != null ? combo.get_emptyMessage() : '';
        combo.set_text(empty_text);
        <%if(AutoPostBack){ %>
            __doPostBack('<%=ddlEntities.ClientID%>','');
        <%} else { %>        
            combo.hideDropDown();
        <%} %>
        return false;
    }
    
    function ListBoxSelect_<%=this.ClientID %>(sender)
    {
        var combo = $find("<%= ddlEntities.ClientID %>");
        var listbox = document.getElementById("<%= lbEntities.ClientID %>");
        var j = 0;
        for (var i = 0; i < listbox.options.length; i++)
            if (listbox.options[i].selected)
                j++;
        switch (j) 
        {
            case 0:
                var empty_text = combo.get_emptyMessage() != null ? combo.get_emptyMessage() : '';
                combo.set_text(empty_text);
                //combo.set_text('');
            break;
            case 1:
                combo.set_text(listbox.options[listbox.selectedIndex].text);
            break;
            default:
                combo.set_text(j + ' ' + "<%=EntitiesName %>");
            break;
        }          
    }
</script>
</telerik:RadCodeBlock>
<telerik:RadComboBox ID="ddlEntities" runat="server" Height="310" Width="243" AutoPostBack="false" EnableItemBindingExpressions="false" DropDownCssClass="WSAClass" DropDownWidth="406px">
    <Items>
        <telerik:RadComboBoxItem runat="server" Text="" />
    </Items>    
    <ItemTemplate>    
        <table cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td colspan="4">                
                    <asp:listbox id="lbEntities" runat="server" Height="218px" Width="400px" SelectionMode="Multiple" ></asp:listbox></td></tr>
            <tr>
                <td colspan="4" style="padding:2px 5px 15px 5px;">Select <%= EntitiesName%>. <br />
                    Hold down the -ctrl- or -command- key to select multiple ones.</td></tr>
            <tr>
                <td align="right"><asp:Button ID="btnFinish" runat="server" Text="Finish" Width="70px"/></td><td>&nbsp;</td>
                <td align="left"><asp:Button ID="btnClear" runat="server" Text="Clear" Width="70px"/></td><td>&nbsp;&nbsp;<br /><br /></td></tr>
        </table>
    </ItemTemplate>
</telerik:RadComboBox>