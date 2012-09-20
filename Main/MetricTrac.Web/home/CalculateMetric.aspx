<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CalculateMetric.aspx.cs" Inherits="MetricTrac.CalculateMetric" MasterPageFile="~/MasterPage.master" %>
<asp:Content ID="cntCalculate" ContentPlaceHolderID="PageBody" runat="server">  
    <asp:Button ID="btnRecalc" runat="server" Text="Recalc All values" 
        onclick="btnRecalc_Click" />  <br /><br />
    <asp:Button ID="btnCalculate" runat="server" Text="Run Calculation" 
            onclick="btnCalculate_Click" /><br /><br />
    <asp:Label ID="lblResult" runat="server" Text=""></asp:Label>
</asp:Content>
