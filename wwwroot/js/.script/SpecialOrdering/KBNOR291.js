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
$(document).ready(() => __awaiter(void 0, void 0, void 0, function* () {
    xSplash.hide();
}));
$("#btnReport").click(() => {
    let obj = {
        Plant: _xLib.GetCookie("plantCode"),
        UserName: _xLib.GetUserName(),
        F_Issued_Date: $("#mthSurveyIssued").val().replace(/-/g, ""),
    };
    for (let key in obj) {
        if (obj[key] === "") {
            return xSwal.error("Error!!", 'Please select all fields.');
        }
    }
    ;
    _xLib.OpenReportObj("/KBNOR291", obj);
});
