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
        yield initial();
    });
});
function hideBlank() {
    $('#FlagFromBlank').hide();
    $('#FlagToBlank').hide();
}
function initial() {
    return xAjax.Post({
        url: 'KBNRT250/F_System_Flag',
        then: function (result) {
            $.each(result.data, function (i, v) {
                return __awaiter(this, void 0, void 0, function* () {
                    console.log(result);
                    $("#F_FlagFrom").append($("<option>", { value: v, text: v }, "</option>"));
                    $("#F_FlagTo").append($("<option>", { value: v, text: v }, "</option>"));
                    yield hideBlank();
                    yield xSplash.hide();
                });
            });
        },
        error: function (result) {
            console.error("Initial Error");
        },
    });
}
function ReportClick() {
    return __awaiter(this, void 0, void 0, function* () {
        var dateFrom = $("#F_DateFrom").val().trim().replaceAll("-", "");
        var dateTo = $("#F_DateTo").val().trim().replaceAll("-", "");
        var flagFrom = $("#F_FlagFrom").val().trim();
        var flagTo = $("#F_FlagTo").val().trim();
        var timeFrom = $("#F_TimeFrom").val().trim().replaceAll(".", ":");
        var timeTo = $("#F_TimeTo").val().trim().replaceAll(".", ":");
        if (flagFrom == "" || flagFrom == undefined || flagTo == "" || flagTo == undefined) {
            return xSwal.error("Invalid Select System Flag", "Please Select System Flag then try again!");
        }
        if (flagFrom > flagTo) {
            return xSwal.error("Select System Flag ERROR!", "Please System Flag From less than System Flag To");
        }
        if (dateFrom > dateTo) {
            return xSwal.error("Select Order Date ERROR!", "Please Select Order Date From less than Order Date To");
        }
        if (timeFrom > timeTo) {
            return xSwal.error("Select Time Stamp ERROR!", "Please Select Time Stamp less than Time Stamp To");
        }
        xAjax.Post({
            url: 'KBNRT250/OnClickReport',
            data: {
                flagFrom: flagFrom,
                flagTo: flagTo,
                dateFrom: dateFrom,
                dateTo: dateTo,
                timeFrom: timeFrom,
                timeTo: timeTo
            },
            then: function (result) {
                if (result.status === "200") {
                    var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
                    var reportUrl = "http://hmmt-app03/Reports/Pages/ReportViewer.aspx?/KB3/";
                    window.location.href = reportUrl + filename + '&Plant=' + result.data2 + '&UserName=' + result.data;
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
xAjax.onClick("#ReportBtn", function () {
    return __awaiter(this, void 0, void 0, function* () {
        yield ReportClick();
    });
});
