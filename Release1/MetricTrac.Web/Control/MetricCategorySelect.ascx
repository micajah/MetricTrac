<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MetricCategorySelect.ascx.cs" Inherits="MetricTrac.Control.MetricCategorySelect" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<script type="text/javascript">

    var ddlMetricCategoryText=null;
    var ddlMetricCategoryValue = null;
    var startMCValue = null;
    
    function InitStartMCValue(val)
    {
        startMCValue = val;
    }

    function DdlMetricCategoryClientNodeClicked(tvMetricCategorys, eventArgs) {
        var node = eventArgs.get_node();
        var combo = $find("<%= ddlMetricCategory.ClientID %>");
        
        ddlMetricCategoryText = node.get_attributes().getAttribute("FullPath");
        ddlMetricCategoryValue = node.get_value();

        combo.set_text(ddlMetricCategoryText);
        combo.set_value(ddlMetricCategoryValue);
        
        if (combo != null)
        {            
            if (ddlMetricCategoryText != startMCValue)
            {
                <%if(mOnClientTextChange!=null){ %>            
                <%=mOnClientTextChange%>();
                <%} %>
                startMCValue = ddlMetricCategoryText;
            }
        }
    }
    function DllMetricCategoryItemReqesting(ddlMetricCategory, eventArgs) {
        ddlMetricCategoryText = null;
        var tvMetricCategorys = $find('<%= ((Telerik.Web.UI.RadTreeView)ddlMetricCategory.Items[0].FindControl("tvMetricCategorys")).ClientID %>');
        var tvMetricCategorysNodeCollection = tvMetricCategorys.get_nodes();
        var SelectedNode = null;
        var FullSelectedNode = null;
    
    
        eventArgs.set_cancel(true);
        var InputText = eventArgs.get_text();
        var EndPos = -1;
        var BeginPos = 0;
        var MetricCategoryText;
        
        while (true) {
            EndPos = InputText.indexOf(">", BeginPos);
            if (EndPos < 0) {
                MetricCategoryText = InputText.substring(BeginPos);
            }
            else {
                MetricCategoryText = InputText.substring(BeginPos, EndPos);
                BeginPos = EndPos + 1;
            }
            
            while (MetricCategoryText.length > 0 && MetricCategoryText.charAt(0) == " ") MetricCategoryText = MetricCategoryText.substring(1);
            while (MetricCategoryText.length > 0 && MetricCategoryText.charAt(MetricCategoryText.length - 1) == " ") MetricCategoryText = MetricCategoryText.substring(0, MetricCategoryText.length - 1);
            if (EndPos < 0) break;
            
            while (MetricCategoryText.length > 0 && MetricCategoryText.charAt(0) == " ") MetricCategoryText = MetricCategoryText.substring(1);
            while (MetricCategoryText.length > 0 && MetricCategoryText.charAt(MetricCategoryText.length - 1) == " ") MetricCategoryText = MetricCategoryText.substring(0, MetricCategoryText.length - 1);
            for (var i = 0; i < tvMetricCategorysNodeCollection.get_count(); i++) {
                if (tvMetricCategorysNodeCollection.getNode(i).get_text().toLowerCase() == MetricCategoryText.toLowerCase()) {
                    SelectedNode = tvMetricCategorysNodeCollection.getNode(i);
                    FullSelectedNode = SelectedNode;
                    tvMetricCategorysNodeCollection = SelectedNode.get_nodes();
                    break;
                }
            }
        }

        if (MetricCategoryText.length > 0) {
            for (var i = 0; i < tvMetricCategorysNodeCollection.get_count(); i++) {
                if (tvMetricCategorysNodeCollection.getNode(i).get_text().length >= MetricCategoryText.length) {
                    if(tvMetricCategorysNodeCollection.getNode(i).get_text().substring(0,MetricCategoryText.length).toLowerCase() == MetricCategoryText.toLowerCase()){
                        SelectedNode = tvMetricCategorysNodeCollection.getNode(i);
                        if(tvMetricCategorysNodeCollection.getNode(i).get_text().toLowerCase() == MetricCategoryText.toLowerCase()) FullSelectedNode = SelectedNode;
                        else FullSelectedNode = null;
                        break;
                    }
                }
            }
        }

        tvMetricCategorysNodeCollection = tvMetricCategorys.get_allNodes();
        for (var i = 0; i < tvMetricCategorysNodeCollection.length; i++) {
            tvMetricCategorysNodeCollection[i].collapse();
            tvMetricCategorysNodeCollection[i].unselect();
        }

        if (SelectedNode != null) {
            SelectedNode.select();
            var Parent = SelectedNode;
            do{
                Parent.expand();
                Parent = Parent.get_parent();
            } while (Parent != null && Parent.get_text)
            SelectedNode.scrollIntoView();
        }

        if (FullSelectedNode == null) {
            ddlMetricCategory.set_value(0);
        } else {
            ddlMetricCategory.set_value(FullSelectedNode.get_value());
        }
    }
    
    var ddlMetricCategoryAlert;
    function MetricCategoryAlert()
    {
        alert('Can not find "' + ddlMetricCategoryAlert.get_text() + '" MetricCategory. Selection is ignored. Correct MetricCategory name and try again.');
    }
    
    function ddlMetricCategorySubmitChangeValue(ddlMetricCategory)
    {
        var val = ddlMetricCategory.get_value();
        var txt = ddlMetricCategory.get_text();
        
        if ((val == null || val==0) && txt!=null && txt!="") {
            <%if(AutoPostBack){ %>
                ddlMetricCategoryAlert = ddlMetricCategory;
                window.setTimeout("MetricCategoryAlert()",1);
            <%} %>
            return;
        }            
        
        <%if(mOnClientMetricCategoryChange!=null){ %>
            <%=mOnClientMetricCategoryChange%>(val);
        <%} %>
        
        <%if(AutoPostBack){ %>
            __doPostBack('<%=ddlMetricCategory.ClientID%>','');
        <%} %>
    }

    function DdlMetricCategoryDropDownClosed(ddlMetricCategory, eventArgs) {
        if (ddlMetricCategoryText != null) {
            ddlMetricCategory.set_text(ddlMetricCategoryText);
            ddlMetricCategory.set_value(ddlMetricCategoryValue);
            ddlMetricCategoryText = null;
        }
        ddlMetricCategorySubmitChangeValue(ddlMetricCategory);
    }
    
    function DdlMetricCategoryKeyPressing(ddlMetricCategory, eventArgs)
    {
        if(eventArgs.get_domEvent().keyCode == 13)
        {
            if (ddlMetricCategory != null)
            {
                var curText = ddlMetricCategory.get_text();
                if (curText != startMCValue)
                {
                    <%if(mOnClientTextChange!=null){ %>            
                    <%=mOnClientTextChange%>();
                    <%} %>
                    startMCValue = curText;
                }
            }
            var dom = eventArgs.get_domEvent();
            dom.rawEvent.returnValue = false;
        }
    }
        
</script>

<telerik:RadComboBox ID="ddlMetricCategory" EnableLoadOnDemand="true" Height="300" Width="505" runat="server" 
    LoadingMessage="Loading Metric Category List..." ItemRequestTimeout="333" OnSelectedIndexChanged="ddlMetricCategory_SelectedIndexChanged"
    OnClientItemsRequesting="DllMetricCategoryItemReqesting" OnClientDropDownClosed="DdlMetricCategoryDropDownClosed" OnClientKeyPressing="DdlMetricCategoryKeyPressing" >
<Items>
    <telerik:RadComboBoxItem runat="server" Text="" />
</Items>
<ItemTemplate>
    <telerik:RadTreeView runat="server" ID="tvMetricCategorys" OnClientNodeClicked="DdlMetricCategoryClientNodeClicked" CollapseAnimation-Type="None" ExpandAnimation-Type="None" Height="297">
    </telerik:RadTreeView>
</ItemTemplate>
</telerik:RadComboBox>