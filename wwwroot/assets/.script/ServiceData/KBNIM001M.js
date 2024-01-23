$(document).ready(function () {

    const xKBNIM001M = new MasterTemplate({
        Controller: _PAGE_,
        Table: 'tblMaster',
        ColumnTitle: {
            "EN": ['Order No.', 'Order Issued', 'Seq No.', 'Type Part', 'Part No.', 'Store Code', 'Qty.', 'Order'],
            "TH": ['Order No.', 'Order Issued', 'Seq No.', 'Type Part', 'Part No.', 'Store Code', 'Qty.', 'Order'],
            "JP": ['Order No.', 'Order Issued', 'Seq No.', 'Type Part', 'Part No.', 'Store Code', 'Qty.', 'Order'],
        },
        ColumnValue: [
            { "data": "F_Plant" },
            { "data": "F_Plant" },
            { "data": "F_OrderType" },
            { "data": "F_Effect_Date" },
            { "data": "F_End_Date" },
            { "data": "F_End_Date" },
            { "data": "F_End_Date" },
            { "data": "F_End_Date" }
        ],
        Modal: 'modalMaster',
        Form: 'frmMaster',
        PostData: [
            { name: 'F_Plant', value: _PLANT_ }
        ],
    });
    

    xSplash.hide();

    xKBNIM001M.prepare();

    xKBNIM001M.initial(function (result) {
        xKBNIM001M.search();
    });

    //onSave = function () {
    //    xKBNIM001M.save(function () {
    //        xKBNIM001M.search();
    //    });
    //}

    //onDelete = function () {
    //    xKBNIM001M.delete(function () {
    //        xKBNIM001M.search();
    //    });
    //}

    //onDeleteAll = function () {
    //    xKBNIM001M.deleteall(function () {
    //        xKBNIM001M.search();
    //    });
    //}

    //onPrint = function () {
    //    xDataTableExport.setConfigPDF({
    //        title: 'OLD TYPE SERVICE CHECK LIST'
    //    });
    //}

    //onExecute = function () {
    //    console.log('onExecute');
    //}

    //xAjax.onChange('#frmCondition #F_Plant', function () {
    //    $('#frmMaster #F_Plant').val($('#frmCondition #F_Plant').val());

    //    xKBNIM001M.search();
    //});


});