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
var _CookieProcessDate = _xLib.GetCookie("processDate");
var _CookieLoginDate = _xLib.GetCookie("loginDate");
$(document).ready(function () {
    $("#txtProcessDate").val(_CookieProcessDate.substring(0, 4) + "-" + _CookieProcessDate.substring(5, 7) + "-" + _CookieProcessDate.substring(8, 10));
    var shift = _CookieProcessDate.substring(10, 11) == "D" ? "1 - Day Shift" : "2 - Night Shift";
    $("#txtProcessShift").val(shift);
    const KBNOR110 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Part No', 'Supplier', 'Store Code', 'Delivery Date', 'Qty', 'Type of Part', 'Import Type'],
            "TH": ['Part No', 'Supplier', 'Store Code', 'Delivery Date', 'Qty', 'Type of Part', 'Import Type'],
            "JP": ['Part No', 'Supplier', 'Store Code', 'Delivery Date', 'Qty', 'Type of Part', 'Import Type'],
        },
        ColumnValue: [
            { "data": "F_Part_No" },
            { "data": "F_Supplier_CD" },
            { "data": "F_Store_CD" },
            { "data": "F_Delivery_Date" },
            { "data": "F_Qty" },
            { "data": "F_QTY_KB" },
            { "data": "F_OrderType" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });
    KBNOR110.prepare();
    KBNOR110.initial(function (result) {
        xSplash.hide();
    });
    xAjax.onClick('btnExit', function () {
        xAjax.redirect('KBNOR100');
    });
    xAjax.onClick('btnInterface', function () {
        return __awaiter(this, void 0, void 0, function* () {
            try {
                MsgBox("Do you want Interface data to Order?", MsgBoxStyle.OkCancel, function () {
                    return __awaiter(this, void 0, void 0, function* () {
                        xItem.progress({ id: 'prgProcess', current: 5, label: 'Start Interface Data from Import Data : {{##.##}} %' });
                        //''Clear Old Data 
                        yield xAjax.xExecuteJSON({
                            data: {
                                "Module": "[exec].[spKBNOR110_INTERFACE_D1]",
                                "@OrderType": "N",
                                "@Plant": ajexHeader.Plant,
                                "@UserCode": ajexHeader.UserCode
                            },
                        });
                        xItem.progress({ id: 'prgProcess', current: 10, label: 'Interface Data : Clear Old Order : {{##.##}} %' });
                        //let _remark = '';
                        var _dtChk = yield xAjax.xExecuteJSON({
                            data: {
                                "Module": "[exec].[spKBNOR110_INTERFACE_M1]",
                                "@OrderType": "N",
                                "@Plant": ajexHeader.Plant,
                                "@UserCode": ajexHeader.UserCode
                            },
                        });
                        xItem.progress({ id: 'prgProcess', current: 100, label: 'Interface Data from Import Data : {{##.##}} %' });
                        if (_dtChk.rows != null)
                            xDataTable.bind('#tblMaster', _dtChk.rows);
                        if (_dtChk.rows == null)
                            MsgBox("ไม่พบข้อมูล Interface Normal Order", MsgBoxStyle.Information, "Interface Normal Data");
                        $("#table-wrapper").css("visibility", "hidden");
                    });
                });
            }
            catch (error) {
                // Code to handle the error
                yield xAjax.Execute({
                    data: {
                        "Module": "[exec].[spKBNOR110_EXCEPTION]",
                        "@OrderType": "N",
                        "@Plant": ajexHeader.Plant,
                        "@UserCode": ajexHeader.UserCode
                    },
                });
                MsgBox("ERROR Interface Data from Import Data.", MsgBoxStyle.Critical, "Interface Urgent Data Error");
                xItem.progress({ id: 'prgProcess', current: 0, label: 'Process Not Complete!!! : {{##.##}} %' });
            }
        });
    });
});
