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
            { title: "Prod YM", data: "F_Prod_YM" },
            { title: "Supplier", data: "F_Supplier_CD" },
            { title: "Supplier Name", data: "F_Name" },
            { title: "Survey Document", data: "F_Survey_Doc" },
            { title: "Price Status", data: "F_Price_Status" },
        ],
        order: [[1, "asc"]],
        scrollCollapse: true,
    });
    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");
    _xLib.AJAX_Get("/api/KBNOR230/LoadSurvey", null, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        success = yield GetCheckFlag(success);
        yield _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        xSplash.hide();
    }), (error) => __awaiter(void 0, void 0, void 0, function* () {
        console.error(error);
    }));
}));
$("#chkAll").click(() => {
    $(".chkSelect").prop("checked", true);
});
$("#unChkAll").click(() => {
    $(".chkSelect").prop("checked", false);
});
$("#btnUpload").click(() => __awaiter(void 0, void 0, void 0, function* () {
    yield Upload();
}));
GetCheckFlag = (success) => __awaiter(void 0, void 0, void 0, function* () {
    for (let i = 0; i < success.data.length; i++) {
        yield _xLib.AJAX_Get("/api/KBNOR230/CheckPriceFlag", { SurveyDoc: success.data[i].F_Survey_Doc }, (chkSuccess) => __awaiter(void 0, void 0, void 0, function* () {
            console.log(chkSuccess);
            if (chkSuccess.data == "0") {
                success.data[i].F_Price_Status = "Price Zero";
            }
            else {
                success.data[i].F_Price_Status = "";
            }
        }), (error) => __awaiter(void 0, void 0, void 0, function* () {
            console.error(error);
        }));
    }
    return success;
});
Upload = () => __awaiter(void 0, void 0, void 0, function* () {
    let data = _xDataTable.GetSelectedDataDT("#tableMain");
    if (data.length == 0) {
        return xSwal.error("Error!!", "Please Select Data to Upload.");
    }
    xSplash.show();
    yield _xLib.AJAX_Post("/api/KBNOR230/Upload", data, (success) => __awaiter(void 0, void 0, void 0, function* () {
        xSplash.hide();
        $("#tableMain input[type='checkbox']:checked").each(function () {
            $("#tableMain").DataTable().row($(this).closest("tr")).remove().draw(false);
        });
        xSwal.success(success.response, success.message);
    }), (error) => __awaiter(void 0, void 0, void 0, function* () {
        xSplash.hide();
        xSwal.xError(error);
        console.error(error);
    }));
});
