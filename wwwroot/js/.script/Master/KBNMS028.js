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
                    title: "DockCD", data: "f_Dock_Cd"
                },
                {
                    title: "LP1", data: "f_short_Logistic1", width: '50%'
                },
                {
                    title: "Remark1", data: "f_Remark1"
                },
                {
                    title: "LP2", data: "f_short_Logistic2"
                },
                {
                    title: "Remark2", data: "f_Remark2"
                },
                {
                    title: "LP3", data: "f_short_Logistic3"
                },
                {
                    title: "Remark3", data: "f_Remark3"
                },
                {
                    title: "LP4", data: "f_short_Logistic4"
                },
                {
                    title: "Remark4", data: "f_Remark4"
                },
                {
                    title: "LP5", data: "f_short_Logistic5"
                },
                {
                    title: "Remark5", data: "f_Remark5"
                },
                {
                    title: "LP6", data: "f_short_Logistic6"
                },
                {
                    title: "Remark6", data: "f_Remark6"
                },
                {
                    title: "LP7", data: "f_short_Logistic7"
                },
                {
                    title: "Remark7", data: "f_Remark7"
                },
                {
                    title: "LP8", data: "f_short_Logistic8"
                },
                {
                    title: "Remark8", data: "f_Remark8"
                },
                {
                    title: "LP9", data: "f_short_Logistic9"
                },
                {
                    title: "Remark9", data: "f_Remark9"
                },
                {
                    title: "LP10", data: "f_short_Logistic10"
                },
                {
                    title: "Remark10", data: "f_Remark10"
                },
                {
                    title: "LP11", data: "f_short_Logistic11"
                },
                {
                    title: "Remark11", data: "f_Remark11"
                },
                {
                    title: "LP12", data: "f_short_Logistic12"
                },
                {
                    title: "Remark12", data: "f_Remark12"
                },
                {
                    title: "LP13", data: "f_short_Logistic13"
                },
                {
                    title: "Remark13", data: "f_Remark13"
                },
                {
                    title: "LP14", data: "f_short_Logistic14"
                },
                {
                    title: "Remark14", data: "f_Remark14"
                },
                {
                    title: "LP15", data: "f_short_Logistic15"
                },
                {
                    title: "Remark15", data: "f_Remark15"
                },
                {
                    title: "LP16", data: "f_short_Logistic16"
                },
                {
                    title: "Remark16", data: "f_Remark16"
                },
            ],
            select: false,
            order: [[0, "asc"]]
        });
        yield GetShortLogistic();
        yield GetDockCode();
        yield GetListData();
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
    return __awaiter(this, void 0, void 0, function* () {
        GetListData();
        $("#divBtn").find("button").prop("disabled", true);
        $(this).prop("disabled", false);
    });
});
$("#btnCan").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        $("#divBtn").find("button").prop("disabled", false);
        $("#formMain").trigger("reset");
        $().resetAllSelectPicker();
    });
});
$(document).on("dblclick", "table tbody tr td", function () {
    return __awaiter(this, void 0, void 0, function* () {
        yield $("#formMain").trigger("reset");
        var row = $(this).closest("tr");
        var obj = $("#tableMain").DataTable().row(row).data();
        $(".selected").removeClass("selected");
        $(this).closest("tr").toggleClass("selected");
        $(this).closest("tr").find("#chkbox").prop("checked", true);
        for (const e of Object.keys(obj)) {
            let _e = "F" + e.substring(1, e.length);
            if (_e.includes("short_Logistic")) {
                if (obj[e] === "")
                    break;
            }
            console.log(_e);
            $(`#${_e}`).val(obj[e]);
            if ($(`#${_e}`).prop("tagName") === "SELECT") {
                $(`#${_e}`).selectpicker("refresh");
            }
        }
        //Object.keys(obj).forEach(async function (e) {
        //    let _e = "F" + e.substring(1, e.length);
        //    if (_e.includes("short_Logistic")) {
        //        if (obj[e] === "") return;
        //    }
        //    console.log(_e);
        //    $(`#${_e}`).val(obj[e]);
        //    if ($(`#${_e}`).prop("tagName") === "SELECT") {
        //        $(`#${_e}`).selectpicker("refresh");
        //    }
        //});
        var action = $("#divBtn").find("button:not(:disabled)").attr("id").split("btn")[1];
        if (action.toLowerCase() == "upd") {
            //$("#F_Dock_Cd").prop("readonly", true);
            $("#F_Dock_Cd").disableSelectPicker();
        }
    });
});
$("#btnSave").click(function () {
    Save();
});
$("#F_Dock_Cd").change(function () {
    GetListData();
});
function GetDockCode() {
    return _xLib.AJAX_Get("/api/KBNMS028/GetDockCode", null, function (success) {
        _xLib.TrimArrayJSON(success.data);
        $("#F_Dock_Cd").addListSelectPicker(success.data, "f_Dock_Cd");
    }, function (error) {
        xSwal.xError(error);
    });
}
function GetShortLogistic() {
    return _xLib.AJAX_Get("/api/KBNMS028/GetShortLogistic", null, function (success) {
        _xLib.TrimArrayJSON(success.data);
        $("select[name*='F_short_Logistic']").each(function () {
            //console.log($(this).attr("id"));
            $(this).addListSelectPicker(success.data, "f_short_Logistic");
        });
    }, function (error) {
        xSwal.xError(error);
    });
}
function GetListData() {
    return __awaiter(this, void 0, void 0, function* () {
        let obj = yield $('#formMain').formToJSON();
        return _xLib.AJAX_Get("/api/KBNMS028/GetListData", obj, function (success) {
            _xLib.TrimArrayJSON(success.data);
            _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
            $("table tbody tr td").addClass("ps-4 pb-2 pt-2 pe-4");
            $("#tableMain").DataTable().columns.adjust().draw();
        }, function (error) {
            xSwal.xError(error);
        });
    });
}
function Save() {
    return __awaiter(this, void 0, void 0, function* () {
        var listObj = [];
        var data = yield $('#formMain').formToJSON();
        listObj.push(data);
        //console.log($("#divBtn").find("button:not(:disabled)").attr("id"));
        var action = $("#divBtn").find("button:not(:disabled)").attr("id").split("btn")[1];
        if (action == "Del") {
            listObj = _xDataTable.GetSelectedDataDT("#tableMain");
        }
        _xLib.AJAX_Post("/api/KBNMS028/Save?action=" + action, listObj, function (success) {
            $("#btnCan").trigger("click");
            GetListData();
            xSwal.xSuccess(success);
        }, function (error) {
            xSwal.xError(error);
        });
    });
}
