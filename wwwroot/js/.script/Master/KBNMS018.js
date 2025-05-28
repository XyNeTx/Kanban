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
    // Define the base columns
    let columns = [
        {
            title: "Flag",
            render(data, type, row) {
                return `<input type="checkbox" class="chkbox" id="chkbox" name="chkbox">`;
            }
        },
        {
            title: "Cycle B",
            data: "f_CycleB"
        }
    ];
    // Dynamically add "Round 1" to "Round 30" columns
    for (let i = 1; i <= 30; i++) {
        columns.push({
            title: `Round ${i}`,
            data: `f_Round${i}`
        });
    }
    // Initialize the DataTable with the dynamically created columns
    _xDataTable.InitialDataTable("#tableMain", {
        processing: false,
        serverSide: false,
        width: '100%',
        paging: false,
        sorting: false,
        searching: false,
        scrollX: true,
        scrollY: "200px",
        scrollCollapse: false,
        columns: columns,
        select: false,
        order: [[0, "asc"]]
    });
    for (let i = 1; i <= 30; i++) {
        $(`#F_Round${i}`).prop("readonly", true);
    }
    addOptionCycle();
    xSplash.hide();
});
$("#F_CycleB").change(function () {
    return __awaiter(this, void 0, void 0, function* () {
        var data = yield $("#formMain").formToJSON();
        _xLib.AJAX_Get("/api/KBNMS018/GetListData", data, function (success) {
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
            _xLib.ObjSetVal(success.data[0]);
            setRoundReadOnly();
        });
    });
});
var action = "";
$("#divBtn").on("click", "button", function () {
    return __awaiter(this, void 0, void 0, function* () {
        $("#divBtn").find("button").prop("disabled", true);
        $(this).prop("disabled", false);
        action = $(this).attr("id").split("btn")[1].toLowerCase();
        if (action == "new" || action == "upd") {
            setRoundReadOnly();
        }
    });
});
$("#btnCan").click(function () {
    $("#divBtn").find("button").prop("disabled", false);
    $("#formMain").trigger("reset");
    $().resetAllSelectPicker();
    $("#tableMain").DataTable().clear().draw();
    for (let i = 1; i <= 30; i++) {
        $(`#F_Round${i}`).prop("readonly", true);
    }
    action = "";
});
$("#btnSave").click(function () {
    Save();
});
function addOptionCycle() {
    $("#F_CycleB").append(`<option value='00'></option>`);
    for (let i = 1; i <= 30; i++) {
        let value = i.toString().length == 1 ? "0" + i.toString() : i.toString();
        $("#F_CycleB").append(`<option value='${value}'>${value}</option>`);
        //$(`#F_Round${i}`).prop("readonly", false);
        //console.log(`F_Round${i}`);
    }
    $("#F_CycleB").selectpicker("refresh");
}
function setRoundReadOnly() {
    var _a;
    let cycleB = (_a = parseInt($("#F_CycleB").val())) !== null && _a !== void 0 ? _a : 0;
    if (action == "new" || action == "upd") {
        for (let i = 1; i <= 30; i++) {
            $(`#F_Round${i}`).prop("readonly", false);
        }
        for (let i = 30; i > cycleB; i--) {
            $(`#F_Round${i}`).prop("readonly", true);
        }
    }
}
function Save() {
    return __awaiter(this, void 0, void 0, function* () {
        let SaveObj = yield $("#formMain").formToJSON();
        SaveObj.F_Plant = ajexHeader.Plant;
        SaveObj.F_Round31 = "0";
        SaveObj.F_Round32 = "0";
        _xLib.AJAX_Post("/api/KBNMS018/Save?action=" + action, SaveObj, function (success) {
            _xLib.AJAX_Get("/api/KBNMS018/GetListData", data, function (success) {
                _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
                _xLib.ObjSetVal(success.data[0]);
                setRoundReadOnly();
            });
            xSwal.xSuccess(success);
        });
    });
}
