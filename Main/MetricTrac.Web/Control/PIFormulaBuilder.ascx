<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PIFormulaBuilder.ascx.cs" Inherits="MetricTrac.Control.PIFormulaBuilder" %>
<%@ Register src="~/Control/OrgLocationSelect.ascx" tagname="OrgLocationSelect" tagprefix="uc" %>
<%@ Register src="~/Control/MetricCategorySelect.ascx" tagname="MetricCategorySelect" tagprefix="uc" %>
<script type="text/javascript">

	var OrgLocationMode;
	//CbMode functions
	function GetCbMode() {
		return $find('<%=cbMode.ClientID%>');
	}
	function ModeChanged(cbMode, eventArgs) {
		var item = eventArgs.get_item();
		var val = item.get_value();
		OrgLocationMode = val;
		SelectedOrgLocation = null;
		cbMode.hideDropDown();
		if (val == "" || val==null || val==undefined) {
			OrgLocationSelectEnable(false);
			cbFunctionSelectEnable(false);
			MetricEnable(false);
		} else if (val.substr(0,6) == "Report") {
			OrgLocationSelectEnable(false);
			cbFunctionSelectEnable(true);
			MetricEnable(false);
		} else {
			OrgLocationSelectEnable(true);
			cbFunctionSelectEnable(false);
			MetricEnable(false);
		}
	}
	
	
	// Function select function
	var SelectedFunction;
	function GetCbFunction() {
		return $find('<%=cbFunction.ClientID%>');
	}
	function cbFunctionChanged(cbFunction, eventArgs) {
		alert("cbFunctionChanged");
		var item = eventArgs.get_item();
		var val = item.get_value();
		SelectedFunction = val;
		if (val == "" || val == null || val == undefined) {
			MetricEnable(false);
		} else {
			MetricEnable(true);
		}
	}

	var cbFunctionEnabled = true;
	function cbFunctionSelectEnable(en) {
		var cbFunction = GetCbFunction();
		if (en) {
			cbFunctionEnabled = true;
			cbFunction.enable();
			cbFunction.toggleDropDown();
			cbFunction.set_text("Sum");
			cbFunction.set_value("Sum(");
		}
		else {
			cbFunctionEnabled = false;
			cbFunction.set_text("Function");
			cbFunction.disable();
		}
	}

	function SetOrgLocationSelectText() {
		if (OrgLocationSelectText != null && OrgLocationSelectText != undefined && OrgLocationSelectText != "") {
			var OrgLocationSelect = GetOrgLocationSelect();
			OrgLocationSelect.set_text(OrgLocationSelectText);
		}
	}
	
	function cbFunctionDropDownOpened() {
		if (OrgLocationSelectText != null && OrgLocationSelectText != undefined && OrgLocationSelectText != "") {
			window.setTimeout("SetOrgLocationSelectText()", 500);
		}
	}

	function cbFunctionDropDownClosed() {
		var cbFunction = GetCbFunction();
		if (cbFunctionEnabled) MetricEnable(true);
	}
	
	
	//OrgLocationSelect functions
	function GetOrgLocationSelect() {
		return $find('<%=cOrgLocationSelect.DdlClientID%>');
	}
	var NeedOpenFunctionSelect = false;
	var OrgLocationSelectText;
	function OrgLocationDdlClosed(OrgLocationSelect) {
		if (NeedOpenFunctionSelect) {
			MetricEnable(false);
			cbFunctionSelectEnable(true);			
		}
		NeedOpenFunctionSelect = false;
	}
	function OrgLocationSelectEnable(en) {
		var OrgLocationSelect = GetOrgLocationSelect();		
		OrgLocationSelectText = null;
		if (en) {
			OrgLocationSelect.enable();
			OrgLocationSelect.toggleDropDown();
			OrgLocationSelect.set_text("");
			NeedOpenFunctionSelect = false;
		}
		else {
			OrgLocationSelect.set_text("Org Location Select");
			OrgLocationSelect.disable();
		}
	}
	var SelectedOrgLocation;
	function OrgLocationSelectChanged(val, txt) {
		OrgLocationSelectText = txt;
		GetOrgLocationSelect().hideDropDown();
		if (val == null || val == "" || val == "0" | val == 0) {
			MetricEnable(false);			
		}
		else {
			NeedOpenFunctionSelect = true;
			var s = new Array('->', '+', '-', '*', '/', '(', ')', '.', ',',' ');
			var newTxt = txt;
			for (i = 0; i < s.length; i++) {
				newTxt = newTxt.replace(s[i],"_");
			}
			SelectedOrgLocation = newTxt;
		}
	}
	
	
	//MetrciSelect
	function GetCbMetric() {
		return $find('<%=cbMetric.ClientID%>');
	}
	function MetricEnable(en) {
		var cbMetric = GetCbMetric();
		var apply = $("a[title=Apply]");
		if (en) {
			cbMetric.enable();
			cbMetric.toggleDropDown();

			var items = cbMetric.get_items();
			if (items.get_count() > 0) {
				var item = items.getItem(0);
				cbMetric.set_text(item.get_text());
				item.select();
			} else {
				cbMetric.set_text("");
			}
			apply.show(555);
		} else {
			cbMetric.set_text("Metric Select");
			cbMetric.disable();
			apply.hide(555);
		}
	}

	function MetricChanged(cbMetric, eventArgs) {
		/*var item = eventArgs.get_item();
		var val = item.get_text();
		if (val == null || val == "" || val == undefined) return;
		var reExpression = $find("<%=reExpression.ClientID%>");
		reExpression.pasteHtml(OrgLocationMode + val + ")");*/
	}
	Telerik.Web.UI.Editor.CommandList["Apply"] = function(commandName, editor, args) {
		var cbMetric = GetCbMetric();
		var item = cbMetric.get_selectedItem();
		if (item == null || item == undefined) {
			return;
		}
		var MetricVal = item.get_value();
		if (MetricVal == "" || MetricVal == null || MetricVal == undefined) {
			return;
		}
		var txt = OrgLocationMode;
		var cbFunction = GetCbFunction();
		txt = txt + cbFunction.get_value();
		if (OrgLocationMode != null && OrgLocationMode != undefined && OrgLocationMode.length > 7 && OrgLocationMode.substr(0, 8) == "Selected") {
			txt = txt + SelectedOrgLocation + ", ";
		}
		txt = txt + MetricVal + ")";
		editor.pasteHtml(txt);
	};

	

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

	function ReplaceTool(OldToolTitle, NewToolID) {
		var OldTool = $("a[title=" + OldToolTitle + "]");
		var NewTool = $("#" + NewToolID);
		NewTool.insertBefore(OldTool);
		OldTool.remove();
	}

	function IntiFormulaBuilder() {
		receiveArg();
		OrgLocationSelectEnable(false);
		cbFunctionSelectEnable(false);
		MetricEnable(false);

		ReplaceTool("Org Location Mode", "eToolTopLeft");
		ReplaceTool("Org Location Select", "eToolTopRight");
		ReplaceTool("Function", "eFunction");
		ReplaceTool("Metric Select", "eToolBottomRight");
	}

