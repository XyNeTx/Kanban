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
    const xKBNOR440 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Type Import', 'Supplier Code', 'Short Name', 'Delivery Date', 'Delivery Trip', 'Order No'],
            "TH": ['Type Import', 'Supplier Code', 'Short Name', 'Delivery Date', 'Delivery Trip', 'Order No'],
            "JP": ['Type Import', 'Supplier Code', 'Short Name', 'Delivery Date', 'Delivery Trip', 'Order No'],
        },
        ColumnValue: [
            { "data": "F_Type_Import" },
            { "data": "F_Supplier" },
            { "data": "F_Short_name" },
            { "data": "F_Delivery_Date" },
            { "data": "F_Delivery_Trip" },
            { "data": "F_OrderNo" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        //processing :false,
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });
    xKBNOR440.prepare();
    xKBNOR440.initial(function (result) {
        xSplash.hide();
        GetCheckSum();
        //console.log(ajexHeader);
    });
    let pdsno = '';
    let currentdate = replaceall(_PROCESSDATE_, '-', '');
    let issueddate = new Date(currentdate.substring(0, 4), currentdate.substring(4, 6) - 1, currentdate.substring(6));
    let facflag = (ajexHeader.Plant == 1 ? '9Y' : (ajexHeader.Plant == 3 ? '7Y' : ''));
    let value = [];
    let data = [];
    let barcode = '';
    xAjax.onClick('btnExit', function () {
        xAjax.redirect('KBNOR400');
    });
    xAjax.onClick('btnGenerate', function () {
        return __awaiter(this, void 0, void 0, function* () {
            MsgBox("Do you want Issued PDS Urgent Data?", MsgBoxStyle.OkCancel, function () {
                return __awaiter(this, void 0, void 0, function* () {
                    xItem.progress({ id: 'prgProcess', current: 5, label: 'Start Process Urgent : {{##.##}} %' });
                    //Start Process Special  '(9Y Delivery_Trip = 1 only)
                    var _dtChk = yield xAjax.xExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR440_CALCULATE]",
                            "@Plant": ajexHeader.Plant,
                            "@UserCode": ajexHeader.UserCode
                        },
                    });
                    xItem.progress({ id: 'prgProcess', current: 25, label: 'Delete TB_PDS_DETAIL : {{##.##}} %' });
                    /*            MsgBox("Process GEN PDS Data for Urgent Order Completed.", MsgBoxStyle.Information, "Process Complete");*/
                    var _dt = yield xAjax.xExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR440_ShowResult]",
                            "OrderType": "U",
                            "Plant": ajexHeader.Plant,
                            "UserCode": ajexHeader.UserCode
                        },
                    });
                    console.log(_dt);
                    if (_dt.rows != null)
                        xDataTable.bind('#tblMaster', _dt.rows);
                    if (_dt.rows == null)
                        MsgBox("ไม่พบข้อมูล PDS Urgent Order", MsgBoxStyle.Information, "Interface Urgent Data");
                    $("#table-wrapper").css("visibility", "hidden");
                    xItem.progress({ id: 'prgProcess', current: 100, label: 'Generate PDS Completed : {{##.##}} %' });
                    xSwal.success('Success', 'Generate PDS Completed');
                });
            });
        });
    });
    $("#btnReport").click(function () {
        var UserName = $(".pcoded-navigatio-lavel").text();
        console.log(UserName);
        _xLib.OpenReport('/KBNOR440', `UserCode=${ajexHeader.UserCode}&Plant=${ajexHeader.Plant}&UserName=${UserName}`);
    });
    ProcessSpecialKPO = function () {
        return __awaiter(this, void 0, void 0, function* () {
            var _dtChk = yield xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR440_06]",
                    "@pOrderType": "U",
                    "@pPlant": ajexHeader.Plant,
                    "@pUserCode": ajexHeader.UserCode
                },
            });
            if (_dtChk.rows != null) {
                for (var i = 0; i < _dtChk.rows.length; i++) {
                    if (i == 0 || _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                        + _dtChk.rows[i].F_Delivery_Trip.padStart(2, '0') != pdsno.substring(2, 8)) {
                        var _dtChkPOM = yield xAjax.ExecuteJSON({
                            data: {
                                "Module": "[exec].[spKBNOR440_06_S]",
                                "@OrderType": "U",
                                "@Plant": ajexHeader.Plant,
                                "@UserCode": ajexHeader.UserCode,
                                "@F_OrderNo": _dtChk.rows[i].F_Delivery_Date.substring(2, 6) + _dtChk.rows[i].F_Delivery_Trip.padStart(2, '0')
                                //"@F_Delivery_Date": _dtChk.rows[i].F_Delivery_Date,
                                //"@F_Delivery_Trip": _dtChk.rows[i].F_Delivery_Trip
                            },
                        });
                        if (_dtChkPOM.rows == null) {
                            pdsno = facflag + String(_dtChk.rows[i].F_Delivery_Date).substring(2, 6)
                                + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                                + '01';
                        }
                        else {
                            pdsno = facflag + _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                                + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                                + String(_dtChkPOM.rows[i].F_OrderNo + 1).padStart(2, '0');
                        }
                    }
                    else { //'of i <> 0 
                        pdsno = facflag + _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                            + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                            + String(Number(pdsno.substring(pdsno.length - 3)) + 1).padStart(3, '0');
                    }
                    barcode = CheckSum(pdsno); //'sPDS_No
                    xItem.progress({ id: 'prgProcess', current: 65, label: 'Update TB_Delivery.F_HMMT_PDS : {{##.##}} %' });
                    yield xAjax.Execute({
                        data: {
                            "Module": "[exec].[spKBNOR440_06_U]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode,
                            "@PDS_No": sPDS_No,
                            "@F_Collect_Date": _dtChkGet.rows[i].F_Collect_Date,
                            "@F_Delivery_Date": _dtChkGet.rows[i].F_Delivery_Date
                        },
                    });
                    var _dtChkPOM = yield xAjax.ExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR440_04]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode
                        },
                    });
                    if (_dtChkPOM.rows != null) {
                        xItem.progress({ id: 'prgProcess', current: 70, label: 'INSERT TB_PDS_HEADER : {{##.##}} %' });
                        yield xAjax.Execute({
                            data: {
                                "Module": "[exec].[spKBNOR440_07_I]",
                                "@pOrderType": "U",
                                "@pPlant": ajexHeader.Plant,
                                "@pUserCode": ajexHeader.UserCode,
                                "@PDS_No": pdsno,
                                "@F_Issued_Date": issueddate,
                                "@F_Dept_Code": _dtChkPOM.rows[i].F_Dept_Code,
                                "@F_Cr": _dtChkPOM.rows[i].F_Cr,
                                "@F_DR": _dtChkPOM.rows[i].F_DR,
                                "@F_Value2": _dtChkPOM.rows[i].F_Value2,
                                "@F_Barcode": barcode,
                                "@F_Delivery_Date": _dtChk.rows[i].F_Delivery_Date,
                                "@F_Delivery_Trip": _dtChk.rows[i].F_Delivery_Trip,
                                "@F_Supplier_Cd": _dtChk.rows[i].F_Supplier_Cd,
                                "@F_Supplier_Plant": _dtChk.rows[i].F_Supplier_Plant,
                                "@F_Store_CD": _dtChk.rows[i].F_Store_CD
                            },
                        });
                    }
                    xItem.progress({ id: 'prgProcess', current: 75, label: 'INSERT TB_PDS_DETAIL : {{##.##}} %' });
                    yield xAjax.Execute({
                        data: {
                            "Module": "[exec].[spKBNOR440_07_I_DT]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode,
                            "@PDS_No": pdsno,
                            "@F_Delivery_Date": _dtChk.rows[i].F_Delivery_Date,
                            "@F_Delivery_Trip": _dtChk.rows[i].F_Delivery_Trip,
                            "@F_Supplier_Cd": _dtChk.rows[i].F_Supplier_Cd,
                            "@F_Supplier_Plant": _dtChk.rows[i].F_Supplier_Plant,
                            "@F_Store_CD": _dtChk.rows[i].F_Store_CD,
                        },
                    });
                    xItem.progress({ id: 'prgProcess', current: 80, label: 'Run F_No to Detail : {{##.##}} %' });
                    _dtChkPOM = yield xAjax.ExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR440_08]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode,
                            "@PDS_No": pdsno
                        },
                    });
                    if (_dtChkPOM.rows != null) {
                        for (var j = 0; j < _dtChkPOM.rows.length; j++) {
                            yield xAjax.Execute({
                                data: {
                                    "Module": "[exec].[spKBNOR440_08_U]",
                                    "@pOrderType": "U",
                                    "@pPlant": ajexHeader.Plant,
                                    "@pUserCode": ajexHeader.UserCode,
                                    "@PDS_No": pdsno,
                                    "@F_No": j + 1,
                                    "@F_Part_No": _dtChkPOM.rows[i].F_Part_No,
                                    "@F_Ruibetsu": _dtChkPOM.rows[i].F_Ruibetsu,
                                    "@F_Kanban_No": _dtChkPOM.rows[i].F_Kanban_No
                                },
                            });
                        }
                    }
                }
            }
        });
    };
    ProcessSpecialEmer = function () {
        return __awaiter(this, void 0, void 0, function* () {
            var _dtChk = yield xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR440_09]",
                    "@pOrderType": "U",
                    "@pPlant": ajexHeader.Plant,
                    "@pUserCode": ajexHeader.UserCode
                },
            });
            if (_dtChk.rows != null) {
                for (var i = 0; i < _dtChk.rows.length; i++) {
                    if (i == 0 || _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                        + _dtChk.rows[i].F_Delivery_Trip.padStart(2, '0') != pdsno.substring(2, 8)) {
                        var _dtChkPOM = yield xAjax.ExecuteJSON({
                            data: {
                                "Module": "[exec].[spKBNOR440_09_S]",
                                "@OrderType": "U",
                                "@Plant": ajexHeader.Plant,
                                "@UserCode": ajexHeader.UserCode,
                                "@PDS_No": pdsno
                            },
                        });
                        console(_Dtchkpom);
                        if (_dtChkPOM.rows == null) {
                            pdsno = facflag + String(_dtChk.rows[i].F_Delivery_Date).substring(2, 6)
                                + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                                + '001';
                        }
                        else {
                            pdsno = facflag + _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                                + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                                + String(_dtChkPOM.rows[i].F_OrderNo + 1).padStart(2, '0');
                        }
                    }
                    else { //'of i <> 0 
                        pdsno = facflag + _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                            + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                            + String(Number(pdsno.substring(pdsno.length - 3)) + 1).padStart(3, '0');
                    }
                    barcode = CheckSum(pdsno); //'sPDS_No
                    xItem.progress({ id: 'prgProcess', current: 65, label: 'Update TB_Delivery.F_HMMT_PDS : {{##.##}} %' });
                    yield xAjax.Execute({
                        data: {
                            "Module": "[exec].[spKBNOR440_09_U]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode,
                            "@PDS_No": pdsno,
                            "@F_Delivery_Date": _dtChk.rows[i].F_Delivery_Date,
                            "@F_Delivery_Trip": _dtChk.rows[i].F_Delivery_Trip,
                            "@F_Supplier_Cd": _dtChk.rows[i].F_Supplier_Cd,
                            "@F_Supplier_Plant": _dtChk.rows[i].F_Supplier_Plant,
                            "@F_Store_CD": _dtChk.rows[i].F_Store_CD
                        },
                    });
                    var _dtChkPOM = yield xAjax.ExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR440_04]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode
                        },
                    });
                    if (_dtChkPOM.rows != null) {
                        xItem.progress({ id: 'prgProcess', current: 70, label: 'INSERT TB_PDS_HEADER : {{##.##}} %' });
                        yield xAjax.Execute({
                            data: {
                                "Module": "[exec].[spKBNOR440_09_I]",
                                "@pOrderType": "U",
                                "@pPlant": ajexHeader.Plant,
                                "@pUserCode": ajexHeader.UserCode,
                                "@PDS_No": pdsno,
                                "@F_Issued_Date": issueddate,
                                "@F_Dept_Code": _dtChkPOM.rows[i].F_Dept_Code,
                                "@F_Cr": _dtChkPOM.rows[i].F_Cr,
                                "@F_DR": _dtChkPOM.rows[i].F_DR,
                                "@F_Value2": _dtChkPOM.rows[i].F_Value2,
                                "@F_Barcode": barcode,
                                "@F_Delivery_Date": _dtChk.rows[i].F_Delivery_Date,
                                "@F_Delivery_Trip": _dtChk.rows[i].F_Delivery_Trip,
                                "@F_Supplier_Cd": _dtChk.rows[i].F_Supplier_Cd,
                                "@F_Supplier_Plant": _dtChk.rows[i].F_Supplier_Plant,
                                "@F_Store_CD": _dtChk.rows[i].F_Store_CD,
                                "@F_PDS_Group": _dtChk.rows[i].F_PDS_Group
                            },
                        });
                    }
                    xItem.progress({ id: 'prgProcess', current: 75, label: 'INSERT TB_PDS_DETAIL : {{##.##}} %' });
                    yield xAjax.Execute({
                        data: {
                            "Module": "[exec].[spKBNOR440_09_I_DT]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode,
                            "@PDS_No": pdsno,
                            "@F_Delivery_Date": _dtChk.rows[i].F_Delivery_Date,
                            "@F_Delivery_Trip": _dtChk.rows[i].F_Delivery_Trip,
                            "@F_Supplier_Cd": _dtChk.rows[i].F_Supplier_Cd,
                            "@F_Supplier_Plant": _dtChk.rows[i].F_Supplier_Plant,
                            "@F_Store_CD": _dtChk.rows[i].F_Store_CD,
                            "@F_PDS_Group": _dtChk.rows[i].F_PDS_Group
                        },
                    });
                    xItem.progress({ id: 'prgProcess', current: 80, label: 'Run F_No to Detail : {{##.##}} %' });
                    _dtChkPOM = yield xAjax.ExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR440_08]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode,
                            "@PDS_No": pdsno
                        },
                    });
                    if (_dtChkPOM.rows != null) {
                        for (var j = 0; j < _dtChkPOM.rows.length; j++) {
                            yield xAjax.Execute({
                                data: {
                                    "Module": "[exec].[spKBNOR440_08_U]",
                                    "@pOrderType": "U",
                                    "@pPlant": ajexHeader.Plant,
                                    "@pUserCode": ajexHeader.UserCode,
                                    "@PDS_No": pdsno,
                                    "@F_No": j + 1,
                                    "@F_Part_No": _dtChkPOM.rows[i].F_Part_No,
                                    "@F_Ruibetsu": _dtChkPOM.rows[i].F_Ruibetsu,
                                    "@F_Kanban_No": _dtChkPOM.rows[i].F_Kanban_No
                                },
                            });
                        }
                    }
                }
                xSwal.success("");
            }
        });
    };
    ProcessOrderTacoma = function (TypePart_, TypeSpc_) {
        return __awaiter(this, void 0, void 0, function* () {
            facflag = '1L';
            var _dtChk = yield xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR440_10]",
                    "@pOrderType": "U",
                    "@pPlant": ajexHeader.Plant,
                    "@pUserCode": ajexHeader.UserCode,
                    "@TypePart": TypePart_,
                    "@TypeSpc": TypeSpc_
                },
            });
            if (_dtChk.rows != null) {
                for (var i = 0; i < _dtChk.rows.length; i++) {
                    if (i == 0 || _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                        + _dtChk.rows[i].F_Delivery_Trip.padStart(2, '0') != pdsno.substring(2, 8)) {
                        var _dtChkPOM = yield xAjax.ExecuteJSON({
                            data: {
                                "Module": "[exec].[spKBNOR440_10_S]",
                                "@OrderType": "U",
                                "@Plant": ajexHeader.Plant,
                                "@UserCode": ajexHeader.UserCode,
                                "@F_OrderNo": _dtChk.rows[i].F_Delivery_Date.substring(2, 6) + _dtChk.rows[i].F_Delivery_Trip.padStart(2, '0'),
                                "@TypePart": TypePart_,
                                "@TypeSpc": TypeSpc_
                            },
                        });
                        if (_dtChkPOM.rows == null) {
                            pdsno = facflag + String(_dtChk.rows[i].F_Delivery_Date).substring(2, 6)
                                + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                                + '01';
                        }
                        else {
                            pdsno = facflag + _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                                + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                                + String(_dtChkPOM.rows[i].F_OrderNo + 1).padStart(2, '0');
                        }
                    }
                    else { //'of i <> 0 
                        pdsno = facflag + _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                            + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                            + String(Number(pdsno.substring(pdsno.length - 3)) + 1).padStart(3, '0');
                    }
                    barcode = CheckSum(pdsno); //'sPDS_No
                    xItem.progress({ id: 'prgProcess', current: 65, label: 'Update TB_Delivery.F_HMMT_PDS : {{##.##}} %' });
                    yield xAjax.Execute({
                        data: {
                            "Module": "[exec].[spKBNOR440_10_U]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode,
                            "@PDS_No": pdsno,
                            "@TypePart": TypePart_,
                            "@TypeSpc": TypeSpc_,
                            "@F_Delivery_Date": _dtChk.rows[i].F_Delivery_Date,
                            "@F_Delivery_Trip": _dtChk.rows[i].F_Delivery_Trip,
                            "@F_Supplier_Cd": _dtChk.rows[i].F_Supplier_Cd,
                            "@F_Supplier_Plant": _dtChk.rows[i].F_Supplier_Plant,
                            "@F_Store_CD": _dtChk.rows[i].F_Store_CD
                        },
                    });
                    var _dtChkPOM = yield xAjax.ExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR440_04]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode
                        },
                    });
                    if (_dtChkPOM.rows != null) {
                        xItem.progress({ id: 'prgProcess', current: 70, label: 'INSERT TB_PDS_HEADER : {{##.##}} %' });
                        yield xAjax.Execute({
                            data: {
                                "Module": "[exec].[spKBNOR440_10_I]",
                                "@pOrderType": "U",
                                "@pPlant": ajexHeader.Plant,
                                "@pUserCode": ajexHeader.UserCode,
                                "@PDS_No": pdsno,
                                "@F_Issued_Date": issueddate,
                                "@F_Dept_Code": _dtChkPOM.rows[i].F_Dept_Code,
                                "@F_Cr": _dtChkPOM.rows[i].F_Cr,
                                "@F_DR": _dtChkPOM.rows[i].F_DR,
                                "@F_Value2": _dtChkPOM.rows[i].F_Value2,
                                "@F_Barcode": barcode,
                                "@F_Delivery_Date": _dtChk.rows[i].F_Delivery_Date,
                                "@F_Delivery_Trip": _dtChk.rows[i].F_Delivery_Trip,
                                "@F_Supplier_Cd": _dtChk.rows[i].F_Supplier_Cd,
                                "@F_Supplier_Plant": _dtChk.rows[i].F_Supplier_Plant,
                                "@F_Store_CD": _dtChk.rows[i].F_Store_CD,
                                "@TypePart": TypePart_,
                                "@TypeSpc": TypeSpc_
                            },
                        });
                    }
                    xItem.progress({ id: 'prgProcess', current: 75, label: 'INSERT TB_PDS_DETAIL : {{##.##}} %' });
                    yield xAjax.Execute({
                        data: {
                            "Module": "[exec].[spKBNOR440_10_I_DT]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode,
                            "@PDS_No": pdsno,
                            "@F_Delivery_Date": _dtChk.rows[i].F_Delivery_Date,
                            "@F_Delivery_Trip": _dtChk.rows[i].F_Delivery_Trip,
                            "@F_Supplier_Cd": _dtChk.rows[i].F_Supplier_Cd,
                            "@F_Supplier_Plant": _dtChk.rows[i].F_Supplier_Plant,
                            "@F_Store_CD": _dtChk.rows[i].F_Store_CD,
                            "@TypePart": TypePart_,
                            "@TypeSpc": TypeSpc_
                        },
                    });
                    xItem.progress({ id: 'prgProcess', current: 80, label: 'Run F_No to Detail : {{##.##}} %' });
                    _dtChkPOM = yield xAjax.ExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR440_08]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode,
                            "@PDS_No": pdsno
                        },
                    });
                    if (_dtChkPOM.rows != null) {
                        for (var j = 0; j < _dtChkPOM.rows.length; j++) {
                            yield xAjax.Execute({
                                data: {
                                    "Module": "[exec].[spKBNOR440_08_U]",
                                    "@pOrderType": "U",
                                    "@pPlant": ajexHeader.Plant,
                                    "@pUserCode": ajexHeader.UserCode,
                                    "@PDS_No": pdsno,
                                    "@F_No": j + 1,
                                    "@F_Part_No": _dtChkPOM.rows[i].F_Part_No,
                                    "@F_Ruibetsu": _dtChkPOM.rows[i].F_Ruibetsu,
                                    "@F_Kanban_No": _dtChkPOM.rows[i].F_Kanban_No
                                },
                            });
                        }
                    }
                }
            }
        });
    };
    ProcessOrderWareHouse = function () {
        return __awaiter(this, void 0, void 0, function* () {
            //'**************Start F_OrderType = 'U' AND F_Type='Tacoma' AND F_Type_Spc=''
            facflag = '1L';
            var _dtChk = yield xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR440_11]",
                    "@pOrderType": "U",
                    "@pPlant": ajexHeader.Plant,
                    "@pUserCode": ajexHeader.UserCode
                },
            });
            if (_dtChk.rows != null) {
                for (var i = 0; i < _dtChk.rows.length; i++) {
                    if (i == 0 || _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                        + _dtChk.rows[i].F_Delivery_Trip.padStart(2, '0') != pdsno.substring(2, 8)) {
                        var _dtChkPOM = yield xAjax.ExecuteJSON({
                            data: {
                                "Module": "[exec].[spKBNOR440_11_S]",
                                "@OrderType": "U",
                                "@Plant": ajexHeader.Plant,
                                "@UserCode": ajexHeader.UserCode,
                                "@F_OrderNo": _dtChk.rows[i].F_Delivery_Date.substring(2, 6) + _dtChk.rows[i].F_Delivery_Trip.padStart(2, '0')
                            },
                        });
                        if (_dtChkPOM.rows == null) {
                            pdsno = facflag + String(_dtChk.rows[i].F_Delivery_Date).substring(2, 6)
                                + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                                + '01';
                        }
                        else {
                            pdsno = facflag + _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                                + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                                + String(_dtChkPOM.rows[i].F_OrderNo + 1).padStart(2, '0');
                        }
                    }
                    else { //'of i <> 0 
                        pdsno = facflag + _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                            + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                            + String(Number(pdsno.substring(pdsno.length - 3)) + 1).padStart(3, '0');
                    }
                    barcode = CheckSum(pdsno); //'sPDS_No
                    xItem.progress({ id: 'prgProcess', current: 65, label: 'Update TB_Delivery.F_HMMT_PDS : {{##.##}} %' });
                    yield xAjax.Execute({
                        data: {
                            "Module": "[exec].[spKBNOR440_11_U]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode,
                            "@PDS_No": pdsno,
                            "@F_Delivery_Date": _dtChk.rows[i].F_Delivery_Date,
                            "@F_Delivery_Trip": _dtChk.rows[i].F_Delivery_Trip,
                            "@F_Supplier_Cd": _dtChk.rows[i].F_Supplier_Cd,
                            "@F_Supplier_Plant": _dtChk.rows[i].F_Supplier_Plant,
                            "@F_Store_CD": _dtChk.rows[i].F_Store_CD
                        },
                    });
                    var _dtChkPOM = yield xAjax.ExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR440_04]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode
                        },
                    });
                    if (_dtChkPOM.rows != null) {
                        xItem.progress({ id: 'prgProcess', current: 70, label: 'INSERT TB_PDS_HEADER : {{##.##}} %' });
                        yield xAjax.Execute({
                            data: {
                                "Module": "[exec].[spKBNOR440_11_I]",
                                "@pOrderType": "U",
                                "@pPlant": ajexHeader.Plant,
                                "@pUserCode": ajexHeader.UserCode,
                                "@PDS_No": pdsno,
                                "@F_Issued_Date": issueddate,
                                "@F_Dept_Code": _dtChkPOM.rows[i].F_Dept_Code,
                                "@F_Cr": _dtChkPOM.rows[i].F_Cr,
                                "@F_DR": _dtChkPOM.rows[i].F_DR,
                                "@F_Value2": _dtChkPOM.rows[i].F_Value2,
                                "@F_Barcode": barcode,
                                "@F_Delivery_Date": _dtChk.rows[i].F_Delivery_Date,
                                "@F_Delivery_Trip": _dtChk.rows[i].F_Delivery_Trip,
                                "@F_Supplier_Cd": _dtChk.rows[i].F_Supplier_Cd,
                                "@F_Supplier_Plant": _dtChk.rows[i].F_Supplier_Plant,
                                "@F_Store_CD": _dtChk.rows[i].F_Store_CD
                            },
                        });
                    }
                    xItem.progress({ id: 'prgProcess', current: 75, label: 'INSERT TB_PDS_DETAIL : {{##.##}} %' });
                    yield xAjax.Execute({
                        data: {
                            "Module": "[exec].[spKBNOR440_11_I_DT]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode,
                            "@PDS_No": pdsno,
                            "@F_Delivery_Date": _dtChk.rows[i].F_Delivery_Date,
                            "@F_Delivery_Trip": _dtChk.rows[i].F_Delivery_Trip,
                            "@F_Supplier_Cd": _dtChk.rows[i].F_Supplier_Cd,
                            "@F_Supplier_Plant": _dtChk.rows[i].F_Supplier_Plant,
                            "@F_Store_CD": _dtChk.rows[i].F_Store_CD
                        },
                    });
                    xItem.progress({ id: 'prgProcess', current: 80, label: 'Run F_No to Detail : {{##.##}} %' });
                    _dtChkPOM = yield xAjax.ExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR440_08]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode,
                            "@PDS_No": pdsno
                        },
                    });
                    if (_dtChkPOM.rows != null) {
                        for (var j = 0; j < _dtChkPOM.rows.length; j++) {
                            yield xAjax.Execute({
                                data: {
                                    "Module": "[exec].[spKBNOR440_08_U]",
                                    "@pOrderType": "U",
                                    "@pPlant": ajexHeader.Plant,
                                    "@pUserCode": ajexHeader.UserCode,
                                    "@PDS_No": pdsno,
                                    "@F_No": j + 1,
                                    "@F_Part_No": _dtChkPOM.rows[i].F_Part_No,
                                    "@F_Ruibetsu": _dtChkPOM.rows[i].F_Ruibetsu,
                                    "@F_Kanban_No": _dtChkPOM.rows[i].F_Kanban_No
                                },
                            });
                        }
                    }
                }
            }
        });
    };
    GetCheckSum = function () {
        for (let i = 0; i < 10; i++) {
            value[i] = i;
            data[i] = i.toString();
        }
        for (let J = 10; J <= 35; J++) {
            value[J] = J;
            data[J] = String.fromCharCode(55 + J);
        }
        value[36] = 36;
        data[36] = "-";
        value[37] = 37;
        data[37] = ".";
        value[38] = 38;
        data[38] = " ";
        value[39] = 39;
        data[39] = "$";
        value[40] = 40;
        data[40] = "/";
        value[41] = 41;
        data[41] = "+";
        value[42] = 42;
        data[42] = "%";
    };
    CheckSum = function (pds_ = '') {
        let nNo = 0;
        for (let i = 0; i < pds_.length; i++) {
            let nCh = pds_.charAt(i);
            let nValue = 0;
            if (nCh <= "9") {
                nValue = value(nCh);
            }
            else {
                nValue = value(nCh.charCodeAt(0)) - 55;
            }
            nNo = nNo + nValue;
        }
        nNo = nNo % 43;
        return pds_ + data[nNo];
    };
    GetDate = function (delivery_) {
        return __awaiter(this, void 0, void 0, function* () {
            let Get_Date = "";
            let _Store_CD = '1A'; // Assuming this is a constant
            let Sql;
            var _dtChk = yield xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR440_GetDate]",
                    "@F_Store_CD": _Store_CD,
                    "@F_YM": delivery_.substring(0, 6)
                },
            });
            if (_dtChk.rows != null) {
                let _MM = parseInt(sDelivery.substring(6, 8));
                if (_MM > 1) {
                    for (let i = _MM - 1; i >= 1; i--) {
                        if (parseInt(_dtChk.rows[0][i * 2]) + parseInt(_dtChk.rows[0][i * 2 + 1]) === 2) {
                            return delivery_.substring(0, 6) + ('0' + i).slice(-2);
                            break;
                        }
                    }
                }
            }
            let sYMLast = new Date(delivery_.substring(4, 6) + "/" + delivery_.substring(6, 8) + "/" + delivery_.substring(0, 4));
            sYMLast.setMonth(sYMLast.getMonth() - 1);
            let sYMLastStr = sYMLast.getFullYear().toString() + ('0' + (sYMLast.getMonth() + 1)).slice(-2);
            _dtChk = yield xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR440_GetDate]",
                    "@F_Store_CD": _Store_CD,
                    "@F_YM": delivery_.substring(0, 6)
                },
            });
            if (_dtChk.rows != null) {
                for (let i = 31; i >= 1; i--) {
                    if (parseInt(_dtChk.Get_Date1[0][i * 2]) + parseInt(_dtChk.Get_Date1[0][i * 2 + 1]) === 2) {
                        return sYMLastStr + ('0' + i).slice(-2);
                        break;
                    }
                }
            }
            return "";
        });
    };
});
