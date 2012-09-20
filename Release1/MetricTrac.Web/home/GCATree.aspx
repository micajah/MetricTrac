<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.master" CodeBehind="GCATree.aspx.cs" Inherits="MetricTrac.GCATree" %>
<asp:Content ID="cntGCA" ContentPlaceHolderID="PageBody" runat="server">
<script language="JavaScript" type="text/javascript" src="../includes/script/CustomTreeview.js"></script>
<telerik:RadCodeBlock id="rcb" runat="server">
<script type="text/javascript">
    function ClientNodeDragStart(sender, eventArgs) {

        var node = eventArgs.get_node();
        var Level = node.get_level();
        var NodeValue = node.get_value();

        var nodes = sender.get_allNodes();
        for (var i = 0; i < nodes.length; i++)
            if (nodes[i].get_level() > 0 &&
            nodes[i].get_level() <= 3 && Level <= 3 && NodeValue != 'PIM' && nodes[i].get_value() != 'PIM' && // additional criterias
            (nodes[i].get_level() == Level - 1 || nodes[i].get_level() == Level) && nodes[i].get_value() != NodeValue)
            nodes[i].set_allowDrop(true);
        else
            nodes[i].set_allowDrop(false);
    }
   
    function GetNodeType(node) {
        var NodeLevel = node.get_level();
        if (NodeLevel == 1) return "Group";
        else if (NodeLevel == 2) return "Category";
        else if (NodeLevel == 3) return "Aspect";
        return "";
    }

    function ClientNodeDropping(sender, eventArgs) {
        var DestNode = eventArgs.get_destNode();
        var SourceNode = eventArgs.get_sourceNode();
        var DestLevel = DestNode.get_level();
        var SourceLevel = SourceNode.get_level();

        if (DestLevel < 1 || DestLevel > 3 || SourceLevel < 1 || SourceLevel > 3 || DestNode.get_value() == 'PIM' || SourceNode.get_value() == 'PIM') {
            eventArgs.set_cancel(true);
            return;
        }

        if (DestLevel == SourceLevel) {
            if (!confirm("Merge '" + SourceNode.get_text() + "' to '" + DestNode.get_text() + "' " + GetNodeType(DestNode) + "?")) {
                eventArgs.set_cancel(true);
                return;
            }
        }
        else {
            if (DestLevel + 1 == SourceLevel) {
                if (ChildNodeExist(DestNode, SourceNode.get_text())) {
                    if (eventArgs.get_domEvent().ctrlKey) {
                        alert("Can not copy '" + SourceNode.get_text() + "' " + GetNodeType(SourceNode) + " ! This " + GetNodeType(SourceNode) + " exist in destination " + GetNodeType(DestNode) + ".");
                        eventArgs.set_cancel(true);
                        return;
                    }
                    if (!confirm("'" + SourceNode.get_text() + "' " + GetNodeType(SourceNode) + " exist in '" + DestNode.get_text() + "' " + GetNodeType(DestNode) + "! Merge it?")) {
                        eventArgs.set_cancel(true);
                        return;
                    }
                }
                var hidden = document.getElementById("CtrlKeyField");
                if (eventArgs.get_domEvent().ctrlKey) {

                    hidden.value = "True";
                    if (!confirm("Copy '" + SourceNode.get_text() + "' " + GetNodeType(SourceNode) + " to '" + DestNode.get_text() + "' " + GetNodeType(DestNode) + "?")) {
                        eventArgs.set_cancel(true);
                        return;
                    }
                }
                else {
                    hidden.value = "False";
                }
            }
        }
    }
