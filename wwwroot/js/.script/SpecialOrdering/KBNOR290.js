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
    yield _xDataTable.InitialDataTable("#tableMain", {
        columns: [
            { title: "Survey Doc. No.", data: "F_Survey_Doc" },
            { title: "Supplier CD.", data: "F_Supplier" },
            { title: "Part No.", data: "F_Part_No" },
            { title: "Qty", data: "F_Qty" },
            { title: "Delivery Date", data: "F_Deli_DT" },
            { title: "Status", data: "F_Status_D" },
            { title: "Comment", data: "F_Remark_Delivery" },
        ],
        order: [[1, "asc"]],
        scrollCollapse: false,
    });
    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");
    YMChange();
    xSplash.hide();
}));
$("#mthSurIssDate").change(() => __awaiter(void 0, void 0, void 0, function* () {
    yield YMChange();
}));
$("#selCustOrder").change(() => __awaiter(void 0, void 0, void 0, function* () {
    yield CustOrderChange();
}));
$("#selSuppCD").change(() => __awaiter(void 0, void 0, void 0, function* () {
    yield SuppCDChange();
}));
YMChange = () => __awaiter(void 0, void 0, void 0, function* () {
    let getQuery = {
        YM: $("#mthSurIssDate").val().replace("-", "")
    };
    _xLib.AJAX_Get("/api/KBNOR290/ProdYMChanged", getQuery, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        // console.log(success);
        $("#selCustOrder").empty();
        $("#selCustOrder").append("<option value='' hidden></option>");
        $.each(success.data, function (index, value) {
            //console.log(value);
            $("#selCustOrder").append(new Option(value.f_PO_Customer, value.f_PO_Customer));
        });
        $("#selCustOrder").selectpicker("refresh");
    }), (error) => __awaiter(void 0, void 0, void 0, function* () {
        console.error(error);
    }));
});
CustOrderChange = () => __awaiter(void 0, void 0, void 0, function* () {
    let getQuery = {
        PO: $("#selCustOrder").val(),
    };
    _xLib.AJAX_Get("/api/KBNOR290/GetSuppCD", getQuery, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        //console.log(success);
        $("#selSuppCD").empty();
        $("#selSuppCD").append("<option value='' hidden></option>");
        $.each(success.data, function (index, value) {
            //console.log(value);
            $("#selSuppCD").append(new Option(value.f_Supplier_CD, value.f_Supplier_CD));
        });
        $("#selSuppCD").selectpicker("refresh");
    }), (error) => __awaiter(void 0, void 0, void 0, function* () {
        console.error(error);
    }));
});
SuppCDChange = () => __awaiter(void 0, void 0, void 0, function* () {
    let getQuery = {
        PO: $("#selCustOrder").val(),
        Supplier: $("#selSuppCD").val(),
    };
    _xLib.AJAX_Get("/api/KBNOR290/GetData", getQuery, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        //console.log(success);
        yield _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
    }), (error) => __awaiter(void 0, void 0, void 0, function* () {
        console.error(error);
    }));
});
$("#btnPrint").click(() => {
    let obj = {
        F_PO_Customer: $("#selCustOrder").val(),
        F_Supplier_CD: $("#selSuppCD").val().split("-")[0],
        F_Supplier_Plant: $("#selSuppCD").val().split("-")[1]
    };
    for (let key in obj) {
        if (obj[key] === "") {
            return xSwal.error("Error!!", 'Please select all fields.');
        }
    }
    ;
    _xLib.OpenReportObj("/KBNOR220_2_Rpt", obj);
});
