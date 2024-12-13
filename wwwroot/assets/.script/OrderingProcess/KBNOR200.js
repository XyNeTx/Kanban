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
    //return console.log(window.location.href);
    //console.log(_redirect);
    if (window.location.href.includes("tpcap")) {
        //return console.log(`/kanban/SpecialOrdering/${_redirect}`);
        return window.location.replace(`/kanban/SpecialOrdering/${_redirect}`);
    }
    return window.location.replace(`/SpecialOrdering/${_redirect}`);
});