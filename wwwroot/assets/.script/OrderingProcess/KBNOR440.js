$(document).ready(function () {


    const xKBNOR440 = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Customer PO', 'Part No', 'Supplier', 'Short Name', 'Store Code', 'Kanban No.', 'Delivery Date', 'Delivery Trip', 'Qty', 'Qty KB', 'Import Type'],
            "TH": ['Customer PO', 'Part No', 'Supplier', 'Short Name', 'Store Code', 'Kanban No.', 'Delivery Date', 'Delivery Trip', 'Qty', 'Qty KB', 'Import Type'],
            "JP": ['Customer PO', 'Part No', 'Supplier', 'Short Name', 'Store Code', 'Kanban No.', 'Delivery Date', 'Delivery Trip', 'Qty', 'Qty KB', 'Import Type'],
        },

        ColumnValue: [
            { "data": "F_PDS_No" },
            { "data": "F_Part_No" },
            { "data": "F_Supplier_CD" },
            { "data": "F_Short_name" },
            { "data": "F_Store_CD" },
            { "data": "F_Kanban_No" },
            { "data": "F_Delivery_Date" },
            { "data": "F_Round" },
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

    xAjax.onClick('btnGenerate', async function () {

        MsgBox("Do you want Issued PDS Urgent Data?", MsgBoxStyle.OkCancel, async function () {

            xItem.progress({ id: 'prgProcess', current: 5, label: 'Start Process Special : {{##.##}} %' });
            //Start Process Special  '(9Y Delivery_Trip = 1 only)

            var _dtChk = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR440_01]",
                    "@OrderType": "U",
                    "@Plant": ajexHeader.Plant,
                    "@UserCode": ajexHeader.UserCode
                },
            });
            xItem.progress({ id: 'prgProcess', current: 10, label: 'Delete TB_PDS_DETAIL : {{##.##}} %' });
            if (_dtChk.rows != null) {
                for (var i = 0; i < _dtChk.rows.length; i++) {
                    await xAjax.Execute({
                        data: {
                            "Module": "[exec].[spKBNOR440_01_D]",
                            "@OrderType": "U",
                            "@Plant": ajexHeader.Plant,
                            "@UserCode": ajexHeader.UserCode,
                            "@F_OrderNo": _dtChk.rows[i].F_PDS_No
                        },
                    });
                    //console.log('F_PDS_No : ' + _dtChk.rows[i].F_PDS_No);
                }
            }
            xItem.progress({ id: 'prgProcess', current: 20, label: 'Delete TB_PDS_HEADER : {{##.##}} %' });
            await xAjax.Execute({
                data: {
                    "Module": "[exec].[spKBNOR440_02]",
                    "@OrderType": "U",
                    "@Plant": ajexHeader.Plant,
                    "@UserCode": ajexHeader.UserCode
                },
            });


            xItem.progress({ id: 'prgProcess', current: 30, label: 'Insert TB_PDS_HEADER and TB_PDS_DETAIL : {{##.##}} %' });
            await xAjax.Execute({
                data: {
                    "Module": "[exec].[spKBNOR440_03]",
                    "@OrderType": "U",
                    "@Plant": ajexHeader.Plant,
                    "@UserCode": ajexHeader.UserCode
                },
            });
            if (_dtChk.rows != null) {
                for (var i = 0; i < _dtChk.rows.length; i++) {
                    if (i == 0 || _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                        + _dtChk.rows[i].F_Delivery_Trip.padStart(2, '0') != pdsno.substring(2, 8)) {

                        var _dtChkPOM = await xAjax.ExecuteJSON({
                            data: {
                                "Module": "[exec].[spKBNOR440_03_S]",
                                "@OrderType": "U",
                                "@Plant": ajexHeader.Plant,
                                "@UserCode": ajexHeader.UserCode,
                                "@F_Delivery_Date": _dtChk.rows[i].F_Delivery_Date,
                                "@F_Delivery_Trip": _dtChk.rows[i].F_Delivery_Trip
                            },
                        });
                        if (_dtChkPOM.rows == null) {
                            pdsno = facflag + String(_dtChk.rows[i].F_Delivery_Date).substring(2, 6)
                                + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                                + '001';
                        }else{
                            pdsno = facflag + _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                                + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                                + String(_dtChkPOM.rows[i].F_OrderNo + 1).padStart(2, '0');
                        }

                    } else {    //'of i <> 0 
                        pdsno = facflag + _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                            + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                            + String(Number(pdsno.substring(pdsno.length - 3)) + 1).padStart(3, '0');
                    }
                    
                    barcode = CheckSum(pdsno) //'sPDS_No
                    xItem.progress({ id: 'prgProcess', current: 35, label: 'Update TB_Delivery.F_HMMT_PDS : {{##.##}} %' });
                    await xAjax.Execute({
                        data: {
                            "Module": "[exec].[spKBNOR440_03_U]",
                            "@OrderType": "U",
                            "@Plant": ajexHeader.Plant,
                            "@UserCode": ajexHeader.UserCode,
                            "@PDS_No": pdsno,
                            "@F_Delivery_Date": _dtChk.rows[i].F_Delivery_Date,
                            "@F_Delivery_Time": _dtChk.rows[i].F_Delivery_Time,
                            "@F_Supplier_Cd": _dtChk.rows[i].F_Supplier_Cd,
                            "@F_Supplier_Plant": _dtChk.rows[i].F_Supplier_Plant,
                            "@F_Store_CD": _dtChk.rows[i].F_Store_CD,
                            "@F_Collect_Date": _dtChk.rows[i].F_Collect_Date
                        },
                    });

                    //'Insert to Header
                    xItem.progress({ id: 'prgProcess', current: 40, label: 'INSERT TB_PDS_HEADER : {{##.##}} %' });
                    _dtChkPOM = await xAjax.ExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR440_04]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode
                        },
                    });
                    if (_dtChkPOM.rows != null) {
                        await xAjax.Execute({
                            data: {
                                "Module": "[exec].[spKBNOR440_04_I]",
                                "@pOrderType": "U",
                                "@pPlant": ajexHeader.Plant,
                                "@pUserCode": ajexHeader.UserCode,
                                "@PDS_No": pdsno,
                                "@F_Delivery_Date": _dtChk.rows[i].F_Delivery_Date,
                                "@F_Delivery_Time": _dtChk.rows[i].F_Delivery_Time,
                                "@F_Supplier_Cd": _dtChk.rows[i].F_Supplier_Cd,
                                "@F_Supplier_Plant": _dtChk.rows[i].F_Supplier_Plant,
                                "@F_Store_CD": _dtChk.rows[i].F_Store_CD,
                                "@F_Collect_Date": _dtChk.rows[i].F_Collect_Date,
                                "@F_TYPE_PART": _dtChk.rows[i].F_TYPE_PART,
                                "@F_Dept_Code": _dtChk.rows[i].F_Dept_Code,
                                "@F_Cr": _dtChk.rows[i].F_Cr,
                                "@F_DR": _dtChk.rows[i].F_DR,
                                "@F_Value2": _dtChk.rows[i].F_Value2,
                                "@F_Issued_Date": _dtChk.rows[i].F_Issued_Date,
                                "@F_Barcode": _dtChk.rows[i].F_Barcode
                            },
                        });
                    }

                    ////'Update F_Collect_Date, F_Collect_Time add by Mr.wiboon 20121130 'F_Dock_Code = T.F_Dock_CD,
                    ////'1.  หาค่า Collect Time ก่อนเลย พอดีเค้าหา Dock Code อยู่แล้วเลยแอบใส่ตัีวนี้ด้วยเลย (ของพี่ทิฝากนิดหนึง กรณี 9Y ไม่ต้องคำนวณตัว Collect Date กะ Time ใส่เป็นว่าง ๆ เลย)

                    ////'2.  ตัวนี้คือกรณีที่ Collect Time = "00:00" แปลว่าเป็นรถที่ขนส่งโดย Milk Run ไม่จำเป็นต้องคำนวณเวลา เลยให้ Collect Date = Delivery Date ส่วน Collect Time ก้อ 00:00 เหมือนเดิม
                    ////''Update in case Date of case Supplier Arrival = 00: 00(Milk Run)

                    ////'3.  ตัวนี้คือกรณีที่ Collect Time >= "07:30" แปลว่าเป็น Part ที่มีการขนส่งปรกติ นั่นคือ ออกจาก Supplier ตอนกะเช้า บ่าย ๆ เข้าโรงงาน เลยให้ Collect Date = Delivery Date 
                    ////''Update in case Date of Arrival = Delivery Date

                    ////'4.  ตัวนี้คือกรณีเก็บตก กรณีที่เข้ากะกลางคืน แปลว่าออกจาก Supplier ตี 2 ถึงเรา ตี 3 เครสนี้ ยังถือว่าเป็นวันทำงานวันเดียวกัน เลยให้  Collect Date = Delivery Date 
                    xItem.progress({ id: 'prgProcess', current: 45, label: 'Update Dock Code : {{##.##}} %' });
                    await xAjax.Execute({
                        data: {
                            "Module": "[exec].[spKBNOR440_04_U]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode
                        },
                    });



                    //'5.  ตัวนี้คือกรณีที่ออกจาก Supplier กลางคืนถึงเราเช้าหลัง 07:30 สรุปว่าจะถอยวัน
                    //'' ตัวเรียกนะ 
                    xItem.progress({ id: 'prgProcess', current: 50, label: 'UPDATE TB_PDS_HEADER : {{##.##}} %' });
                    var _dtChkGet = await xAjax.ExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR440_05]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode
                        },
                    });
                    if (_dtChkGet.rows != null) {
                        for (var i = 0; i < _dtChkGet.rows.length; i++) {
                            var _delivery = GetDate(Trim(_dtChkGet.rows[i][0].ToString()));
                            await xAjax.Execute({
                                data: {
                                    "Module": "[exec].[spKBNOR440_05_U]",
                                    "@pOrderType": "U",
                                    "@pPlant": ajexHeader.Plant,
                                    "@pUserCode": ajexHeader.UserCode,
                                    "@PDS_No": pdsno,
                                    "@F_Collect_Date": _dtChkGet.rows[i].F_Collect_Date,
                                    "@F_Delivery_Date": _dtChkGet.rows[i].F_Delivery_Date
                                },
                            });
                        }
                    }


                    xItem.progress({ id: 'prgProcess', current: 55, label: 'INSERT TB_PDS_DETAIL : {{##.##}} %' });
                    await xAjax.Execute({
                        data: {
                            "Module": "[exec].[spKBNOR440_05_I]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode,
                            "@PDS_No": pdsno,
                            "@F_Delivery_Date": _dtChk.rows[i].F_Delivery_Date,
                            "@F_Delivery_Time": _dtChk.rows[i].F_Delivery_Time,
                            "@F_Supplier_Cd": _dtChk.rows[i].F_Supplier_Cd,
                            "@F_Supplier_Plant": _dtChk.rows[i].F_Supplier_Plant,
                            "@F_Store_CD": _dtChk.rows[i].F_Store_CD,
                            "@F_Collect_Date": _dtChk.rows[i].F_Collect_Date,
                            "@F_TYPE_PART": _dtChk.rows[i].F_TYPE_PART
                        },
                    });



                }
            }


            xItem.progress({ id: 'prgProcess', current: 60, label: 'Process Special KPO : {{##.##}} %' });
            await ProcessSpecialKPO();
            xItem.progress({ id: 'prgProcess', current: 90, label: 'Process Special Emer : {{##.##}} %' });
            await ProcessSpecialEmer();

            await ProcessOrderTacoma("T", "T");  //''Assy
            await ProcessOrderTacoma("T", "S");   //''Service
            await ProcessOrderWareHouse();

            await xAjax.xExecute({
                data: {
                    "Module": "[dbo].[SP_Update_KPO_Order]",
                    "@pOrderType": "U",
                    "@pPlant": ajexHeader.Plant
                },
            });

            xItem.progress({ id: 'prgProcess', current: 92, label: 'Update Dock Code AGAIN-2 : {{##.##}} %' });
            await xAjax.Execute({
                data: {
                    "Module": "[exec].[spKBNOR440_12]",
                    "@pOrderType": "U",
                    "@pPlant": ajexHeader.Plant,
                    "@pUserCode": ajexHeader.UserCode,
                },
            });

            xItem.progress({ id: 'prgProcess', current: 94, label: 'Update in case Date of Arrival < Delivery Date : {{##.##}} %' });
            var _dtChk = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR440_13]",
                    "@pOrderType": "U",
                    "@pPlant": ajexHeader.Plant,
                    "@pUserCode": ajexHeader.UserCode
                },
            });
            if (_dtChk.rows != null) {
                for (var i = 0; i < _dtChk.rows.length; i++) {

                    var _delivery = GetDate(Trim(_dtChk.rows[i][0].ToString()));

                    var _dtChk = await xAjax.ExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR440_13_U]",
                            "@pOrderType": "U",
                            "@pPlant": ajexHeader.Plant,
                            "@pUserCode": ajexHeader.UserCode,
                            "@F_Collect_Date": _delivery,
                            "@F_Delivery_Date": _dtChk.rows[i][0].ToString()
                        },
                    });

                }

            }

            xItem.progress({ id: 'prgProcess', current: 96, label: 'Update TEXT DATA : RE-CHECK AGAIN : {{##.##}} %' });
            _dtChk = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR440_14]",
                    "@pOrderType": "U",
                    "@pPlant": ajexHeader.Plant,
                    "@pUserCode": ajexHeader.UserCode
                },
            });

            xItem.progress({ id: 'prgProcess', current: 98, label: 'Process #TEMP_UPDOC : {{##.##}} %' });
            _dtChk = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR440_15]",
                    "@pOrderType": "U",
                    "@pPlant": ajexHeader.Plant,
                    "@pUserCode": ajexHeader.UserCode
                },
            });

            xItem.progress({ id: 'prgProcess', current: 98, label: 'Process display data : {{##.##}} %' });
            _dtChk = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR440_16]",
                    "@pOrderType": "U",
                    "@pPlant": ajexHeader.Plant,
                    "@pUserCode": ajexHeader.UserCode
                },
            });

            xItem.progress({ id: 'prgProcess', current: 99, label: 'Addition for Generate to OLT ONLY : {{##.##}} %' });
            _dtChk = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR440_17]",
                    "@pOrderType": "U",
                    "@pPlant": ajexHeader.Plant,
                    "@pUserCode": ajexHeader.UserCode
                },
            });

            xItem.progress({ id: 'prgProcess', current: 100, label: 'Generate PDS Completed : {{##.##}} %' });

        })
    });

    ProcessSpecialKPO = async function () {   //'incase S
        var _dtChk = await xAjax.ExecuteJSON({
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

                    var _dtChkPOM = await xAjax.ExecuteJSON({
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
                    } else {
                        pdsno = facflag + _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                            + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                            + String(_dtChkPOM.rows[i].F_OrderNo + 1).padStart(2, '0');
                    }

                } else {    //'of i <> 0 
                    pdsno = facflag + _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                        + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                        + String(Number(pdsno.substring(pdsno.length - 3)) + 1).padStart(3, '0');
                }

                barcode = CheckSum(pdsno) //'sPDS_No
                xItem.progress({ id: 'prgProcess', current: 65, label: 'Update TB_Delivery.F_HMMT_PDS : {{##.##}} %' });
                await xAjax.Execute({
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

                var _dtChkPOM = await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNOR440_04]",
                        "@pOrderType": "U",
                        "@pPlant": ajexHeader.Plant,
                        "@pUserCode": ajexHeader.UserCode
                    },
                });
                if (_dtChkPOM.rows != null) {
                    xItem.progress({ id: 'prgProcess', current: 70, label: 'INSERT TB_PDS_HEADER : {{##.##}} %' });
                    await xAjax.Execute({
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
                await xAjax.Execute({
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
                _dtChkPOM = await xAjax.ExecuteJSON({
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
                        await xAjax.Execute({
                            data: {
                                "Module": "[exec].[spKBNOR440_08_U]",
                                "@pOrderType": "U",
                                "@pPlant": ajexHeader.Plant,
                                "@pUserCode": ajexHeader.UserCode,
                                "@PDS_No": pdsno,
                                "@F_No": j+1,
                                "@F_Part_No": _dtChkPOM.rows[i].F_Part_No,
                                "@F_Ruibetsu": _dtChkPOM.rows[i].F_Ruibetsu,
                                "@F_Kanban_No": _dtChkPOM.rows[i].F_Kanban_No
                            },
                        });
                    }
                }
            }
        }
    }
    ProcessSpecialEmer = async function () {   //'Incase Others
        var _dtChk = await xAjax.ExecuteJSON({
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

                    var _dtChkPOM = await xAjax.ExecuteJSON({
                        data: {
                            "Module": "[exec].[spKBNOR440_09_S]",
                            "@OrderType": "U",
                            "@Plant": ajexHeader.Plant,
                            "@UserCode": ajexHeader.UserCode,
                            "@PDS_No": pdsno
                        },
                    });
                    if (_dtChkPOM.rows == null) {
                        pdsno = facflag + String(_dtChk.rows[i].F_Delivery_Date).substring(2, 6)
                            + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                            + '001';
                    } else {
                        pdsno = facflag + _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                            + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                            + String(_dtChkPOM.rows[i].F_OrderNo + 1).padStart(2, '0');
                    }

                } else {    //'of i <> 0 
                    pdsno = facflag + _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                        + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                        + String(Number(pdsno.substring(pdsno.length - 3)) + 1).padStart(3, '0');
                }

                barcode = CheckSum(pdsno) //'sPDS_No
                xItem.progress({ id: 'prgProcess', current: 65, label: 'Update TB_Delivery.F_HMMT_PDS : {{##.##}} %' });
                await xAjax.Execute({
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

                var _dtChkPOM = await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNOR440_04]",
                        "@pOrderType": "U",
                        "@pPlant": ajexHeader.Plant,
                        "@pUserCode": ajexHeader.UserCode
                    },
                });
                if (_dtChkPOM.rows != null) {
                    xItem.progress({ id: 'prgProcess', current: 70, label: 'INSERT TB_PDS_HEADER : {{##.##}} %' });
                    await xAjax.Execute({
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
                await xAjax.Execute({
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
                _dtChkPOM = await xAjax.ExecuteJSON({
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
                        await xAjax.Execute({
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



    }
    ProcessOrderTacoma = async function (TypePart_, TypeSpc_) {    //''Assy
        facflag = '1L';
        var _dtChk = await xAjax.ExecuteJSON({
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

                    var _dtChkPOM = await xAjax.ExecuteJSON({
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
                    } else {
                        pdsno = facflag + _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                            + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                            + String(_dtChkPOM.rows[i].F_OrderNo + 1).padStart(2, '0');
                    }

                } else {    //'of i <> 0 
                    pdsno = facflag + _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                        + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                        + String(Number(pdsno.substring(pdsno.length - 3)) + 1).padStart(3, '0');
                }

                barcode = CheckSum(pdsno) //'sPDS_No
                xItem.progress({ id: 'prgProcess', current: 65, label: 'Update TB_Delivery.F_HMMT_PDS : {{##.##}} %' });
                await xAjax.Execute({
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

                var _dtChkPOM = await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNOR440_04]",
                        "@pOrderType": "U",
                        "@pPlant": ajexHeader.Plant,
                        "@pUserCode": ajexHeader.UserCode
                    },
                });
                if (_dtChkPOM.rows != null) {
                    xItem.progress({ id: 'prgProcess', current: 70, label: 'INSERT TB_PDS_HEADER : {{##.##}} %' });
                    await xAjax.Execute({
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
                await xAjax.Execute({
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
                _dtChkPOM = await xAjax.ExecuteJSON({
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
                        await xAjax.Execute({
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

    }
    ProcessOrderWareHouse = async function () {   //'Incase Tacoma Project 
        //'**************Start F_OrderType = 'U' AND F_Type='Tacoma' AND F_Type_Spc=''
        facflag = '1L';
        var _dtChk = await xAjax.ExecuteJSON({
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

                    var _dtChkPOM = await xAjax.ExecuteJSON({
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
                    } else {
                        pdsno = facflag + _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                            + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                            + String(_dtChkPOM.rows[i].F_OrderNo + 1).padStart(2, '0');
                    }

                } else {    //'of i <> 0 
                    pdsno = facflag + _dtChk.rows[i].F_Delivery_Date.substring(2, 6)
                        + String(_dtChk.rows[i].F_Delivery_Trip).padStart(2, '0')
                        + String(Number(pdsno.substring(pdsno.length - 3)) + 1).padStart(3, '0');
                }

                barcode = CheckSum(pdsno) //'sPDS_No
                xItem.progress({ id: 'prgProcess', current: 65, label: 'Update TB_Delivery.F_HMMT_PDS : {{##.##}} %' });
                await xAjax.Execute({
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

                var _dtChkPOM = await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNOR440_04]",
                        "@pOrderType": "U",
                        "@pPlant": ajexHeader.Plant,
                        "@pUserCode": ajexHeader.UserCode
                    },
                });
                if (_dtChkPOM.rows != null) {
                    xItem.progress({ id: 'prgProcess', current: 70, label: 'INSERT TB_PDS_HEADER : {{##.##}} %' });
                    await xAjax.Execute({
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
                await xAjax.Execute({
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
                _dtChkPOM = await xAjax.ExecuteJSON({
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
                        await xAjax.Execute({
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
    }

    GetCheckSum = function() {
        for (let i = 0; i < 10; i++) {
            value[i] = i;
            data[i] = i.toString();
        }
        for (let J = 10; J <= 35; J++) {
            value[J] = J;
            data[J] = String.fromCharCode(55 + J);
        }
        value[36] = 36; data[36] = "-";
        value[37] = 37; data[37] = ".";
        value[38] = 38; data[38] = " ";
        value[39] = 39; data[39] = "$";
        value[40] = 40; data[40] = "/";
        value[41] = 41; data[41] = "+";
        value[42] = 42; data[42] = "%";
    }

    CheckSum = function (pds_ = '') {
        let nNo = 0;
        for (let i = 0; i < pds_.length; i++) {
            let nCh = pds_.charAt(i);
            let nValue = 0;
            if (nCh <= "9") {
                nValue = value(nCh);
            } else {
                nValue = value(nCh.charCodeAt(0)) - 55;
            }
            nNo = nNo + nValue;
        }
        nNo = nNo % 43;
        return pds_ + data[nNo];
    }

    GetDate = async function (delivery_) {
        let Get_Date = "";
        let _Store_CD = '1A'; // Assuming this is a constant
        let Sql;

        var _dtChk = await xAjax.ExecuteJSON({
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

        _dtChk = await xAjax.ExecuteJSON({
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
    }









    //KBNOR440_02 = function () {
    //    xItem.progress({ id: 'prgProcess', current: 20, label: 'Delete TB_PDS_HEADER : {{##.##}} %' });
    //    xAjax.Post({
    //        url: 'EXEC/eExecuteJSON',
    //        data: {
    //            "Module": "[exec].[spKBNOR440_02]",
    //            "OrderType": "U",
    //            "Plant": ajexHeader.Plant,
    //            "UserCode": ajexHeader.UserCode
    //        },
    //        then: function (result) {
    //            //console.log('deletePDSHeader');
    //            if (result.response == 'OK') {
    //                KBNOR440_03();
    //            }

    //        }
    //    })
    //}


    //KBNOR440_03 = function () {
    //    xItem.progress({ id: 'prgProcess', current: 30, label: 'Insert TB_PDS_HEADER and TB_PDS_DETAIL : {{##.##}} %' });
    //    xAjax.Post({
    //        url: 'KBNOR440/KBNOR440_03',
    //        data: {
    //            "OrderType": "U",
    //            "Plant": ajexHeader.Plant,
    //            "UserCode": ajexHeader.UserCode
    //        },
    //        then: function (result) {
    //            //console.log('deletePDSHeader');
    //            if (result.response == 'OK') {
    //                KBNOR440_06();
    //            }

    //        }
    //    })
    //}

    //KBNOR440_06 = function () {
    //    xItem.progress({ id: 'prgProcess', current: 60, label: 'Process Special KPO : {{##.##}} %' });
    //    xAjax.Post({
    //        url: 'KBNOR440/KBNOR440_06',
    //        data: {
    //            "OrderType": "U",
    //            "Plant": ajexHeader.Plant,
    //            "UserCode": ajexHeader.UserCode
    //        },
    //        then: function (result) {
    //            //console.log('deletePDSHeader');
    //            if (result.response == 'OK') {
    //                updateRemark();
    //            }

    //        }
    //    })
    //}




    //xAjax.onClick('btnReport', async function () {
    //    //updateRemark();
    //    console.clear();

    //    //console.log('Start A');
    //    var _result = await xAjax.ExecuteJSON({
    //        data: {
    //            "Module": "[exec].[spTest]",
    //            "Plant": 3,
    //            "len": 1000,
    //        },
    //    });
    //    for (var i = 0; i < _result.data.length; i++) {
    //        _dtB = await xAjax.ExecuteJSON({
    //            data: {
    //                "Module": "[exec].[spTest]",
    //                "Plant": 3,
    //                "len": 5,
    //            },
    //        });

    //        //console.log('Sub Process [' + (i) + '] ');

    //        xItem.progress({ id: 'prgProcess', current: i, max: _result.data.length, label: 'Process A : {{##.##}} %' });
    //    }
    //    //console.log('End A');


    //    //console.log('Start B');
    //    _result = await xAjax.ExecuteJSON({
    //        data: {
    //            "Module": "[exec].[spTest]",
    //            "Plant": 3,
    //            "len": 1000,
    //        },
    //    });
    //    for (var i = 0; i < _result.data.length; i++) {
    //        xItem.progress({ id: 'prgProcess', current: i, max: _result.data.length, label: 'Process B : {{##.##}} %' });
    //        //console.log('item B [' + i + '] : ' + _result.data[i].F_PDS_No);
    //    }
    //    //console.log('End B');
    //});





    ////xAjax.onClick('btnReport', async function () {
    ////    //updateRemark();

    ////    console.log('Start A');
    ////    var _result = await getData(50000);
    ////    for (var i = 0; i < _result.data.length; i++) {
    ////        console.log('item A [' + i + '] : ' + _result.data[i].F_PDS_No);
    ////    }
    ////    console.log('End A');


    ////    console.log('Start B');
    ////    _result = await getData(10000);
    ////    for (var i = 0; i < _result.data.length; i++) {
    ////        console.log('item B [' + i + '] : ' + _result.data[i].F_PDS_No);
    ////    }
    ////    console.log('End B');
    ////});

    //PostAsync= function(pConfig = null) {
    //    if (pConfig != null) {
    //        //console.log(pConfig.data);

    //        let _url = (_NAMESPACE_ != '' ? '/' + _NAMESPACE_ : '') + '/' + pConfig.url;
    //        let _postData = (pConfig.data != undefined ? ajaxPostData(pConfig.data) : null);

    //        //console.log(_postData);

    //        return $.ajax({
    //            type: "POST",
    //            contentType: "application/json; charset=utf-8",
    //            dataType: "json",
    //            headers: ajexHeader,
    //            url: '/EXEC/ExecuteJSON',
    //            data: _postData,
    //            success: function (result) {

    //                //console.log(result);

    //                if (result.response == 'OK') {
    //                    console.log('Process Complete');
    //                }

    //            }
    //        });

    //        //return $.ajax({
    //        //    type: "POST",
    //        //    contentType: "application/json; charset=utf-8",
    //        //    dataType: "json",
    //        //    headers: ajexHeader,
    //        //    url: _url,
    //        //    data: _postData,
    //        //    success: function (result) {
    //        //        console.log('PostAsync Complete');

    //        //    },
    //        //    error: function (result) {
    //        //        console.error('Ajax.Post: ' + result.responseText);
    //        //        xSplash.hide();
    //        //    }
    //        //});

    //    }
    //}


    //getData = function (p) {
    //    var data = {
    //        "Module": "[exec].[spTest]",
    //        "Plant": 3,
    //        "len": p,
    //    };


    //    let _postData = (data != undefined ? ajaxPostData(data) : null);
    //    return $.ajax({
    //        type: "POST",
    //        contentType: "application/json; charset=utf-8",
    //        dataType: "json",
    //        headers: ajexHeader,
    //        url: '/EXEC/ExecuteJSON',
    //        data: _postData,
    //        success: function (result) {

    //            //console.log(result);

    //            if (result.response == 'OK') {
    //                console.log('Process Complete');
    //            }

    //        }
    //    });

    //}
})