</script>
<span id="eToolTopLeft"><telerik:RadComboBox runat="server" ID="cbMode" Width="130px" OnClientSelectedIndexChanged="ModeChanged" DropDownWidth="250px"><Items>
		<telerik:RadComboBoxItem Text="Mode" Value="" />
		<telerik:RadComboBoxItem Text="Report Sub-locations" Value="ReportSub_" />
		<telerik:RadComboBoxItem Text="Report top level Location" Value="ReportTop_" />
        <telerik:RadComboBoxItem Text="Selected Sub-locations" Value="SelectedSub_" />
        <telerik:RadComboBoxItem Text="Selected Location" Value="SelectedTop_" />
</Items></telerik:RadComboBox></span>
<span id="eToolTopRight"> <uc:orgLocationSelect ID="cOrgLocationSelect" runat="server" Width="130px" RooOrgLocationEqualToNull="true" ShowNullAsRooOrgLocation="true" OnClientValueOrTextChanged="OrgLocationSelectChanged" OnClientDropDownClosed="OrgLocationDdlClosed" DropDownWidth="250px" /></span>
<span id="eFunction"> <telerik:RadComboBox runat="server" ID="cbFunction" Width="130px" OnClientSelectedIndexChanged="cbFunctionChanged" DropDownWidth="250px" OnClientDropDownOpened="cbFunctionDropDownOpened" OnClientDropDownClosed="cbFunctionDropDownClosed"><Items>
		<telerik:RadComboBoxItem Text="Sum" Value="Sum(" />
		<telerik:RadComboBoxItem Text="Average" Value="Average(" />
        <telerik:RadComboBoxItem Text="RMS" Value="RMS(" />
</Items></telerik:RadComboBox></span>
<span id="eToolBottomRight"> <telerik:RadComboBox runat="server" ID="cbMetric" Width="130px" OnItemDataBound="cbMetric_ItemDataBound" DataValueField="FormulaCode" DataTextField="Name" OnClientSelectedIndexChanged="MetricChanged" DropDownWidth="250px" /></span>
<telerik:radeditor ID="reExpression" runat="server" EditModes="Design" OnClientLoad="IntiFormulaBuilder" Height="240px" Width="600px"> 
   <Tools>
	<telerik:EditorToolGroup>
		<telerik:EditorTool Name="Add" ShowIcon="false" ShowText="true" Text="&nbsp;+&nbsp;" />
		<telerik:EditorTool Name="Sub" ShowIcon="false" ShowText="true" Text="&nbsp;-&nbsp;" />
		<telerik:EditorTool Name="Multiple" ShowIcon="false" ShowText="true" Text="&nbsp;*&nbsp;" />
		<telerik:EditorTool Name="Divide" ShowIcon="false" ShowText="true" Text="&nbsp;/&nbsp;" />
	</telerik:EditorToolGroup>
	<telerik:EditorToolGroup>
		<telerik:EditorTool Name="scOpen" ShowIcon="false" ShowText="true" Text="&nbsp;(&nbsp;" />
		<telerik:EditorTool Name="scClose" ShowIcon="false" ShowText="true" Text="&nbsp;)&nbsp;" />
		<telerik:EditorTool Name="scDot" ShowIcon="false" ShowText="true" Text="&nbsp;.&nbsp;" />
 	</telerik:EditorToolGroup>
 	<telerik:EditorToolGroup>
 		<telerik:EditorDropDown Name="ModeDropdown" Text="Org Location Mode" width="130px" height="50px" popupwidth="450px" popupheight="250px" />
 		<telerik:EditorDropDown Name="OrgLocationDropdown" Text="Org Location Select" width="130px" height="50px" popupwidth="450px" popupheight="250px" />
 		<telerik:EditorDropDown Name="FunctionDropdown" Text="Function" width="130px" height="50px" popupwidth="450px" popupheight="250px" />
 		<telerik:EditorDropDown Name="MetricSelect" Text="Metric Select" width="130px" popupwidth="450px" popupheight="250px"/>
 		<telerik:EditorTool Name="Apply" ShowIcon="false" ShowText="true"/>
 	</telerik:EditorToolGroup>
  </Tools>
</telerik:radeditor>
