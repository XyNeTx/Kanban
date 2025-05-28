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
        $("#tableMain").DataTable({
            "processing": true,
            "columns": [
                { "title": "Supplier Code", "data": "F_Supplier_Code" },
                { "title": "Cycle Time", "data": "F_Cycle" },
                { "title": "Delivery Start", "data": "F_Start_Date" },
                { "title": "Delivery End", "data": "F_End_Date" },
                { "title": "* Order Start", "data": "F_Start_Order_Date" },
                { "title": "* Order End", "data": "F_End_Order_Date" },
                { "title": "Delivery Trip", "data": "F_Delivery_Trip" },
                { "title": "Time", "data": "F_Delivery_Time" },
            ],
            "order": [[0, "asc"]],
            "paging": false,
            "info": false,
            "searching": true,
            "scrollY": "350px",
            "scrollCollapse": true,
            "width": "100%",
        });
        yield _xLib.AJAX_Get("/api/KBNMS022/GetList", null, function (success) {
            if (success.status == "200") {
                success = _xLib.JSONparseAndTrim(success);
                $("#tableMain").DataTable().clear().rows.add(success.data).draw();
                //console.log(success);
                $("table th, #tableMain td").addClass("text-center");
                $("table thead").addClass("table-dark");
            }
        }, function (error) {
            xSwal.error("Error !!", "Can't get data from server.");
        });
        xSplash.hide();
    });
});
