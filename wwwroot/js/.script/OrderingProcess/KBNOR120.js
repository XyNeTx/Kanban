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
var _CookieProcessDate = _xLib.GetCookie("processDate");
var _CookieLoginDate = _xLib.GetCookie("loginDate");
$(document).ready(function () {
    xAjax.onClick('btnExit', function () {
        xAjax.redirect('KBNOR100');
    });
    $("#txtProcessDate").val(_CookieProcessDate.substring(0, 4) + "-" + _CookieProcessDate.substring(5, 7) + "-" + _CookieProcessDate.substring(8, 10));
    var shift = _CookieProcessDate.substring(10, 11) == "D" ? "1 - Day Shift" : "2 - Night Shift";
    $("#txtProcessShift").val(shift);
    _xLib.AJAX_Get("/api/KBNOR120/OnLoad", { Shift: $("#txtProcessShift").val() }, function (success) {
        if (success.status == "200") {
            xSplash.hide();
        }
    });
});
$("#btnCalculate").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        $("#divProgressBar").css("visibility", "visible");
        $("#widthProgressBar").css("width", "0%").attr("aria-valuenow", 0).removeClass("bg-success");
        $("#btnCalculate").prop("disabled", true);
        const _interval = setInterval(function () {
            _xLib.AJAX_Get("/api/KBNOR120/GetProcessCount", '', function (success) {
                return __awaiter(this, void 0, void 0, function* () {
                    if (success.status == "200") {
                        if (success.data == 100) {
                            yield clearInterval(_interval);
                        }
                        $("#widthProgressBar").css("width", success.data + "%").attr("aria-valuenow", success.data);
                        $("#spanProgressBar").text("Calculating Normal Order " + success.data + "%");
                    }
                });
            });
        }, 3000);
        var _url = "/api/KBNOR120/Process_Order";
        if ($("#txtProcessForShift").val().includes("Night")) {
            _url = "/api/KBNOR120/Process_Order_Night";
        }
        var eventProcessOrder = yield _xLib.AJAX_Get(_url, { sDate: $("#txtProcessDate").val() }, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                if (success.status == "200") {
                    //xSwal.success("Success", "Order Processed Successfully");
                    $("#widthProgressBar").css("width", "100%").attr("aria-valuenow", 100).addClass("bg-success");
                    $("#spanProgressBar").text("Calculating Normal Order Completed");
                    return success;
                }
            });
        }, function (error) {
            $("#btnCalculate").prop("disabled", false);
            $("#divProgressBar").css("visibility", "hidden");
            xSwal.error("Error", "Error in Processing Order");
            clearInterval(_interval);
        });
        if (eventProcessOrder.status == "200") {
            yield _xLib.AJAX_Get("/api/KBNOR120/Calculate", '', function (success) {
                return __awaiter(this, void 0, void 0, function* () {
                    if (success.status == "200") {
                        yield clearInterval(_interval);
                        xSplash.hide();
                        $("#btnCalculate").prop("disabled", false);
                        $("#divProgressBar").css("visibility", "hidden");
                        xSwal.success("Success", "Data Calculated Successfully");
                    }
                });
            });
            var oShell = new ActiveXObject("Shell.Application");
            var commandtoRun = "\\\\hmmta-tpcap\\kanban\\wwwroot\\Storage\\New_Kanban_F3_AutoRun_RecalculateBL.exe";
            oShell.ShellExecute(commandtoRun, "", "", "open", "1");
        }
    });
});
