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
        yield _xDataTable.InitialDataTable("#tblMaster", {
            columns: [
                {
                    title: `<input type='checkbox' class='chkBoxDT' id='chkAll'>`, data: null, orderable: false, width: "5%",
                    render: function (data, type, row) {
                        return "<input type='checkbox' class='chkBoxDT' data-id='" + row.F_PDS_No + "'>";
                    }
                },
                { title: "Customer PO", data: "F_PDS_No" },
                { title: "Part No", data: "F_Part_No" },
                { title: "Supplier", data: "F_Supplier_CD" },
                { title: "Short Name", data: "F_Short_name" },
                { title: "Store Code", data: "F_Store_CD" },
                { title: "Kanban No.", data: "F_Kanban_No" },
                { title: "Delivery Date", data: "F_Delivery_Date" },
                { title: "Delivery Trip", data: "F_Round" },
                { title: "Qty", data: "F_Qty" },
                { title: "Qty KB", data: "F_QTY_KB" },
                { title: "Import Type", data: "F_OrderType" },
            ],
            order: [[1, "asc"]],
            scrollCollapse: true,
        });
        yield ListData();
        xSplash.hide();
    });
});
$("#btnGenerate").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        yield GeneratePicking_Click();
    });
});
$("#btnRegister").click(function () {
    return __awaiter(this, void 0, void 0, function* () {
        yield Register();
    });
});
function ListData() {
    return __awaiter(this, void 0, void 0, function* () {
        return yield _xLib.AJAX_Get("/api/KBNOR360/List_Data", null, function (success) {
            success = _xLib.JSONparseMixData(success);
            _xDataTable.ClearAndAddDataDT("#tblMaster", success.data);
        }, function (error) {
            console.error(error);
            //xSwal.xError(error)
        });
    });
}
function Register() {
    return __awaiter(this, void 0, void 0, function* () {
        var dataList = _xDataTable.GetSelectedDataDT("#tblMaster");
        if (dataList.length == 0) {
            xSwal.error("Please select data to register.");
            return;
        }
        _xLib.AJAX_Post("/api/KBNOR360/Register", dataList, function (success) {
            xSwal.xSuccess("Registration successful.");
            _xLib.ClearData("#tblMaster");
            ListData();
        }, function (error) {
            xSwal.xError(error);
        });
    });
}
function GeneratePicking_Click() {
    return __awaiter(this, void 0, void 0, function* () {
        _xLib.AJAX_Post("/api/KBNOR360/GeneratePicking_Click", null, function (success) {
            xSwal.xSuccess("Picking generation successful.");
            ListData();
            let obj = {
                Plant: _xLib.GetCookie("plantCode"),
                UserName: _xLib.GetUserName(),
                PI_Date_RemainShelf: success.data[1],
                PI_Time_RemainShelf: success.data[2],
            };
            _xLib.OpenReportObj("/KBNOR360", obj);
        }, function (error) {
            xSwal.xError(error);
        });
    });
}
