$(document).ready(function () {



    const xKBNOR410 = new MasterTemplate({
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



    xKBNOR410.prepare();




    xKBNOR410.initial(function (result) {
        xSplash.hide();
        //xKBNOR401.search();
    });


    xAjax.onClick('btnExit', function () {
        xAjax.redirect('KBNOR400');
    });

    
    xAjax.onClick('btnSearch', async function () {
        var _dt = await xAjax.xExecuteJSON({
            data: {
                "Module": "[dbo].[SP_DisplayUrgent]",
                "OrderType": "U",
                "Plant": ajexHeader.Plant,
                "UserCode": ajexHeader.UserCode
            },
        });

        console.log(_dt);

        if (_dt.rows != null) xDataTable.bind('#tblMaster', _dt.rows);
        if (_dt.rows == null) MsgBox("ไม่พบข้อมูล Urgent Order", MsgBoxStyle.Information, "Interface Urgent Data");

        xSplash.hide();
    });


    xAjax.onClick('btnInterface', async function () {
        
        try{
            MsgBox("Do you want Interface data to Order?", MsgBoxStyle.OkCancel, async function () {

                xItem.progress({ id: 'prgProcess', current: 5, label: 'Start Interface Data from Import Data : {{##.##}} %' });

                await xAjax.xExecuteJSON({
                    data: {
                        "Module": "[exec].[spKBNOR410_INTERFACE]",                        
                        "@Plant": ajexHeader.Plant,
                        "@UserCode": ajexHeader.UserCode
                    },
                });
                xItem.progress({ id: 'prgProcess', current: 10, label: 'Interface Data : Clear Old Order : {{##.##}} %' });

                xItem.progress({ id: 'prgProcess', current: 100, label: 'Process Data Complete : {{##.##}} %' });

                MsgBox("Process Interface Data for Urgent Order Completed.", MsgBoxStyle.Information, "Process Complete");
            })



        } catch (error) {
            // Code to handle the error
            await xAjax.Execute({
                data: {
                    "Module": "[exec].[spKBNOR410_EXCEPTION]",
                    "@OrderType": "U",
                    "@Plant": ajexHeader.Plant,
                    "@UserCode": ajexHeader.UserCode
                },
            });
            MsgBox("ERROR Interface Data from Import Data.", MsgBoxStyle.Critical, "Interface Urgent Data Error");

            xItem.progress({ id: 'prgProcess', current: 0, label: 'Process Not Complete!!! : {{##.##}} %' });


        }
    });



    $("#btnReport").click(function () {
        var UserName = $(".pcoded-navigatio-lavel").text();
        console.log(UserName);
        _xLib.OpenReport('/KBNOR410', `UserCode=${ajexHeader.UserCode}&Plant=${ajexHeader.Plant}&UserName=${UserName}`);
    });


})

