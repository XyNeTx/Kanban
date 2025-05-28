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
    return __awaiter(this, void 0, void 0, function* () {
        setTimeout(300);
        yield ProdMonthChange();
    });
});
function hideBlank() {
    $('#FlagFromBlank').hide();
    $('#FlagToBlank').hide();
}
xAjax.onChange("#F_ProdMonth", function () {
    return __awaiter(this, void 0, void 0, function* () {
        yield ProdMonthChange();
    });
});
function ProdMonthChange() {
    var prodMonth = $("#F_ProdMonth").val().trim();
    $("#Revision").val("");
    $("#Version").val("");
    return xAjax.Post({
        url: 'KBNRT260/Display_Detail',
        data: { prodMonth: prodMonth },
        then: function (result) {
            return __awaiter(this, void 0, void 0, function* () {
                console.log(result);
                yield $("#Revision").val(result.data[0].F_Revision);
                yield $("#Version").val(result.data[0].F_Version);
                xSplash.hide();
            });
        },
        error: function (result) {
            console.error("Initial Error", result);
        },
    });
}
xAjax.onClick("#ReportBtn", function () {
    return __awaiter(this, void 0, void 0, function* () {
        yield ReportClick();
    });
});
function ReportClick() {
    return __awaiter(this, void 0, void 0, function* () {
        var checkedValue = $("input[name='radio']:checked").val();
        var prodMonth = $("#F_ProdMonth").val().trim();
        var Revision = $("#Revision").val().trim();
        xAjax.Post({
            url: 'KBNRT260/OnReportClicked',
            data: {
                checkedValue: checkedValue,
                prodMonth: prodMonth
            },
            then: function (result) {
                if (result.status === "200") {
                    var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
                    var reportUrl = "http://hmmt-app03/Reports/Pages/ReportViewer.aspx?/KB3/";
                    prodMonth = prodMonth.replaceAll("-", "");
                    window.location.href = reportUrl + "KBNRT260" + '&prodMonth=' + prodMonth + '&checkedValue=' + checkedValue + '&UserName=' + result.data + '&Revision=' + Revision;
                }
                else {
                    return xSwal.Error(result.title, result.message);
                }
            },
            error: function (result) {
                console.error("Initial Error " + result);
            },
        });
    });
}
