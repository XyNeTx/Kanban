$(document).ready(function () {



    const xKBNOR320 = new MasterTemplate({
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


    xKBNOR320.prepare();


    xKBNOR320.initial(function (result) {
        xSplash.hide();
    });


    xAjax.onClick('btnExit', function () {
        xAjax.redirect('KBNOR300');
    });


    xAjax.onClick('btnCalculate', async function () {
        try {
            MsgBox("Do you want Calculate CKD In-House Order?", MsgBoxStyle.OkCancel, async function () {

                xItem.progress({ id: 'prgProcess', current: 5, label: 'Start Calculate CKD In-House : {{##.##}} %' });

                //var _dt = await xAjax.ExecuteJSON({
                //    data: {
                //        "StoreName": "[exec].[spMSParameter]",
                //        "@pUserCode": ajexHeader.UserCode,
                //        "@pCode": "CI_CKD",
                //        "@pValue1": "",
                //        "@pValue2": "3",
                //        "@pValue3": "",
                //        "@pValue4": "",
                //        "@pValue5": "",
                //        "@pDeptCode": "",
                //        "@pCR": "",
                //        "@pDR": ""
                //    },
                //});

                var _dt = await xAjax.xExecuteJSON({
                    data: {
                        "StoreName": "[exec].[spMSParameter] '"
                            + ajexHeader.UserCode + "' "
                            + ", 'CI_CKD', @pValue2='3' "
                            + ", @ErrorMessage='' "
                    },
                });

                //console.log(_dt);






                var _dtPartControl = await xAjax.xExecuteJSON({
                    data: {
                        "StoreName": "[exec].[spKBNOR320]" 
                    },
                });
                console.log(_dtPartControl);

                if (_dtPartControl.rows != null) {

                    //'หาวันที่เริ่มต้น ที่มีการเปลี่ยนแปลงข้อมูล
                    for (var i = 0; i < _arMaster.length; i++) {

                        _dt = await xAjax.xExecuteJSON({
                            data: {
                                "StoreName": "[exec].[spKBNOR320_01]",
                                "@F_Supplier_Code": _dtPartControl.rows[i].F_Supplier_Code,
                                "@F_Supplier_Plant": _dtPartControl.rows[i].F_Supplier_Plant,
                                "@F_Part_No": _dtPartControl.rows[i].F_Part_No,
                                "@F_Ruibetsu": _dtPartControl.rows[i].F_Ruibetsu,
                                "@F_Kanban_No": _dtPartControl.rows[i].F_Kanban_No,
                                "@F_Store_Code": _dtPartControl.rows[i].F_Store_Code
                            },
                        });
                        var _OrderType = 'Daily';
                        console.log(_OrderType);
                        if (_dt.rows != null) _OrderType = _dt.rows[0].F_Type_Order.trim();



                        _dt = await xAjax.xExecuteJSON({
                            data: {
                                "StoreName": "[exec].[spKBNOR320_02]",
                                "@F_Supplier_Code": _dtPartControl.rows[i].F_Supplier_Code,
                                "@F_Supplier_Plant": _dtPartControl.rows[i].F_Supplier_Plant,
                                "@F_Part_No": _dtPartControl.rows[i].F_Part_No,
                                "@F_Ruibetsu": _dtPartControl.rows[i].F_Ruibetsu,
                                "@F_Kanban_No": _dtPartControl.rows[i].F_Kanban_No,
                                "@F_Store_Code": _dtPartControl.rows[i].F_Store_Code
                            },
                        });
                        var _StartDate = xDate.Date('yyyyMMdd');
                        console.log(_StartDate);
                        if (_dt.rows != null) _StartDate = _dt.rows[0].F_Process_Date.trim();


                        var _storeName = '[CKD_Inhouse].[sp_NumberOfDayToPreview]';
                        if (_OrderType == 'Non Daily') {
                            //'หาวันที่สุดท้าย ที่มีต้องใช้แสดงข้อมูล เพื่อเอามาเป็นวันที่สิ้นสุดที่จะต้องคำนวน ไม่ต้องคำนวนทั้งเดือน
                            _storeName = '[CKD_Inhouse].[sp_Non_NumberOfDayToPreview]';
                        }

                        _dt = await xAjax.xExecuteJSON({
                            data: {
                                "StoreName": _storeName,
                                "@Plant": '3',
                                "@Supplier_Code": _dtPartControl.rows[i].F_Supplier_Code,
                                "@Supplier_Plant": _dtPartControl.rows[i].F_Supplier_Plant,
                                "@Store_Code": '1A',
                                "@Date": xDate.Date('yyyyMMdd')
                            },
                        });
                        var _EndDate = xDate.Date('yyyyMMdd');
                        console.log(_EndDate);
                        if (_dt.rows != null) _EndDate = _dt.rows[0].End_Date.trim();



                        //'ส่งวันที่ ที่มีการเปลี่ยนแปลงข้อมูลไปคำนวน BL ใหม่
                        //re_Calculate_Trail(start_Date, end_Date, rowIndex)

                        //'ดึงข้อมูล Last Trip ของวันที่ start_Date - 1 จะเริ่มคำนวณตั้งแต่ Trip1 ของวัน start_Date ที่ถูกส่งมาเลย
                        //dateLast_Trip = Date.ParseExact(Mid(start_Date, 7, 2) & "/" & Mid(start_Date, 5, 2) & "/" & Mid(start_Date, 1, 4), "dd/MM/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo)
                        //dateECI = get_ECIDate(start_Date, end_Date, index)
                        _dateLastTrip = '';
                        dateECI = '';

                        //'ดึงข้อมูล Balance ของวันก่อนหน้านั้น 1 วัน
                        _dt = await xAjax.xExecuteJSON({
                            data: {
                                "StoreName": '[CKD_Inhouse].[sp_autoRecalculateBL_First]',
                                "@Date": _dateLastTrip-1,
                                "@Supplier_Code": _dtPartControl.rows[i].F_Supplier_Code,
                                "@Supplier_Plant": _dtPartControl.rows[i].F_Supplier_Plant,
                                "@Part_No": _dtPartControl.rows[i].F_Part_No,
                                "@Ruibetsu": _dtPartControl.rows[i].F_Ruibetsu,
                                "@Kanban_No": _dtPartControl.rows[i].F_Kanban_No,
                                "@Store_Code": _dtPartControl.rows[i].F_Store_Code
                            },
                        });
                        var LastBLPlan = 0, LastBLActual = 0, FromSetStock = false;
                        if (_dt.rows != null) {
                            LastBLPlan = (LastBLPlan >= 0 ? LastBLPlan : 0);
                            LastBLActual = (LastBLActual >= 0 ? LastBLActual : 0);
                            FromSetStock = dt.rows[0].F_Not_Recalculate;
                        }



                        //'ดึงข้อมูลที่จำเป็นต่อการคำนวน BL
                        _dt = await xAjax.xExecuteJSON({
                            data: {
                                "StoreName": '[CKD_Inhouse].[sp_autoRecalculateBL_Second]',
                                "@StartDate": _dateLastTrip,
                                "@EndDate": _EndDate,
                                "@Supplier_Code": _dtPartControl.rows[i].F_Supplier_Code,
                                "@Supplier_Plant": _dtPartControl.rows[i].F_Supplier_Plant,
                                "@Part_No": _dtPartControl.rows[i].F_Part_No,
                                "@Ruibetsu": _dtPartControl.rows[i].F_Ruibetsu,
                                "@Kanban_No": _dtPartControl.rows[i].F_Kanban_No,
                                "@Store_Code": _dtPartControl.rows[i].F_Store_Code
                            },
                        });



                        //'ดึงข้อมูล Actaul Receive ที่จำเป็นต่อการคำนวน 
                        _dt = await xAjax.xExecuteJSON({
                            data: {
                                "StoreName": '[CKD_Inhouse].[sp_autoRecalculateBL_Third]',
                                "@StartDate": _dateLastTrip,
                                "@EndDate": _EndDate,
                                "@Supplier_Code": _dtPartControl.rows[i].F_Supplier_Code,
                                "@Supplier_Plant": _dtPartControl.rows[i].F_Supplier_Plant,
                                "@Part_No": _dtPartControl.rows[i].F_Part_No,
                                "@Ruibetsu": _dtPartControl.rows[i].F_Ruibetsu,
                                "@Kanban_No": _dtPartControl.rows[i].F_Kanban_No,
                                "@Store_Code": _dtPartControl.rows[i].F_Store_Code
                            },
                        });








                    }











                }


            })

        } catch (error) {

            await xAjax.xExecuteJSON({
                data: {
                    "StoreName": "[exec].[spMSParameter] '"
                        + ajexHeader.UserCode + "' "
                        + ", 'CI_CKD', @pValue2='1' "
                        + ", @ErrorMessage='' "
                },
            });
            MsgBox("Error Sub : BGWorker_DoWork.", MsgBoxStyle.Critical, "ERROR");

            xItem.progress({ id: 'prgProcess', current: 0, label: 'Process Not Complete!!! : {{##.##}} %' });
        }
    });

    



})

