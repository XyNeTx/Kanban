$(document).ready(function () {


    initial = async function () {

        var tblMaster = xDataTable.Initial({
            name: 'tblMaster',
            dom: '<"clear">',
            running: 0,
            orderNo: 0,
            columnTitle: {
                "EN": ['Part No', 'Kanban No.', 'Qty/Pack', 'Qty(PCS)'],
                "TH": ['Part No', 'Kanban No.', 'Qty/Pack', 'Qty(PCS)'],
                "JP": ['Part No', 'Kanban No.', 'Qty/Pack', 'Qty(PCS)'],
            },
            column: [
                { "data": "F_Part_No" },
                { "data": "F_Kanban_No" },
                { "data": "F_Box_Qty" },
                { "data": "F_Unit_Amount" }
            ],
            addnew: false,
            rowclick: (row) => {
            },
            then: function (config) {
                //xSplash.hide();
            }
        });

        btnRecover.readonly = true;

        var _dt = await xAjax.ExecuteJSON({
            data: {
                "Module": "[exec].[spKBNOR640]",
                "pUserCode": ajexHeader.UserCode,
                "F_Plant": ajexHeader.Plant
            },
        });

        if (_dt.rows != null) xDropDownList.bind('#ddlPDSNo', _dt.rows, 'F_OrderNo', 'F_OrderNo');

        //console.log(ddlPDSNo);

        xSplash.hide();
    }
    initial();



    xAjax.onClick('btnExit', function () {
        ////ddlPDSNo.title = '1A24022701023';
        //ddlPDSNo.value = '1A24022701023';
        ////ddlPDSNo.value = '1D24022801C02';
        ////ddlPDSNo.value = '1A24021701001';

        xAjax.redirect('KBNOR600');
    });


    //''Show Data 
    xAjax.onClick('btnSearch', async function () {
        try {
            btnRecover.readonly = true;

            if (ddlPDSNo.value == null || ddlPDSNo.value == undefined) {
                MsgBox(`กรุณาเลือกเบอร์ PDS ก่อนทำการค้นหาข้อมูล`, MsgBoxStyle.Critical);
                return false;
            }
            
            var _dt = await xAjax.ExecuteJSON({
                data: {
                    "Module": "[exec].[spKBNOR640_S1]",
                    "F_OrderNo": ddlPDSNo.value
                },
            });
            if (_dt.rows == null) {
                MsgBox(`ไม่พบเบอร์ PDS นี้ รบกวนตรวจสอบข้อมูลอีกครั้ง.`, MsgBoxStyle.Critical, `Data Mistake`);
                return false;
            }
            if (_dt.rows[0].F_Status != 'D') {
                MsgBox(`เบอร์ PDS นี้ไม่ได้ถูกยกเลิก ระบบไม่สามารถกู้คืนข้อมูลได้.`, MsgBoxStyle.Critical, `Data Mistake`);
                return false;
            }

            if (_dt.rows != null) {
                txtSupplierCode.value = _dt.rows[0].F_Supplier_Code + '-' + _dt.rows[0].F_Supplier_Plant;
                txtSupplierName.value = _dt.rows[0].F_name;
                txtDeliveryDate.value = _dt.rows[0].F_Delivery_Date;
                txtDeliveryTrip.value = _dt.rows[0].F_Delivery_Trip;
                txtStoreCode.value = _dt.rows[0].F_Delivery_Dock;
                txtStatus.value = (_dt.rows[0].F_Status == 'N' ? 'NORMAL' : (_dt.rows[0].F_Status == 'D' ? 'DELETED - Can Recovery' : ''));
                txtOrderType.value = _dt.rows[0].F_OrderType_DESC;


                var _dtChk = await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNOR640_S2]",
                        "F_OrderNo": ddlPDSNo.value
                    },
                });
                if (_dtChk.rows != null) xDataTable.bind('#tblMaster', _dtChk.rows);

                btnRecover.readonly = false;
                if (_dt.rows[0].F_Status == 'D') btnRecover.readonly = false;
            }
            if (_dt.rows == null) MsgBox(`Not Found data...`, MsgBoxStyle.Information);

            xSplash.hide();


        } catch (error) {       // Code to handle the error
            xItem.progress({ id: 'prgProcess', current: 0, label: 'Process Not Complete!!! : {{##.##}} %' });

            MsgBox("ERROR : Can not search data.", MsgBoxStyle.Critical);
        }
    });


    //''Recover data
    xAjax.onClick('btnRecover', async function () {
        try {
            if (ddlPDSNo.value == null || ddlPDSNo.value == undefined) {
                MsgBox(`กรุณาเลือกเบอร์ PDS ก่อนทำการค้นหาข้อมูล`, MsgBoxStyle.Critical);
                return false;
            }

            MsgBox(`ท่านต้องการกู้คืนข้อมูล PDS ที่ยกเลิกแล้วใช่หรือไม่?`, MsgBoxStyle.OkCancel, async function () {

                xItem.progress({ id: 'prgProcess', current: 5, label: 'Processing Please wait : {{##.##}} %' });


                await xAjax.Execute({
                    data: {
                        "Module": "[exec].[spKBNOR640_RECOVERY]",
                        "@pUserCode": ajexHeader.UserCode,
                        "@F_Plant": ajexHeader.Plant,
                        "@F_OrderNo": ddlPDSNo.value
                    },
                });

                xItem.progress({ id: 'prgProcess', current: 100, label: 'Process Complete : {{##.##}} %' });

                txtStatus.value = 'Recovery Completed.';
                btnRecover.readonly = true;


                MsgBox("กู้คืนข้อมูลเรียบร้อยแล้ว", MsgBoxStyle.Information, "PROCESS COMPLETE");

            });



        } catch (error) {       // Code to handle the error
            xItem.progress({ id: 'prgProcess', current: 0, label: 'Process Not Complete!!! : {{##.##}} %' });

            await xAjax.Execute({
                data: {
                    "Module": "[exec].[spKBNOR640_EXCEPTION]",
                    "@OrderType": "N",
                    "@Plant": ajexHeader.Plant,
                    "@UserCode": ajexHeader.UserCode
                },
            });
            MsgBox("ERROR : Can not delete data.", MsgBoxStyle.Critical);

        }
    });





})

