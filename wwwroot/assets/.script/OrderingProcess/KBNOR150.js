var _CookieProcessDate = _xLib.GetCookie("processDate");
var _CookieLoginDate = _xLib.GetCookie("loginDate");
$(document).ready(function () {

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
            { "data": "F_Delivery_Trip" },
            { "data": "F_Remark" }
        ],
        addnew: false,
    });


    initial = async function () {
        //xItem.progress({ id: 'prgProcess', current: 0, label: 'Process : {{##.##}} %' });

        var _dt = await xAjax.ExecuteJSON({
            data: {
                "Module": "[exec].[spKBNOR150_INITIAL]",
                "@OrderType": "N",
                "@Plant": ajexHeader.Plant,
                "@UserCode": ajexHeader.UserCode
            },
        });

        if (_dt.rows[0].F_Count > 0) {
            MsgBox("กรุณากด Generate PDS อีกครั้ง เนื่องจาก PDS No. นี้มีแล้วในระบบ", MsgBoxStyle.Critical, "MISTAKE DATA")
        } else {
            //''Clear Status Data
            //''Update Status Price of Normal Part: สำหรับกรณีที่กะที่แล้วมีการ Update Flag ไว้และกะนี้เจอราคาแล้ว ให้ Update และ Up ไป E - Pro
            var _dt = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR150]",
                    "@OrderType": "N",
                    "@Plant": ajexHeader.Plant,
                    "@UserCode": ajexHeader.UserCode
                },
            });
            if (_dt.rows != null) xDataTable.bind('#tblMaster', _dt.rows);
            //console.log(_dt);

            var _dtDetail = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR150_DETAIL]",
                    "@OrderType": "N",
                    "@Plant": ajexHeader.Plant,
                    "@UserCode": ajexHeader.UserCode
                },
            });
            if (_dtDetail.rows != null) xDataTable.bind('#tblDetail', _dtDetail.rows);
            if (_dtDetail.rows != null) xDataTable.draw('#tblDetail', function (table) {
                table.rows().every(function () {
                    const _node = this.node();
                    if (this.data().F_Status.trim() != '') {
                        $(_node).css("background-color", "red");
                        $(_node).css("color", "white");
                    }

                });
            });

        }

        xSplash.hide();
    }
    initial();


    xAjax.onClick('btnExit', function () {
        xAjax.redirect('KBNOR100');
    });


    xAjax.onClick('btnExportData', function () {
        xAjax.redirect('KBNOR150EX');
    });



    xAjax.onClick('btnRegister', async function () {
        var F_OrderNo = '';

        try {
            xItem.progress({ id: 'prgProcess', current: 0, label: 'Start Processing... : {{##.##}} %' });
            MsgBox("Do you want Registration Data for Normal Order?",
                MsgBoxStyle.OkCancel,
                async function () {

                    xItem.progress({ id: 'prgProcess', current: 10, label: 'INSERT TB_REC_HEADER AND TB_REC_DETAIL : {{##.##}} %' });

                    var _ProcessDate = _CookieProcessDate.substring(0, 10)
                    var _ProcessShift = _CookieProcessDate.substring(10, 11)

                    await xAjax.xExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR150_Regis]",
                            "@pPlant": ajexHeader.Plant,
                            "@pUser": ajexHeader.UserCode,
                            "@pOrderType": "N",
                            "@pProcessDate": _ProcessDate.replaceAll("-", ""),
                            "@pProcessShift": _ProcessShift,
                        },
                    });

                    xItem.progress({ id: 'prgProcess', current: 100, label: 'Process Regis Completed : {{##.##}} %' });


                    this.initial();
                    MsgBox("Process Registration Data for Normal Order Completed.", MsgBoxStyle.Information, "Process Complete");


                },
                function () {
                    xItem.progress({ id: 'prgProcess', current: 0, label: 'Process : {{##.##}} %' });

                });

        } catch (error) {
            // Code to handle the error
            for (var i = 0; i < _arMaster.length; i++) {

                await xAjax.Execute({
                    data: {
                        "Module": "[exec].[spKBNOR150_EXCEPTION]",
                        "@OrderType": "N",
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
                var _dt = await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNOR150_03]",
                        "@OrderType": "N",
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
                        await xAjax.ExecuteJSON({
                            data: {
                                "Module": "[exec].[spKBNOR150_EXCEPTION_U]",
                                "@OrderType": "N",
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

















})

