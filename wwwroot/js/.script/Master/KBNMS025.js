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
        _xDataTable.InitialDataTable("#tableMain", {
            "processing": false,
            "serverSide": false,
            width: '100%',
            paging: false,
            sorting: false,
            searching: false,
            scrollX: false,
            scrollY: "200px",
            scrollCollapse: true,
            "columns": [
                {
                    title: "Flag", render(data, type, row) {
                        return `<input type="checkbox" class="chkbox" id="chkbox" name="chkbox" value="${row.f_Supplier_Plant}">`;
                    }
                },
                {
                    title: "Logistic Supplier", data: "F_Logistic"
                },
                {
                    title: "Start Date", data: "F_Start_Date"
                },
                {
                    title: "End Date", data: "F_End_Date"
                },
                {
                    title: "Truck Type", data: "F_Truck_Type"
                },
                {
                    title: "Weight", data: "F_Weight"
                },
            ],
            select: false,
            order: [[0, "asc"]]
        });
        $("table tbody tr td").addClass("text-center");
        $("table thead tr th").addClass("text-center");
        $("#inpStartDate").initDatepicker();
        $("#inpEndDate").initDatepicker();
        xSplash.hide();
    });
});
$("#btnSelAll").click(function () {
    $(".chkbox").prop("checked", true);
});
$("#btnDeselAll").click(function () {
    $(".chkbox").prop("checked", false);
});
$("#divBtn").on("click", "button", function () {
    $("#divBtn").find("button").prop("disabled", true);
    $(this).prop("disabled", false);
    if ($(this).attr("id") == "btnNew") {
        $("#inpEndDate").val("31/12/2999");
        $("#inpStartDate").val(moment().format("DD/MM/YYYY"));
        if ($("#inpSupplierCode").is("select")) {
            $("#inpSupplierCode").parent().parent().append("<input type='text' id='inpSupplierCode' class='form-control col-7' placeholder='Logistic Supplier'>");
            $("#inpSupplierCode").parent().remove();
        }
        $("#inpStartDate").prop("disabled", false);
        $("#inpWeight").prop("disabled", false);
        $("#inpEndDate").prop("disabled", false);
    }
    else if ($(this).attr("id") == "btnUpd") {
        $("#inpWeight").prop("disabled", false);
        $("#inpEndDate").prop("disabled", false);
    }
    else {
        $("#inpStartDate").prop("disabled", true);
        $("#inpWeight").prop("disabled", true);
        $("#inpEndDate").prop("disabled", true);
    }
    $(document).find("select[disabled]").each(function () {
        $(this).prop("disabled", false);
    });
    $(document).find("select").each(function () {
        $(this).resetSelectPicker();
    });
    GetLogisticSupplier();
    GetTruckType();
});
$("#btnCan").click(function () {
    $("#divBtn").find("button").prop("disabled", false);
    $("#tableMain").DataTable().clear().draw();
    $(document).find("input").each(function () {
        $(this).attr("type") == "number" ? $(this).val(0) : $(this).val("");
    });
    $("#inpWeight").prop("disabled", true);
    $("#inpStartDate").prop("disabled", true);
    $("#inpEndDate").prop("disabled", true);
    $(document).find("select").each(function () {
        $(this).val("");
        $(this).prop("disabled", true);
    });
    $(document).find("select").each(function () {
        $(this).val("");
        $(this).resetSelectPicker();
    });
    if ($("#inpSupplierCode").is("input")) {
        $("#inpSupplierCode").parent().append(`
        <select id="inpSupplierCode" class="selectpicker form-control col-7 p-0" data-live-search="true" data-size="8" data-width="100%" disabled>
            <option value="" hidden></option>
        </select>`);
        $("#inpSupplierCode").parent().find("input").remove();
        $("#inpSupplierCode").resetSelectPicker();
    }
});
$(document).on("change", "#inpSupplierCode", function () {
    GetTruckType();
    GetListData();
});
$("#inpTruckType").on("change", function () {
    GetListData();
    TruckTypeSelected();
});
$("#btnSave").click(function () {
    Save();
});
function GetLogisticSupplier() {
    return __awaiter(this, void 0, void 0, function* () {
        let obj = {
            TruckType: $("#inpTruckType").val(),
        };
        _xLib.AJAX_Get("/api/KBNMS025/GetLogisticSupplier", obj, function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            console.log(success);
            $("#inpSupplierCode").addListSelectPicker(success.data, "f_Logistic");
        }, function (error) {
        });
    });
}
function GetTruckType() {
    return __awaiter(this, void 0, void 0, function* () {
        let obj = {
            Logistic: $("#inpSupplierCode").val(),
            isNew: !$("#btnNew").prop("disabled"),
        };
        _xLib.AJAX_Get("/api/KBNMS025/GetTruckType", obj, function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#inpTruckType").addListSelectPicker(success.data, "F_Truck_Type");
        }, function (error) {
        });
    });
}
function TruckTypeSelected() {
    return __awaiter(this, void 0, void 0, function* () {
        let obj = {
            TruckType: $("#inpTruckType").val(),
            isNew: !$("#btnNew").prop("disabled"),
            Logistic: $("#inpSupplierCode").val(),
        };
        _xLib.AJAX_Get("/api/KBNMS025/TruckTypeSelected", obj, function (success) {
            success = _xLib.JSONparseMixData(success);
            console.log(success);
            $("#inpWeight").val(success.data[0].F_Weight);
            $("#inpWidth").val(success.data[0].F_Width);
            $("#inpHeight").val(success.data[0].F_High);
            $("#inpLong").val(success.data[0].F_Long);
            $("#inpStartDate").val(success.data[0].F_Start_Date);
            $("#inpEndDate").val(success.data[0].F_End_Date);
        }, function (error) {
        });
    });
}
function GetListData() {
    return __awaiter(this, void 0, void 0, function* () {
        let obj = {
            Logistic: $("#inpSupplierCode").val(),
            TruckType: $("#inpTruckType").val(),
        };
        _xLib.AJAX_Get("/api/KBNMS025/GetListData", obj, function (success) {
            success = _xLib.JSONparseMixData(success);
            //console.table(success.data);
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        }, function (error) {
        });
    });
}
$(document).on("click", "#tableMain tbody tr", function () {
    $(this).closest("tr").toggleClass("selected");
    let data = $("#tableMain").DataTable().row(".selected").data();
    console.log(data);
    $("#inpSupplierCode").val(data.F_Logistic);
    $("#inpTruckType").val(data.F_Truck_Type);
    $("#inpWeight").val(data.F_Weight);
    $("#inpWidth").val(data.F_Width);
    $("#inpHeight").val(data.F_High);
    $("#inpLong").val(data.F_Long);
    $("#inpStartDate").val(data.F_Start_Date);
    $("#inpEndDate").val(data.F_End_Date);
    $("#inpM3").val(data.F_M3);
    $("#inpSupplierCode").selectpicker("refresh");
    $("#inpTruckType").selectpicker("refresh");
});
function Save() {
    return __awaiter(this, void 0, void 0, function* () {
        let listObj = [];
        var obj = {
            f_Logistic: $("#inpSupplierCode").val(),
            f_Truck_Type: $("#inpTruckType").val(),
            f_Weight: $("#inpWeight").val(),
            f_Width: $("#inpWidth").val(),
            f_High: $("#inpHeight").val(),
            f_Long: $("#inpLong").val(),
            f_Start_Date: moment($("#inpStartDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
            f_End_Date: moment($("#inpEndDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
            f_M3: $("#inpM3").val() == "" ? 0 : $("#inpM3").val(),
        };
        listObj.push(obj);
        if ($("#divBtn").find("button:not([disabled])").attr("id") == "btnDel") {
            listObj = _xDataTable.GetSelectedDataDT("#tableMain");
            if (listObj.length == 0) {
                xSwal.error("Please select data.");
                return;
            }
            listObj.forEach(function (item) {
                item.f_Start_Date = moment(item.F_Start_Date, "DD/MM/YYYY").format("YYYYMMDD");
                item.f_End_Date = moment(item.F_End_Date, "DD/MM/YYYY").format("YYYYMMDD");
            });
        }
        let actionQ = $("#divBtn").find("button:not([disabled])").attr("id").split("btn")[1];
        _xLib.AJAX_Post("/api/KBNMS025/Save?action=" + actionQ, listObj, function (success) {
            xSwal.success(success.response, success.message);
            $("#btnCan").trigger("click");
        }, function (error) {
        });
    });
}
