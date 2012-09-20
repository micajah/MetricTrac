<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FormulaBuilder.ascx.cs" Inherits="MetricTrac.Control.FormulaBuilder" %>
<script type="text/javascript">
    function OnClientCommandExecuting(editor, args) {
        var name = args.get_name();
        var val = args.get_value();        
        if (name == "MetricDropdown") {
            editor.pasteHtml(val);            
            args.set_cancel(true);     
        }
    }

    Telerik.Web.UI.Editor.CommandList["Add"] = function(commandName, editor, args) {
        editor.pasteHtml(' + ');
    };

    Telerik.Web.UI.Editor.CommandList["Sub"] = function(commandName, editor, args) {
        editor.pasteHtml(' - ');
    };

    Telerik.Web.UI.Editor.CommandList["Multiple"] = function(commandName, editor, args) {
        editor.pasteHtml(' * ');
    };

    Telerik.Web.UI.Editor.CommandList["Divide"] = function(commandName, editor, args) {
        editor.pasteHtml(' / ');
    };

    Telerik.Web.UI.Editor.CommandList["scOpen"] = function(commandName, editor, args) {
        editor.pasteHtml(' (');
    };

    Telerik.Web.UI.Editor.CommandList["scClose"] = function(commandName, editor, args) {
        editor.pasteHtml(') ');
    };

    Telerik.Web.UI.Editor.CommandList["scDot"] = function(commandName, editor, args) {
        editor.pasteHtml('.');
    };    
    

</script>
<style type="text/css">
.reTool .MetricDropdown
{
  display:block;
}
</style>   
<telerik:radeditor ID="reExpression" runat="server" 
    EditModes="Design"
    OnClientLoad="receiveArg" OnClientCommandExecuting="OnClientCommandExecuting"
     Height="240px" Width="500px"> 
   <Tools></Tools>
</telerik:radeditor>
