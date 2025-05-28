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
            {
                title: "Flag", render: function (data, type, row, meta) {
                    return "<input type='checkbox' class='chkFlag' data-id='" + row.f_OrderNo + "' checked/>";
                }
            },
            { title: "PDS No", data: "f_OrderNo" },
            { title: "Customer OrderNo", data: "f_PO_Customer" },
            { title: "Supplier Code", data: "f_Supp_CD" },
            { title: "Delivery Date", data: "f_Delivery_Date" },
            { title: "Status", data: "f_Status" },
            {
                title: "Approve", data: "f_Approver", render: function () {
                    return "";
                }
            }
        ],
        order: [[1, "asc"]],
        scrollCollapse: false,
    });
    yield _xDataTable.InitialDataTable("#modalTableMain", {
        columns: [
            {
                title: "Flag", render: function (data, type, row, meta) {
                    return "<input type='checkbox' class='chkFlag' data-id='" + row.f_OrderNo + "' checked/>";
                }
            },
            { title: "PDS No", data: "f_OrderNo" },
            { title: "Survey Doc.", data: "f_PO_Customer" },
            { title: "Supplier Code", data: "f_Supp_CD" },
            { title: "Delivery Date", data: "f_Delivery_Date" },
            { title: "Status", data: "f_Status" },
            { title: "Approve", data: "f_Approver" }
        ],
        order: [[1, "asc"]],
        scrollCollapse: false,
    });
    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");
    yield GetApproverList();
    yield GetPDSDataNoApprove();
    xSplash.hide();
}));
$("#btnChkAll").click(() => __awaiter(void 0, void 0, void 0, function* () {
    $(".chkFlag").prop("checked", true);
}));
$("#btnUnChkAll").click(() => __awaiter(void 0, void 0, void 0, function* () {
    $(".chkFlag").prop("checked", false);
}));
$("#modalBtnChkAll").click(() => __awaiter(void 0, void 0, void 0, function* () {
    $("#modalTableMain .chkFlag").prop("checked", true);
}));
$("#modalBtnUnChkAll").click(() => __awaiter(void 0, void 0, void 0, function* () {
    $("#modalTableMain .chkFlag").prop("checked", false);
}));
$("#btnSendApprove").click(() => __awaiter(void 0, void 0, void 0, function* () {
    yield SendApprove();
}));
GetPDSDataNoApprove = () => __awaiter(void 0, void 0, void 0, function* () {
    let getQuery = {
        fac: _xLib.GetCookie("plantCode")
    };
    _xLib.AJAX_Get("/api/KBNOR260/GetPDSDataNoApprove", getQuery, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        console.log(success);
        yield _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
    }));
});
GetApproverList = () => __awaiter(void 0, void 0, void 0, function* () {
    _xLib.AJAX_Get("/api/KBNOR260/GetApproverList", null, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        console.log(success);
        $("#selApprove").addListSelectPicker(success.data, "F_Name");
        $("#modalSelApprove").addListSelectPicker(success.data, "F_Name");
    }));
});
SendApprove = () => __awaiter(void 0, void 0, void 0, function* () {
    if (!$("#selApprove").val()) {
        yield xSwal.error("Error !!!", "Please select approver.");
        return;
    }
    let postQuery = _xDataTable.GetSelectedDataDT("#tableMain");
    postQuery.forEach((item) => {
        item.f_Approver = $("#selApprove").val();
    });
    console.log(postQuery);
    _xLib.AJAX_Post("/api/KBNOR260/SendApprove", postQuery, (success) => __awaiter(void 0, void 0, void 0, function* () {
        yield xSwal.success(success.response, success.message);
        yield GetPDSDataNoApprove();
    }), (error) => __awaiter(void 0, void 0, void 0, function* () {
        xSwal.xError(error);
    }));
});
$(document).on('show.bs.modal', '#KBNOR260_1', (e) => __awaiter(void 0, void 0, void 0, function* () {
    xSplash.show();
    yield $("#KBNOR260_1").on('shown.bs.modal', (e) => __awaiter(void 0, void 0, void 0, function* () {
        $("#modalTableMain").DataTable().columns.adjust().clear().draw();
        $("table thead tr th").css("text-align", "center");
        $("table tbody tr td").css("text-align", "center");
        yield GetPDSWaitApprove();
        xSplash.hide();
    }));
}));
GetPDSWaitApprove = () => __awaiter(void 0, void 0, void 0, function* () {
    let getQuery = {
        fac: _xLib.GetCookie("plantCode")
    };
    _xLib.AJAX_Get("/api/KBNOR260/GetPDSWaitApprove", getQuery, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        console.log(success);
        yield _xDataTable.ClearAndAddDataDT("#modalTableMain", success.data);
    }));
});
$("#modalBtnResend").click(() => __awaiter(void 0, void 0, void 0, function* () {
    if (!$("#modalSelApprove").val()) {
        yield xSwal.error("Error !!!", "Please select approver.");
        return;
    }
    let postQuery = _xDataTable.GetSelectedDataDT("#modalTableMain");
    postQuery.forEach((item) => {
        item.f_Approver = $("#modalSelApprove").val();
    });
    console.log(postQuery);
    _xLib.AJAX_Post("/api/KBNOR260/SendApprove", postQuery, (success) => __awaiter(void 0, void 0, void 0, function* () {
        yield xSwal.success(success.response, success.message);
        yield GetPDSDataNoApprove();
    }), (error) => __awaiter(void 0, void 0, void 0, function* () {
        xSwal.xError(error);
    }));
}));
