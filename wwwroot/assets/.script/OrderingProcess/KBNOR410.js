$(document).ready(function () {



    const xKBNOR401 = new MasterTemplate({
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



    xKBNOR401.prepare();




    xKBNOR401.initial(function (result) {
        xSplash.hide();
        //xKBNOR401.search();
    });


    xAjax.onClick('btnExit', function () {
        xAjax.redirect('KBNOR400');
    });


    xAjax.onClick('btnSearch', function () {

        xAjax.Post({
            url: 'KBNOR410/search',
            data: {
                "OrderType": "U"
            },
            then: function (result) {
                if (result.data != null) xDataTable.bind('#tblMaster', result.data);
                if (result.data.length == 0) MsgBox("ไม่พบข้อมูล Urgent Order", MsgBoxStyle.Information, "Interface Urgent Data");

            }
        })
    });


    xAjax.onClick('btnInterface', function () {
        MsgBox("Do you want Interface data to Order?", MsgBoxStyle.OkCancel, function () {
            xAjax.Post({
                url: 'KBNOR410/interfaceData',
                data: {
                    "OrderType": "U"
                },
                then: function (result) {
                    console.log(result);
                }
            })
        })
    });






})

