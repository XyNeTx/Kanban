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
                    return "<input type='checkbox' class='chkFlag' data-id='" + row.F_Survey_Doc + "' />";
                }
            },
            {
                title: "No.", render: function (data, type, row, meta) {
                    return meta.row + 1;
                }
            },
            { title: "Plant", data: "F_Factory_Code" },
            { title: "Supplier Code", data: "F_Supplier_code" },
            { title: "Survey Document", data: "F_Survey_Doc" },
            { title: "Delivery Date", data: "F_Delivery_Date" },
            { title: "PDS No./Customer PO.", data: "F_PO_Customer" },
            {
                title: "Status", render: function (data, type, row, meta) {
                    return "";
                }
            },
        ],
        order: [[1, "asc"]],
        scrollCollapse: false,
    });
    $("table thead tr th").css("text-align", "center");
    $("table tbody tr td").css("text-align", "center");
    yield GetSurveyNoPDS();
}));
$("#btnChkAll").click(() => {
    $(".chkFlag").prop("checked", true);
});
$("#btnUnChkAll").click(() => {
    $(".chkFlag").prop("checked", false);
});
$("#mthDeliYM").change(() => __awaiter(void 0, void 0, void 0, function* () {
    yield GetSurveyNoPDS();
}));
$("#btnRefresh").click(() => __awaiter(void 0, void 0, void 0, function* () {
    yield Refresh();
}));
$("#btnUnlock").click(() => __awaiter(void 0, void 0, void 0, function* () {
    let isConfrim = yield xSwal.confirm("This action can not undone !", "Are you sure to unlock the selected rows?");
    if (!isConfrim)
        return;
    yield Unlock();
}));
$("#btnGenerate").click(() => __awaiter(void 0, void 0, void 0, function* () {
    let isConfrim = yield xSwal.confirm("This action can not undone !", "Are you sure to generate the selected rows?");
    if (!isConfrim)
        return;
    yield Generate();
}));
GetSurveyNoPDS = () => __awaiter(void 0, void 0, void 0, function* () {
    let getQuery = {
        DeliYM: $("#mthDeliYM").val().replace("-", ""),
        Fac: _xLib.GetCookie("plantCode"),
    };
    _xLib.AJAX_Get("/api/KBNOR250/GetSurveyNoPDS", getQuery, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        console.log(success);
        yield _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
        $.each(success.data, function (index, value) {
            return __awaiter(this, void 0, void 0, function* () {
                yield CheckPriceAndPackageFlag(value.F_Survey_Doc);
            });
        });
        xSplash.hide();
    }), (error) => {
        console.log(error);
    });
});
CheckPriceAndPackageFlag = (surveyDoc) => __awaiter(void 0, void 0, void 0, function* () {
    let getQuery = {
        SurveyDoc: surveyDoc
    };
    _xLib.AJAX_Get("/api/KBNOR250/CheckPriceAndPackageFlag", getQuery, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        console.log(success);
        if (success.data.length > 0) {
            let status = success.data[0].F_Status;
            if (status.toLowerCase() == "package not found") {
                $("#tableMain tbody tr").find("td:eq(4) == " + surveyDoc).text(status);
                if (status.toLowerCase() == "price zero" || status.toLowerCase() == "package not found"
                    || status.toLowerCase() == "price zero & package not found") {
                    $("#tableMain tbody tr").find("td:eq(0) == " + surveyDoc).find("input").prop("checked", true);
                }
            }
        }
    }), (error) => {
        console.log(error);
    });
});
Refresh = () => __awaiter(void 0, void 0, void 0, function* () {
    _xLib.AJAX_Get("/api/KBNOR250/Refresh", null, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        console.log(success);
        xSplash.hide();
        xSwal.success(success.response, success.message);
        yield GetSurveyNoPDS();
    }));
});
Unlock = () => __awaiter(void 0, void 0, void 0, function* () {
    let listObj = [];
    $("input.chkFlag:checked").each(function () {
        let row = $(this).closest("tr");
        let obj = $("#tableMain").DataTable().row(row).data();
        listObj.push(obj);
    });
    if (listObj.length == 0) {
        xSwal.error("Please select at least one row.");
        return;
    }
    let postQuery = {
        ListObj: listObj
    };
    _xLib.AJAX_Post("/api/KBNOR250/Unlock", postQuery, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        console.log(success);
        xSplash.hide();
        xSwal.success(success.response, success.message);
        $("#tableMain input[type='checkbox']:checked").each(function () {
            $("#tableMain").DataTable().row($(this).closest("tr")).remove().draw(false);
        });
        //await $("#mthDeliYM").trigger("change");
    }), (error) => __awaiter(void 0, void 0, void 0, function* () {
        xSwal.xError(error);
    }));
});
Generate = () => __awaiter(void 0, void 0, void 0, function* () {
    let postQuery = [];
    $("input.chkFlag:checked").each(function () {
        let row = $(this).closest("tr");
        let obj = $("#tableMain").DataTable().row(row).data();
        postQuery.push(obj);
    });
    if (postQuery.length == 0) {
        xSwal.error("Please select at least one row.");
        return;
    }
    _xLib.AJAX_Post("/api/KBNOR250/Generate?DeliYM=" + $("#mthDeliYM").val().replace("-", ""), postQuery, (success) => __awaiter(void 0, void 0, void 0, function* () {
        success = _xLib.JSONparseMixData(success);
        console.log(success);
        xSplash.hide();
        xSwal.success(success.response, success.message);
        $("#tableMain input[type='checkbox']:checked").each(function () {
            $("#tableMain").DataTable().row($(this).closest("tr")).remove().draw(false);
        });
        //await $("#mthDeliYM").trigger("change");
    }), (error) => __awaiter(void 0, void 0, void 0, function* () {
        xSwal.xError(error);
    }));
});
