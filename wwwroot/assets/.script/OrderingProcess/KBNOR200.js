$(document).ready(function () {

    xSplash.hide();

});

$("#btnKBNMS004").click(function () {
    window.location.replace("/Master/KBNMS004");
});

$("button").click(function (e) {
    if(e.target.id == "btnKBNMS004") return;
    var _redirect = e.target.id.replace('btn', '');
    _xLib.SetProcessCookie(_redirect);
    if (window.location.pathname.includes("tpcap")) {
        return window.location.replace(`/kanban/SpecialOrdering/${_redirect}`);
    }
    return window.location.replace(`/SpecialOrdering/${_redirect}`);
});