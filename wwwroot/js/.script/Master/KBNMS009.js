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
        _xDataTable.InitialDataTable("tableSup", {
            "processing": false,
            "serverSide": false,
            width: '100%',
            paging: false,
            sorting: false,
            searching: false,
            scrollX: false,
            scrollY: "250px",
            scrollCollapse: true,
            "columns": [
                {
                    title: "Supplier Code", data: "f_supplier_cd"
                },
            ],
            select: true,
            order: [[0, "asc"]]
        });
        _xDataTable.InitialDataTable("tableMain", {
            "processing": false,
            "serverSide": false,
            width: '100%',
            paging: false,
            sorting: false,
            searching: false,
            scrollX: false,
            scrollY: true,
            scrollCollapse: true,
            "columns": [
                {
                    title: "Supplier Code", data: "f_Supplier_Code"
                },
                {
                    title: "Supplier Plant", data: "f_Supplier_Plant"
                },
                {
                    title: "Kanban No", data: "f_Kanban_No"
                },
                {
                    title: "Dock", data: "f_Supply_Code"
                },
                {
                    title: "Number", data: "f_Number"
                },
            ],
            select: false,
            order: [[0, "asc"]]
        });
        $("table tbody tr td , table thead tr th").addClass("text-center");
        yield GetSupplier();
        xSplash.hide();
    });
});
function GetSupplier() {
    return __awaiter(this, void 0, void 0, function* () {
        xSplash.show();
        yield _xLib.AJAX_Get("/api/KBNMS009/GetSupplier", null, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                yield _xDataTable.ClearAndAddDataDT("tableSup", success);
            });
        });
        xSplash.hide();
    });
}
$(document).on("click", "#tableSup tbody tr", function () {
    return __awaiter(this, void 0, void 0, function* () {
        xSplash.show();
        var data = _xDataTable.GetDataDT("tableSup", this);
        yield _xLib.AJAX_Get("/api/KBNMS009/SupplierClicked", { Supplier: data.f_supplier_cd }, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                yield _xDataTable.ClearAndAddDataDT("tableMain", success);
            });
        });
        xSplash.hide();
    });
});
$(document).on("click", "#tableMain tbody tr td", function () {
    return __awaiter(this, void 0, void 0, function* () {
        console.log($(this).index());
        if ($(this).index() == 4) {
            var value = $(this).text();
            console.log(value);
            $(this).empty();
            $(this).append("<input type='text' class='form-control' value='" + value + "' />");
            $(this).find("input").focus();
        }
    });
});
$(document).on("focusout", "#tableMain tbody tr td", function () {
    return __awaiter(this, void 0, void 0, function* () {
        var value = $(this).find("input").val();
        console.log(value);
        $("#tableMain").DataTable().cell(this).data(value).draw();
        var data = _xDataTable.GetDataDT("tableMain", this);
        console.log(data);
    });
});
$("#btnSave").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        yield Save();
    });
});
$("#btnCancel").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        yield GetSupplier();
        $("#tableMain").DataTable().clear().draw();
    });
});
$("#btnPrint").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        let obj = $("#tableMain").DataTable().rows().data().toArray()[0];
        let reportObj = {
            F_Update_By: obj.f_Update_By,
        };
        if (obj == null) {
            xSwal.error("Warning", "No data to print");
            return;
        }
        _xLib.OpenReportObj("/KBNMS009", reportObj);
    });
});
function Save() {
    return __awaiter(this, void 0, void 0, function* () {
        xSplash.show();
        var data = $("#tableMain").DataTable().rows().data().toArray();
        yield _xLib.AJAX_Post("/api/KBNMS009/Save", data, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                xSwal.success("Success", "Data has been saved successfully");
                xSplash.hide();
            });
        });
    });
}
