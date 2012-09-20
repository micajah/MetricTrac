<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListManager.ascx.cs" Inherits="MetricTrac.Control.DataView.ListManager" %>
<script type="text/javascript">
	function GetListManagerTr(a) {
		return a.parentNode.parentNode.parentNode.parentNode.parentNode.parentNode;
	}
	function GetManagerTable(tr) {
		for (var i = 0; i < 33; i++) {
			var ManagerTable = tr.cells[0].childNodes[i];
			if (ManagerTable != null) if(ManagerTable.id == "lmTable") return ManagerTable;
			ManagerTable = tr.cells[tr.cells.length-1].childNodes[i];
			if (ManagerTable != null) if(ManagerTable.id == "lmTable") return ManagerTable;
		}
		return null;
	}

	function GetHidenField(tr) {
		var temp = GetManagerTable(tr).rows[0];
		temp = temp.cells[temp.cells.length - 1];
		temp = temp.childNodes[temp.childNodes.length - 1];
		return temp;
	}

	function ListManagerAddClick(a) {
		var tr = GetListManagerTr(a);
		var CurHidenField = GetHidenField(tr);
		CurHidenField.value = "0,1";

		var ManagerTable = GetManagerTable(tr);
		var table = tr.parentNode.parentNode;
		if (table.rows.length <= tr.rowIndex + 1) return;

		ManagerTable.style.display = "none";
		tr = table.rows[tr.rowIndex + 1];
		tr.style.display = "";
		var NextHidenField = GetHidenField(tr);
		NextHidenField.value = "0,0";
	}
	function ListManagerRemoveClick(a) {
		var tr = GetListManagerTr(a);
		var CurHidenField = GetHidenField(tr);
		if (tr.rowIndex == 0) return;
		CurHidenField.value = "1,0";
		tr.style.display = "none";

		var table = tr.parentNode.parentNode;
		tr = table.rows[tr.rowIndex - 1];
		tr.style.display = "";
		var PrevHidenField = GetHidenField(tr);
		var ManagerTable = GetManagerTable(tr);
		ManagerTable.style.display = "";
		PrevHidenField.value = "0,0";
	}
</script>
<table style='<%=GetStyle()%>' id="lmTable"><tr>
	<td>
		<%if(First){%>
			<img src="../images/null.gif" width="16" height="16" alt="Add" title="Add" />
		<%}else{%>
			<a href=" " onclick="ListManagerRemoveClick(this); return false;"><img src="../images/buttons/Remove.gif" width="16" height="16" alt="Remove" title="Remove" border="0" /></a>
		<%}%>
	</td><td>
		<%if(Last){%>
			<img src="../images/null.gif" width="16" height="16" alt="Add" title="Add" />
		<%}else{%>
			<a href=" " onclick="ListManagerAddClick(this); return false;"><img src="../images/buttons/Add.gif" width="16" height="16" alt="Add" title="Add" border="0" /></a>
		<%}%>
	<asp:HiddenField runat="server" ID="hfListManager" /></td>
</tr></table>
