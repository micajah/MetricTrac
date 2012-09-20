<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PIFOrgLocationJunc.aspx.cs" Inherits="MetricTrac.PIFOrgLocationJunc" MasterPageFile="~/MasterPage.master"%>
<asp:Content ID="cntFormOrgTreeJunc" ContentPlaceHolderID="PageBody" runat="server">    
    <mits:EntityTreeView runat="server" ID="OrgTree" CheckBoxes="True" EntityId="4cda22f3-4f01-4768-8608-938dc6a06825" AllowRootNodeSelection="false"  /> 
    <asp:Button ID="btnAdd" runat="server" Text="Save Assigned Org Locations" class="SaveButtonCaption" Width="240px" onclick="btnAdd_Click" /><span style="width:10px;"> </span>or<span style="width:10px;"></span>
    <asp:LinkButton ID="lnkCancel" runat="server" onclick="lnkCancel_Click" CssClass="Mf_Cb">Cancel</asp:LinkButton><br /><br />
</asp:Content>
