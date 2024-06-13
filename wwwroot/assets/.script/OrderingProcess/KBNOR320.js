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
                //        "Module": "[exec].[spMSParameter]",
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
                        "Module": "[exec].[spMSParameter] '"
                            + ajexHeader.UserCode + "' "
                            + ", 'CI_CKD', @pValue2='3' "
                            + ", @ErrorMessage='' "
                    },
                });
                //console.log(_dt);


                var _dtPartControl = await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNOR320]"
                    },
                });
                xItem.progress({ id: 'prgProcess', current: 10, label: 'Get CKD In-House Data for Calculate : {{##.##}} %' });
                //console.log(_dtPartControl);

                if (_dtPartControl.rows != null) {


                    //'หาวันที่เริ่มต้น ที่มีการเปลี่ยนแปลงข้อมูล
                    for (var rowIndex = 0; rowIndex < _dtPartControl.rows.length; rowIndex++) {

                        var _percent = ((rowIndex + 1) / _dtPartControl.rows.length) * 80;

                        console.log(_dtPartControl.rows[rowIndex]);
                        //console.log(_dtPartControl.rows[rowIndex].F_Supplier_Plant);

                        _dt = await xAjax.xExecuteJSON({
                            data: {
                                "Module": "[exec].[spKBNOR320_01]",
                                "@F_Supplier_Code": _dtPartControl.rows[rowIndex].F_Supplier_Code,
                                "@F_Supplier_Plant": _dtPartControl.rows[rowIndex].F_Supplier_Plant,
                                "@F_Part_No": _dtPartControl.rows[rowIndex].F_Part_No,
                                "@F_Ruibetsu": _dtPartControl.rows[rowIndex].F_Ruibetsu,
                                "@F_Kanban_No": _dtPartControl.rows[rowIndex].F_Kanban_No,
                                "@F_Store_Code": _dtPartControl.rows[rowIndex].F_Store_Code
                            },
                        });
                        var _OrderType = 'Daily';
                        if (_dt.rows != null) _OrderType = _dt.rows[0].F_Type_Order.trim();

                        //console.log(_OrderType);


                        _dt = await xAjax.xExecuteJSON({
                            data: {
                                "Module": "[exec].[spKBNOR320_02]",
                                "@F_Supplier_Code": _dtPartControl.rows[rowIndex].F_Supplier_Code,
                                "@F_Supplier_Plant": _dtPartControl.rows[rowIndex].F_Supplier_Plant,
                                "@F_Part_No": _dtPartControl.rows[rowIndex].F_Part_No,
                                "@F_Ruibetsu": _dtPartControl.rows[rowIndex].F_Ruibetsu,
                                "@F_Kanban_No": _dtPartControl.rows[rowIndex].F_Kanban_No,
                                "@F_Store_Code": _dtPartControl.rows[rowIndex].F_Store_Code
                            },
                        });
                        var _StartDate = xDate.Date('yyyyMMdd', 'dd=-2');
                        if (_dt.rows != null) _StartDate = _dt.rows[0].F_Process_Date.trim();
                        //console.log(_StartDate);


                        var _store = '[CKD_Inhouse].[sp_NumberOfDayToPreview]';
                        if (_OrderType == 'Non Daily') {
                            //'หาวันที่สุดท้าย ที่มีต้องใช้แสดงข้อมูล เพื่อเอามาเป็นวันที่สิ้นสุดที่จะต้องคำนวน ไม่ต้องคำนวนทั้งเดือน
                            _store = '[CKD_Inhouse].[sp_Non_NumberOfDayToPreview]';
                        }

                        _dt = await xAjax.xExecuteJSON({
                            data: {
                                "Module": _store,
                                "@Plant": '3',
                                "@Supplier_Code": _dtPartControl.rows[rowIndex].F_Supplier_Code,
                                "@Supplier_Plant": _dtPartControl.rows[rowIndex].F_Supplier_Plant,
                                "@Store_Code": '1A',
                                "@Date": xDate.Date('yyyyMMdd')
                            },
                        });
                        var _EndDate = xDate.Date('yyyyMMdd', 'dd=14');
                        if (_dt.rows != null) _EndDate = _dt.rows[0].End_Date.trim();
                        if (_dt.rows != null) _EndDate = (_StartDate == xDate.Date('yyyyMMdd', 'dd=-2') ? _dt.rows[0].Start_Date.trim() : _StartDate);
                        //console.log(_EndDate);



                        //'ส่งวันที่ ที่มีการเปลี่ยนแปลงข้อมูลไปคำนวน BL ใหม่
                        //console.log(_StartDate);
                        console.log(rowIndex);
                        //re_Calculate_Trail(_StartDate, _EndDate, rowIndex, _dtPartControl);
                        xItem.progress({ id: 'prgProcess', current: _percent + 10, label: 'Calculate CKD In-House : {{##.##}} %' });


                    }


                    xItem.progress({ id: 'prgProcess', current: 90, label: 'Update Parameter [CI_CKD] : {{##.##}} %' });
                    await xAjax.xExecuteJSON({
                        data: {
                            "Module": "[exec].[spMSParameter] '"
                                + ajexHeader.UserCode + "' "
                                + ", 'CI_CKD', @pValue2='2' "
                                + ", @pValue3='" + ReplaceAll($('#txtProcessDate').val(), "-", "") + ($('#txtProcessShift').val() == "1 - Day Shift" ? "D" : "N") + "' "
                                + ", @ErrorMessage='' "
                        },
                    });

                    xItem.progress({ id: 'prgProcess', current: 95, label: 'Update Parameter [ST_CKD] : {{##.##}} %' });
                    await xAjax.xExecuteJSON({
                        data: {
                            "Module": "[exec].[spMSParameter] '"
                                + ajexHeader.UserCode + "' "
                                + ", 'ST_CKD', @pValue2='2' "
                                + ", @ErrorMessage='' "
                        },
                    });
                    xItem.progress({ id: 'prgProcess', current: 100, label: 'Calculate CKD In-House Complete : {{##.##}} %' });


                }


            })

        } catch (error) {

            await xAjax.xExecuteJSON({
                data: {
                    "Module": "[exec].[spMSParameter] '"
                        + ajexHeader.UserCode + "' "
                        + ", 'CI_CKD', @pValue2='1' "
                        + ", @ErrorMessage='' "
                },
            });
            MsgBox("Error Sub : BGWorker_DoWork.", MsgBoxStyle.Critical, "ERROR");

            xItem.progress({ id: 'prgProcess', current: 0, label: 'Process Not Complete!!! : {{##.##}} %' });
        }
    });


    re_Calculate_Trail = async function (pStartDate = '', pEndDate = '', pIndex = '', _dtPartControl = '') {

        //'ดึงข้อมูล Last Trip ของวันที่ start_Date - 1 จะเริ่มคำนวณตั้งแต่ Trip1 ของวัน start_Date ที่ถูกส่งมาเลย
        //dateLast_Trip = Date.ParseExact(Mid(start_Date, 7, 2) & "/" & Mid(start_Date, 5, 2) & "/" & Mid(start_Date, 1, 4), "dd/MM/yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo)
        //dateECI = get_ECIDate(start_Date, end_Date, index)
        var _dateLastTrip = pStartDate.substring(6) + '/' + pStartDate.substring(4, 6) + '/' + pStartDate.substring(0, 4);
        var _dateECI = await getECIDate(pStartDate, pEndDate, pIndex, _dtPartControl);
        //console.log(_dateECI);

        //'ดึงข้อมูล Balance ของวันก่อนหน้านั้น 1 วัน
        _dt = await xAjax.xExecuteJSON({
            data: {
                "Module": '[CKD_Inhouse].[sp_autoRecalculateBL_First]',
                "@Date": xDate.Format(pStartDate, 'yyyyMMdd', 'dd=-1'),
                "@Shift": (ajexHeader.Shift == 1 ? 'D' : 'N'),
                "@Supplier_Code": _dtPartControl.rows[pIndex].F_Supplier_Code,
                "@Supplier_Plant": _dtPartControl.rows[pIndex].F_Supplier_Plant,
                "@Part_No": _dtPartControl.rows[pIndex].F_Part_No,
                "@Ruibetsu": _dtPartControl.rows[pIndex].F_Ruibetsu,
                "@Kanban_No": _dtPartControl.rows[pIndex].F_Kanban_No,
                "@Store_Code": _dtPartControl.rows[pIndex].F_Store_Code
            },
        });
        var _LastBLPlan = 0, _LastBLActual = 0, _blnFromSetStock = false;
        if (_dt.rows != null) {
            _LastBLPlan = (parseInt(_dt.rows[0].F_BL_SET_Plan) >= 0 ? _dt.rows[0].F_BL_SET_Plan : 0);
            _LastBLActual = (parseInt(_dt.rows[0].F_BL_SET_Actual) >= 0 ? _dt.rows[0].F_BL_SET_Actual : 0);
            _blnFromSetStock = _dt.rows[0].F_Not_Recalculate;
        }


        //console.log(pStartDate);
        //console.log(pEndDate);

        //'ดึงข้อมูลที่จำเป็นต่อการคำนวน BL
        var _dt = await xAjax.xExecuteJSON({
            data: {
                "Module": '[CKD_Inhouse].[sp_autoRecalculateBL_Second]',
                "@StartDate": pStartDate,
                "@EndDate": pEndDate,
                "@Supplier_Code": _dtPartControl.rows[pIndex].F_Supplier_Code,
                "@Supplier_Plant": _dtPartControl.rows[pIndex].F_Supplier_Plant,
                "@Part_No": _dtPartControl.rows[pIndex].F_Part_No,
                "@Ruibetsu": _dtPartControl.rows[pIndex].F_Ruibetsu,
                "@Kanban_No": _dtPartControl.rows[pIndex].F_Kanban_No,
                "@Store_Code": _dtPartControl.rows[pIndex].F_Store_Code
            },
        });
        //console.log(_dt);



        //'ดึงข้อมูล Actaul Receive ที่จำเป็นต่อการคำนวน 
        var _dtActual = await xAjax.xExecuteJSON({
            data: {
                "Module": '[CKD_Inhouse].[sp_autoRecalculateBL_Third]',
                "@StartDate": pStartDate,
                "@EndDate": pEndDate,
                "@Supplier_Code": _dtPartControl.rows[pIndex].F_Supplier_Code,
                "@Supplier_Plant": _dtPartControl.rows[pIndex].F_Supplier_Plant,
                "@Part_No": _dtPartControl.rows[pIndex].F_Part_No,
                "@Ruibetsu": _dtPartControl.rows[pIndex].F_Ruibetsu,
                "@Kanban_No": _dtPartControl.rows[pIndex].F_Kanban_No,
                "@Store_Code": _dtPartControl.rows[pIndex].F_Store_Code
            },
        });
        //console.log(_dtActual);

        if (_dt.rows != null) {
            //console.log(_dt.rows);
            //console.log('L >>>> ' + _dt.rows.length);
            for (var rowIndex = 0; rowIndex < parseInt(_dt.rows.length); rowIndex++) {

                //'มี Row ที่มี Process_Date และ Process Round เหมือนกัน 2Row เป็นเพราะ Effect จากChgCycleTime
                //'Check รอบที่มากกว่า 0 เพราะถ้า 0-1 = -1 จะ Error Can' find row(-1)
                if (rowIndex > 0) {

                    //'ถ้า This Row กับ Last Row มี Process Date กับ Process Round เหมือนกัน ต้องหาค่า Last_BL_Plan,Last_BL_Actual ใหม่
                    if (_dt.rows[rowIndex].F_Process_Round == _dt.rows[rowIndex - 1].F_Process_Round
                        && _dt.rows[rowIndex].F_Process_Date == _dt.rows[rowIndex - 1].F_Process_Date) {

                        //'หา Last Order/Trip
                        var _dateDelivery = _dt.rows[rowIndex].F_Process_Date;
                        _dtLastBL = await xAjax.xExecuteJSON({
                            data: {
                                "Module": '[CKD_Inhouse].[sp_autoRecalculateBL_First]',
                                "@Date": xDate.Format(_dateDelivery, 'yyyyMMdd', 'dd=-1'),
                                "@Supplier_Code": _dtPartControl.rows[pIndex].F_Supplier_Code,
                                "@Supplier_Plant": _dtPartControl.rows[pIndex].F_Supplier_Plant,
                                "@Part_No": _dtPartControl.rows[pIndex].F_Part_No,
                                "@Ruibetsu": _dtPartControl.rows[pIndex].F_Ruibetsu,
                                "@Kanban_No": _dtPartControl.rows[pIndex].F_Kanban_No,
                                "@Store_Code": _dtPartControl.rows[pIndex].F_Store_Code
                            },
                        });

                        var _LastBLPlan = 0
                        var _LastBLActual = 0

                        if (_dtLastBL.rows != null) {
                            _LastBLPlan = (parseInt(_dtLastBL.rows[0].F_BL_SET_Plan) >= 0 ? _dtLastBL.rows[0].F_BL_SET_Plan : 0)
                            _LastBLActual = (parseInt(_dtLastBL.rows[0].F_BL_SET_Actual) >= 0 ? _dtLastBL.rows[0].F_BL_SET_Actual : 0)
                        }

                    }
                }

                //'หา In Actual
                var _DRReceive = '';
                //console.log(_dtActual);
                if (_dtActual.rows != null) {
                    _dtActual.rows.forEach(function (item) {
                        //console.log(item);
                        if (item.F_Delivery_Trip == _dt.rows[rowIndex].F_Process_Round
                            && item.F_Receive_Date == _dt.rows[rowIndex].F_Process_Date
                            && item.F_Supplier_code == _dt.rows[rowIndex].F_Supplier_Code
                            && item.F_Supplier_Plant == _dt.rows[rowIndex].F_Supplier_Plant
                            && item.F_PART_No == _dt.rows[rowIndex].F_Part_No
                            && item.F_RUibetsu == _dt.rows[rowIndex].F_Ruibetsu
                            && item.F_Store_Cd == _dt.rows[rowIndex].F_Store_Code
                            //if (item.F_Delivery_Trip == null //_dt.rows[rowIndex].F_Process_Round
                            //    && item.F_Receive_Date == "20240311"    //_dt.rows[rowIndex].F_Process_Date
                            //    && item.F_Supplier_code == "9995"   //_dt.rows[rowIndex].F_Supplier_Code
                            //    && item.F_Supplier_Plant == "A" //_dt.rows[rowIndex].F_Supplier_Plant
                            //    && item.F_PART_No == "4130204020  " //_dt.rows[rowIndex].F_Part_No
                            //    && item.F_RUibetsu == "00"  //_dt.rows[rowIndex].F_Ruibetsu
                            //    && item.F_Store_Cd == "1D"  //_dt.rows[rowIndex].F_Store_Code
                        ) {
                            _DRReceive = item;
                        }
                    });
                }
                var _InActual = 0;
                if (_DRReceive != '') {
                    _InActual = _DRReceive.IN_ACTUAL
                }
                //console.log(_InActual);


                var _InRec = _dt.rows[rowIndex].IN_Plan
                //if ((parseInt(_dt.rows[rowIndex].F_Flag_Pattern) > 0 ? _dtLastBL.rows[0].F_Flag_Pattern : 0) == 1) {
                if (_dt.rows[rowIndex].IN_Plan == 1) {
                    _InRec = _dt.rows[rowIndex].F_Adj_Pattern
                }
                //console.log(_InRec);


                //console.log(_dt.rows[rowIndex].F_Process_Date);
                //console.log(_dateECI);

                var _BLPlan = '', _BLActual = '';
                var _BLPlanSolution = '', _BLActualSolution = '';

                //'ถ้าวันที่ F_Process_Date น้อยกว่าวันที่เริ่มใช้ Part. --> Begining_Date ต้องคิด Balance โดยที่ไม่เอา MRP ไปหักลบ
                if (_dt.rows[rowIndex].F_Process_Date < _dateECI.BeginingDate) {
                    if (_dt.rows[rowIndex].F_Process_Round == "1") {
                        //'ถ้าถูก Set Stock มาแล้วไม่ต้อง ลบ Abnormal เพราะที่นับเป็นจำนวน Part ที่มีอยู่จริง ไม่มี Abnormal อยู่แล้ว
                        if (_blnFromSetStock) {
                            //'สูตร BL T1 = [ BL(Last Trip) + In(Rec) Pcs ] + Urgent 
                            _BLPlan = (_LastBLPlan + _InRec)
                                + _dt.rows[rowIndex].F_Urgent_Order;
                            _BLPlanSolution = `BL = ( BF + In(Rec) ) + Urgent 
BLPlan: ` + _BLPlan + ` = (` + _LastBLPlan + ` + ` + _InRec + `) + `
                                + _dt.rows[rowIndex].F_Urgent_Order;

                            _BLActual = (_LastBLActual + _InActual); //'+ DT.Rows(rowIndex).Item("F_Urgent_Order").ToString โจบอกเอาออกนะ
                            _BLActualSolution = "BLActual : " + _BLActual + " = (" + _LastBLActual + " + " + _InActual + ") "


                            if (_dt.rows[rowIndex].Flag_HalfChg_BL_Stock == false) {
                                //'สูตร BL T1 = [ BL(Last Trip) + In(Rec) Pcs ] - MRP + Urgent  
                                _BLPlan = (_LastBLPlan + _InRec)
                                    + _dt.rows[rowIndex].F_MRP
                                    + _dt.rows[rowIndex].F_Urgent_Order;
                                _BLPlanSolution = `BL = ( BF + In(Rec) ) - MRP + Urgent 
BLPlan : ` + _BLPlan + ` = (` + _LastBLPlan + ` + ` + _InRec + `) - `
                                    + _dt.rows[rowIndex].F_MRP + ` + `
                                    + _dt.rows[rowIndex].F_Urgent_Order;

                                _BLActual = (_LastBLActual + _InActual)
                                    - _dt.rows[rowIndex].F_MRP;
                                    //'+ DT.Rows(rowIndex).Item("F_Urgent_Order").ToString โจบอกเอาออกนะ
                                _BLActualSolution = "BLActual : " + _BLActual + " = (" + _LastBLActual + " + " + _InActual + ") - "
                                    + _dt.rows[rowIndex].F_MRP;


                            } else if (_dt.rows[rowIndex].Flag_HalfChg_BL_Stock == true) {
                                //'กรณีที่ Supplier ส่งวันละรอบ คือมี CycleB = 1 ทั้งที่ส่งแค่เช้า และทั้งที่ส่งแค่ค่ำ
                                //'ถ้ามีการ Set Stock ให้กะ Day จะถูก Set Flag_HalfChg_BL_Stock = True จาก Store Procedure
                                if (_dt.rows[rowIndex].F_Process_Shift == 'D') {
                                    _BLPlan = _LastBLPlan - (_dt.rows[rowIndex].F_MRP / 2)
                                    _BLPlanSolution = `BL = BF - MRP/2 
BLPlan : ` + _BLPlan + ` = ` + _LastBLPlan + ` - ` + (_dt.rows[rowIndex].F_MRP / 2)

                                    _BLActual = _LastBLActual - (_dt.rows[rowIndex].F_MRP / 2)
                                    _BLActualSolution = "BLActual : " + _BLActual + " = " + _LastBLActual + " - "
                                        + (_dt.rows[rowIndex].F_MRP / 2)


                                } else {
                                    _BLPlan = (_LastBLPlan + _InRec)
                                        - (_dt.rows[rowIndex].F_MRP / 2)
                                        + _dt.rows[rowIndex].F_Urgent_Order
                                    _BLPlanSolution = `BL = (BF + In(Rec) ) - MRP/2 + Urgent 
BLPlan : ` + _BLPlan + ` = (` + _LastBLPlan + ` + ` + _InRec + `) - `
                                        + (_dt.rows[rowIndex].F_MRP / 2) + ` + `
                                        + _dt.rows[rowIndex].F_Urgent_Order

                                    _BLActual = (_LastBLActual + _InRec)
                                        - (_dt.rows[rowIndex].F_MRP / 2)
                                        + _dt.rows[rowIndex].F_Urgent_Order
                                    _BLActualSolution = "BLActual : " + _BLActual + " = (" + _LastBLActual + " + " + _InRec + ") - "
                                        + (_dt.rows[rowIndex].F_MRP / 2).ToString + " + "
                                        + _dt.rows[rowIndex].F_Urgent_Order


                                }

                            }

                        } else {
                            //'สูตร BL T1 = [ BL(Last Trip) + In(Rec) Pcs ] + Urgent - Abnormal 
                            _BLPlan = (_LastBLPlan + _InRec)
                                + _dt.rows[rowIndex].F_Urgent_Order
                                - _dt.rows[rowIndex].F_AbNormal_Part
                            _BLPlanSolution = `BL = ( BF + In(Rec) ) + Urgent - Abnormal 
BLPlan : ` + _BLPlan + ` = (` + _LastBLPlan + ` + ` + _InRec + `) + `
                                + _dt.rows[rowIndex].F_Urgent_Order + ` - `
                                + _dt.rows[rowIndex].F_AbNormal_Part 

                            _BLActual = (_LastBLActual + _InActual)
                                - _dt.rows[rowIndex].F_AbNormal_Part
                                //'+ DT.Rows(rowIndex).Item("F_Urgent_Order").ToString โจบอกเอาออกนะ
                            _BLActualSolution = `BLActual : ` + _BLActual + ` = (` + _LastBLActual + ` + ` + _InActual + `) - `
                                + _dt.rows[rowIndex].F_AbNormal_Part;

                        }

                        _LastBLPlan = _BLPlan;
                        _LastBLActual = _BLActual;

                    } else {
                        //'สูตร BL Tn = [ BL(Last Trip) + Qty Pcs ] + Urgent
                        _BLPlan = (_LastBLPlan + _InRec)
                            + _dt.rows[rowIndex].F_Urgent_Order;
                        _BLPlanSolution = `BL = ( BF + In(Rec) ) + Urgent 
BLPlan : ` + _BLPlan + ` = (` + _LastBLPlan + ` + ` + _InRec + `) + `
                            + _dt.rows[rowIndex].F_Urgent_Order;

                        _BLActual = (_LastBLActual + _InActual);
                        _BLActualSolution = "BLActual : " + _BLActual + " = (" + _LastBLActual + " + " + _InActual + ") ";

                        _LastBLPlan = _BLPlan
                        _LastBLActual = _BLActual
                    }

                } else {
                    if (_dt.rows[rowIndex].F_Process_Round == "1") {
                        //'ถ้าถูก Set Stock มาแล้วไม่ต้อง ลบ Abnormal เพราะที่นับเป็นจำนวน Part ที่มีอยู่จริง ไม่มี Abnormal อยู่แล้ว
                        if (_blnFromSetStock) {

                            if (_dt.rows[rowIndex].Flag_HalfChg_BL_Stock == false) {
                                //'สูตร BL T1 = [ BL(Last Trip) + In(Rec) Pcs ] - MRP + Urgent  
                                _BLPlan = (_LastBLPlan + _InRec)
                                    - _dt.rows[rowIndex].F_MRP
                                    + _dt.rows[rowIndex].F_Urgent_Order
                                _BLPlanSolution = `BL = ( BF + In(Rec) ) - MRP + Urgent 
BLPlan : ` + _BLPlan + ` = (` + _LastBLPlan + ` + ` + _InRec + `) - `
                                    + _dt.rows[rowIndex].F_MRP + ` + `
                                    + _dt.rows[rowIndex].F_Urgent_Order

                                _BLActual = (_LastBLActual + _InActual)
                                    - _dt.rows[rowIndex].F_MRP
                                    //'+ DT.Rows(rowIndex).Item("F_Urgent_Order").ToString โจบอกเอาออกนะ
                                _BLActualSolution = "BLActual : " + _BLActual + " = (" + _LastBLActual + " + " + _InActual + ") - "
                                    + _dt.rows[rowIndex].F_MRP


                            } else if (_dt.rows[rowIndex].Flag_HalfChg_BL_Stock == true) {
                                //'กรณีที่ Supplier ส่งวันละรอบ คือมี CycleB = 1 ทั้งที่ส่งแค่เช้า และทั้งที่ส่งแค่ค่ำ
                                //'ถ้ามีการ Set Stock ให้กะ Day จะถูก Set Flag_HalfChg_BL_Stock = True จาก Store Procedure
                                if (_dt.rows[rowIndex].F_Process_Shift == 'D') {
                                    _BLPlan = _LastBLPlan
                                        - (_dt.rows[rowIndex].F_MRP / 2)
                                    _BLPlanSolution = `BL = BF - MRP/2 
BLPlan : ` + _BLPlan + ` = ` + _LastBLPlan + ` - `
                                        + (_dt.rows[rowIndex].F_MRP / 2)

                                    _BLActual = _LastBLActual
                                        - (_dt.rows[rowIndex].F_MRP / 2)
                                    _BLActualSolution = "BLActual : " + _BLActual + " = " + _LastBLActual + " - "
                                        + (_dt.rows[rowIndex].F_MRP / 2)


                                } else {
                                    _BLPlan = (_LastBLPlan + _InRec) - (_dt.rows[rowIndex].F_MRP / 2)
                                        + _dt.rows[rowIndex].F_Urgent_Order
                                    _BLPlanSolution = `BL = (BF + In(Rec) ) - MRP/2 + Urgent 
BLPlan : ` + _BLPlan + ` = (` + _LastBLPlan + ` + ` + _InRec + `) - `
                                        + (_dt.rows[rowIndex].F_MRP / 2) + ` + `
                                        + _dt.rows[rowIndex].F_Urgent_Order

                                    _BLActual = (_LastBLActual + _InRec) - (_dt.rows[rowIndex].F_MRP / 2) + _dt.rows[rowIndex].F_Urgent_Order
                                    _BLActualSolution = "BLActual : " + _BLActual + " = (" + _LastBLActual + " + " + _InRec + ") - "
                                        + (_dt.rows[rowIndex].F_MRP / 2) + " + "
                                        + _dt.rows[rowIndex].F_Urgent_Order


                                }
                            }



                        } else {
                            //'สูตร BL T1 = [ BL(Last Trip) + In(Rec) Pcs ] - MRP + Urgent - Abnormal 
                            _BLPlan = (_LastBLPlan + _LastBLPlan)
                                - _dt.rows[rowIndex].F_MRP
                                + _dt.rows[rowIndex].F_Urgent_Order
                                - _dt.rows[rowIndex].F_AbNormal_Part
                            _BLPlanSolution = `BL = ( BF + In(Rec) ) - MRP + Urgent - Abnormal 
BLPlan : ` + _BLPlan + ` = (` + _LastBLPlan + ` + ` + _InRec + `) - `
                                + _dt.rows[rowIndex].F_MRP + ` + `
                                + _dt.rows[rowIndex].F_Urgent_Order + ` - `
                                + _dt.rows[rowIndex].F_AbNormal_Part

                            _BLActual = (_LastBLActual + _InActual)
                                - _dt.rows[rowIndex].F_MRP
                                - _dt.rows[rowIndex].F_AbNormal_Part
                                //'+ DT.Rows(rowIndex).Item("F_Urgent_Order").ToString โจบอกเอาออกนะ
                            _BLActualSolution = "BLActual : " + _BLActual + " = (" + _LastBLActual + " + " + _InActual + ") - "
                                + _dt.rows[rowIndex].F_MRP + " - "
                                + _dt.rows[rowIndex].F_AbNormal_Part

                        }

                        _LastBLPlan = _BLPlan
                        _LastBLActual = _BLActual

                    } else {
                        //'สูตร BL Tn = [ BL(Last Trip) + Qty Pcs ] - MRP + Urgent
                        _BLPlan = (_LastBLPlan + _LastBLPlan)
                            - _dt.rows[rowIndex].F_MRP
                            + _dt.rows[rowIndex].F_Urgent_Order
                        _BLPlanSolution = `BL = ( BF + In(Rec) ) - MRP + Urgent 
BLPlan : ` + _BLPlan + ` = (` + _LastBLPlan + ` + ` + _InRec + `) - `
                            + _dt.rows[rowIndex].F_MRP + ` + `
                            + _dt.rows[rowIndex].F_Urgent_Order

                        _BLActual = (_LastBLActual + _InActual)
                            - _dt.rows[rowIndex].F_MRP
                            //'+ DT.Rows(rowIndex).Item("F_Urgent_Order").ToString โจบอกเอาออกนะ
                        _BLActualSolution = "BLActual : " + _BLActual + " = (" + _LastBLActual + " + " + _InActual + ") - "
                            + _dt.rows[rowIndex].F_MRP

                        _LastBLPlan = _BLPlan
                        _LastBLActual = _BLActual

                    }

                }


                var _result = await xAjax.xExecute({
                    data: {
                        "Module": '[CKD_Inhouse].[sp_autoRecalculateBL_UpdateBL]',
                        "@Process_Date": _dt.rows[rowIndex].F_Process_Date,
                        "@Process_Shift": _dt.rows[rowIndex].F_Process_Shift,
                        "@Process_Round": _dt.rows[rowIndex].F_Process_Round,
                        "@Supplier_Code": _dt.rows[rowIndex].F_Supplier_Code,
                        "@Supplier_Plant": _dt.rows[rowIndex].F_Supplier_Plant,
                        "@Part_No": _dt.rows[rowIndex].F_Part_No,
                        "@Ruibetsu": _dt.rows[rowIndex].F_Ruibetsu,
                        "@Kanban_No": _dt.rows[rowIndex].F_Kanban_No,
                        "@Store_Code": _dt.rows[rowIndex].F_Store_Code,
                        "@BL_Plan": _BLPlan,
                        "@BL_Actual": _BLActual,
                        "@Not_Recalculate": _dt.rows[rowIndex].F_Not_Recalculate
                    },
                });

                //console.log('_result');
                //console.log(_result);

                var _message = `Update TB_Calculate_D : Not Complete 
` + _BLPlanSolution + `
` + _BLActualSolution + `
`;
                if (_result.response == 'OK') {
                    _message = `Update TB_Calculate_D : Complete 
` + _BLPlanSolution + `
` + _BLActualSolution + `
`;
                }
                await xLog.WriteLog(_message, "CKD");
                if (_dt.rows != null) {

                    if (_dt.rows[rowIndex].F_Not_Recalculate == 'true') {
                        _LastBLPlan = _dt.rows[rowIndex].F_BL_SET_Plan;
                        _LastBLActual = _dt.rows[rowIndex].F_BL_SET_Actual;
                        _blnFromSetStock = _dt.rows[rowIndex].F_Not_Recalculate;
                    } else {
                        _blnFromSetStock = false;
                    }

                    //'ถ้า F_Process_Date เป๋นวันที่เริ่มต้นของการคำนวณ ให้หาค่า Balance
                    if (_dt.rows[rowIndex].F_Process_Date == _dateECI.BeginingCalculate) {
                        //_dateDelivery = 

                        //'หา Last Order/Trip
                        _dateDelivery = _dt.rows[rowIndex].F_Process_Date;
                        _dtLastBL = await xAjax.xExecuteJSON({
                            data: {
                                "Module": '[CKD_Inhouse].[sp_autoRecalculateBL_First]',
                                "@Date": xDate.Format(_dateDelivery, 'yyyyMMdd', 'dd=-1'),
                                "@Supplier_Code": _dtPartControl.rows[pIndex].F_Supplier_Code,
                                "@Supplier_Plant": _dtPartControl.rows[pIndex].F_Supplier_Plant,
                                "@Part_No": _dtPartControl.rows[pIndex].F_Part_No,
                                "@Ruibetsu": _dtPartControl.rows[pIndex].F_Ruibetsu,
                                "@Kanban_No": _dtPartControl.rows[pIndex].F_Kanban_No,
                                "@Store_Code": _dtPartControl.rows[pIndex].F_Store_Code
                            },
                        });



                        var _LastBLPlan = 0
                        var _LastBLActual = 0

                        if (_dtLastBL.rows != null) {
                            _LastBLPlan = (parseInt(_dtLastBL.rows[0].F_BL_SET_Plan) >= 0 ? _dtLastBL.rows[0].F_BL_SET_Plan : 0)
                            _LastBLActual = (parseInt(_dtLastBL.rows[0].F_BL_SET_Actual) >= 0 ? _dtLastBL.rows[0].F_BL_SET_Actual : 0)
                        }
                    }
                }


            }   //END FOR


        }



    }



    getECIDate = async function (pStartDate = '', pEndDate = '', pIndex = '', _dtPartControl = '') {
        var _dateLastTrip = pStartDate.substring(6) + '/' + pStartDate.substring(4, 6) + '/' + pStartDate.substring(0, 4);

        //'Support Case ECI : ดึงวันที่ F_TC_Str ของ Part จาก [HMMT-PPM].[PPMDB].[T_Construction]
        //'หากวันที่ Process Date น้อยกว่าวันที่ BOM เริ่มใช้งาน จะต้องไม่ลบ MRP

        _dt = await xAjax.xExecuteJSON({
            data: {
                "Module": '[exec].[spKBNOR320_ECI_01]',
                "@F_Part_No": _dtPartControl.rows[pIndex].F_Part_No,
                "@F_Ruibetsu": _dtPartControl.rows[pIndex].F_Ruibetsu,
                "@F_Store_cd": _dtPartControl.rows[pIndex].F_Store_Code,
                "@F_supplier_cd": _dtPartControl.rows[pIndex].F_Supplier_Code,
                "@F_plant": _dtPartControl.rows[pIndex].F_Supplier_Plant,
                "@F_Sebango": _dtPartControl.rows[pIndex].F_Kanban_No.substring(1)
            },
        });
        var _BeginingDate = pStartDate;
        if (_dt.rows != null) _BeginingDate = _dt.rows[0].F_TC_Str;

        //'Support Case ECI : ดึงวันที่เริ่มต้นของการคำนวณที่ TB_Calculate_D
        //'หากต้องคำนวณ Balance ของวันแรก จะต้องไปดึงค่าที่ TB_BL_SET ใหม่อีกครั้งนึง
        _dt = await xAjax.xExecuteJSON({
            data: {
                "Module": '[exec].[spKBNOR320_ECI_02]',
                "@F_Part_No": _dtPartControl.rows[pIndex].F_Part_No,
                "@F_Ruibetsu": _dtPartControl.rows[pIndex].F_Ruibetsu,
                "@F_Store_cd": _dtPartControl.rows[pIndex].F_Store_Code,
                "@F_supplier_cd": _dtPartControl.rows[pIndex].F_Supplier_Code,
                "@F_plant": _dtPartControl.rows[pIndex].F_Supplier_Plant,
                "@F_Sebango": _dtPartControl.rows[pIndex].F_Kanban_No.substring(1)
            },
        });
        var _BeginingCalculate = pStartDate;
        if (_dt.rows != null) _BeginingCalculate = _dt.rows[0].F_Process_Date;


        return {
            "BeginingDate": _BeginingDate,
            "BeginingCalculate": _BeginingCalculate
        }

    }

})

