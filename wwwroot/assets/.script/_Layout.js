var _i18n_ = '';
var i18nLayout = '';
var _swal = '';
var _menu_name = "";
var _PAGE_ = "";
var _CONTROLLER_ = "";
var _APPNAME_ = 'KANBAN';
var _PLANT_ = _xLib.GetCookie("plantCode");
//console.log(window.location.hostname);
//console.log(window.location.pathname);
//console.log(window.location.pathname.split("/"));

if (window.location.hostname.includes("localhost")) {
    _CONTROLLER_ = window.location.pathname.split("/")[1];
    //console.log(_CONTROLLER_);
    _PAGE_ = window.location.pathname.split("/")[2];
    //console.log(_PAGE_);
}
else {
    _CONTROLLER_ = window.location.pathname.split("/")[2];
    //console.log(_CONTROLLER_);
    _PAGE_ = window.location.pathname.split("/")[3];
    //console.log(_PAGE_);
}


var ajexHeader = {
    authorization: `Bearer ${localStorage.getItem('TOKEN')}`,
    UserCode: sessionStorage.getItem("UserCode"),
    Plant: sessionStorage.getItem("Factory"),
    Shift: sessionStorage.getItem("shift"),
    Controller: _CONTROLLER_,
    Action: _PAGE_
};

$(document).ready(async function () {
    xSplash.show();
    //$("#vueApp").toggleClass("d-none");

    var date = _xLib.GetCookie("loginDate").slice(0, 10);
    //console.log(date);
    $("#nr_Date").text(moment(date, "YYYY-MM-DD").format("DD/MM/YYYY"))
    $("#nr_Plant").text(_xLib.GetCookie("plantCode"));
    $("#nr_Shift").text(_xLib.GetCookie("loginDate").slice(10) == "D" ? "1 - Day Shift" : "2 - Night Shift");

    var onThisPage = $("li.pcoded-hasmenu").find(`a[href='${window.location.pathname}']`);

    onThisPage.css("color", "#00FFFF");
    if (onThisPage.parent().parent().parent().prop("tagName") == "LI") {
        onThisPage.parent().parent().parent().addClass("pcoded-trigger")
    }
    if (onThisPage.parent().parent().parent().parent().parent().prop("tagName") == "LI") {
        onThisPage.parent().parent().parent().parent().parent().addClass("pcoded-trigger")
    }

    if (_xLib.GetCookie("isDev") == 1) {
        $(".navbar-logo a b").append(" (Dev)");
    }

    $(".btn-toolbar[role='toolbar']").addClass("d-none");

    $("#mCSB_1_scrollbar_vertical").attr("class", "");
    $("#mCSB_1_dragger_vertical").attr("class", "");

    var _NAVDATETIME_ = setInterval(function () {
        $('#_NAVDATETIME_').html('Today : ' + moment().format("DD/MM/YYYY HH:mm:ss"));
    }, 1000);

    await _xLib.AJAX_Get("/xapi/GetAuthorizeProgram", null,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            //$("ul.pcoded-item.pcoded-left-item").find("li").each(function () {
            //    var id = $(this).attr("id");
            //    !success.data.some(x => id.trim() == x.F_Menu_ID.trim()) ? $(this).remove() : null;
            //});

            if (success.data.filter(x => _PAGE_ == x.F_Menu_ID).length > 0) {
                _menu_name = success.data.filter(x => _PAGE_ == x.F_Menu_ID)[0].F_Menu_Name

                if (_menu_name == undefined) _menu_name = "";
                if (_menu_name.includes(".")) _menu_name = _menu_name.split(".")[1].trim();

                $("#spanTitle").text(`${_PAGE_} : ${_menu_name}`)
                $(document).find("title").text(`${_PAGE_} : ${_menu_name}`)

                $("#txtPlant").val(_PLANT_);
            }
            else {
                $("#spanTitle").text(`HOME`)
                $(document).find("title").text(`HOME`)
            }

            return //$("#vueApp").toggleClass("d-none");
        },
        async (error) => {
            console.error(error);
            return //$("#vueApp").toggleClass("d-none");
        }
    );



});