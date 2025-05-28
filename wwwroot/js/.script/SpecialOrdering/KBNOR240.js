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
                    return "<input type='checkbox' class='chkSelect' checked/>";
                },
            },
            { title: "Supplier", data: "F_Supplier_CD" },
            { title: "Supplier Name", data: "F_Name" },
            { title: "Survey Document", data: "F_Survey_Doc" },
        ],
        order: [[1, "asc"]],
        scrollCollapse: false,
    });
    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");
    yield LoadSurvey();
    xSplash.hide();
}));
$("#chkAll").click(() => {
    $(".chkSelect").prop("checked", true);
});
$("#unChkAll").click(() => {
    $(".chkSelect").prop("checked", false);
});
$("#btnGetStatus").click(() => __awaiter(void 0, void 0, void 0, function* () {
    let data = _xDataTable.GetSelectedDataDT("#tableMain");
    if (data.length === 0) {
        return xSwal.error("Please select at least one row.");
    }
    xSplash.show();
    _xLib.AJAX_Post("/api/KBNOR240/DownloadClicked", data, (success) => __awaiter(void 0, void 0, void 0, function* () {
        xSplash.hide();
        xSwal.success(success.response, success.message);
        yield LoadSurvey();
    }), (error) => __awaiter(void 0, void 0, void 0, function* () {
        xSplash.hide();
        xSwal.xError(error);
        console.error(error);
    }));
}));
LoadSurvey = () => __awaiter(void 0, void 0, void 0, function* () {
    _xLib.AJAX_Get("/api/KBNOR240/LoadSurvey", null, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        //success = await GetCheckFlag(success);
        yield _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
    }), (error) => __awaiter(void 0, void 0, void 0, function* () {
        console.error(error);
    }));
});
