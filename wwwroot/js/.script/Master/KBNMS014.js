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
                    title: "Print", data: "f_Flag_Print"
                },
                {
                    title: "Supplier CD", data: "f_Supplier_Code"
                },
                {
                    title: "Supplier Plant", data: "f_Supplier_Plant"
                },
                {
                    title: "Short Name", data: "f_Short_Name"
                },
            ],
            select: false,
            order: [[0, "asc"]]
        });
        $("table tbody tr td").addClass("text-center");
        $("table thead tr th").addClass("text-center");
        xSplash.hide();
    });
});
$("#divBtn button").on("click", function () {
    return __awaiter(this, void 0, void 0, function* () {
        $("#divBtn").find("button").each(function () {
            $(this).prop("disabled", true);
        });
        $(this).prop("disabled", false);
        $(document).find("input:disabled , select:disabled").each(function () {
            $(this).prop("disabled", false);
            $(this).selectpicker("refresh");
        });
        yield GetSupplierCode();
    });
});
$("#btnCan").on("click", function () {
    $("#divBtn").find("button").each(function () {
        $(this).prop("disabled", false);
    });
    $(document).find("input:enabled , select:enabled").each(function () {
        $(this).prop("disabled", true);
        $(this).resetSelectPicker("refresh");
    });
    $("#inpShortName").val("");
    $("#tableMain").DataTable().clear().draw();
    $("#tableMain").DataTable().columns.adjust().draw();
});
function GetSupplierCode() {
    return __awaiter(this, void 0, void 0, function* () {
        let getBody = {
            isNew: !$("#btnNew").prop("disabled")
        };
        _xLib.AJAX_Get("/api/KBNMS014/GetSupplierCode", getBody, function (success) {
            success = _xLib.JSONparseMixData(success);
            $("#inpSupplierCode").addListSelectPicker(success.data, "F_Supplier_Code");
        }, function (error) {
            console.log(error);
        });
    });
}
function GetSupplierPlant() {
    return __awaiter(this, void 0, void 0, function* () {
        let getBody = {
            isNew: !$("#btnNew").prop("disabled"),
            SupplierCode: $("#inpSupplierCode").val()
        };
        _xLib.AJAX_Get("/api/KBNMS014/GetSupplierPlant", getBody, function (success) {
            success = _xLib.JSONparseMixData(success);
            $("#inpSupplierPlant").addListSelectPicker(success.data, "F_Supplier_Plant");
        }, function (error) {
            console.log(error);
        });
    });
}
function GetShortName() {
    return __awaiter(this, void 0, void 0, function* () {
        let getBody = {
            SupplierCode: $("#inpSupplierCode").val(),
            SupplierPlant: $("#inpSupplierPlant").val(),
            isNew: !$("#btnNew").prop("disabled")
        };
        //console.log(getBody);
        _xLib.AJAX_Get("/api/KBNMS014/GetShortName", getBody, function (success) {
            success = _xLib.JSONparseMixData(success);
            //console.log(success);
            $("#inpSupplierName").val(success.data.F_Short_Name);
        }, function (error) {
            console.log(error);
        });
    });
}
function GetListData() {
    return __awaiter(this, void 0, void 0, function* () {
        let getData = {
            SupplierCode: $("#inpSupplierCode").val(),
            SupplierPlant: $("#inpSupplierPlant").val()
        };
        _xLib.AJAX_Get("/api/KBNMS014/GetListData", getData, function (success) {
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        }, function (error) {
            console.log(error);
        });
    });
}
$("#inpSupplierCode").change(function () {
    GetSupplierPlant();
    GetListData();
});
$("#inpSupplierPlant").change(function () {
    return __awaiter(this, void 0, void 0, function* () {
        yield GetShortName();
        GetListData();
    });
});
$("#btnSave").on("click", function () {
    Save();
});
function Save() {
    return __awaiter(this, void 0, void 0, function* () {
        let listObj = [];
        let obj = {
            SupplierCode: $("#inpSupplierCode").val(),
            SupplierPlant: $("#inpSupplierPlant").val(),
            SupplierName: $("#inpSupplierName").val(),
            FlagPrint: $("#inpPrint").val()
        };
        listObj.push(obj);
        if (!$("#btnDel").prop("disabled")) {
            listObj = _xDataTable.GetSelectedDataDT("#tableMain");
            listObj.forEach(function (item) {
                item.SupplierCode = item.f_Supplier_Code;
                item.SupplierPlant = item.f_Supplier_Plant;
                item.SupplierName = item.f_Short_Name;
                item.FlagPrint = item.f_Flag_Print;
            });
        }
        let action = $("#divBtn").find("button:not(:disabled)").attr("id").split("btn")[1];
        _xLib.AJAX_Post("/api/KBNMS014/Save?action=" + action, listObj, function (success) {
            GetListData();
            xSwal.success(success.response, success.message);
        }, function (error) {
            xSwal.error(error.responseJSON.response, error.responseJSON.message);
            console.log(error);
        });
    });
}
$("#btnRpt").click(function () {
    //console.log(ajexHeader);
    //console.log($("#Name").val());
    let reportParam = {
        Plant: ajexHeader.Plant,
        UserName: _xLib.GetUserName(),
        F_Supplier_Code: $("#inpSupplierCode").val(),
        F_Supplier_Plant: $("#inpSupplierPlant").val()
    };
    _xLib.OpenReportObj("/KBNMS014", reportParam);
});