</script>
<table cellpadding="0" cellspacing="0" border="0" width="100%">
    <tbody>
    <tr>
        <td valign="top">
            <telerik:RadAjaxPanel runat="server" ID="rapTreeView" LoadingPanelID="ralpTreeView" EnableEmbeddedScripts="true" EnableAJAX="true">    
            <telerik:RadTreeView runat="server" ID="rtvGCA" 
                AllowNodeEditing="false" EnableDragAndDrop="true" EnableDragAndDropBetweenNodes="true"
                OnClientContextMenuItemClicking="ClientContextMenuItemClicking"
                OnClientNodeEdited="ClientNodeEdited" OnClientNodeClicked="ClientNodeClicked" 
                OnClientDoubleClick="DoubleClick"
                oncontextmenuitemclick="rtvGCA_ContextMenuItemClick"        
                onnodeedit="rtvGCA_NodeEdit"                
                OnClientNodeDragStart ="ClientNodeDragStart" OnClientNodeDragging="ClientNodeDragging" 
                OnClientNodeDropping="ClientNodeDropping"
                onnodedrop="rtvGCA_NodeDrop"
                >
                <ContextMenus>
                    <telerik:RadTreeViewContextMenu runat="server" ID="RootContextMenu" ClickToOpen="true" >
                        <Items>
                            <telerik:RadMenuItem Value="Add Group" Text="Add Group" ImageUrl="~/images/AddRecord.gif" />
                        </Items>
                    </telerik:RadTreeViewContextMenu>
                    <telerik:RadTreeViewContextMenu runat="server" ID="GroupContextMenu">
                        <Items>
                            <telerik:RadMenuItem Value="Add Category" Text="Add Category" ImageUrl="~/images/AddRecord.gif" />                    
                            <telerik:RadMenuItem Value="Rename Group" Text="Rename Group" ImageUrl="~/images/Update.gif" />
                            <telerik:RadMenuItem Value="Delete Group" Text="Delete Group" ImageUrl="~/images/Delete.gif" />                    
                        </Items>
                    </telerik:RadTreeViewContextMenu>
                     <telerik:RadTreeViewContextMenu runat="server" ID="CategoryContextMenu">
                        <Items>
                            <telerik:RadMenuItem Value="Add Aspect" Text="Add Aspect" ImageUrl="~/images/AddRecord.gif" />                    
                            <telerik:RadMenuItem Value="Rename Category" Text="Rename Category" ImageUrl="~/images/Update.gif" />
                            <telerik:RadMenuItem Value="Delete Category" Text="Delete Category" ImageUrl="~/images/Delete.gif" />                    
                        </Items>
                    </telerik:RadTreeViewContextMenu>
                    <telerik:RadTreeViewContextMenu runat="server" ID="AspectContextMenu">
                        <Items>                    
                            <telerik:RadMenuItem Value="Rename Aspect" Text="Rename Aspect" ImageUrl="~/images/Update.gif" />
                            <telerik:RadMenuItem Value="Delete Aspect" Text="Delete Aspect" ImageUrl="~/images/Delete.gif" />                    
                        </Items>
                    </telerik:RadTreeViewContextMenu>            
                </ContextMenus>
            </telerik:RadTreeView> 
            <input type="hidden" id="CtrlKeyField" name="CtrlKeyField" value="False" />            
            <telerik:RadAjaxLoadingPanel runat="server" ID="ralpTreeView" Wrap="true" Transparency="33" BackColor="LightGray" ><table style="height:100%; width:100%">
                <tr><td valign="middle" align="center"><img alt="Loading ..." src="../images/loading6.gif" /></td></tr>
            </table></telerik:RadAjaxLoadingPanel>
        </telerik:RadAjaxPanel></td>
    <td valign="top" width="150">
        <ul style="color:Gray">                                
            <li>Move Item:<br />Drag and drop item into its new parent item.</li><li>Merge Item:<br />Drag and drop source item into some level destination item.</li><li><nobr>Copy Item without subitem:</nobr><br />&lt;Ctrl&gt; key + drag and drop item into its new parent item.</li><li>Context menu:<br />right or left mouse click.</li></span>
        </ul></td></tr></tbody>
</table>
</telerik:RadCodeBlock>
</asp:Content>
