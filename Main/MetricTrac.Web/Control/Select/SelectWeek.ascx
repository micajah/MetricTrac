<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelectWeek.ascx.cs" Inherits="MetricTrac.Control.SelectWeek" %>
<%@ Register src="~/Control/Select/SelectYear.ascx" tagname="SelectYear" tagprefix="mts" %>
<script type="text/javascript">
	function GetDddlValue(clientid)
	{
		var ddl = $get(clientid);
		if(ddl.selectedIndex<1) return 0;
		var v = ddl.options[ddl.selectedIndex].value;
		var vv = parseInt(v);
		return vv;
	}
	
	function GetStrTwoChar(s){
		var ss = s+"";
		if(ss.length<2) ss = "0"+s;
		if(ss.length<2) ss = "0"+s;
		return ss;
	}
	function OnDdlChange_<%=this.ClientID %>()
	{
		var Year = GetDddlValue("<%=cSelectYear.DdlClientID%>");
		var Week = GetDddlValue("<%=ddlWeek.ClientID%>");
		var StrDate="__/__/____";
		if(Year>0 && Week>0)
		{
			var d = new Date(Year,0,1);
			var w = d.getDay();
			d = new Date(Year,0,(8-w));
			d = new Date(d.valueOf()+(1000*60*60*24*7*(Week-1)));
			StrDate = GetStrTwoChar(d.getMonth()+1)+"/"+GetStrTwoChar(d.getDate())+"/"+d.getFullYear();
		}
		var lbInfo=$get("<%=lbInfo.ClientID%>");
		lbInfo.innerHTML = StrDate;
	}
	function Init_<%=this.ClientID %>()
	{
		var ddls = $("#<%=cSelectYear.DdlClientID%>").add("#<%=ddlWeek.ClientID%>");
		ddls.change(OnDdlChange_<%=this.ClientID %>);
		OnDdlChange_<%=this.ClientID %>();
	}
	$(Init_<%=this.ClientID %>);
</script>
<mts:SelectYear runat="server" ID="cSelectYear" />
<asp:DropDownList runat="server" ID="ddlWeek" />
<asp:Label runat="server" ID="lbInfo" />