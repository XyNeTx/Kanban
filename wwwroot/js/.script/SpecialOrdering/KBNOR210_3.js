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
        yield Initial();
        let loginDate = _xLib.GetCookie("loginDate");
        let plant = _xLib.GetCookie("plantCode");
        //console.log(loginDate);
        //console.log(plant);
        $("#readProcessDate").val(moment(loginDate.slice(0, 10), "YYYY-MM-DD").format("DD/MM/YYYY"));
        $("#readProcessShift").val(loginDate.slice(10, 11) == "D" ? "1:Day Shift" : "2:Night Shift");
        $("#readPlant").val(plant);
        yield LoadOrderNo();
        yield xSplash.hide();
    });
});
$("#btnExit").click(function () {
    if (window.location.hostname.includes("tpcap")) {
        return window.location.replace("/kanban/OrderingProcess/KBNOR200");
    }
    return window.location.replace("/OrderingProcess/KBNOR200");
});
function Initial() {
    return __awaiter(this, void 0, void 0, function* () {
        yield $("#tableMain").DataTable({
            width: '100%',
            paging: false,
            scrollCollapse: true,
            "processing": false,
            "serverSide": false,
            scrollX: false,
            scrollY: '400px',
            searching: false,
            info: false,
            ordering: false,
            columns: [
                { title: "Prod YM", data: "f_ProdYM" },
                { title: "Customer OrderNo.", data: "f_PDS_No" },
                { title: "Delivery Date", data: "f_Delivery_Date" },
            ],
            order: [[1, 'asc']]
        });
        $("table thead tr th").css("text-align", "center");
        $("table tbody tr td").css("text-align", "center");
    });
}
function LoadOrderNo() {
    return __awaiter(this, void 0, void 0, function* () {
        yield _xLib.AJAX_Get("/api/KBNOR210_3/LoadOrderNo", "", function (success) {
            console.log(success);
            //success.data = _xLib.TrimArrayJSON(success.data);
            $("#inpNewCustomerOrderNo").empty();
            $("#inpNewCustomerOrderNo").append("<option value='' hidden></option>");
            success.data.forEach(function (item) {
                //console.log(item);
                $("#inpNewCustomerOrderNo").append(`<option value="${item.trim()}">${item.trim()}</option>`);
            });
            $("#inpNewCustomerOrderNo").selectpicker("refresh");
        }, function (error) {
            console.error(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        });
    });
}
;
function LoadCustomerPO() {
    return __awaiter(this, void 0, void 0, function* () {
        yield _xLib.AJAX_Get("/api/KBNOR210_3/LoadCustomerPO", { NewCusPO: $("#inpNewCustomerOrderNo").val() }, function (success) {
            console.log(success);
            success.data = _xLib.TrimArrayJSON(success.data);
            $("#tableMain").DataTable().clear().rows.add(success.data).draw();
            $("#tableMain tbody tr td").css("text-align", "center");
            $("#tableMain thead tr th").css("text-align", "center");
            $("#btnUnmerge").prop("disabled", false);
        }, function (error) {
            console.error(error);
            //xSwal.error(error.responseJSON.response, error.responseJSON.message);
        });
    });
}
;
$("#inpNewCustomerOrderNo").change(function () {
    return __awaiter(this, void 0, void 0, function* () {
        yield LoadCustomerPO();
    });
});
$("#btnUnmerge").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        $("#btnUnmerge").prop("disabled", true);
        yield xSplash.show();
        let listObj = $("#tableMain").DataTable().rows().data().toArray();
        for (let i = 0; i < listObj.length; i++) {
            listObj[i].f_PDS_No_New = $("#inpNewCustomerOrderNo").val();
        }
        if (listObj.length == 0) {
            xSwal.error("Error", "No data to unmerge.");
            return;
        }
        _xLib.AJAX_Post("/api/KBNOR210_3/Unmerge", JSON.stringify(listObj), function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                yield xSplash.hide();
                console.log(success);
                xSwal.success("Success", success.message);
                $("#inpNewCustomerOrderNo").val("");
                $("#tableMain").DataTable().clear().draw();
                LoadCustomerPO();
            });
        }, function (error) {
            return __awaiter(this, void 0, void 0, function* () {
                yield xSplash.hide();
                console.error(error);
                xSwal.error(error.responseJSON.response, error.responseJSON.message);
            });
        });
    });
});
