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
        yield $("#tableMain").DataTable({
            "processing": true,
            "serverSide": false,
            width: '100%',
            paging: false,
            sorting: false,
            searching: false,
            scrollX: true,
            scrollY: "300px", // "300px"
            scrollCollapse: true,
            ordering: false,
            "columns": [
                { title: "Supplier Name", data: "f_short_Logistic" },
                { title: "Supplier Code", data: "f_Supplier_Code" },
                { title: "Route", data: "f_Route" },
                { title: "Cycle Time", data: "f_Cycle_Time" },
            ],
        });
        $("#inpStartDate").datepicker({
            format: "dd/mm/yyyy",
            todayHighlight: true,
            showRightIcon: false,
            value: moment().format("DD/MM/YYYY")
        });
        $("#inpProdMonth").datepicker({
            format: "mm/yyyy",
            todayHighlight: true,
            showRightIcon: false,
            value: moment().format("MM/YYYY")
        });
        var obj = {
            YM: moment($("#inpProdMonth").val(), "MM/YYYY").format("YYYYMM"),
        };
        prodMonth = $("#inpProdMonth").val();
        _xLib.AJAX_Get("/api/KBNLC190/GetRev", obj, function (success) {
            //console.log(success);
            $("#inpRev").val(success.data.f_Rev);
        }, function (error) {
            console.log(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        });
        $("#inpProdMonth").parent().addClass("col-5 ps-0 pe-0");
        $("table thead tr th").addClass("text-center");
        xSplash.hide();
    });
});
let prodMonth = "";
$("#inpProdMonth").on("change", function (e) {
    if (prodMonth != $(this).val()) {
        prodMonth = $(this).val();
        var obj = {
            YM: moment($(this).val(), "MM/YYYY").format("YYYYMM"),
        };
        _xLib.AJAX_Get("/api/KBNLC190/GetRev", obj, function (success) {
            //console.log(success);
            $("#inpRev").val(success.data.f_Rev);
        }, function (error) {
            console.log(error);
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
        });
    }
    else {
        return;
    }
});
$("#btnSearch").on("click", function () {
    var obj = {
        YM: moment($("#inpProdMonth").val(), "MM/YYYY").format("YYYYMM"),
        Rev: $("#inpRev").val(),
    };
    _xLib.AJAX_Get("/api/KBNLC190/Search", obj, function (success) {
        success.data = _xLib.TrimArrayJSON(success.data);
        console.log(success.data);
        $("#tableMain").DataTable().clear();
        $("#tableMain").DataTable().rows.add(success.data).draw();
        $("table thead tr th").addClass("text-center");
        $("table tbody tr td").addClass("text-center");
    }, function (error) {
        console.log(error);
        xSwal.error(error.responseJSON.response, error.responseJSON.message);
    });
});
$("#btnInterface").click(function () {
    var obj = {
        YM: moment($("#inpProdMonth").val(), "MM/YYYY").format("YYYYMM"),
        Rev: $("#inpRev").val(),
        StartDate: moment($("#inpStartDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
    };
    _xLib.AJAX_Post("/api/KBNLC190/Interface", JSON.stringify(obj), function (success) {
        console.log(success);
        xSwal.success("Success", success.message);
    }, function (error) {
        console.log(error);
        xSwal.error(error.responseJSON.response, error.responseJSON.message);
        let obj = {
            UserID: ajexHeader.UserCode,
            Type: "KBNLC190",
        };
        if (error.responseJSON.message.includes("Error Found")) {
            _xLib.OpenReportObj("/KBNIMERR", obj);
        }
    });
});
