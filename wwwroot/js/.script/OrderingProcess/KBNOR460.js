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
    return __awaiter(this, void 0, void 0, function* () {
        $("#txtProcessDate").val(_CookieProcessDate.substring(0, 4) + "-" + _CookieProcessDate.substring(5, 7) + "-" + _CookieProcessDate.substring(8, 10));
        var shift = _CookieProcessDate.substring(10, 11) == "D" ? "1 - Day Shift" : "2 - Night Shift";
        $("#txtProcessShift").val(shift);
        tblMaster = xDataTable.Initial({
            name: 'tblMaster',
            checking: 0,
            running: 0,
            order: -1,
            columnTitle: {
                "EN": ['PDS No.', 'Supplier', 'Delivery Date', 'Delivery Trip', 'Remark'],
                "TH": ['PDS No.', 'Supplier', 'Delivery Date', 'Delivery Trip', 'Remark'],
                "JP": ['PDS No.', 'Supplier', 'Delivery Date', 'Delivery Trip', 'Remark'],
            },
            column: [
                { "data": "F_OrderNo" },
                { "data": "F_Supplier_Code" },
                { "data": "F_Delivery_Date" },
                { "data": "F_Delivery_Trip" },
                { "data": "F_Remark" }
            ],
            addnew: false,
        });
        tblDetail = xDataTable.Initial({
            name: 'tblDetail',
            checking: 0,
            running: 0,
            order: -1,
            columnTitle: {
                "EN": ['PDS No.', 'Supplier', 'Delivery Date', 'Part No.', 'Remark'],
                "TH": ['PDS No.', 'Supplier', 'Delivery Date', 'Part No.', 'Remark'],
                "JP": ['PDS No.', 'Supplier', 'Delivery Date', 'Part No.', 'Remark'],
            },
            column: [
                { "data": "F_OrderNo" },
                { "data": "F_Supplier_Code" },
                { "data": "F_Delivery_Date" },
                { "data": "Part_No" },
                { "data": "F_Remark" }
            ],
            addnew: false,
        });
        initial = function () {
            return __awaiter(this, void 0, void 0, function* () {
                //xItem.progress({ id: 'prgProcess', current: 0, label: 'Process : {{##.##}} %' });
                var _dt = yield xAjax.ExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNOR460_INITIAL]",
                        "@OrderType": "U",
                        "@Plant": ajexHeader.Plant,
                        "@UserCode": ajexHeader.UserCode
                    },
                });
                if (_dt.rows[0].F_Count > 0) {
                    MsgBox("กรุณากด Generate PDS อีกครั้ง เนื่องจาก PDS No. นี้มีแล้วในระบบ", MsgBoxStyle.Critical, "MISTAKE DATA");
                }
                else {
                    //''Clear Status Data
                    //''Update Status Price of Normal Part: สำหรับกรณีที่กะที่แล้วมีการ Update Flag ไว้และกะนี้เจอราคาแล้ว ให้ Update และ Up ไป E - Pro
                    var _dt = yield xAjax.ExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR460]",
                            "@OrderType": "U",
                            "@Plant": ajexHeader.Plant,
                            "@UserCode": ajexHeader.UserCode
                        },
                    });
                    if (_dt.rows != null)
                        xDataTable.bind('#tblMaster', _dt.rows);
                    //console.log(_dt);
                    var _dtDetail = yield xAjax.ExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR460_DETAIL]",
                            "@OrderType": "U",
                            "@Plant": ajexHeader.Plant,
                            "@UserCode": ajexHeader.UserCode
                        },
                    });
                    if (_dtDetail.rows != null)
                        xDataTable.bind('#tblDetail', _dtDetail.rows);
                    if (_dtDetail.rows != null)
                        xDataTable.draw('#tblDetail', function (table) {
                            table.rows().every(function () {
                                const _node = this.node();
                                if (this.data().F_Status.trim() != '') {
                                    $(_node).css("background-color", "red");
                                    $(_node).css("color", "white");
                                }
                            });
                        });
                }
                $("#table-wrapper").css("visibility", "hidden");
                xSplash.hide();
            });
        };
        initial();
        xAjax.onClick('btnExit', function () {
            xAjax.redirect('KBNOR400');
        });
        xAjax.onClick('btnExportData', function () {
            xAjax.redirect('KBNOR460EX');
        });
        xAjax.onClick('btnRegister', function () {
            return __awaiter(this, void 0, void 0, function* () {
                var F_OrderNo = '';
                try {
                    xItem.progress({ id: 'prgProcess', current: 0, label: 'Start Processing... : {{##.##}} %' });
                    MsgBox("Do you want Registration Data for Urgent Order?", MsgBoxStyle.OkCancel, function () {
                        return __awaiter(this, void 0, void 0, function* () {
                            xItem.progress({ id: 'prgProcess', current: 10, label: 'INSERT TB_REC_HEADER AND TB_REC_DETAIL : {{##.##}} %' });
                            //'*****************1.INSERT DATA TO REGIST**************************
                            var _arMaster = yield xDataTable.selected('#tblMaster');
                            for (var i = 0; i < _arMaster.length; i++) {
                                yield xAjax.xExecuteJSON({
                                    data: {
                                        "Module": "[exec].[spKBNOR460_Regis]",
                                        "@Plant": ajexHeader.Plant,
                                        "@UserCode": ajexHeader.UserCode,
                                        "@F_OrderNo": _arMaster[i].F_OrderNo
                                    },
                                });
                            }
                            xItem.progress({ id: 'prgProcess', current: 100, label: 'Process Regis Completed : {{##.##}} %' });
                            this.initial();
                            MsgBox("Process Registration Data for Urgent Order Completed.", MsgBoxStyle.Information, "Process Complete");
                            yield SendEmailZeroPrice();
                        });
                    }, function () {
                        xItem.progress({ id: 'prgProcess', current: 0, label: 'Process : {{##.##}} %' });
                    });
                }
                catch (error) {
                    // Code to handle the error
                    for (var i = 0; i < _arMaster.length; i++) {
                        yield xAjax.Execute({
                            data: {
                                "Module": "[exec].[spKBNOR460_EXCEPTION]",
                                "@OrderType": "U",
                                "@Plant": ajexHeader.Plant,
                                "@UserCode": ajexHeader.UserCode,
                                "@F_Delivery_Date": _arMaster[i].F_Delivery_Date,
                                "@F_Delivery_Trip": _arMaster[i].F_Delivery_Trip,
                                "@F_Supplier_Code": _arMaster[i].F_Supplier_Code,
                                "@F_Supplier_Plant": _arMaster[i].F_Supplier_Plant,
                                "@F_OrderNo": _arMaster[i].F_OrderNo
                            },
                        });
                        //'2.UPDATE TB_Transaction F_Reg_Flg='3' **3.DELETE DATA TO TB_ORDER**4.DELETE DATA TO TB_Delivery
                        var _dt = yield xAjax.ExecuteJSON({
                            data: {
                                "Module": "[exec].[spKBNOR460_03]",
                                "@OrderType": "U",
                                "@Plant": ajexHeader.Plant,
                                "@UserCode": ajexHeader.UserCode,
                                "@F_Delivery_Date": _arMaster[i].F_Delivery_Date,
                                "@F_Delivery_Trip": _arMaster[i].F_Delivery_Trip,
                                "@F_Supplier_Code": _arMaster[i].UserCode,
                                "@F_Supplier_Plant": _arMaster[i].F_Supplier_Plant,
                                "@F_OrderNo": _arMaster[i].F_OrderNo
                            },
                        });
                        if (_dt.rows != null) {
                            for (var j = 0; j < _dt.rows.length; j++) {
                                //'*****************2.UPDATE TB_Transaction F_Reg_Flg='3'***************************
                                yield xAjax.ExecuteJSON({
                                    data: {
                                        "Module": "[exec].[spKBNOR460_EXCEPTION_U]",
                                        "@OrderType": "U",
                                        "@Plant": ajexHeader.Plant,
                                        "@UserCode": ajexHeader.UserCode,
                                        "@F_Delivery_Date": _dt.rows[j].F_Delivery_Date,
                                        "@F_Supplier_Cd": _dt.rows[j].F_Supplier_Cd,
                                        "@F_Supplier_Plant": _dt.rows[j].F_Supplier_Plant,
                                        "@F_Part_No": _dt.rows[j].F_Part_No,
                                        "@F_Ruibetsu": _dt.rows[j].F_Ruibetsu,
                                        "@F_Store_CD": _dt.rows[j].F_Store_CD,
                                        "@F_Kanban_No": _dt.rows[j].F_Kanban_No,
                                    },
                                });
                            }
                        }
                    }
                    console.log(error);
                    MsgBox("Error Number : " + '' + "(" + '' + ")", MsgBoxStyle.Critical, "Process Not Complete");
                    xItem.progress({ id: 'prgProcess', current: 0, label: 'Process Not Complete!!! : {{##.##}} %' });
                }
            });
        });
    });
});
function SendEmailZeroPrice() {
    return __awaiter(this, void 0, void 0, function* () {
        console.log($("div .pcoded-navigatio-lavel ").text());
        var obj = {
            User_Name: $("div .pcoded-navigatio-lavel ").text().trim(),
            Program: "KBNOR460 : Registration Data for Urgent Order",
            ProcessDate: $("#txtProcessDate").val(),
            ProcessShift: $("#txtProcessShift").val().substring(4, 5)
        };
        console.log(obj);
        _xLib.AJAX_Post("/api/KBNOR460/SendEmailZeroPrice", JSON.stringify(obj), function (success) {
            if (success.status == 200) {
                console.log(success);
                xSwal.success("Success", "Send Email with Zero Price Complete");
            }
        }, function (error) {
            xSwal.error("Error", "Can't Send Email with Zero Price");
        });
    });
}
