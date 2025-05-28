"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
$(document).ready(function () {
    checkProgramAuthorize();
});
//$("#btnKBNMS004").click(function () {
//    window.location.replace("/Master/KBNMS004");
//});
$("button").click(function (e) {
    //if(e.target.id == "btnKBNMS004") return;
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
function checkProgramAuthorize() {
    _xLib.AJAX_Get("/xapi/GetAuthorizeProgram", null, (success) => __awaiter(this, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        console.log(success);
        $(".card-body").find("button").each(function (e) {
            let id = $(this).attr("id");
            //console.log($(this).attr("id"));
            console.log(success.data.some(x => id == "btn" + x.F_Menu_ID));
            !success.data.some(x => id == "btn" + x.F_Menu_ID) ? $(this).prop("disabled", true) : $(this).prop("disabled", false);
        });
        xSplash.hide();
    }), (error) => __awaiter(this, void 0, void 0, function* () {
        console.error(error);
    }));
}
