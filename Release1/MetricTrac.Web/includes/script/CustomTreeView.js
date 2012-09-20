function ClientNodeClicked(sender, eventArgs) {
    var node = eventArgs.get_node();
    var NodeLevel = node.get_level();
    if ((NodeLevel > 3) || (node.get_value().indexOf('PIM') > -1)) return;
    var menu = node.get_contextMenu();
    var domEvent = eventArgs.get_domEvent();
    sender._contextMenuNode = node;
    menu.show(domEvent);
}

function DoubleClick(sender, eventArgs) {
    eventArgs.set_cancel(true);
}

function ClientNodeEdited(sender, eventArgs) {
    var node = eventArgs.get_node();
    node.set_allowEdit(false);
}

function ClientContextMenuItemClicking(sender, eventArgs) {
    var item = eventArgs.get_menuItem();
    var val = item.get_value();
    var node = eventArgs.get_node();
    var menu = node.get_contextMenu();

    if (val == null || val == "") return;

    if (val.indexOf("Delete ") == 0) {
        if (confirm("Delete '" + node.get_text() + "'?")) return;
        menu.hide();
        eventArgs.set_cancel(true);
        return;
    }
    if (val.indexOf("Rename ") == 0) {
        eventArgs.set_cancel(true);
        node.set_allowEdit(true);
        node.startEdit();
        menu.hide();
        return;
    }

    if (val.indexOf("Add ") == 0) {
        eventArgs.set_cancel(true);
        menu.hide();
        var tree = node.get_treeView();
        tree.trackChanges();
        var NewNode = new Telerik.Web.UI.RadTreeNode();
        NewNode.set_text("");
        //NewNode.set_imageUrl(node.get_imageUrl());
        //NewNode.set_expandedImageUrl(node.get_expandedImageUrl());
        node.get_nodes().add(NewNode);
        node.expand();
        tree.commitChanges();
        NewNode.set_allowEdit(true);
        NewNode.startEdit();
        NewNode.IsNewCreatedNode = true;
        window.setTimeout(function() {
            NewNode.set_allowEdit(true);
            NewNode.startEdit();
        }, 300);

        tree.commitChanges();
    }
}

function ClientNodeDragging(sender, eventArgs) {
    eventArgs.set_cancel(true);
}

function ChildNodeExist(ParentNode, ChildText) {
    var ChildNodes = ParentNode.get_nodes();
    var ChildCount = ChildNodes.get_count();
    for (var i = 0; i < ChildCount; i++) {
        var node = ChildNodes.getNode(i);
        if (node.get_text() == ChildText) return true;
    }
    return false;
}