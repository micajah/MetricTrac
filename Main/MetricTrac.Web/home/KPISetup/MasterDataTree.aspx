<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MasterDataTree.aspx.cs" Inherits="MetricTrac.MasterDataTree" MasterPageFile="~/MasterPage.master"%>
<asp:Content ID="cntGCA" ContentPlaceHolderID="PageBody" runat="server">
<script language="text/javascript" type="text/javascript">
    function ClientNodeClicked(sender, eventArgs)
    {
        var node = eventArgs.get_node();
        node.toggle();
    }

    function ClientNodeDoubleClick(sender, eventArgs) {
        var node = eventArgs.get_node();
        var url = node.get_value()
        if (url != null && url != '')
            location.href = url;
    }
</script>
<telerik:RadTreeView runat="server" ID="rtvMaster"
    OnClientNodeClicked="ClientNodeClicked" OnClientDoubleClick="ClientNodeDoubleClick" OnNodeExpand="rtvMaster_NodeExpand"
    LoadingStatusPosition="BeforeNodeText" LoadingMessage="Loading... ">
</telerik:RadTreeView>
</asp:Content>

