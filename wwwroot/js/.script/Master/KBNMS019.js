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
            scrollX: true,
            scrollY: "200px",
            scrollCollapse: false,
            "columns": [
                {
                    title: "Flag", render(data, type, row) {
                        return `<input type="checkbox" class="chkbox" id="chkbox" name="chkbox">`;
                    }
                },
                {
                    title: "Supplier Code", data: "f_Supplier"
                },
                {
                    title: "Kanban No.", data: "f_KanbanNo"
                },
                {
                    title: "Part No.", data: "f_PartNo"
                },
                {
                    title: "Part Name", data: "f_PartName"
                },
                {
                    title: "Store Code", data: "f_StoreCD"
                },
                {
                    title: "Max/Trip", data: "f_Max_Trip"
                },
            ],
            select: false,
            order: [[0, "asc"]]
        });
        //await GetSupplier();
        xSplash.hide();
    });
});
$(document).on("dblclick", "#tableMain tbody tr", function () {
    return __awaiter(this, void 0, void 0, function* () {
        var row = $(this).closest("tr");
        $(row).find("input[type='checkbox']").prop("checked", true);
        $(this).addClass("selected");
        $(".selected").removeClass("selected");
        $(this).closest("tr").toggleClass("selected");
        var data = _xDataTable.GetDataDT("#tableMain", row);
        data.f_Total_Max_Trip = $("#F_Total_Max_Trip").val();
        //console.log($("#F_Total_Max_Trip").val());
        yield $("#formMain").trigger("reset");
        yield _xLib.ObjSetVal(data, true);
    });
});
$("#divBtn").on("click", "button", function () {
    return __awaiter(this, void 0, void 0, function* () {
        $("#divBtn").find("button").prop("disabled", true);
        $(this).prop("disabled", false);
        GetSupplier();
    });
});
$("#F_Supplier").change(function () {
    GetPartNo();
    GetStoreCD();
    GetKanbanNo();
    GetListMaxArea();
});
$("#F_PartNo").change(function () {
    GetStoreCD();
    GetPartName();
});
$("#F_StoreCD").change(function () {
    GetKanbanNo();
});
$("#F_KanbanNo").change(function () {
    GetMaxTrip();
});
$("#btnSave").click(function () {
    Save();
});
$("#btnCan").click(function () {
    $("#divBtn").find("button").prop("disabled", false);
    $("#formMain").trigger("reset");
    $().resetAllSelectPicker();
    $("#tableMain").DataTable().clear().draw();
});
function ObjGet() {
    return __awaiter(this, void 0, void 0, function* () {
        var obj = yield $("#formMain").formToJSON();
        obj.isNew = (yield $("#divBtn").find("button:not(:disabled)").attr("id")) == "btnNew";
        return obj;
    });
}
function GetSupplier() {
    return __awaiter(this, void 0, void 0, function* () {
        var getObj = yield ObjGet();
        _xLib.AJAX_Get("/api/KBNMS019/GetSupplier", getObj, function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            $("#F_Supplier").addListSelectPicker(success.data, "f_Supplier_Code");
        });
    });
}
function GetPartNo() {
    return __awaiter(this, void 0, void 0, function* () {
        var getObj = yield ObjGet();
        _xLib.AJAX_Get("/api/KBNMS019/GetPartNo", getObj, function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            $("#F_PartNo").addListSelectPicker(success.data, "f_Part_No");
        });
    });
}
function GetListMaxArea() {
    return __awaiter(this, void 0, void 0, function* () {
        var getObj = yield ObjGet();
        _xLib.AJAX_Get("/api/KBNMS019/GetListMaxArea", getObj, function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
            if (success.data.length > 1) {
                $("#F_Total_Max_Trip").val(success.maxTotal);
            }
            else {
                $("#F_Total_Max_Trip").val(success.data[0].f_Max_Trip);
            }
        });
    });
}
function GetStoreCD() {
    return __awaiter(this, void 0, void 0, function* () {
        var getObj = yield ObjGet();
        _xLib.AJAX_Get("/api/KBNMS019/GetStoreCD", getObj, function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            $("#F_StoreCD").addListSelectPicker(success.data, "f_Store_Code");
        });
    });
}
function GetKanbanNo() {
    return __awaiter(this, void 0, void 0, function* () {
        var getObj = yield ObjGet();
        _xLib.AJAX_Get("/api/KBNMS019/GetKanbanNo", getObj, function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
            $("#F_KanbanNo").addListSelectPicker(success.data, "f_Kanban_No");
        });
    });
}
function GetPartName() {
    return __awaiter(this, void 0, void 0, function* () {
        var getObj = yield ObjGet();
        _xLib.AJAX_Get("/api/KBNMS019/GetPartName", getObj, function (success) {
            //success.data = _xLib.TrimArrayJSON(success.data);
            $("#F_PartName").val(success.data.trim());
        });
    });
}
function GetMaxTrip() {
    return __awaiter(this, void 0, void 0, function* () {
        var getObj = yield ObjGet();
        _xLib.AJAX_Get("/api/KBNMS019/GetMaxTrip", getObj, function (success) {
            success.data = _xLib.TrimArrayJSON(success.data);
        });
    });
}
function Save() {
    return __awaiter(this, void 0, void 0, function* () {
        var Obj = yield ObjGet();
        var action = $("#divBtn").find("button:not(:disabled)").attr("id").split("btn")[1];
        var listObj = [];
        if (action.toLowerCase() != "del") {
            Obj.F_Total_Max_Trip = Obj.F_Total_Max_Trip == "" ? 0 : Obj.F_Total_Max_Trip;
            Obj.F_Max_Trip = Obj.F_Max_Trip == "" ? 0 : Obj.F_Max_Trip;
            listObj.push(Obj);
        }
        else {
            listObj = _xDataTable.GetSelectedDataDT("#tableMain");
        }
        _xLib.AJAX_Post("/api/KBNMS019/Save?action=" + action, listObj, function (success) {
            GetListMaxArea();
            xSwal.xSuccess(success);
        }, function (error) {
            xSwal.xError(error);
        });
    });
}
