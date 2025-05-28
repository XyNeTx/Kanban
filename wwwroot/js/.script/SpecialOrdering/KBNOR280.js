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
                    return "<input type='checkbox' class='chkFlag' checked/>";
                }
            },
            { title: "PDS No", data: "F_OrderNo" },
            { title: "Supplier Code", data: "F_Supp_CD" },
            { title: "Cycle Time", data: "F_Delivery_Cycle" },
            { title: "Delivery Date", data: "F_Delivery_Date" },
            { title: "Delivery Trip", data: "F_Delivery_Trip" },
        ],
        order: [[1, "asc"]],
        scrollCollapse: false,
    });
    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");
    yield GetPDSData();
    xSplash.hide();
}));
$("#btnChkAll").click(() => {
    $(".chkFlag").prop("checked", true);
});
$("#btnUnChkAll").click(() => {
    $(".chkFlag").prop("checked", false);
});
$("#mthDeliYM").change(() => __awaiter(void 0, void 0, void 0, function* () {
    yield GetPDSData();
}));
$("#btnRegister").click(() => __awaiter(void 0, void 0, void 0, function* () {
    yield Register();
    $("#tableMain").DataTable().clear().draw();
    //await GetPDSData();
}));
GetPDSData = () => __awaiter(void 0, void 0, void 0, function* () {
    let GetQuery = {
        DeliYM: $("#mthDeliYM").val().replace("-", ""),
    };
    _xLib.AJAX_Get("/api/KBNOR280/GetPDSData", GetQuery, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        console.log(success);
        yield _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
    }));
});
Register = () => __awaiter(void 0, void 0, void 0, function* () {
    xSplash.show();
    let PostData = [];
    $("#tableMain tbody tr").find(".chkFlag:checked").each((i, e) => {
        //console.log($(this));
        //console.log($(e).closest("tr"));
        //var row = $(this).closest("tr");
        let data = $("#tableMain").DataTable().row($(e).closest("tr")).data();
        //console.log(data);
        PostData.push(data);
    });
    //return console.log(PostData);
    _xLib.AJAX_Post("/api/KBNOR280/Register", PostData, (success) => __awaiter(void 0, void 0, void 0, function* () {
        yield xSplash.hide();
        yield xSwal.success(success.response, success.message);
        $("#tableMain input[type='checkbox']:checked").each(function () {
            $("#tableMain").DataTable().row($(this).closest("tr")).remove().draw(false);
        });
    }), (error) => __awaiter(void 0, void 0, void 0, function* () {
        xSwal.xError(error);
        console.error(error);
    }));
});
//------------------------------------------ Modal --------------------------------
$(document).on("show.bs.modal", "#KBNOR280_EX", () => __awaiter(void 0, void 0, void 0, function* () {
    $("#inpExDeliveryFrom").initDatepicker();
    $("#inpExDeliveryTo").initDatepicker();
    yield ExGetPO();
    yield ExGetSupplier();
    $(".selectpicker").selectpicker("refresh");
}));
$("#chkboxExCustOrder").change(() => __awaiter(void 0, void 0, void 0, function* () {
    if ($("#chkboxExCustOrder").prop("checked")) {
        $("#inpExIssuedYM").prop("disabled", false);
        $("#inpExPONoFrom").prop("disabled", false);
        $("#inpExPONoTo").prop("disabled", false);
    }
    else {
        $("#inpExIssuedYM").prop("disabled", true);
        $("#inpExPONoFrom").prop("disabled", true);
        $("#inpExPONoTo").prop("disabled", true);
    }
    $(".selectpicker").selectpicker("refresh");
}));
$("#chkboxExPDSNo").change(() => __awaiter(void 0, void 0, void 0, function* () {
    if ($("#chkboxExPDSNo").prop("checked")) {
        $("#inpExPDSNoFrom").prop("disabled", false);
        $("#inpExPDSNoTo").prop("disabled", false);
        yield ExGetPDS();
    }
    else {
        $("#inpExPDSNoFrom").prop("disabled", true);
        $("#inpExPDSNoTo").prop("disabled", true);
    }
    $(".selectpicker").selectpicker("refresh");
}));
$("#chkboxExSupplier").change(() => __awaiter(void 0, void 0, void 0, function* () {
    if ($("#chkboxExSupplier").prop("checked")) {
        $("#inpExSupplierFrom").prop("disabled", false);
        $("#inpExSupplierTo").prop("disabled", false);
    }
    else {
        $("#inpExSupplierFrom").prop("disabled", true);
        $("#inpExSupplierTo").prop("disabled", true);
    }
    $(".selectpicker").selectpicker("refresh");
}));
$("#chkboxExDelivery").change(() => __awaiter(void 0, void 0, void 0, function* () {
    if ($("#chkboxExDelivery").prop("checked")) {
        $("#inpExDeliveryFrom").prop("disabled", false);
        $("#inpExDeliveryTo").prop("disabled", false);
    }
    else {
        $("#inpExDeliveryFrom").prop("disabled", true);
        $("#inpExDeliveryTo").prop("disabled", true);
    }
}));
$("#inpExIssuedYM").change(() => __awaiter(void 0, void 0, void 0, function* () {
    yield ExGetPO();
}));
$("#btnExExport").click(() => __awaiter(void 0, void 0, void 0, function* () {
    yield ExExportData();
}));
ExGetSupplier = () => __awaiter(void 0, void 0, void 0, function* () {
    _xLib.AJAX_Get("/api/KBNOR280/GetSupplier", null, (success) => __awaiter(void 0, void 0, void 0, function* () {
        $("#inpExSupplierFrom").empty();
        $("#inpExSupplierTo").empty();
        $("#inpExSupplierFrom").append("<option value='' hidden>-- Select Supplier --</option>");
        $("#inpExSupplierTo").append("<option value='' hidden>-- Select Supplier --</option>");
        success.data.forEach((e) => {
            $("#inpExSupplierFrom").append(`<option value='${e.f_Supplier_Code}'>${e.f_Supplier_Code}</option>`);
            $("#inpExSupplierTo").append(`<option value='${e.f_Supplier_Code}'>${e.f_Supplier_Code}</option>`);
        });
        $(".selectpicker").selectpicker("refresh");
        //console.log(success);
    }));
});
ExGetPO = () => __awaiter(void 0, void 0, void 0, function* () {
    let GetQuery = {
        IssuedYM: $("#inpExIssuedYM").val().replace("-", ""),
    };
    _xLib.AJAX_Get("/api/KBNOR280/GetPO", GetQuery, (success) => __awaiter(void 0, void 0, void 0, function* () {
        //console.log(success);
        $("#inpExPONoFrom").empty();
        $("#inpExPONoTo").empty();
        $("#inpExPONoFrom").append("<option value='' hidden>-- Select PO --</option>");
        $("#inpExPONoTo").append("<option value='' hidden>-- Select PO --</option>");
        success.data.forEach((e) => {
            $("#inpExPONoFrom").append(`<option value='${e.f_PO_Customer}'>${e.f_PO_Customer}</option>`);
            $("#inpExPONoTo").append(`<option value='${e.f_PO_Customer}'>${e.f_PO_Customer}</option>`);
        });
        $(".selectpicker").selectpicker("refresh");
    }));
});
ExGetPDS = () => __awaiter(void 0, void 0, void 0, function* () {
    let GetQuery = {
        POFrom: $("#inpExPONoFrom").val(),
        POTo: $("#inpExPONoTo").val(),
    };
    _xLib.AJAX_Get("/api/KBNOR280/GetPDS", GetQuery, (success) => __awaiter(void 0, void 0, void 0, function* () {
        //console.log(success);
        $("#inpExPDSNoFrom").empty();
        $("#inpExPDSNoTo").empty();
        $("#inpExPDSNoFrom").append("<option value='' hidden>-- Select PDS --</option>");
        $("#inpExPDSNoTo").append("<option value='' hidden>-- Select PDS --</option>");
        success.data.forEach((e) => {
            $("#inpExPDSNoFrom").append(`<option value='${e.f_PDS_No}'>${e.f_PDS_No}</option>`);
            $("#inpExPDSNoTo").append(`<option value='${e.f_PDS_No}'>${e.f_PDS_No}</option>`);
        });
        $(".selectpicker").selectpicker("refresh");
    }));
});
ExExportData = () => __awaiter(void 0, void 0, void 0, function* () {
    let GetQuery = {};
    if ($("#chkboxExCustOrder").prop("checked")) {
        if (!($("#inpExIssuedYM").val()) || !($("#inpExPONoFrom").val()) || !($("#inpExPONoTo").val())) {
            return xSwal.error("Error !!!", "Please Select IssuedYM, PONo From And PONo To");
        }
        GetQuery.PONoFrom = $("#inpExPONoFrom").val();
        GetQuery.PONoTo = $("#inpExPONoTo").val();
    }
    if ($("#chkboxExPDSNo").prop("checked")) {
        if (!($("#inpExPDSNoFrom").val()) || !($("#inpExPDSNoTo").val())) {
            return xSwal.error("Error !!!", "Please Select PDSNo From And PDSNo To");
        }
        GetQuery.PDSNoFrom = $("#inpExPDSNoFrom").val();
        GetQuery.PDSNoTo = $("#inpExPDSNoTo").val();
    }
    if ($("#chkboxExSupplier").prop("checked")) {
        if (!($("#inpExSupplierFrom").val()) || !($("#inpExSupplierTo").val())) {
            return xSwal.error("Error !!!", "Please Select Supplier From And Supplier To");
        }
        GetQuery.SupplierFrom = $("#inpExSupplierFrom").val();
        GetQuery.SupplierTo = $("#inpExSupplierTo").val();
    }
    if ($("#chkboxExDelivery").prop("checked")) {
        if (!($("#inpExDeliveryFrom").val()) || !($("#inpExDeliveryTo").val())) {
            return xSwal.error("Error !!!", "Please Select Delivery From And Delivery To");
        }
        GetQuery.DeliveryFrom = moment($("#inpExDeliveryFrom").val()).format("YYYYMMDD");
        GetQuery.DeliveryTo = moment($("#inpExDeliveryTo").val()).format("YYYYMMDD");
    }
    _xLib.AJAX_Get("/api/KBNOR280/ExportData", GetQuery, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        console.log(success);
        yield ExXLSXjsonToCSV(success.data);
    }));
});
ExXLSXjsonToCSV = (data) => __awaiter(void 0, void 0, void 0, function* () {
    data.forEach((e) => {
        var _a;
        e["PDS No"] = e["F_OrderNo"];
        e["Supplier"] = e["F_Supplier_Code"];
        e["Plant"] = e["F_Supplier_Plant"];
        e["Short Name"] = e["F_short_name"];
        e["Name"] = e["F_name"];
        e["IssueDate"] = e["F_Issued_Date"];
        e["DeliveryDate"] = e["F_Delivery_Date"];
        e["Trip"] = e["F_Delivery_Trip"];
        e["Str"] = "";
        e["Flag"] = "";
        e["Cycle"] = e["F_Delivery_Cycle"];
        e["No."] = e["F_No"];
        e["Part No"] = e["F_Part_No"];
        e["Part Name"] = e["F_Part_Name"];
        e["Q'Ty/Pack"] = e["F_Box_Qty"];
        e["Q'Ty Order Total"] = e["F_Unit_Amount"];
        e["Price"] = 0;
        e["%"] = e["F_Vat"];
        e["P/O No."] = e["F_PO_Customer"];
        e["Type Order"] = e["F_OrderType"];
        e["Remark1"] = e["F_Remark"];
        e["Remark2"] = e["F_Remark2"];
        e["Remark3"] = e["F_Remark3"];
        e["Sebango"] = "";
        e["Location"] = e["F_Plant"];
        e["Attn"] = "";
        e["KanbanID"] = e["F_Kanban_No"];
        e["Store No"] = e["F_Delivery_Dock"];
        e["DeliveryTime"] = e["F_Delivery_Time"];
        e["Kanban Remark"] = e["F_Remark_KB"];
        e["Dockcode"] = e["F_Dock_Code"];
        e["Collect Date"] = e["F_Collect_Date"];
        e["Collect Time"] = e["F_Collect_Time"];
        e["Delivery By"] = e["F_Transportor"];
        e["Approval"] = e["F_Approver"];
        e["Address"] = e["F_Address"];
        e["Type Version"] = e["F_Type_Version"];
        e["PDS Backup"] = (_a = e["F_OrderNO_Old"]) !== null && _a !== void 0 ? _a : "";
        e["Dept Code"] = e["F_Dept"];
        e["DAcctNo"] = e["F_DR"];
        e["WKCode"] = e["F_WK_Code"];
        e["Sparetext1"] = "";
        e["Sparetext2"] = "";
        e["SpareNum1"] = "";
        e["SpareNum2"] = "";
        Object.keys(e).forEach((key) => {
            if (key.includes("F_")) {
                delete e[key];
            }
        });
    });
    //console.log(data);
    const worksheet = XLSX.utils.json_to_sheet(data);
    const csv = XLSX.utils.sheet_to_csv(worksheet);
    let newBlob = new Blob([csv], { type: "text/csv" });
    let url = window.URL.createObjectURL(newBlob);
    let a = document.createElement("a");
    a.href = url;
    a.download = "KBNOR280_" + moment().format("DDMMYYYY_HHmmss") + ".csv";
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);
});
