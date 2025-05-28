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
var isNew = false;
$(document).ready(function () {
    return __awaiter(this, void 0, void 0, function* () {
        yield _xDataTable.InitialDataTable("#tableMaster", {
            columns: [
                {
                    title: "Order Clear Module", data: "F_Flg_ClearModule",
                    render: function (data, type, row) {
                        if (data == true) {
                            return "<input type='checkbox' class='chkBoxDT' checked>";
                        }
                        else {
                            return "<input type='checkbox' class='chkBoxDT'>";
                        }
                    }
                },
                { title: "Supplier Code", data: "F_Supplier_Code" },
                { title: "Cycle", data: "F_Cycle" },
                { title: "Kanban No", data: "F_Kanban_No" },
                { title: "Store Code", data: "F_Store_Code" },
                { title: "Part No.", data: "F_Part_No" },
                { title: "Start Date", data: "F_Start_Date" },
                { title: "End Date", data: "F_End_Date" },
                { title: "Type Order", data: "F_Type_Order" },
                //{ title: "Supplier Name", data: "F_name" },
                //{ title: "Part Name", data: "F_Part_nm" },
            ],
            order: [[1, "asc"]],
            scrollY: "250px",
            scrollCollapse: false,
        });
        $("#Start_Date").initDatepicker();
        $("#End_Date").initDatepicker("31/12/2999");
        GetDataList();
        xSplash.hide();
    });
});
$("#btnInq").on("click", function () {
    $("#Supplier_Code").prop("disabled", false);
    $("#Store_Code").prop("disabled", false);
    $("#Kanban_No").prop("disabled", false);
    $("#Part_No").prop("disabled", false);
    $("#divRowData").find("input").val("");
    $("#Start_Date").val(moment().format("DD/MM/YYYY"));
    $("#End_Date").val("31/12/2999");
    isNew = false;
    $("#btnSave").prop("disabled", true);
    $("#btnCancel").prop("disabled", false);
    $("#btnInq").prop("disabled", false);
    $("#btnNew").prop("disabled", false);
    $("#btnUpd").prop("disabled", false);
    $("#btnDel").prop("disabled", false);
});
$("#btnNew").on("click", function () {
    $("#Supplier_Code").prop("disabled", false);
    $("#Store_Code").prop("disabled", false);
    $("#Kanban_No").prop("disabled", false);
    $("#Part_No").prop("disabled", false);
    $("#Type_Order").prop("disabled", false);
    $("#Start_Date").prop("disabled", false);
    $("#End_Date").prop("disabled", false);
    $("#divRowData").find("input").val("");
    $("#Start_Date").val(moment().format("DD/MM/YYYY"));
    $("#End_Date").val("31/12/2999");
    isNew = true;
    $("#btnSave").prop("disabled", false);
    $("#btnCancel").prop("disabled", false);
    $("#btnInq").prop("disabled", true);
    $("#btnNew").prop("disabled", true);
    $("#btnUpd").prop("disabled", true);
    $("#btnDel").prop("disabled", true);
});
$("#btnUpd").on("click", function () {
    $("#Supplier_Code").prop("disabled", false);
    $("#Store_Code").prop("disabled", false);
    $("#Kanban_No").prop("disabled", false);
    $("#Part_No").prop("disabled", false);
    $("#Type_Order").prop("disabled", true);
    $("#End_Date").prop("disabled", false);
    $("#divRowData").find("input").val("");
    $("#Start_Date").val(moment().format("DD/MM/YYYY"));
    $("#End_Date").val("31/12/2999");
    isNew = true;
    $("#btnSave").prop("disabled", false);
    $("#btnCancel").prop("disabled", false);
    $("#btnInq").prop("disabled", true);
    $("#btnNew").prop("disabled", true);
    $("#btnUpd").prop("disabled", true);
    $("#btnDel").prop("disabled", true);
});
$("#btnEdit").on("click", function () {
    $("#Supplier_Code").prop("disabled", false);
    $("#Store_Code").prop("disabled", false);
    $("#Kanban_No").prop("disabled", false);
    $("#Part_No").prop("disabled", false);
    isNew = false;
    $("#btnSave").prop("disabled", false);
    $("#btnCancel").prop("disabled", false);
    $("#btnInq").prop("disabled", true);
    $("#btnNew").prop("disabled", true);
    $("#btnUpd").prop("disabled", true);
    $("#btnDel").prop("disabled", true);
});
$("#btnCancel").on("click", function () {
    $("#Supplier_Code").prop("disabled", true);
    $("#Store_Code").prop("disabled", true);
    $("#Kanban_No").prop("disabled", true);
    $("#Part_No").prop("disabled", true);
    $("#divRowData").find("input").val("");
    $("#Start_Date").val(moment().format("DD/MM/YYYY"));
    $("#End_Date").val("31/12/2999");
    $("#btnSave").prop("disabled", true);
    $("#btnCancel").prop("disabled", true);
    $("#btnInq").prop("disabled", false);
    $("#btnNew").prop("disabled", false);
    $("#btnDel").prop("disabled", false);
    $("#btnUpd").prop("disabled", false);
});
$(document).on("focus", "input[list]", function () {
    $(this).val("");
});
$(document).on("change", ".chkBoxDT", function () {
    let row = $(this).closest("tr");
    let data = $("#tableMaster").DataTable().row(row).data();
    let checked = $(this).is(":checked");
    //console.log(checked);
    if (checked) {
        data.F_Flg_ClearModule = true;
    }
    else {
        data.F_Flg_ClearModule = false;
    }
    $("#tableMaster").DataTable().row(row).data(data).draw(false);
});
$("#Supplier_Code").on("change", function () {
    GetSupplier();
    GetDataList();
    GetList();
});
$("#Store_Code").on("change", function () {
    GetDataList();
    $("#readStore").val($("#Store_Code").val());
    GetList();
});
$("#Kanban_No").on("change", function () {
    GetDataList();
    $("#readKanban").val($("#Kanban_No").val());
    GetList();
});
$("#Part_No").on("change", function () {
    GetPartNo();
    GetDataList();
    $("#readPartNo").val($("#Part_No").val());
    GetList();
});
$("#btnSave").on("click", function () {
    return __awaiter(this, void 0, void 0, function* () {
        if (yield xSwal.confirm("Do you want to save data?")) {
            yield UpdateFlgClearModule();
        }
    });
});
$("#btnReport").click(function () {
    let obj = {
        Sup: $("#Supplier_Code").val(),
        KB: $("#Kanban_No").val(),
        Store: $("#Store_Code").val(),
        Part: $("#Part_No").val(),
        Plant: _xLib.GetCookie("plantCode"),
        UserName: _xLib.GetUserName(),
    };
    _xLib.OpenReportObj("/KBNOR361", obj);
});
function GetDataList() {
    return __awaiter(this, void 0, void 0, function* () {
        let data = yield $("#formQueryData").formToJSON();
        data.isNew = isNew;
        //console.log(data);
        _xLib.AJAX_Get("/api/KBNOR361/GetDataList", data, function (success) {
            //console.log(success);
            if ($("#Supplier_Code").val() == "") {
                $("#DataSupList").addOptionDataList(success.data[0]);
            }
            if ($("#Kanban_No").val() == "") {
                $("#DataKBList").addOptionDataList(success.data[1]);
            }
            if ($("#Store_Code").val() == "") {
                $("#DataStoreList").addOptionDataList(success.data[2]);
            }
            if ($("#Part_No").val() == "") {
                $("#DataPartList").addOptionDataList(success.data[3]);
            }
        }, function (error) {
            xSwal.xError(error);
        });
    });
}
function GetSupplier() {
    return __awaiter(this, void 0, void 0, function* () {
        let data = yield $("#formQueryData").formToJSON();
        //console.log(data);
        _xLib.AJAX_Get("/api/KBNOR361/GetSupplier", data, function (success) {
            //console.log(success);
            $("#readSupName").val(success.data.f_name.trim());
            let cycle = "";
            cycle += success.data.f_Cycle_A.length == 1 ? "0" + success.data.f_Cycle_A + "-" : success.data.f_Cycle_A + "-";
            cycle += success.data.f_Cycle_B.length == 1 ? "0" + success.data.f_Cycle_B + "-" : success.data.f_Cycle_B + "-";
            cycle += success.data.f_Cycle_C.length == 1 ? "0" + success.data.f_Cycle_C : success.data.f_Cycle_C;
            $("#readCycle").val(cycle.trim());
        }, function (error) {
            xSwal.xError(error);
        });
    });
}
function GetPartNo() {
    return __awaiter(this, void 0, void 0, function* () {
        let data = yield $("#formQueryData").formToJSON();
        //console.log(data);
        _xLib.AJAX_Get("/api/KBNOR361/GetPartNo", data, function (success) {
            //console.log(success);
            $("#readPartName").val(success.data.f_Part_nm.trim());
        }, function (error) {
            xSwal.xError(error);
        });
    });
}
function GetList() {
    return __awaiter(this, void 0, void 0, function* () {
        let data = yield $("#formQueryData").formToJSON();
        //console.log(data);
        _xLib.AJAX_Get("/api/KBNOR361/GetList", data, function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            _xDataTable.ClearAndAddDataDT("#tableMaster", success.data);
        }, function (error) {
            xSwal.xError(error);
        });
    });
}
function UpdateFlgClearModule() {
    return __awaiter(this, void 0, void 0, function* () {
        let data = $("#tableMaster").DataTable().rows().data().toArray();
        console.log(data);
        _xLib.AJAX_Post("/api/KBNOR361/UpdateFlgClearModule", data, function (success) {
            //console.log(success);
            xSwal.xSuccess(success.message);
            GetList();
        }, function (error) {
            xSwal.xError(error);
        });
    });
}
