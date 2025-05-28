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
        yield _xDataTable.InitialDataTable("#tableMain", {
            columns: [
                {
                    title: "Flag", render(data, type, row) {
                        return `<input type="checkbox" class="chkbox" checked />`;
                    }
                },
                { title: "Order No.", data: "F_PDS_No" },
                { title: "Order Issued Date", data: "F_PDS_ISSUED_DATE" },
                { title: "Delivery Date", data: "F_Delivery_Date" },
            ],
            scrollX: false,
            order: [[1, "asc"]],
            scrollCollapse: false,
            scrollY: 200,
        });
        $("table thead th").addClass("text-center");
        $("table tbody td").addClass("text-center");
        $("#txtDateFrom").initDatepicker();
        $("#txtDateTo").initDatepicker();
        yield GetPDS();
        yield GetUser();
        xSplash.hide();
    });
});
$("#chkboxDeliDate").change(function () {
    if (this.checked) {
        $("#txtDateFrom").prop("disabled", false);
        $("#txtDateTo").prop("disabled", false);
    }
    else {
        $("#txtDateFrom").prop("disabled", true);
        $("#txtDateTo").prop("disabled", true);
    }
});
//$("#selOrder").change(function () {
//    Search();
//});
//$("#selCreate").change(function () {
//    Search();
//});
$("#btnUpdCycle").click(function () {
    Update_Cycle();
});
$("#btnConfirm").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        yield Confirm();
        $("#tableMain").DataTable().clear().draw();
        //setTimeout(() => {
        //    Search();
        //}, 100);
        //clearTimeout();
    });
});
$("#btnSelectAll").click(function () {
    $(".chkbox").prop("checked", true);
});
$("#btnDeselectAll").click(function () {
    $(".chkbox").prop("checked", false);
});
$("#btnSearch").click(function () {
    Search();
});
function GetPDS() {
    return __awaiter(this, void 0, void 0, function* () {
        let obj = ObjGet();
        _xLib.AJAX_Get("/api/KBNIM007C/GetPDS", obj, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                success = _xLib.JSONparseMixData(success);
                console.log(success);
                $("#selOrder").addListSelectPicker(success.data, "F_PDS_No");
            });
        });
    });
}
function GetUser() {
    return __awaiter(this, void 0, void 0, function* () {
        let obj = ObjGet();
        _xLib.AJAX_Get("/api/KBNIM007C/GetUser", obj, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                success = _xLib.JSONparseMixData(success);
                console.log(success);
                $("#selCreate").addListSelectPicker(success.data, "F_Update_By");
            });
        });
    });
}
function Search() {
    return __awaiter(this, void 0, void 0, function* () {
        let data = yield ObjGet();
        _xLib.AJAX_Get("/api/KBNIM007C/GetListData", data, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                success = _xLib.JSONparseMixData(success);
                console.log(success);
                _xDataTable.ClearAndAddDataDT("#tableMain", success.data);
            });
        });
    });
}
function Update_Cycle() {
    return __awaiter(this, void 0, void 0, function* () {
        _xLib.AJAX_Post("/api/KBNIM007C/Update_Cycle", null, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                xSwal.success("Success", "Update Cycle Success");
            });
        });
    });
}
function Confirm() {
    return __awaiter(this, void 0, void 0, function* () {
        var listObj = yield _xDataTable.GetSelectedDataDT("#tableMain");
        if (listObj.length === 0) {
            xSwal.error("Error", "Please select at least one item.");
            return;
        }
        yield _xLib.AJAX_Post("/api/KBNIM007C/Confirm", listObj, function (success) {
            return __awaiter(this, void 0, void 0, function* () {
                xSwal.success("Success", "Confirm Success");
            });
        });
    });
}
function ObjGet() {
    return __awaiter(this, void 0, void 0, function* () {
        let obj = {
            PDSNo: $("#selOrder").val(),
            User: $("#selCreate").val().split(":")[0],
            DeliDateChk: $("#chkboxDeliDate").is(":checked"),
        };
        if (obj.DeliDateChk) {
            obj.DeliDateFrom = $("#txtDateFrom").val();
            obj.DeliDateTo = $("#txtDateTo").val();
        }
        return obj;
    });
}
