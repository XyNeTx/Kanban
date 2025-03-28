$(document).ready(function () {

    _xLib.AJAX_Get("/api/KBNOR310/Onload", "",
        function (success) {
            //xSwal.xSuccess(success);
        },
        function (error) {
            //xSwal.xError(error);
        }
    );

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


})

$("#btnCalculate").click(async function () {

    let isConfirm = await xSwal.confirm("Are you Sure to Calculate CKD In-House Order");

    if (isConfirm) {
        _xLib.AJAX_Post("/api/KBNOR320/Calculate", "",
            function (success) {
                xSwal.xSuccess(success);
            },
            function (error) {
                xSwal.xError(error);
            }
        );
    }
    
})

