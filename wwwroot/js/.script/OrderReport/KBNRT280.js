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
        yield xSplash.hide();
    });
});
$("#ReportRadioDiv").on("click", function () {
    return __awaiter(this, void 0, void 0, function* () {
        var ReportChecked = yield $("#ReportRadioDiv input[name='ReportRadio']:checked").val();
        console.log(ReportChecked);
        if (ReportChecked === "D") {
            $("#delayCheckBox").attr('disabled', true);
        }
        else if (ReportChecked === "S") {
            $("#delayCheckBox").attr('disabled', false);
        }
    });
});
$("#ReportBtn").on("click", function () {
    return __awaiter(this, void 0, void 0, function* () {
        var ReportChecked = yield $("#ReportRadioDiv input[name='ReportRadio']:checked").val();
        var OrderChecked = yield $("#OrderRadioDiv input[name='OrderRadio']:checked").val();
        var DateFrom = yield $("#F_DeliDateFrom").val().trim().replaceAll("-", "");
        var DateTo = yield $("#F_DeliDateTo").val().trim().replaceAll("-", "");
        var DateFromShow = DateFrom.substring(6, 8) + "/" + DateFrom.substring(4, 6) + "/" + DateFrom.substring(0, 4);
        var DateToShow = DateTo.substring(6, 8) + "/" + DateTo.substring(4, 6) + "/" + DateTo.substring(0, 4);
        var DelayCheck = yield $("#delayCheckBox").prop("checked");
        var url = "";
        if (DateFrom > DateTo) {
            return xSwal.Error("Error", "Please select Delivery Date from less than Delivery Date To");
        }
        if (ReportChecked === "S") {
            url = "KBNRT280/PrintReportSummary";
        }
        else if (ReportChecked === "D") {
            url = "KBNRT280/PrintReportDetail";
        }
        xAjax.Post({
            url: url,
            data: {
                orderChecked: OrderChecked,
                delayChecked: DelayCheck,
                dateFrom: DateFrom,
                dateTo: DateTo,
            },
            then: function (result) {
                console.log(result);
                if (result.status === "200") {
                    var filename = location.pathname.substring(location.pathname.lastIndexOf('/') + 1);
                    var reportUrl = "http://hmmt-app03/Reports/Pages/ReportViewer.aspx?/KB3/";
                    if (ReportChecked === "S") {
                        filename += "SUM";
                    }
                    else if (ReportChecked === "D") {
                        filename += "DETAIL";
                    }
                    window.location.href = reportUrl + filename + '&userName=' + result.data + '&delayChecked=' + DelayCheck +
                        '&Plant=' + result.data2 + '&Type=' + OrderChecked + '&DateFrom=' + DateFromShow + '&DateTo=' + DateToShow;
                }
                else {
                    return xSwal.error(result.title, result.message);
                }
            },
            error: function (result) {
                return console.error(result);
            }
        });
    });
});
