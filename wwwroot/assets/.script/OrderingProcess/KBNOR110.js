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

    
    //xAjax.onClick('btnSearch', async function () {
    //    var _dt = await xAjax.ExecuteJSON({
    //        data: {
    //            "Module": "[dbo].[SP_DisplayUrgent]",
    //            "OrderType": "SRV",
    //            "Plant": ajexHeader.Plant,
    //            "UserCode": ajexHeader.UserCode
    //        },
    //    });

    //    if (_dt.rows != null) xDataTable.bind('#tblMaster', _dt.rows);
    //    if (_dt.rows == null) MsgBox("ไม่พบข้อมูล Urgent Order", MsgBoxStyle.Information, "Interface Urgent Data");

    //});


    xAjax.onClick('btnInterface', async function () {
        
        try{
            MsgBox("Do you want Interface data to Order?", MsgBoxStyle.OkCancel, async function () {

                xItem.progress({ id: 'prgProcess', current: 5, label: 'Start Interface Data from Import Data : {{##.##}} %' });

                //''Clear Old Data 
                await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNOR110_INTERFACE_D1]",
                        "@OrderType": "N",
                        "@Plant": ajexHeader.Plant,
                        "@UserCode": ajexHeader.UserCode
                    },
                });
                xItem.progress({ id: 'prgProcess', current: 10, label: 'Interface Data : Clear Old Order : {{##.##}} %' });

                //let _remark = '';

                var _dtChk = await xAjax.ExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNOR110_INTERFACE_M1]",
                        "@OrderType": "N",
                        "@Plant": ajexHeader.Plant,
                        "@UserCode": ajexHeader.UserCode
                    },
                });
                xItem.progress({ id: 'prgProcess', current: 40, label: 'Interface Data from Import Data : {{##.##}} %' });

                if (_dtChk.rows != null) xDataTable.bind('#tblMaster', _dtChk.rows);
                if (_dtChk.rows == null) MsgBox("ไม่พบข้อมูล Interface Normal Order", MsgBoxStyle.Information, "Interface Normal Data");

                $("#table-wrapper").css("visibility", "hidden");

                //if (_dtChk.rows != null) {
                //    for (var i = 0; i < _dtChk.rows.length; i++) {
                //        var _dt = await xAjax.Execute({
                //            data: {
                //                "Module": "[exec].[spKBNOR110_INTERFACE_M2]",
                //                "@OrderType": "N",
                //                "@Plant": ajexHeader.Plant,
                //                "@UserCode": ajexHeader.UserCode,
                //                "@F_Delivery_Date": _dtChk.rows[i].F_Delivery_Date,
                //                "@F_Store_Cd": _dtChk.rows[i].F_Store_Cd,
                //                "@F_Supplier_Cd": _dtChk.rows[i].F_Supplier_Cd,
                //                "@F_Supplier_Plant": _dtChk.rows[i].F_Supplier_Plant,
                //                "@F_Country": _dtChk.rows[i].F_Country,
                //                "@F_Plant": _dtChk.rows[i].F_Plant
                //            },
                //        });

                //        if (_dt.rows != null) {
                //            for (var j = 0; j < _dt.rows.length; j++) {
                //                _remark = _remark + _dt.rows[j].F_Remark + ',';
                //            }
                //            _remark.substring(0, remark.length - 1);
                //        }

                //        //===Update Volume
                //        await xAjax.Execute({
                //            data: {
                //                "Module": "[exec].[spKBNOR110_INTERFACE_M3]",
                //                "@OrderType": "N",
                //                "@Plant": ajexHeader.Plant,
                //                "@UserCode": ajexHeader.UserCode,
                //                "@F_Delivery_Date": _dtChk.rows[i].F_Delivery_Date,
                //                "@F_Store_Cd": _dtChk.rows[i].F_Store_Cd,
                //                "@F_Supplier_Cd": _dtChk.rows[i].F_Supplier_Cd,
                //                "@F_Supplier_Plant": _dtChk.rows[i].F_Supplier_Plant,
                //                "@F_Country": _dtChk.rows[i].F_Country,
                //                "@F_Plant": _dtChk.rows[i].F_Plant,
                //                "@pRemark": _remark
                //            },
                //        });
                //        xItem.progress({ id: 'prgProcess', current: 30, label: 'Update Remark Again : {{##.##}} %' });

                //    }
                //}
                ////''======= End of Remark

                //await xAjax.Execute({
                //    data: {
                //        "Module": "[exec].[spKBNOR110_INTERFACE_M4]",
                //        "@OrderType": "N",
                //        "@Plant": ajexHeader.Plant,
                //        "@UserCode": ajexHeader.UserCode
                //    },
                //});
                //xItem.progress({ id: 'prgProcess', current: 40, label: 'Interface Data from Import Data : {{##.##}} %' });


                ////''Tacoma Only
                //await xAjax.Execute({
                //    data: {
                //        "Module": "[exec].[spKBNOR110_INTERFACE_TACOMA]",
                //        "@OrderType": "N",
                //        "@Plant": ajexHeader.Plant,
                //        "@UserCode": ajexHeader.UserCode
                //    },
                //});
                //xItem.progress({ id: 'prgProcess', current: 60, label: 'Interface Data from Import Data[Tacoma] : {{##.##}} %' });



                //xItem.progress({ id: 'prgProcess', current: 80, label: 'Update F_Reg_Flg=2 : {{##.##}} %' });
                ////''Update F_Reg_Flg=2
                //await xAjax.Execute({
                //    data: {
                //        "Module": "[exec].[spKBNOR110_INTERFACE_M5]",
                //        "@OrderType": "N",
                //        "@Plant": ajexHeader.Plant,
                //        "@UserCode": ajexHeader.UserCode
                //    },
                //});
                //xItem.progress({ id: 'prgProcess', current: 100, label: 'Process Data Complete : {{##.##}} %' });




            //xAjax.Post({
            //    url: 'KBNOR110/interfaceData',
            //    data: {
            //        "OrderType": "N"
            //    },
            //    then: function (result) {
            //        console.log(result);

            //        //if (result.data != null) xDataTable.bind('#tblMaster', result.data);
            //    }
            //})
            })



        } catch (error) {
            // Code to handle the error
            await xAjax.Execute({
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






})

