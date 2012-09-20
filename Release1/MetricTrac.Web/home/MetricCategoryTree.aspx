<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MetricCategoryTree.aspx.cs" MasterPageFile="~/MasterPage.master" Inherits="MetricTrac.MetricCategoryTree" %>
<asp:Content ID="cntMetricCategory" ContentPlaceHolderID="PageBody" runat="server">
<script language="JavaScript" type="text/javascript" src="../includes/script/CustomTreeview.js"></script>
<telerik:RadCodeBlock id="rcb" runat="server">
<script type="text/javascript">
    var MetricSuffix = 'PIM';
    var EmptyNodeID = 'ENS';
    function ClientNodeDragStart(sender, eventArgs) {
        var node = eventArgs.get_node();
        var sourceLevel = node.get_level();
        var sourceValue = node.get_value();

        var nodes = sender.get_allNodes();
        for (var i = 0; i < nodes.length; i++)
        {
            var destLevel = nodes[i].get_level();
            var destValue = nodes[i].get_value();
            if (sourceLevel > 0 &&
                sourceValue != EmptyNodeID &&
                destLevel > 0 &&
                destLevel <= 3 &&
                destValue != sourceValue)
            {
                if (sourceValue.indexOf(MetricSuffix) > -1) // start drag metric
                {
                    if (destValue.indexOf(MetricSuffix) == -1 || destValue == EmptyNodeID)
                        nodes[i].set_allowDrop(true);
                    else
                        nodes[i].set_allowDrop(false);
                }
                else // start drag metric category
                {
                    if (sourceLevel <= 3 && destValue.indexOf(MetricSuffix) == -1 && destValue != EmptyNodeID && (destLevel == sourceLevel - 1 || destLevel == sourceLevel))
                        nodes[i].set_allowDrop(true);
                    else
                        nodes[i].set_allowDrop(false);
                }
            }
            else
                nodes[i].set_allowDrop(false);                
        }
    }
    
    function ClientNodeDropping(sender, eventArgs) {
        var DestNode = eventArgs.get_destNode();
        var SourceNode = eventArgs.get_sourceNode();
        var SourceLevel = SourceNode.get_level();
        var DestLevel = DestNode.get_level();
        var SourceValue = SourceNode.get_value();
        var DestValue = DestNode.get_value();

        var dropResult = false;
        if (SourceLevel > 0 &&
            SourceValue != EmptyNodeID &&
            DestLevel > 0 &&
            DestLevel <= 3 &&
            DestValue != SourceValue)
        {
            if (SourceValue.indexOf(MetricSuffix) > -1)
            {
                if (DestValue.indexOf(MetricSuffix) == -1 || DestValue == EmptyNodeID)
                    dropResult = confirm("Change category of metric '" + SourceNode.get_text() + "' to '" + DestNode.get_text() + "'?");
            }
            else
            {
                if (SourceLevel <= 3 && DestValue.indexOf(MetricSuffix) == -1 && DestValue != EmptyNodeID && (DestLevel == SourceLevel - 1 || DestLevel == SourceLevel))
                {
                    if (DestLevel == SourceLevel)
                    {
                        dropResult = confirm("Merge '" + SourceNode.get_text() + "' to '" + DestNode.get_text() + "'?");
                    }
                    else
                    {
                        var hidden = document.getElementById("CtrlKeyField");
                        var IsSameExistedChild = ChildNodeExist(DestNode, SourceNode.get_text());
                        if (eventArgs.get_domEvent().ctrlKey)
                        {
                            if (IsSameExistedChild)
                                alert("Can not copy '" + SourceNode.get_text() + "'! It already exists in destination Category.");
                            else
                            {
                                hidden.value = "True";
                                dropResult = confirm("Copy '" + SourceNode.get_text() + "' to '" + DestNode.get_text() + "'?");
                            }
                        }
                        else
                        {
                            hidden.value = "False";
                            if (IsSameExistedChild)
                                dropResult = confirm("'" + SourceNode.get_text() + "' exist in '" + DestNode.get_text() + "'! Merge it ?");
                            else
                                dropResult = true;
                        }                      
                    }
                }
            }
        }
        eventArgs.set_cancel(!dropResult);
        return;
    }
</script>
<table cellpadding="0" cellspacing="0" border="0" width="100%">
    <tbody>
    <tr>
        <td valign="top">
            <telerik:RadAjaxPanel runat="server" ID="rapTreeView" LoadingPanelID="ralpTreeView" EnableEmbeddedScripts="true" EnableAJAX="true">    
            <telerik:RadTreeView runat="server" ID="rtvMetricCategory"
                AllowNodeEditing="false" EnableDragAndDrop="true" EnableDragAndDropBetweenNodes="true"
                OnClientNodeClicked="ClientNodeClicked" 
                OnClientDoubleClick="DoubleClick"
                OnClientContextMenuItemClicking="ClientContextMenuItemClicking"
                OnClientNodeEdited="ClientNodeEdited"
                OnClientNodeDragStart ="ClientNodeDragStart" OnClientNodeDragging="ClientNodeDragging" 
                OnClientNodeDropping ="ClientNodeDropping"
                oncontextmenuitemclick="rtvMetricCategory_ContextMenuItemClick"        
                onnodeedit="rtvMetricCategory_NodeEdit"
                OnNodeDrop="rtvMetricCategory_NodeDrop">
                <ContextMenus>
                    <telerik:RadTreeViewContextMenu runat="server" ID="RootContextMenu" ClickToOpen="true" >
                        <Items>
                            <telerik:RadMenuItem Value="Add Sub Metric Category" Text="Add Sub Metric Category" ImageUrl="~/images/AddRecord.gif" />
                        </Items>
                    </telerik:RadTreeViewContextMenu>
                    <telerik:RadTreeViewContextMenu runat="server" ID="MiddleContextMenu">
                        <Items>
                            <telerik:RadMenuItem Value="Add Sub Metric Category" Text="Add Sub Metric Category" ImageUrl="~/images/AddRecord.gif" />                    
                            <telerik:RadMenuItem Value="Rename Metric Category" Text="Rename Metric Category" ImageUrl="~/images/Update.gif" />
                            <telerik:RadMenuItem Value="Delete Metric Category" Text="Delete Metric Category" ImageUrl="~/images/Delete.gif" />                    
                        </Items>
                    </telerik:RadTreeViewContextMenu>            
                    <telerik:RadTreeViewContextMenu runat="server" ID="LeafContextMenu">
                        <Items>                    
                            <telerik:RadMenuItem Value="Rename Metric Category" Text="Rename Metric Category" ImageUrl="~/images/Update.gif" />
                            <telerik:RadMenuItem Value="Delete Metric Category" Text="Delete Metric Category" ImageUrl="~/images/Delete.gif" />                    
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
