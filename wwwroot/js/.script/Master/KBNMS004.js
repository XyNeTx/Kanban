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
            scrollX: false,
            scrollY: true,
            scrollCollapse: true,
            "columns": [
                {
                    title: "Flag", render: function (data, type, row) {
                        return "<input type='checkbox' name='chkFlag' value='" + row.F_Supplier_Code + "-" + row.F_Supplier_Plant + "'>";
                    }
                },
                { title: "Supplier Code", data: "F_Supplier_Code", },
                { title: "Cycle", data: "F_Cycle", },
                { title: "Kanban No", data: "F_Kanban_No", },
                { title: "Store Code", data: "F_Store_Code", },
                { title: "Part No", data: "F_Part_No" },
                { title: "Start Date", data: "F_Start_Date" },
                { title: "End Date", data: "F_End_Date", },
                { title: "Type Order", data: "F_Type_Special", },
            ],
            select: false,
            order: [[0, "asc"]]
        });
        yield $("#inpStartDate").datepicker({
            format: 'dd/mm/yyyy',
            autoclose: true,
            uilibrary: 'bootstrap5',
            showRightIcon: false,
            value: moment().format("DD/MM/YYYY")
        });
        yield $("#inpEndDate").datepicker({
            format: 'dd/mm/yyyy',
            autoclose: true,
            uilibrary: 'bootstrap5',
            showRightIcon: false,
            value: "31/12/2999"
        });
        //await GetSelectList();
        $("table thead tr th , table tbody tr td").addClass("text-center");
        $("#divDetail").find("select").prop("disabled", true);
        $("#divDetail").find("input[type='text']").prop("readonly", true);
        xSplash.hide();
    });
});
$("#divButton").find("button").on("click", function () {
    return __awaiter(this, void 0, void 0, function* () {
        //await Inquiry();
        $("#tableMain").DataTable().clear().draw();
        //$(".selectpicker").each(function () {
        //    //$(this).prop("disabled", false);
        //    $(this).selectpicker("refresh");
        //});
        $("#inpStartDate").prop("readonly", false);
        $("#inpEndDate").prop("readonly", false);
        yield $("#divButton").find("button").prop("disabled", true);
        $(this).prop("disabled", false);
        yield GetSelectList();
        $(".selectpicker").each(function () {
            $(this).prop("disabled", false);
            $(this).resetSelectPicker();
        });
    });
});
function GetSelectList() {
    return __awaiter(this, void 0, void 0, function* () {
        xSplash.show();
        var obj = {
            kanban: $("#inpKanban").val(),
            supplier: $("#inpSupplierCode").val(),
            partno: $("#inpPartNo").val(),
            storecd: $("#inpStore").val(),
            isNew: $("#divButton").find("button:not([disabled])").attr("id") == "btnNew" ? true : false,
        };
        _xLib.AJAX_Get("/api/KBNMS004/GetSelectList", obj, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                success = yield _xLib.JSONparseMixData(success);
                //console.log(success);
                //let SupplierSet = [... new Set(data.data.map(x => x.F_supplier_cd + "-" + x.F_plant))];
                //SupplierSet = SupplierSet.sort();
                //let PartNoSet = [... new Set(data.data.map(x => x.F_Part_no + "-" + x.F_Ruibetsu))];
                //PartNoSet = PartNoSet.sort();
                //let StoreSet = [... new Set(data.data.map(x => x.F_Store_cd))];
                //StoreSet = StoreSet.sort();
                //let KanbanNoSet = [... new Set(data.data.map(x => "0" + x.F_Sebango))];
                //KanbanNoSet = KanbanNoSet.sort();
                //SupplierSet = SupplierSet.map(x => {
                //    return { Supplier: x }
                //});
                //PartNoSet = PartNoSet.map(x => {
                //    return { PartNo : x }
                //});
                //StoreSet = StoreSet.map(x => {
                //    return { Store : x }
                //});
                //KanbanNoSet = KanbanNoSet.filter(x => x != "0").map(x => {
                //    return { Kanban: x }
                //});
                //if ($("#inpSupplierCode").val() == "") {
                //    await $("#inpSupplierCode").addListSelectPicker(SupplierSet, "Supplier");
                //}
                //if ($("#inpPartNo").val() == "") {
                //    await $("#inpPartNo").addListSelectPicker(PartNoSet, "PartNo");
                //}
                //if ($("#inpStore").val() == "") {
                //    await $("#inpStore").addListSelectPicker(StoreSet, "Store");
                //}
                //if ($("#inpKanban").val() == "") {
                //    await $("#inpKanban").addListSelectPicker(KanbanNoSet, "Kanban");
                //}
                if ($("#inpSupplierCode").val() == "") {
                    yield $("#inpSupplierCode").addListSelectPicker(success.data[0], "F_Supplier");
                }
                if ($("#inpPartNo").val() == "") {
                    yield $("#inpPartNo").addListSelectPicker(success.data[1], "F_PartNo");
                }
                if ($("#inpStore").val() == "") {
                    yield $("#inpStore").addListSelectPicker(success.data[2], "F_StoreCD");
                }
                if ($("#inpKanban").val() == "") {
                    yield $("#inpKanban").addListSelectPicker(success.data[3], "F_KanbanNo");
                }
                return xSplash.hide();
            });
        });
    });
}
$(".selectpicker").on("change", function () {
    return __awaiter(this, void 0, void 0, function* () {
        yield GetListData();
        yield GetSelectList();
        if ($(this).attr("id") == "inpSupplierCode") {
            yield SelectedSupplier();
        }
        else if ($(this).attr("id") == "inpPartNo") {
            yield SelectedPartNo();
        }
        $(`#${$(this).attr("id")}Read`).val($(this).val());
    });
});
$("#btnSave").on("click", function () {
    return __awaiter(this, void 0, void 0, function* () {
        yield Save();
    });
});
function SelectedSupplier() {
    return __awaiter(this, void 0, void 0, function* () {
        let obj = yield ObjGet();
        _xLib.AJAX_Get("/api/KBNMS004/SelectedSupplier", obj, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                //console.log(data);
                //console.log(success.data.f_Cycle_A.length);
                $("#inpSupplierName").val(success.data.f_name.trim());
                let cycle = (success.data.f_Cycle_A.length == 1) ? "0" + success.data.f_Cycle_A : success.data.f_Cycle_A;
                cycle += "-" + success.data.f_Cycle_B;
                cycle += "-" + success.data.f_Cycle_C;
                $("#inpCycle").val(cycle);
            });
        });
    });
}
function SelectedPartNo() {
    return __awaiter(this, void 0, void 0, function* () {
        let obj = yield ObjGet();
        _xLib.AJAX_Get("/api/KBNMS004/SelectedPartNo", obj, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                $("#inpPartNameRead").val(success.data.f_Part_nm.trim());
            });
        });
    });
}
$("#btnCancel").on("click", function () {
    return __awaiter(this, void 0, void 0, function* () {
        $("#divDetail").find("select").prop("disabled", true);
        $("#divDetail").find("input[type='text']").prop("readonly", true);
        $("#divButton").find("button").prop("disabled", false);
        $(".selectpicker").each(function () {
            $(this).selectpicker("refresh");
            $(this).resetSelectPicker();
        });
        $("#divDetail").find("input[type='text']:not([class*='datepicker'])").val("");
        $("#inpStartDate").val(moment().format("DD/MM/YYYY"));
        $("#inpEndDate").val("31/12/2999");
    });
});
$(document).on("click", "#tableMain tbody tr td", function () {
    var row = $(this).closest("tr");
    //console.log(row);
    //row.find("input[type='checkbox']").prop("checked", true);
    var data = $("#tableMain").DataTable().row(row).data();
    //console.log(data);
    $("#inpSupplierCode").val(data.F_Supplier_Code);
    //$("#inpSupplierName").val(data.F_Supplier_Name);
    $("#inpPartNo").val(data.F_Part_No);
    //$("#inpPartNameRead").val(data.F_Part_Name);
    $("#inpStore").val(data.F_Store_Code);
    $("#inpKanban").val(data.F_Kanban_No);
    $("#inpCycle").val(data.F_Cycle);
    $("#inpStartDate").val(data.F_Start_Date);
    $("#inpEndDate").val(data.F_End_Date);
    $("#inpTypeOrder").val(data.F_Type_Special);
    $("#divDetail").find("select").each(function () {
        $("#" + $(this).attr("id") + "Read").val($(this).val());
    });
    SelectedPartNo();
    SelectedSupplier();
    $(".selectpicker").each(function () {
        $(this).selectpicker("refresh");
    });
    if ($("#btnUpdate").prop("disabled") == false) {
        //$("#tableMain").find("input[type='checkbox']").prop("checked", true);
        $("#tableMain").find("input[type='checkbox']").prop("checked", false);
        $(this).closest("tr").find("input[type='checkbox']").prop("checked", true);
        $("#divDetail").find("select:not([id*='inpTypeOrder'])").prop("disabled", true);
        $("#divDetail").find("input[type='text']:not([id*='inpEndDate'])").prop("readonly", true);
        $(".selectpicker").each(function () {
            $(this).selectpicker("refresh");
        });
    }
});
function ObjGet() {
    return __awaiter(this, void 0, void 0, function* () {
        var obj = {
            kanban: $("#inpKanban").val(),
            supplier: $("#inpSupplierCode").val(),
            partno: $("#inpPartNo").val(),
            storecd: $("#inpStore").val(),
            type: $("#inpTypeOrder").val(),
            isNew: $("#divButton").find("button:not([disabled])").attr("id") == "btnNew" ? true : false,
        };
        return obj;
    });
}
function ObjPost() {
    return __awaiter(this, void 0, void 0, function* () {
        var obj = {
            kanban: $("#inpKanban").val(),
            supplier: $("#inpSupplierCode").val(),
            partno: $("#inpPartNo").val(),
            storecd: $("#inpStore").val(),
            TypeOrder: $("#inpTypeOrder").val(),
            startDate: moment($("#inpStartDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
            endDate: moment($("#inpEndDate").val(), "DD/MM/YYYY").format("YYYYMMDD"),
            cycle: $("#inpCycle").val(),
            action: $("#divButton").find("button:not([disabled])").attr("id"),
        };
        let listObj = [];
        listObj.push(obj);
        if (obj.action == "btnDelete") {
            listObj = [];
            _xDataTable.GetSelectedDataDT("#tableMain").forEach(function (data) {
                listObj.push({
                    kanban: data.F_Kanban_No,
                    supplier: data.F_Supplier_Code,
                    partno: data.F_Part_No,
                    storecd: data.F_Store_Code,
                    TypeOrder: data.F_Type_Special,
                    startDate: moment(data.F_Start_Date, "DD/MM/YYYY").format("YYYYMMDD"),
                    endDate: moment(data.F_End_Date, "DD/MM/YYYY").format("YYYYMMDD"),
                    cycle: data.F_Cycle,
                    action: obj.action,
                });
            });
        }
        return listObj;
    });
}
function Save() {
    return __awaiter(this, void 0, void 0, function* () {
        yield xSplash.show();
        var obj = yield ObjPost();
        if (obj.length == 0) {
            yield xSplash.hide();
            xSwal.error("Error", "Please select data to Save");
            return;
        }
        _xLib.AJAX_Post("/api/KBNMS004/Save", obj, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                yield xSplash.hide();
                xSwal.success("Success", success.message);
                yield GetListData();
            });
        }, function (error) {
            return __awaiter(this, void 0, void 0, function* () {
                yield xSplash.hide();
                xSwal.error(error.responseJSON.response, error.responseJSON.message);
            });
        });
    });
}
function GetListData() {
    return __awaiter(this, void 0, void 0, function* () {
        var obj = {
            kanban: $("#inpKanban").val(),
            supplier: $("#inpSupplierCode").val(),
            partno: $("#inpPartNo").val(),
            storecd: $("#inpStore").val(),
            type: $("#inpTypeOrder").val(),
        };
        yield _xLib.AJAX_Get("/api/KBNMS004/GetListData", obj, function (data) {
            return __awaiter(this, void 0, void 0, function* () {
                data = yield _xLib.JSONparseMixData(data);
                //console.log(data);
                _xDataTable.ClearAndAddDataDT("#tableMain", data.data);
            });
        });
    });
}
