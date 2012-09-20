// MetricList.aspx / Save Filter
function GetRadWindow() {
    var oWindow = null;
    if (window.radWindow)
        oWindow = window.radWindow;
    else if (window.frameElement.radWindow)
        oWindow = window.frameElement.radWindow;
    return oWindow;
}

function CloseOnReload(IsCancel) {
    var w = GetRadWindow();
    if (IsCancel) w.argument = null;
    else w.argument = "Save";
    w.close();
}

// MetricList.ascx
function openRadWindow(url) {
    var oWnd = radopen(url, "rwPopup");
    oWnd.Center();
}

//PerformanceIndicatorInfo.aspx
function CloseInfoWindow() {
    var w = GetRadWindow();
    w.close();
}