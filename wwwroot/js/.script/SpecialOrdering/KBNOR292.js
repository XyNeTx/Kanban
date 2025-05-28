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
    yield GetSupplierSurvey();
    xSplash.hide();
}));
$("#mthSurveyIssued").change(() => __awaiter(void 0, void 0, void 0, function* () {
    yield GetSupplierSurvey();
}));
$("#selSuppCD").change(() => __awaiter(void 0, void 0, void 0, function* () {
    yield GetSupplierSurvey();
}));
GetSupplierSurvey = () => __awaiter(void 0, void 0, void 0, function* () {
    let getQuery = {
        IssueDate: $("#mthSurveyIssued").val().replace(/-/g, ""),
        SupplierCD: $("#selSuppCD").val()
    };
    _xLib.AJAX_Get("/api/KBNOR292/GetSupplierSurvey", getQuery, (result) => __awaiter(void 0, void 0, void 0, function* () {
        result = _xLib.JSONparseMixData(result);
        if (getQuery.SupplierCD) {
            $("#spanSuppName").html(result.data[0].F_Supplier_INT + ":" + result.data[0].F_Supplier_Name);
        }
        else {
            $("#selSuppCD").addListSelectPicker(result.data, "F_Supplier_CD");
        }
    }));
});
$("#btnReport").click(() => {
    let obj = {
        Plant: _xLib.GetCookie("plantCode"),
        UserName: _xLib.GetUserName(),
        F_Issued_YM: $("#mthSurveyIssued").val().replace(/-/g, ""),
        F_Supplier_CD: $("#selSuppCD").val()
    };
    for (let key in obj) {
        if (obj[key] === "") {
            return xSwal.error("Error!!", 'Please select all fields.');
        }
    }
    ;
    _xLib.OpenReportObj("/KBNOR292", obj);
});
