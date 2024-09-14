$(document).ready(function () {

    xSplash.hide();

});
$("button").click(function (e) {
    var _redirect = e.target.id.replace('btn', '');
    _xLib.SetProcessCookie(_redirect);
    if (window.location.pathname.includes("tpcap")) {
        return window.location.replace(`/kanban/SpecialOrdering/${_redirect}`);
    }
    return window.location.replace(`/SpecialOrdering/${_redirect}`);
});