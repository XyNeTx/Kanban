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
                    return "<input type='checkbox' class='chkFlag' data-id='" + row.F_OrderNO + "' checked/>";
                }
            },
            { title: "PDS No", data: "F_OrderNO" },
            { title: "PDS Issued Date", data: "F_PDS_Issued" },
            { title: "Supplier Code", data: "F_Supplier_CD" },
            { title: "Issued By", data: "F_Issued_By" },
            { title: "Send Approve Date", data: "F_Send_Date" },
            {
                title: "Preview", render: function (data, type, row, meta) {
                    return "<a class='btn text-light btn-rounded btn-primary' onclick=Preview($(this))>Preview</a>";
                }
            }
        ],
        order: [[1, "asc"]],
        scrollCollapse: false,
    });
    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");
    yield GetPDSWaitApprove();
    xSplash.hide();
}));
$("#btnChkAll").click(() => __awaiter(void 0, void 0, void 0, function* () {
    $(".chkFlag").prop("checked", true);
}));
$("#btnUnChkAll").click(() => __awaiter(void 0, void 0, void 0, function* () {
    $(".chkFlag").prop("checked", false);
}));
$("#btnApprove").click(() => __awaiter(void 0, void 0, void 0, function* () {
    yield Approve();
}));
Approve = () => __awaiter(void 0, void 0, void 0, function* () {
    let data = _xDataTable.GetSelectedDataDT("#tableMain");
    if (data.length == 0) {
        xSwal.error("Please select data.");
        return;
    }
    _xLib.AJAX_Post(`/api/KBNOR261/Approve`, data, (success) => __awaiter(void 0, void 0, void 0, function* () {
        xSwal.success(success.response, success.message);
        $("#tableMain").DataTable().clear().draw();
        //GetPDSWaitApprove();
    }), (error) => __awaiter(void 0, void 0, void 0, function* () {
        xSwal.xError(error);
    }));
});
Preview = (e) => __awaiter(void 0, void 0, void 0, function* () {
    console.log(e);
    let row = $(e).closest("tr");
    let data = _xDataTable.GetDataDT("#tableMain", row);
    console.log(data);
    _xLib.AJAX_Post(`/api/KBNOR261/Preview`, data, (success) => __awaiter(void 0, void 0, void 0, function* () {
        console.log(success);
        xSwal.success(success.response, success.message);
        let link = `http://hmmta-tpcap/E-Report/Report.aspx?Register=REC&PDSNoFrom=${data.F_OrderNO}&PDSNoTo=${data.F_OrderNO}&DateFrom=2024-07-01&DateTo=2999-12-31`;
        window.open(link, "_blank");
    }), (error) => __awaiter(void 0, void 0, void 0, function* () {
        xSwal.xError(error);
    }));
});
GetPDSWaitApprove = () => __awaiter(void 0, void 0, void 0, function* () {
    _xLib.AJAX_Get(`/api/KBNOR261/GetPDSWaitApprove`, null, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
    }));
});
