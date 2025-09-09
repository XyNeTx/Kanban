var _i18n_ = '';
var i18nLayout = '';
var _swal = '';
var _menu_name = "";

var _PAGE_ = window.location.pathname.split("/")[2];
var _CONTROLLER_ = window.location.pathname.split("/")[1];
var _APPNAME_ = 'KANBAN';
var _PLANT_ = _xLib.GetCookie("plantCode");

$(document).ready(async function () {
    xSplash.show();
    var date = _xLib.GetCookie("loginDate").slice(0, 10);
    console.log(date);
    $("#nr_Date").text(moment(date, "YYYY-MM-DD").format("DD/MM/YYYY"))
    $("#nr_Plant").text(_xLib.GetCookie("plantCode"));
    $("#nr_Shift").text(_xLib.GetCookie("loginDate").slice(10) == "D" ? "1 - Day Shift" : "2 - Night Shift");

    await _xLib.AJAX_Get("/xapi/GetAuthorizeProgram", null,
        async (success) => {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("ul.pcoded-item.pcoded-left-item").find("li").each(function () {
                //console.log($(this));
                var id = $(this).attr("id");
                //var href = $(this).attr("href");
                //console.log(id);
                //console.log(href);
                !success.data.some(x => id.trim() == x.F_Menu_ID.trim()) ? $(this).remove() : null;
                //console.log(success.data.some(x => id == x.F_Menu_ID));
            });
            //console.log(success.data.filter(x => _PAGE_ == x.F_Menu_ID));
            // console.log(_PAGE_);
            // console.log(success.data.some(x => _PAGE_ == x.F_Menu_ID));
            if (!success.data.some(x => _PAGE_ == x.F_Menu_ID) && _PAGE_ != "Index" && _CONTROLLER_ != "Home" ) {
                $("#vueApp").hide();
                xSwal.error('Error', "You don't have permission to access this page.");
                $(".swal2-confirm.swal2-styled").on('click', function () {
                    window.location.href = "/Home/Index"
                });
            }

            _menu_name = success.data.filter(x => _PAGE_ == x.F_Menu_ID)[0].F_Menu_Name

            if (_menu_name == undefined) _menu_name = "";
            if (_menu_name.includes(".")) _menu_name = _menu_name.split(".")[1].trim();

            $("#spanTitle").text(`${_PAGE_} : ${_menu_name}`)
            $(document).find("title").text(`${_PAGE_} : ${_menu_name}`)

            $("#txtPlant").val(_PLANT_);
        },
        async (error) => {
            console.error(error);
        }
    );

    $(".btn-toolbar[role='toolbar']").addClass("d-none");
    $("#mCSB_1_scrollbar_vertical").attr("class", "");
    $("#mCSB_1_dragger_vertical").attr("class", "");

    var _NAVDATETIME_ = setInterval(function () {
        $('#_NAVDATETIME_').html('Today : ' + moment().format("DD/MM/YYYY HH:mm:ss"));
    }, 1000);

});














// xSplash.show();

// var _i18n_ = '';
// var i18nLayout = '';
// var _swal = '';
// var _menu_name = "";

// var _PAGE_ = window.location.pathname.split("/")[2];
// var _CONTROLLER_ = window.location.pathname.split("/")[1];
// var _APPNAME_ = 'KANBAN';
// var _PLANT_ = _xLib.GetCookie("plantCode");

// $(document).ready(async function()
// {
//     var date = _xLib.GetCookie("loginDate").slice(0, 10);
//     console.log(date);
//     $("#nr_Date").text(moment(date, "YYYY-MM-DD").format("DD/MM/YYYY"))
//     $("#nr_Plant").text(_xLib.GetCookie("plantCode"));
//     $("#nr_Shift").text(_xLib.GetCookie("loginDate").slice(10) == "D" ? "1 - Day Shift" : "2 - Night Shift");

//     await _xLib.AJAX_Get("/xapi/GetAuthorizeProgram", null,
//         async (success) => {
//             success = _xLib.JSONparseMixData(success);
//             console.log(success);
//             $("ul.pcoded-item.pcoded-left-item").find("li").each(function () {
//                 console.log($(this));
//                 var id = $(this).attr("id");
//                 //var href = $(this).attr("href");
//                 console.log(id);
//                 //console.log(href);
//                 !success.data.some(x => id.trim() == x.F_Menu_ID.trim()) ? $(this).remove() : null;
//                 console.log(success.data.some(x => id == x.F_Menu_ID));
//             });
//             //console.log(success.data.filter(x => _PAGE_ == x.F_Menu_ID));
//             _menu_name = success.data.filter(x => _PAGE_ == x.F_Menu_ID)[0].F_Menu_Name

//             if (_menu_name == undefined) _menu_name = "";
//             if (_menu_name.includes(".")) _menu_name = _menu_name.split(".")[1].trim();

//             $("#spanTitle").text(`${_PAGE_} : ${_menu_name}`)
//             $(document).find("title").text(`${_PAGE_} : ${_menu_name}`)

//             if (!success.data.some(x => _PAGE_ == x.F_Menu_ID)){
//                 window.location.href("~/Home/Index")
//             }
//             $("#txtPlant").val(_PLANT_);
//         },
//         async (error) => {
//             console.error(error);
//         }
//     );

//     $(".btn-toolbar[role='toolbar']").addClass("d-none");

//     var _NAVDATETIME_ = setInterval(function () {
//             $('#_NAVDATETIME_').html('Today : ' + moment().format("DD/MM/YYYY HH:mm:ss"));
//         }, 1000);

// });